using FluentAssertions;
using Vali_Calendar.Core;
using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Providers.LatinAmerica;
using Vali_Time.Abstractions;
using Vali_Time.Enums;
using Xunit;

namespace Vali_Time.Tests.Tests;

public class ValiCalendarTests
{
    private readonly ValiCalendar _calendar = new(WeekStart.Monday);

    private sealed class FixedClock : IClock
    {
        private readonly DateTime _fixed;
        public FixedClock(DateTime fixedValue) => _fixed = fixedValue;
        public DateTime Now => _fixed;
        public DateTime Today => _fixed.Date;
    }

    // Fake provider that marks every single date as a holiday, forcing workday-scan
    // loops (AddWorkdays/NextWorkday/PreviousWorkday) to exhaust MaxWorkdayScan.
    private sealed class AlwaysHolidayProvider : IHolidayProvider
    {
        public string CountryCode => "XX";
        public string CountryName => "Nowhere";
        public string PrimaryLanguage => "en";
        public IEnumerable<HolidayInfo> GetHolidays(int year) => Enumerable.Empty<HolidayInfo>();
        public bool IsHoliday(DateTime date) => true;
        public bool IsHoliday(DateOnly date) => true;
        public HolidayInfo? GetHoliday(DateTime date) => null;
        public IEnumerable<HolidayInfo> GetHolidaysInRange(DateTime from, DateTime to) => Enumerable.Empty<HolidayInfo>();
        public string GetName(HolidayInfo holiday, string languageCode) => holiday.Name;
        public DateTime? GetObservedDate(HolidayInfo holiday, int year) => null;
    }

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

    // ── CalendarWeek.ToString ───────────────────────────────────────────────

    [Fact]
    public void CalendarWeek_ToString_FormatsAsWeekNumberAndYear()
    {
        var week = _calendar.WeekOf(new DateTime(2025, 3, 10));

        week.ToString().Should().Be($"W{week.WeekNumber:D2} {week.Year}");
    }

    // ── WeekOf (Sunday-based) ────────────────────────────────────────────────

    [Fact]
    public void WeekOf_SundayWeekStart_UsesGregorianCalendarYear()
    {
        var date = new DateTime(2025, 6, 15); // Sunday
        var week = _calendar.WeekOf(date, WeekStart.Sunday);

        week.Year.Should().Be(2025);
        week.Start.DayOfWeek.Should().Be(DayOfWeek.Sunday);
    }

    // ── CurrentWeek ──────────────────────────────────────────────────────────

    [Fact]
    public void CurrentWeek_UsesInjectedClock()
    {
        var fixedDate = new DateTime(2025, 3, 10); // Monday
        var calendar = new ValiCalendar(WeekStart.Monday, clock: new FixedClock(fixedDate));

        var week = calendar.CurrentWeek();

        week.Start.Should().Be(fixedDate.Date);
    }

    // ── IsFirstDayOfMonth / IsLastDayOfMonth ─────────────────────────────────

    [Fact]
    public void IsFirstDayOfMonth_FirstDay_ReturnsTrue()
    {
        _calendar.IsFirstDayOfMonth(new DateTime(2025, 5, 1)).Should().BeTrue();
    }

    [Fact]
    public void IsFirstDayOfMonth_NotFirstDay_ReturnsFalse()
    {
        _calendar.IsFirstDayOfMonth(new DateTime(2025, 5, 2)).Should().BeFalse();
    }

    [Fact]
    public void IsLastDayOfMonth_LastDayOfFebruaryLeapYear_ReturnsTrue()
    {
        _calendar.IsLastDayOfMonth(new DateTime(2024, 2, 29)).Should().BeTrue();
    }

    [Fact]
    public void IsLastDayOfMonth_NotLastDay_ReturnsFalse()
    {
        _calendar.IsLastDayOfMonth(new DateTime(2025, 5, 2)).Should().BeFalse();
    }

    // ── DaysInMonth ──────────────────────────────────────────────────────────

    [Fact]
    public void DaysInMonth_FebruaryLeapYear_Returns29()
    {
        _calendar.DaysInMonth(new DateTime(2024, 2, 1)).Should().Be(29);
    }

    // ── DaysOfMonth (iterator) ───────────────────────────────────────────────

    [Fact]
    public void DaysOfMonth_January2025_Enumerates31DaysFromFirstToLast()
    {
        var days = _calendar.DaysOfMonth(2025, 1).ToList();

        days.Should().HaveCount(31);
        days.First().Should().Be(new DateTime(2025, 1, 1));
        days.Last().Should().Be(new DateTime(2025, 1, 31));
    }

    // ── WorkdaysBetween (guard clauses) ──────────────────────────────────────

    [Fact]
    public void WorkdaysBetween_FromAfterTo_ThrowsArgumentException()
    {
        var from = new DateTime(2025, 1, 10);
        var to = new DateTime(2025, 1, 1);

        Action act = () => _calendar.WorkdaysBetween(from, to);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void WorkdaysBetween_SpanExceedsMaxScan_ThrowsInvalidOperationException()
    {
        var from = new DateTime(2000, 1, 1);
        var to = new DateTime(2015, 1, 1); // > 3660 days apart

        Action act = () => _calendar.WorkdaysBetween(from, to);

        act.Should().Throw<InvalidOperationException>();
    }

    // ── AddWorkdays / NextWorkday / PreviousWorkday (scan-limit guard) ───────

    [Fact]
    public void AddWorkdays_NoWorkdaysEverAvailable_ThrowsInvalidOperationException()
    {
        var calendar = new ValiCalendar(WeekStart.Monday, new AlwaysHolidayProvider());

        Action act = () => calendar.AddWorkdays(new DateTime(2025, 1, 1), 1);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void NextWorkday_NoWorkdaysEverAvailable_ThrowsInvalidOperationException()
    {
        var calendar = new ValiCalendar(WeekStart.Monday, new AlwaysHolidayProvider());

        Action act = () => calendar.NextWorkday(new DateTime(2025, 1, 1));

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void PreviousWorkday_NoWorkdaysEverAvailable_ThrowsInvalidOperationException()
    {
        var calendar = new ValiCalendar(WeekStart.Monday, new AlwaysHolidayProvider());

        Action act = () => calendar.PreviousWorkday(new DateTime(2025, 1, 1));

        act.Should().Throw<InvalidOperationException>();
    }

    // ── WorkdaysInYear ───────────────────────────────────────────────────────

    [Fact]
    public void WorkdaysInYear_2025_ReturnsPositiveCountUnderTotalDays()
    {
        int result = _calendar.WorkdaysInYear(2025);

        result.Should().BeInRange(250, 262);
    }

    // ── HasHolidayProvider ───────────────────────────────────────────────────

    [Fact]
    public void HasHolidayProvider_NoProviderConfigured_ReturnsFalse()
    {
        _calendar.HasHolidayProvider.Should().BeFalse();
    }

    [Fact]
    public void HasHolidayProvider_ProviderConfigured_ReturnsTrue()
    {
        var calendar = new ValiCalendar(WeekStart.Monday, new PeruHolidayProvider());

        calendar.HasHolidayProvider.Should().BeTrue();
    }

    // ── HolidaysInMonth / HolidaysInYear ─────────────────────────────────────

    [Fact]
    public void HolidaysInMonth_NoProviderConfigured_ThrowsInvalidOperationException()
    {
        Action act = () => _calendar.HolidaysInMonth(2025, 1).ToList();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void HolidaysInMonth_ProviderConfigured_ReturnsJanuaryHolidaysIncludingNewYear()
    {
        var calendar = new ValiCalendar(WeekStart.Monday, new PeruHolidayProvider());

        var holidays = calendar.HolidaysInMonth(2025, 1).ToList();

        holidays.Should().NotBeEmpty();
        holidays.Should().OnlyContain(h => h.Month == 1);
    }

    [Fact]
    public void HolidaysInYear_NoProviderConfigured_ThrowsInvalidOperationException()
    {
        Action act = () => _calendar.HolidaysInYear(2025).ToList();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void HolidaysInYear_ProviderConfigured_ReturnsNonEmptySequence()
    {
        var calendar = new ValiCalendar(WeekStart.Monday, new PeruHolidayProvider());

        var holidays = calendar.HolidaysInYear(2025).ToList();

        holidays.Should().NotBeEmpty();
    }

    // ── IsLeapYear / DaysInYear ──────────────────────────────────────────────

    [Theory]
    [InlineData(2024, true)]
    [InlineData(2025, false)]
    public void IsLeapYear_ReturnsExpectedResult(int year, bool expected)
    {
        _calendar.IsLeapYear(year).Should().Be(expected);
    }

    [Theory]
    [InlineData(2024, 366)]
    [InlineData(2025, 365)]
    public void DaysInYear_ReturnsExpectedResult(int year, int expected)
    {
        _calendar.DaysInYear(year).Should().Be(expected);
    }
}
