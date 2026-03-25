using FluentAssertions;
using Vali_Time.Core;
using Vali_Time.Enums;
using Xunit;

namespace Vali_Time.Tests.Tests;

public class ValiDateTests
{
    private readonly ValiDate _vali = new();

    // ── Diff ────────────────────────────────────────────────────────────────

    [Fact]
    public void Diff_KnownDates_InDays_ReturnsCorrect()
    {
        var from = new DateTime(2025, 1, 1);
        var to   = new DateTime(2025, 1, 31);
        decimal result = _vali.Diff(from, to, TimeUnit.Days);
        result.Should().Be(30m);
    }

    [Fact]
    public void Diff_KnownDates_InHours_ReturnsCorrect()
    {
        var from = new DateTime(2025, 1, 1, 0, 0, 0);
        var to   = new DateTime(2025, 1, 2, 0, 0, 0);
        decimal result = _vali.Diff(from, to, TimeUnit.Hours);
        result.Should().Be(24m);
    }

    [Fact]
    public void Diff_KnownDates_InMonths_ReturnsCorrect()
    {
        var from = new DateTime(2025, 1, 1);
        var to   = new DateTime(2025, 4, 1);
        decimal result = _vali.Diff(from, to, TimeUnit.Months);
        result.Should().Be(3m);
    }

    [Fact]
    public void Diff_KnownDates_InYears_ReturnsApproximate()
    {
        var from = new DateTime(2020, 1, 1);
        var to   = new DateTime(2025, 1, 1);
        decimal result = _vali.Diff(from, to, TimeUnit.Years);
        result.Should().BeApproximately(5m, 0.01m);
    }

    // ── Add ─────────────────────────────────────────────────────────────────

    [Fact]
    public void Add_30Days_ReturnsCorrectDate()
    {
        var date = new DateTime(2025, 1, 1);
        DateTime result = _vali.Add(date, 30, TimeUnit.Days);
        result.Should().Be(new DateTime(2025, 1, 31));
    }

    [Fact]
    public void Add_1Month_ReturnsCorrectDate()
    {
        var date = new DateTime(2025, 1, 1);
        DateTime result = _vali.Add(date, 1, TimeUnit.Months);
        result.Should().Be(new DateTime(2025, 2, 1));
    }

    [Fact]
    public void Add_1Year_ReturnsCorrectDate()
    {
        var date = new DateTime(2025, 1, 1);
        DateTime result = _vali.Add(date, 1, TimeUnit.Years);
        result.Should().Be(new DateTime(2026, 1, 1));
    }

    // ── Subtract ────────────────────────────────────────────────────────────

    [Fact]
    public void Subtract_30Days_ReturnsCorrectDate()
    {
        var date = new DateTime(2025, 1, 31);
        DateTime result = _vali.Subtract(date, 30, TimeUnit.Days);
        result.Should().Be(new DateTime(2025, 1, 1));
    }

    [Fact]
    public void Subtract_1Month_ReturnsCorrectDate()
    {
        var date = new DateTime(2025, 2, 1);
        DateTime result = _vali.Subtract(date, 1, TimeUnit.Months);
        result.Should().Be(new DateTime(2025, 1, 1));
    }

    [Fact]
    public void Subtract_1Year_ReturnsCorrectDate()
    {
        var date = new DateTime(2026, 1, 1);
        DateTime result = _vali.Subtract(date, 1, TimeUnit.Years);
        result.Should().Be(new DateTime(2025, 1, 1));
    }

    // ── StartOf ─────────────────────────────────────────────────────────────

    [Fact]
    public void StartOf_Day_EliminatesTimeComponent()
    {
        var date = new DateTime(2025, 6, 15, 14, 30, 45);
        DateTime result = _vali.StartOf(date, DatePart.Day);
        result.Should().Be(new DateTime(2025, 6, 15, 0, 0, 0));
    }

    [Fact]
    public void StartOf_Week_ReturnsMondayOfWeek()
    {
        // 2025-06-18 is a Wednesday; Monday of that week is 2025-06-16
        var date = new DateTime(2025, 6, 18);
        DateTime result = _vali.StartOf(date, DatePart.Week);
        result.DayOfWeek.Should().Be(DayOfWeek.Monday);
        result.Should().Be(new DateTime(2025, 6, 16));
    }

    [Fact]
    public void StartOf_Month_ReturnsFirstDayOfMonth()
    {
        var date = new DateTime(2025, 6, 18);
        DateTime result = _vali.StartOf(date, DatePart.Month);
        result.Should().Be(new DateTime(2025, 6, 1));
    }

    [Theory]
    [InlineData(1,  1)]   // Q1 starts Jan 1
    [InlineData(4,  4)]   // Q2 starts Apr 1
    [InlineData(7,  7)]   // Q3 starts Jul 1
    [InlineData(10, 10)]  // Q4 starts Oct 1
    public void StartOf_Quarter_ReturnsFirstDayOfQuarter(int month, int expectedMonth)
    {
        var date = new DateTime(2025, month, 15);
        DateTime result = _vali.StartOf(date, DatePart.Quarter);
        result.Should().Be(new DateTime(2025, expectedMonth, 1));
    }

    [Fact]
    public void StartOf_Year_ReturnsJan1()
    {
        var date = new DateTime(2025, 8, 22);
        DateTime result = _vali.StartOf(date, DatePart.Year);
        result.Should().Be(new DateTime(2025, 1, 1));
    }

    // ── EndOf ───────────────────────────────────────────────────────────────

    [Fact]
    public void EndOf_Month_ReturnsLastDayAtLastTickOfDay()
    {
        var date = new DateTime(2025, 2, 10);
        DateTime result = _vali.EndOf(date, DatePart.Month);
        result.Date.Should().Be(new DateTime(2025, 2, 28));
        result.TimeOfDay.Should().Be(TimeSpan.FromTicks(TimeSpan.TicksPerDay - 1));
    }

    [Fact]
    public void EndOf_Month_LeapYear_ReturnsFeb29()
    {
        var date = new DateTime(2024, 2, 10);
        DateTime result = _vali.EndOf(date, DatePart.Month);
        result.Day.Should().Be(29);
        result.Month.Should().Be(2);
    }

    [Fact]
    public void EndOf_Day_TimeOfDayIsLastTickOfDay()
    {
        var date = new DateTime(2025, 6, 15, 10, 30, 0);
        DateTime result = _vali.EndOf(date, DatePart.Day);
        result.TimeOfDay.Should().Be(TimeSpan.FromTicks(TimeSpan.TicksPerDay - 1));
    }

    [Fact]
    public void EndOf_Day_DateComponentIsUnchanged()
    {
        var date = new DateTime(2025, 6, 15, 10, 30, 0);
        DateTime result = _vali.EndOf(date, DatePart.Day);
        result.Date.Should().Be(new DateTime(2025, 6, 15));
    }

    [Fact]
    public void EndOf_Day_ContainsSubMillisecondInstantsOfSameDay()
    {
        var date = new DateTime(2025, 6, 15);
        DateTime endOfDay = _vali.EndOf(date, DatePart.Day);

        // A DateTime with sub-millisecond ticks on the same day must be <= endOfDay
        var subMillisecond = date.Date.AddTicks(TimeSpan.TicksPerDay - 2);
        subMillisecond.Should().BeOnOrBefore(endOfDay);
    }

    [Fact]
    public void EndOf_Day_LastTickIsLessThanMidnightOfNextDay()
    {
        var date = new DateTime(2025, 6, 15);
        DateTime endOfDay = _vali.EndOf(date, DatePart.Day);
        DateTime midnight = date.Date.AddDays(1);
        endOfDay.Should().BeBefore(midnight);
    }

    // ── QuarterOf ───────────────────────────────────────────────────────────

    [Theory]
    [InlineData(1,  1)]
    [InlineData(2,  1)]
    [InlineData(3,  1)]
    [InlineData(4,  2)]
    [InlineData(5,  2)]
    [InlineData(6,  2)]
    [InlineData(7,  3)]
    [InlineData(8,  3)]
    [InlineData(9,  3)]
    [InlineData(10, 4)]
    [InlineData(11, 4)]
    [InlineData(12, 4)]
    public void QuarterOf_AllMonths_ReturnsCorrectQuarter(int month, int expectedQuarter)
    {
        var date = new DateTime(2025, month, 1);
        int result = _vali.QuarterOf(date);
        result.Should().Be(expectedQuarter);
    }

    // ── QuarterName ─────────────────────────────────────────────────────────

    [Fact]
    public void QuarterName_ReturnsFormattedString()
    {
        var date = new DateTime(2025, 5, 15);
        string result = _vali.QuarterName(date);
        result.Should().Be("Q2 2025");
    }

    // ── ProgressInQuarter ───────────────────────────────────────────────────

    [Fact]
    public void ProgressInQuarter_StartOfQuarter_ReturnsNearZero()
    {
        var date = new DateTime(2025, 1, 1);
        decimal result = _vali.ProgressInQuarter(date);
        result.Should().BeApproximately(0m, 0.02m);
    }

    [Fact]
    public void ProgressInQuarter_EndOfQuarter_ReturnsNearOne()
    {
        var date = new DateTime(2025, 3, 31);
        decimal result = _vali.ProgressInQuarter(date);
        result.Should().BeApproximately(1m, 0.02m);
    }

    [Fact]
    public void ProgressInQuarter_MidQuarter_ReturnsApproximateHalf()
    {
        // Q1 2025 is 90 days; day 45 is ~midpoint
        var date = new DateTime(2025, 2, 14);
        decimal result = _vali.ProgressInQuarter(date);
        result.Should().BeInRange(0.4m, 0.6m);
    }

    // ── DaysInQuarter ───────────────────────────────────────────────────────

    [Fact]
    public void DaysInQuarter_Q1_NonLeapYear_Returns90()
    {
        var date = new DateTime(2025, 1, 1);
        int result = _vali.DaysInQuarter(date);
        result.Should().Be(90);
    }

    [Fact]
    public void DaysInQuarter_Q1_LeapYear_Returns91()
    {
        var date = new DateTime(2024, 1, 1);
        int result = _vali.DaysInQuarter(date);
        result.Should().Be(91);
    }

    // ── IsWeekend ───────────────────────────────────────────────────────────

    [Fact]
    public void IsWeekend_Saturday_ReturnsTrue()
    {
        var saturday = new DateTime(2025, 6, 14);
        _vali.IsWeekend(saturday).Should().BeTrue();
    }

    [Fact]
    public void IsWeekend_Sunday_ReturnsTrue()
    {
        var sunday = new DateTime(2025, 6, 15);
        _vali.IsWeekend(sunday).Should().BeTrue();
    }

    [Fact]
    public void IsWeekend_Monday_ReturnsFalse()
    {
        var monday = new DateTime(2025, 6, 16);
        _vali.IsWeekend(monday).Should().BeFalse();
    }

    // ── IsSameDay ───────────────────────────────────────────────────────────

    [Fact]
    public void IsSameDay_SameDate_ReturnsTrue()
    {
        var a = new DateTime(2025, 6, 15, 10, 0, 0);
        var b = new DateTime(2025, 6, 15, 22, 0, 0);
        _vali.IsSameDay(a, b).Should().BeTrue();
    }

    [Fact]
    public void IsSameDay_DifferentDates_ReturnsFalse()
    {
        var a = new DateTime(2025, 6, 15);
        var b = new DateTime(2025, 6, 16);
        _vali.IsSameDay(a, b).Should().BeFalse();
    }

    // ── IsSamePeriod ────────────────────────────────────────────────────────

    [Fact]
    public void IsSamePeriod_SameMonth_ReturnsTrue()
    {
        var a = new DateTime(2025, 6, 1);
        var b = new DateTime(2025, 6, 30);
        _vali.IsSamePeriod(a, b, DatePart.Month).Should().BeTrue();
    }

    [Fact]
    public void IsSamePeriod_DifferentMonths_ReturnsFalse()
    {
        var a = new DateTime(2025, 6, 1);
        var b = new DateTime(2025, 7, 1);
        _vali.IsSamePeriod(a, b, DatePart.Month).Should().BeFalse();
    }

    [Fact]
    public void IsSamePeriod_SameYear_ReturnsTrue()
    {
        var a = new DateTime(2025, 1, 1);
        var b = new DateTime(2025, 12, 31);
        _vali.IsSamePeriod(a, b, DatePart.Year).Should().BeTrue();
    }

    // ── WeekOfYear ──────────────────────────────────────────────────────────

    [Fact]
    public void WeekOfYear_Jan1_2025_ReturnsWeek1()
    {
        // 2025-01-01 is a Wednesday; ISO week 1
        var date = new DateTime(2025, 1, 1);
        int result = _vali.WeekOfYear(date);
        result.Should().Be(1);
    }

    [Fact]
    public void WeekOfYear_KnownDate_ReturnsExpected()
    {
        // 2025-06-16 is a Monday of week 25 by ISO 8601
        var date = new DateTime(2025, 6, 16);
        int result = _vali.WeekOfYear(date);
        result.Should().Be(25);
    }

    // ── IsLeapYear ──────────────────────────────────────────────────────────

    [Fact]
    public void IsLeapYear_2024_ReturnsTrue()
    {
        _vali.IsLeapYear(2024).Should().BeTrue();
    }

    [Fact]
    public void IsLeapYear_2025_ReturnsFalse()
    {
        _vali.IsLeapYear(2025).Should().BeFalse();
    }

    // ── Additional StartOf tests ─────────────────────────────────────────────

    [Fact]
    public void StartOf_Month_ReturnsFirstDay()
    {
        var date = new DateTime(2025, 8, 22);
        DateTime result = _vali.StartOf(date, DatePart.Month);
        result.Day.Should().Be(1);
    }

    [Fact]
    public void StartOf_Year_ReturnsJanFirst()
    {
        var date = new DateTime(2025, 9, 15);
        DateTime result = _vali.StartOf(date, DatePart.Year);
        result.Should().Be(new DateTime(2025, 1, 1));
    }

    // ── Additional EndOf tests ───────────────────────────────────────────────

    [Fact]
    public void EndOf_Month_LeapYear2024_Feb29()
    {
        var date = new DateTime(2024, 2, 5);
        DateTime result = _vali.EndOf(date, DatePart.Month);
        result.Day.Should().Be(29);
    }

    [Fact]
    public void EndOf_Year_IsDecember31()
    {
        var date = new DateTime(2025, 6, 15);
        DateTime result = _vali.EndOf(date, DatePart.Year);
        result.Month.Should().Be(12);
        result.Day.Should().Be(31);
    }

    // ── Additional Diff tests ────────────────────────────────────────────────

    [Fact]
    public void Diff_Weeks_ThreeWeeks()
    {
        var from = new DateTime(2025, 1, 1);
        var to   = from.AddDays(21);
        decimal result = _vali.Diff(from, to, TimeUnit.Weeks);
        result.Should().Be(3m);
    }

    [Fact]
    public void Diff_Months_ThreeMonths_Approximate()
    {
        var from = new DateTime(2025, 1, 1);
        var to   = new DateTime(2025, 4, 1);
        decimal result = _vali.Diff(from, to, TimeUnit.Months);
        result.Should().BeApproximately(3m, 0.01m);
    }

    // ── Additional IsLeapYear tests ──────────────────────────────────────────

    [Fact]
    public void IsLeapYear_2000_ReturnsTrue()
    {
        _vali.IsLeapYear(2000).Should().BeTrue();
    }

    [Fact]
    public void IsLeapYear_1900_ReturnsFalse()
    {
        _vali.IsLeapYear(1900).Should().BeFalse();
    }

    // ── Additional QuarterOf tests ───────────────────────────────────────────

    [Fact]
    public void QuarterOf_January_Returns1()
    {
        var date = new DateTime(2025, 1, 15);
        _vali.QuarterOf(date).Should().Be(1);
    }

    [Fact]
    public void QuarterOf_April_Returns2()
    {
        var date = new DateTime(2025, 4, 10);
        _vali.QuarterOf(date).Should().Be(2);
    }

    [Fact]
    public void QuarterOf_October_Returns4()
    {
        var date = new DateTime(2025, 10, 5);
        _vali.QuarterOf(date).Should().Be(4);
    }

    // ── QuarterStart / QuarterEnd ────────────────────────────────────────────

    [Fact]
    public void QuarterStart_Q1_IsJanFirst()
    {
        var date = new DateTime(2025, 2, 14);
        DateTime result = _vali.QuarterStart(date);
        result.Should().Be(new DateTime(2025, 1, 1));
    }

    [Fact]
    public void QuarterEnd_Q4_IsDecember31()
    {
        var date = new DateTime(2025, 11, 10);
        DateTime result = _vali.QuarterEnd(date);
        result.Month.Should().Be(12);
        result.Day.Should().Be(31);
    }

    // ── IsInSameQuarter ──────────────────────────────────────────────────────

    [Fact]
    public void IsInSameQuarter_SameQuarterDates_ReturnsTrue()
    {
        var a = new DateTime(2025, 1, 10);
        var b = new DateTime(2025, 3, 25);
        _vali.IsInSameQuarter(a, b).Should().BeTrue();
    }

    [Fact]
    public void IsInSameQuarter_DifferentQuarterDates_ReturnsFalse()
    {
        var a = new DateTime(2025, 1, 10);
        var b = new DateTime(2025, 4, 1);
        _vali.IsInSameQuarter(a, b).Should().BeFalse();
    }

    // ── T-1: MonthsDiff wrong denominator bug fixes ──────────────────────────

    [Fact]
    public void MonthsDiff_WhenDayDiffNegative_FractionBelowOne()
    {
        // from = Jan 31, to = Mar 1: dayDiff = 1 - 31 = -20 (negative)
        // Whole months = 1 (Jan→Feb skipped back to Jan, so months-1=1)
        // Fraction should be computed against days-in-Feb (28), not days-in-Jan (31)
        // Result should be ~1.03, definitely < 2
        var from = new DateTime(2025, 1, 31);
        var to   = new DateTime(2025, 3, 1);
        decimal result = _vali.Diff(from, to, TimeUnit.Months);
        result.Should().BeGreaterThan(1m).And.BeLessThan(2m);
    }

    [Fact]
    public void MonthsDiff_CrossMonthBoundary_FractionIsCorrect()
    {
        // from = Jan 31 2025, to = Feb 28 2025: dayDiff = 28-31 = -3 (negative branch)
        // prevMonth = Jan, daysInPrevMonth = 31
        // effectiveFromDay = min(31,31) = 31; dayDiff = 28+(31-31) = 28; fraction = 28/31 ≈ 0.903
        // Result is positive and less than 1: fraction stays in [0,1) — the key invariant of the fix
        var from = new DateTime(2025, 1, 31);
        var to   = new DateTime(2025, 2, 28);
        decimal result = _vali.Diff(from, to, TimeUnit.Months);
        result.Should().BeGreaterThan(0m).And.BeLessThan(1m);
    }

    [Fact]
    public void MonthsDiff_FromLongMonthToShortMonth_NoOverflow()
    {
        // from = Jan 31, to = Apr 1: should be ~2.03, not > 3 or nonsensical
        var from = new DateTime(2025, 1, 31);
        var to   = new DateTime(2025, 4, 1);
        decimal result = _vali.Diff(from, to, TimeUnit.Months);
        result.Should().BeGreaterThan(2m).And.BeLessThan(3m);
    }

    // ── VD-1: TryPreviousQuarterStart ────────────────────────────────────────

    [Fact]
    public void TryPreviousQuarterStart_NormalDate_ReturnsTrueAndCorrectDate()
    {
        // May 15, 2025 is in Q2; previous quarter start is Jan 1, 2025
        bool ok = _vali.TryPreviousQuarterStart(new DateTime(2025, 5, 15), out var result);
        ok.Should().BeTrue();
        result.Should().Be(new DateTime(2025, 1, 1));
    }

    [Fact]
    public void TryPreviousQuarterStart_DateInQ1Year1_ReturnsFalse()
    {
        // Feb 1 of year 1 is in Q1 of year 1 — no previous quarter exists
        bool ok = _vali.TryPreviousQuarterStart(new DateTime(1, 2, 1), out var result);
        ok.Should().BeFalse();
        result.Should().Be(DateTime.MinValue);
    }

    [Fact]
    public void TryPreviousQuarterStart_DateInQ2_ReturnsPreviousQ1()
    {
        // Apr 1, 2025 is the first day of Q2; previous quarter start is Jan 1, 2025
        bool ok = _vali.TryPreviousQuarterStart(new DateTime(2025, 4, 1), out var result);
        ok.Should().BeTrue();
        result.Should().Be(new DateTime(2025, 1, 1));
    }
}
