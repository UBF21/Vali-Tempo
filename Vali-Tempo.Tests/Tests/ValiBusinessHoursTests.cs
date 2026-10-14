using FluentAssertions;
using Vali_BusinessHours.Core;
using Vali_BusinessHours.Models;
using Vali_Calendar.Core;
using Vali_TimeZone.Core;
using Xunit;

namespace Vali_Time.Tests.Tests;

public class ValiBusinessHoursTests
{
    private readonly IValiTimeZone _timeZone = new ValiTimeZone();
    private readonly IValiBusinessHours _businessHours;

    public ValiBusinessHoursTests()
    {
        _businessHours = new ValiBusinessHours(_timeZone);
    }

    [Fact]
    public void Overlap_SameHoursSameRelativeOffset_ReturnsFullOverlap()
    {
        // Lima (UTC-5, no DST) and a fixed-offset-equivalent window on the same reference date,
        // both 09:00-18:00 local. With no offset difference, the whole window overlaps.
        var a = new BusinessHoursWindow("America/Lima", new TimeOnly(9, 0), new TimeOnly(18, 0));
        var b = new BusinessHoursWindow("America/Lima", new TimeOnly(9, 0), new TimeOnly(18, 0));

        var result = _businessHours.Overlap(a, b, new DateTime(2025, 6, 2));

        result.HasOverlap.Should().BeTrue();
        result.StartInZoneA!.Value.TimeOfDay.Should().Be(new TimeSpan(9, 0, 0));
        result.EndInZoneA!.Value.TimeOfDay.Should().Be(new TimeSpan(18, 0, 0));
    }

    [Fact]
    public void Overlap_LimaAndMadrid_ResolvesPartialOverlapInEachLocalTime()
    {
        // Lima UTC-5 (no DST), Madrid UTC+2 in June (DST/CEST) — 7h difference.
        // Lima 09:00-18:00 local = 14:00-23:00 UTC.
        // Madrid 09:00-18:00 local = 07:00-16:00 UTC.
        // Overlap in UTC: max(14:00,07:00) .. min(23:00,16:00) = 14:00-16:00 UTC.
        var lima = new BusinessHoursWindow("America/Lima", new TimeOnly(9, 0), new TimeOnly(18, 0));
        var madrid = new BusinessHoursWindow("Europe/Madrid", new TimeOnly(9, 0), new TimeOnly(18, 0));

        var result = _businessHours.Overlap(lima, madrid, new DateTime(2025, 6, 2));

        result.HasOverlap.Should().BeTrue();
        result.StartUtc!.Value.Hour.Should().Be(14);
        result.EndUtc!.Value.Hour.Should().Be(16);
        // 14:00 UTC in Lima (UTC-5) is 09:00 local — the start of Lima's window.
        result.StartInZoneA!.Value.TimeOfDay.Should().Be(new TimeSpan(9, 0, 0));
        // 16:00 UTC in Madrid (UTC+2 summer) is 18:00 local — the end of Madrid's window.
        result.EndInZoneB!.Value.TimeOfDay.Should().Be(new TimeSpan(18, 0, 0));
        // 14:00 UTC in Madrid (UTC+2 summer) is 16:00 local — the start of the overlap in zone B.
        result.StartInZoneB!.Value.TimeOfDay.Should().Be(new TimeSpan(16, 0, 0));
    }

    [Fact]
    public void Overlap_NoSharedWindow_ReturnsHasOverlapFalseWithNullMembers()
    {
        // Lima 09:00-11:00 vs a zone far enough ahead that its matching window doesn't reach back.
        var lima = new BusinessHoursWindow("America/Lima", new TimeOnly(9, 0), new TimeOnly(11, 0));
        var tokyo = new BusinessHoursWindow("Asia/Tokyo", new TimeOnly(9, 0), new TimeOnly(11, 0));

        var result = _businessHours.Overlap(lima, tokyo, new DateTime(2025, 6, 2));

        result.HasOverlap.Should().BeFalse();
        result.StartUtc.Should().BeNull();
        result.EndUtc.Should().BeNull();
        result.StartInZoneA.Should().BeNull();
        result.EndInZoneB.Should().BeNull();
    }

    [Fact]
    public void Overlap_MidnightCrossing_FindsWindowOnAdjacentCalendarDay()
    {
        // Los Angeles (UTC-7 in June, PDT) 09:00-18:00 local = 16:00 Jun2 - 01:00 Jun3 UTC.
        // Tokyo (UTC+9, no DST) 09:00-18:00 local on Jun 3 = 00:00-09:00 Jun3 UTC.
        // Searching Tokyo's window on reference date Jun 2 alone would miss it — the resolver
        // must also check Jun 3 for Tokyo relative to LA's Jun 2 reference.
        var la = new BusinessHoursWindow("America/Los_Angeles", new TimeOnly(9, 0), new TimeOnly(18, 0));
        var tokyo = new BusinessHoursWindow("Asia/Tokyo", new TimeOnly(9, 0), new TimeOnly(18, 0));

        var result = _businessHours.Overlap(la, tokyo, new DateTime(2025, 6, 2));

        result.HasOverlap.Should().BeTrue();
        // Overlap: max(16:00 Jun2, 00:00 Jun3) .. min(01:00 Jun3, 09:00 Jun3) = 00:00-01:00 Jun3 UTC.
        result.StartUtc!.Value.Should().Be(new DateTimeOffset(2025, 6, 3, 0, 0, 0, TimeSpan.Zero));
        result.EndUtc!.Value.Should().Be(new DateTimeOffset(2025, 6, 3, 1, 0, 0, TimeSpan.Zero));
    }

    [Fact]
    public void Overlap_InvalidZone_ThrowsTimeZoneNotFoundException()
    {
        var a = new BusinessHoursWindow("Not/AZone", new TimeOnly(9, 0), new TimeOnly(18, 0));
        var b = new BusinessHoursWindow("America/Lima", new TimeOnly(9, 0), new TimeOnly(18, 0));

        var act = () => _businessHours.Overlap(a, b, new DateTime(2025, 6, 2));

        act.Should().Throw<TimeZoneNotFoundException>();
    }

    [Fact]
    public void Overlap_WindowWithStartAfterEnd_ThrowsArgumentException()
    {
        var a = new BusinessHoursWindow("America/Lima", new TimeOnly(18, 0), new TimeOnly(9, 0));
        var b = new BusinessHoursWindow("America/Lima", new TimeOnly(9, 0), new TimeOnly(18, 0));

        var act = () => _businessHours.Overlap(a, b, new DateTime(2025, 6, 2));

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void OverlapOnWorkday_NotAWorkdayForOneSide_ReturnsNoOverlapWithoutComputing()
    {
        var saturday = new DateTime(2025, 6, 7);
        var lima = new BusinessHoursWindow("America/Lima", new TimeOnly(9, 0), new TimeOnly(18, 0));
        var madrid = new BusinessHoursWindow("Europe/Madrid", new TimeOnly(9, 0), new TimeOnly(18, 0));
        IValiCalendar calendarA = new ValiCalendar();
        IValiCalendar calendarB = new ValiCalendar();

        var result = _businessHours.OverlapOnWorkday((lima, calendarA), (madrid, calendarB), saturday);

        result.HasOverlap.Should().BeFalse();
    }

    [Fact]
    public void OverlapOnWorkday_WorkdayForBoth_DelegatesToOverlap()
    {
        var monday = new DateTime(2025, 6, 2);
        var lima = new BusinessHoursWindow("America/Lima", new TimeOnly(9, 0), new TimeOnly(18, 0));
        var madrid = new BusinessHoursWindow("Europe/Madrid", new TimeOnly(9, 0), new TimeOnly(18, 0));
        IValiCalendar calendarA = new ValiCalendar();
        IValiCalendar calendarB = new ValiCalendar();

        var result = _businessHours.OverlapOnWorkday((lima, calendarA), (madrid, calendarB), monday);

        result.HasOverlap.Should().BeTrue();
    }

    [Fact]
    public void Constructor_NullTimeZone_ThrowsArgumentNullException()
    {
        var act = () => new ValiBusinessHours(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
