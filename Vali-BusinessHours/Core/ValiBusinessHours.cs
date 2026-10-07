using Vali_BusinessHours.Models;
using Vali_Calendar.Core;
using Vali_TimeZone.Core;

namespace Vali_BusinessHours.Core;

/// <summary>
/// Default <see cref="IValiBusinessHours"/> implementation, backed by an injected
/// <see cref="IValiTimeZone"/> for offset/DST-aware conversion between timezones.
/// </summary>
public class ValiBusinessHours : IValiBusinessHours
{
    private readonly IValiTimeZone _timeZone;

    /// <summary>
    /// Initializes a new instance of <see cref="ValiBusinessHours"/>.
    /// </summary>
    /// <param name="timeZone">The timezone service used to resolve offsets and conversions.</param>
    public ValiBusinessHours(IValiTimeZone timeZone)
    {
        _timeZone = timeZone ?? throw new ArgumentNullException(nameof(timeZone));
    }

    /// <inheritdoc/>
    public BusinessHoursOverlapResult Overlap(BusinessHoursWindow a, BusinessHoursWindow b, DateTime referenceDate)
    {
        if (a.Start >= a.End)
            throw new ArgumentException("Window 'a' must have Start earlier than End.", nameof(a));
        if (b.Start >= b.End)
            throw new ArgumentException("Window 'b' must have Start earlier than End.", nameof(b));

        var day = referenceDate.Date;

        var (aStartUtc, aEndUtc) = ResolveWindowUtc(a, day);

        // 'b' may fall on the calendar day before or after `day` once projected against `a`,
        // due to the offset difference between the two zones (midnight-crossing). Only one of
        // the three candidate days can realistically overlap a window shorter than 24h, but all
        // three are checked rather than assuming which one.
        foreach (var candidateDay in new[] { day.AddDays(-1), day, day.AddDays(1) })
        {
            var (bStartUtc, bEndUtc) = ResolveWindowUtc(b, candidateDay);

            var overlapStart = aStartUtc > bStartUtc ? aStartUtc : bStartUtc;
            var overlapEnd = aEndUtc < bEndUtc ? aEndUtc : bEndUtc;

            if (overlapStart >= overlapEnd) continue;

            return BusinessHoursOverlapResult.Found(
                utc: (overlapStart, overlapEnd),
                inZoneA: (_timeZone.ConvertOffset(overlapStart, a.ZoneId), _timeZone.ConvertOffset(overlapEnd, a.ZoneId)),
                inZoneB: (_timeZone.ConvertOffset(overlapStart, b.ZoneId), _timeZone.ConvertOffset(overlapEnd, b.ZoneId)));
        }

        return BusinessHoursOverlapResult.None();
    }

    /// <inheritdoc/>
    public BusinessHoursOverlapResult OverlapOnWorkday(
        (BusinessHoursWindow Window, IValiCalendar Calendar) a,
        (BusinessHoursWindow Window, IValiCalendar Calendar) b,
        DateTime referenceDate)
    {
        if (a.Calendar is null) throw new ArgumentNullException(nameof(a));
        if (b.Calendar is null) throw new ArgumentNullException(nameof(b));

        if (!a.Calendar.IsWorkday(referenceDate) || !b.Calendar.IsWorkday(referenceDate))
            return BusinessHoursOverlapResult.None();

        return Overlap(a.Window, b.Window, referenceDate);
    }

    /// <summary>
    /// Resolves <paramref name="window"/>'s local Start/End on <paramref name="localDay"/> to
    /// absolute UTC instants, using the zone's real offset (DST-aware) at each instant.
    /// </summary>
    private (DateTimeOffset Start, DateTimeOffset End) ResolveWindowUtc(BusinessHoursWindow window, DateTime localDay)
    {
        var startLocal = localDay + window.Start.ToTimeSpan();
        var endLocal = localDay + window.End.ToTimeSpan();

        var startOffset = _timeZone.ToDateTimeOffset(startLocal, window.ZoneId);
        var endOffset = _timeZone.ToDateTimeOffset(endLocal, window.ZoneId);

        return (startOffset.ToUniversalTime(), endOffset.ToUniversalTime());
    }
}
