using Vali_BusinessHours.Models;
using Vali_Calendar.Core;

namespace Vali_BusinessHours.Core;

/// <summary>
/// Resolves the simultaneous business-hours overlap window between two IANA timezones,
/// for a single calendar reference date. Composes <c>Vali-Calendar</c> and
/// <c>Vali-TimeZone</c> without coupling either package to the other.
/// </summary>
/// <remarks>
/// <b>v1 scope:</b> exactly two timezones per call, a single daily business-hours block
/// per zone (no recurrence/exceptions), and no automatic holiday filtering — see the
/// package README for the full list of v1 non-goals.
/// </remarks>
public interface IValiBusinessHours
{
    /// <summary>
    /// Resolves the overlap between <paramref name="a"/> and <paramref name="b"/> for the
    /// calendar date <paramref name="referenceDate"/>, interpreted in <paramref name="a"/>'s
    /// timezone. The offset of each zone (including DST) is resolved at the specific instant
    /// evaluated, never a cached base offset, and the search accounts for <paramref name="b"/>'s
    /// window falling on the calendar day before or after <paramref name="referenceDate"/> once
    /// projected to <paramref name="a"/>'s zone (midnight-crossing).
    /// </summary>
    /// <param name="a">The first timezone's daily business-hours window.</param>
    /// <param name="b">The second timezone's daily business-hours window.</param>
    /// <param name="referenceDate">
    /// The calendar date to evaluate, interpreted as a local date in <paramref name="a"/>'s timezone
    /// (time-of-day component is ignored).
    /// </param>
    /// <returns>
    /// A <see cref="BusinessHoursOverlapResult"/> describing the overlap window, or a result with
    /// <see cref="BusinessHoursOverlapResult.HasOverlap"/> set to <see langword="false"/> when the
    /// two windows never coincide on <paramref name="referenceDate"/>.
    /// </returns>
    /// <exception cref="TimeZoneNotFoundException">
    /// Thrown when <paramref name="a"/>'s or <paramref name="b"/>'s <see cref="BusinessHoursWindow.ZoneId"/>
    /// cannot be resolved.
    /// </exception>
    BusinessHoursOverlapResult Overlap(BusinessHoursWindow a, BusinessHoursWindow b, DateTime referenceDate);

    /// <summary>
    /// Same as <see cref="Overlap"/>, but first requires <paramref name="referenceDate"/> to be a
    /// workday in both <paramref name="a"/>'s and <paramref name="b"/>'s calendars (as resolved by
    /// each calendar's own <c>IsWorkday</c>, e.g. configured with each side's own holiday provider).
    /// </summary>
    /// <param name="a">The first timezone's business-hours window, paired with the calendar used to decide whether <paramref name="referenceDate"/> is a workday for it.</param>
    /// <param name="b">The second timezone's business-hours window, paired with its own workday calendar.</param>
    /// <param name="referenceDate">The calendar date to evaluate (see <see cref="Overlap"/>).</param>
    /// <returns>
    /// A result with <see cref="BusinessHoursOverlapResult.HasOverlap"/> set to <see langword="false"/>
    /// when <paramref name="referenceDate"/> is not a workday for either side, or when the windows
    /// otherwise don't overlap; the real overlap window otherwise.
    /// </returns>
    BusinessHoursOverlapResult OverlapOnWorkday(
        (BusinessHoursWindow Window, IValiCalendar Calendar) a,
        (BusinessHoursWindow Window, IValiCalendar Calendar) b,
        DateTime referenceDate);
}
