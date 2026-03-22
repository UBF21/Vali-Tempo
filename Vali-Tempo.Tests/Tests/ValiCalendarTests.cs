using FluentAssertions;
using Vali_Calendar.Core;
using Vali_Time.Enums;
using Xunit;

namespace Vali_Time.Tests.Tests;

public class ValiCalendarTests
{
    private readonly ValiCalendar _calendar = new(WeekStart.Monday);

    // ── WeekCountInMonth ─────────────────────────────────────────────────────

    [Fact]
    public void WeekCountInMonth_January2025_ReturnsBetweenFourAndSix()
    {
        int count = _calendar.WeekCountInMonth(2025, 1);

        count.Should().BeInRange(4, 6);
    }

    [Fact]
    public void WeekCountInMonth_February2025_ReturnsFourOrMore()
    {
        int count = _calendar.WeekCountInMonth(2025, 2);

        count.Should().BeGreaterThanOrEqualTo(4);
    }

    [Fact]
    public void WeekCountInMonth_IsConsistentAcrossMultipleCalls()
    {
        int first  = _calendar.WeekCountInMonth(2025, 3);
        int second = _calendar.WeekCountInMonth(2025, 3);

        first.Should().Be(second);
    }

    // ── WeekOf ───────────────────────────────────────────────────────────────

    [Fact]
    public void WeekOf_January1_2025_WeekNumberIsAtLeastOne()
    {
        var week = _calendar.WeekOf(new DateTime(2025, 1, 1));

        week.WeekNumber.Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public void WeekOf_December28_2025_YearMayBeNextIsoYear()
    {
        // ISO week containing 2025-12-28 belongs to year 2025 or 2026
        var week = _calendar.WeekOf(new DateTime(2025, 12, 28));

        week.Year.Should().BeOneOf(2025, 2026);
    }

    [Fact]
    public void WeekOf_CalledOneHundredTimesInLoop_DoesNotThrow()
    {
        var start = new DateTime(2025, 1, 1);
        Action act = () =>
        {
            for (int i = 0; i < 100; i++)
                _calendar.WeekOf(start.AddDays(i));
        };

        act.Should().NotThrow();
    }

    [Fact]
    public void WeekOf_ReturnsWeekWithCorrectStartAndEnd()
    {
        // 2025-01-06 is a Monday; week should start on 2025-01-06 and end on 2025-01-12
        var week = _calendar.WeekOf(new DateTime(2025, 1, 8)); // Wednesday

        week.Start.Should().Be(new DateTime(2025, 1, 6));
        week.End.Should().Be(new DateTime(2025, 1, 12));
    }

    // ── WeeksInMonth ─────────────────────────────────────────────────────────

    [Fact]
    public void WeeksInMonth_January2025_ReturnsUniqueWeeks()
    {
        var weeks = _calendar.WeeksInMonth(2025, 1).ToList();

        var keys = weeks.Select(w => (w.WeekNumber, w.Year)).ToList();
        keys.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void WeeksInMonth_AllWeeksContainAtLeastOneDayInMonth()
    {
        var weeks = _calendar.WeeksInMonth(2025, 3).ToList();

        foreach (var week in weeks)
        {
            bool overlaps = week.Start.Month == 3 || week.End.Month == 3
                            || (week.Start < new DateTime(2025, 3, 1) && week.End >= new DateTime(2025, 3, 1));
            overlaps.Should().BeTrue();
        }
    }

    // ── WorkdaysBetween ──────────────────────────────────────────────────────

    [Fact]
    public void WorkdaysBetween_OneWorkweek_Returns5()
    {
        // 2025-01-06 Monday → 2025-01-10 Friday = 5 workdays
        var from = new DateTime(2025, 1, 6);
        var to   = new DateTime(2025, 1, 10);
        int result = _calendar.WorkdaysBetween(from, to);
        result.Should().Be(5);
    }

    // ── AddWorkdays ──────────────────────────────────────────────────────────

    [Fact]
    public void AddWorkdays_5WorkdaysFromMonday_ReturnsMonday()
    {
        // 2025-01-06 is Monday; +5 workdays skips the weekend and lands on 2025-01-13 (next Monday)
        var monday = new DateTime(2025, 1, 6);
        DateTime result = _calendar.AddWorkdays(monday, 5);
        result.Should().Be(new DateTime(2025, 1, 13));
    }

    // ── NextWorkday ──────────────────────────────────────────────────────────

    [Fact]
    public void NextWorkday_OnFriday_ReturnsMonday()
    {
        // 2025-01-10 is Friday; NextWorkday on a workday returns the same day
        // We need the NEXT workday AFTER Friday = Monday 2025-01-13
        // But NextWorkday returns current day if already a workday, so test from Saturday
        var saturday = new DateTime(2025, 1, 11);
        DateTime result = _calendar.NextWorkday(saturday);
        result.Should().Be(new DateTime(2025, 1, 13));
    }

    // ── PreviousWorkday ──────────────────────────────────────────────────────

    [Fact]
    public void PreviousWorkday_OnMonday_ReturnsFriday()
    {
        // 2025-01-13 is Monday; PreviousWorkday on a workday returns the same day
        // Test from Sunday to get the previous workday = Friday
        var sunday = new DateTime(2025, 1, 12);
        DateTime result = _calendar.PreviousWorkday(sunday);
        result.Should().Be(new DateTime(2025, 1, 10));
    }

    // ── IsWorkday ────────────────────────────────────────────────────────────

    [Fact]
    public void IsWorkday_Saturday_ReturnsFalse()
    {
        var saturday = new DateTime(2025, 1, 11);
        _calendar.IsWorkday(saturday).Should().BeFalse();
    }

    [Fact]
    public void IsWorkday_Monday_ReturnsTrue()
    {
        var monday = new DateTime(2025, 1, 6);
        _calendar.IsWorkday(monday).Should().BeTrue();
    }

    // ── WorkdaysInMonth ──────────────────────────────────────────────────────

    [Fact]
    public void WorkdaysInMonth_January2025_Returns23()
    {
        // January 2025: starts on Wednesday; 31 days total → 23 weekdays
        int result = _calendar.WorkdaysInMonth(2025, 1);
        result.Should().Be(23);
    }
}
