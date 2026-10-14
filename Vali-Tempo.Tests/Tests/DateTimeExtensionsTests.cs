using FluentAssertions;
using Vali_Time.Enums;
using Vali_Time.Extensions;
using Xunit;

namespace Vali_Time.Tests.Tests;

/// <summary>
/// Covers <see cref="DateTimeExtensions"/>: week/month/year/quarter/day boundaries,
/// weekday/weekend checks, calendar info, and same-period comparisons.
/// </summary>
public class DateTimeExtensionsTests
{
    // ── StartOfWeek / EndOfWeek ────────────────────────────────────────────

    [Fact]
    public void StartOfWeek_Monday_Default_ReturnsMonday()
    {
        var wednesday = new DateTime(2025, 1, 15); // Wednesday
        wednesday.StartOfWeek().Should().Be(new DateTime(2025, 1, 13));
    }

    [Fact]
    public void StartOfWeek_Sunday_ReturnsSunday()
    {
        var wednesday = new DateTime(2025, 1, 15); // Wednesday
        wednesday.StartOfWeek(WeekStart.Sunday).Should().Be(new DateTime(2025, 1, 12));
    }

    [Fact]
    public void StartOfWeek_AtMinValue_DoesNotUnderflow()
    {
        Action act = () => DateTime.MinValue.StartOfWeek();
        act.Should().NotThrow();
        DateTime.MinValue.StartOfWeek().Should().Be(DateTime.MinValue.Date);
    }

    [Fact]
    public void EndOfWeek_Monday_ReturnsSundayAtLastTick()
    {
        var wednesday = new DateTime(2025, 1, 15);
        var result = wednesday.EndOfWeek();
        result.Date.Should().Be(new DateTime(2025, 1, 19));
        result.TimeOfDay.Should().Be(TimeSpan.FromTicks(TimeSpan.TicksPerDay - 1));
    }

    [Fact]
    public void EndOfWeek_NearMaxValue_DoesNotOverflow()
    {
        var date = DateTime.MaxValue.AddDays(-1);
        Action act = () => date.EndOfWeek();
        act.Should().NotThrow();
        date.EndOfWeek().Should().Be(DateTime.MaxValue);
    }

    // ── StartOfMonth / EndOfMonth ──────────────────────────────────────────

    [Fact]
    public void StartOfMonth_ReturnsFirstDay()
    {
        new DateTime(2025, 6, 17).StartOfMonth().Should().Be(new DateTime(2025, 6, 1));
    }

    [Fact]
    public void EndOfMonth_ReturnsLastDayAtLastTick()
    {
        var result = new DateTime(2025, 6, 17).EndOfMonth();
        result.Date.Should().Be(new DateTime(2025, 6, 30));
        result.TimeOfDay.Should().Be(TimeSpan.FromTicks(TimeSpan.TicksPerDay - 1));
    }

    [Fact]
    public void EndOfMonth_February_LeapYear_ReturnsFeb29()
    {
        new DateTime(2024, 2, 10).EndOfMonth().Date.Should().Be(new DateTime(2024, 2, 29));
    }

    [Fact]
    public void EndOfMonth_February_NonLeapYear_ReturnsFeb28()
    {
        new DateTime(2025, 2, 10).EndOfMonth().Date.Should().Be(new DateTime(2025, 2, 28));
    }

    // ── StartOfYear / EndOfYear ────────────────────────────────────────────

    [Fact]
    public void StartOfYear_ReturnsJan1()
    {
        new DateTime(2025, 8, 20).StartOfYear().Should().Be(new DateTime(2025, 1, 1));
    }

    [Fact]
    public void EndOfYear_ReturnsDec31AtLastTick()
    {
        var result = new DateTime(2025, 8, 20).EndOfYear();
        result.Date.Should().Be(new DateTime(2025, 12, 31));
        result.TimeOfDay.Should().Be(TimeSpan.FromTicks(TimeSpan.TicksPerDay - 1));
    }

    // ── StartOfQuarter / EndOfQuarter ──────────────────────────────────────

    [Theory]
    [InlineData(1, 1)]
    [InlineData(3, 1)]
    [InlineData(4, 4)]
    [InlineData(6, 4)]
    [InlineData(7, 7)]
    [InlineData(10, 10)]
    public void StartOfQuarter_AllMonths_ReturnsCorrectFirstMonth(int month, int expectedFirstMonth)
    {
        new DateTime(2025, month, 15).StartOfQuarter().Should().Be(new DateTime(2025, expectedFirstMonth, 1));
    }

    [Fact]
    public void EndOfQuarter_Q1_ReturnsMarch31()
    {
        new DateTime(2025, 2, 10).EndOfQuarter().Date.Should().Be(new DateTime(2025, 3, 31));
    }

    [Fact]
    public void EndOfQuarter_Q4_ReturnsDecember31()
    {
        new DateTime(2025, 11, 1).EndOfQuarter().Date.Should().Be(new DateTime(2025, 12, 31));
    }

    // ── IsWeekend / IsWeekday ──────────────────────────────────────────────

    [Theory]
    [InlineData(DayOfWeek.Saturday, true)]
    [InlineData(DayOfWeek.Sunday, true)]
    [InlineData(DayOfWeek.Monday, false)]
    [InlineData(DayOfWeek.Friday, false)]
    public void IsWeekend_VariousDays_ReturnsExpected(DayOfWeek day, bool expected)
    {
        var date = new DateTime(2025, 1, 12 + (int)day); // Jan 12, 2025 is Sunday
        date.IsWeekend().Should().Be(expected);
    }

    [Theory]
    [InlineData(DayOfWeek.Saturday, false)]
    [InlineData(DayOfWeek.Monday, true)]
    public void IsWeekday_VariousDays_ReturnsExpected(DayOfWeek day, bool expected)
    {
        var date = new DateTime(2025, 1, 12 + (int)day);
        date.IsWeekday().Should().Be(expected);
    }

    // ── WeekOfYear / QuarterOfYear ──────────────────────────────────────────

    [Fact]
    public void WeekOfYear_Monday_UsesIso8601()
    {
        new DateTime(2025, 1, 1).WeekOfYear().Should().Be(1);
    }

    [Fact]
    public void WeekOfYear_Sunday_UsesGregorianFirstDayRule()
    {
        int week = new DateTime(2025, 1, 1).WeekOfYear(WeekStart.Sunday);
        week.Should().Be(1);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(3, 1)]
    [InlineData(4, 2)]
    [InlineData(6, 2)]
    [InlineData(7, 3)]
    [InlineData(9, 3)]
    [InlineData(10, 4)]
    [InlineData(12, 4)]
    public void QuarterOfYear_AllMonths_ReturnsCorrectQuarter(int month, int expectedQuarter)
    {
        new DateTime(2025, month, 1).QuarterOfYear().Should().Be(expectedQuarter);
    }

    // ── Same-period comparisons ─────────────────────────────────────────────

    [Fact]
    public void IsSameDay_SameDate_ReturnsTrue()
    {
        new DateTime(2025, 1, 1, 10, 0, 0).IsSameDay(new DateTime(2025, 1, 1, 22, 0, 0)).Should().BeTrue();
    }

    [Fact]
    public void IsSameDay_DifferentDate_ReturnsFalse()
    {
        new DateTime(2025, 1, 1).IsSameDay(new DateTime(2025, 1, 2)).Should().BeFalse();
    }

    [Fact]
    public void IsSameMonth_SameMonth_ReturnsTrue()
    {
        new DateTime(2025, 3, 1).IsSameMonth(new DateTime(2025, 3, 31)).Should().BeTrue();
    }

    [Fact]
    public void IsSameMonth_DifferentMonth_ReturnsFalse()
    {
        new DateTime(2025, 3, 1).IsSameMonth(new DateTime(2025, 4, 1)).Should().BeFalse();
    }

    [Fact]
    public void IsSameYear_SameYear_ReturnsTrue()
    {
        new DateTime(2025, 1, 1).IsSameYear(new DateTime(2025, 12, 31)).Should().BeTrue();
    }

    [Fact]
    public void IsSameYear_DifferentYear_ReturnsFalse()
    {
        new DateTime(2024, 12, 31).IsSameYear(new DateTime(2025, 1, 1)).Should().BeFalse();
    }

    // ── StartOfDay / EndOfDay ───────────────────────────────────────────────

    [Fact]
    public void StartOfDay_ReturnsMidnight()
    {
        new DateTime(2025, 5, 5, 14, 30, 0).StartOfDay().Should().Be(new DateTime(2025, 5, 5));
    }

    [Fact]
    public void EndOfDay_ReturnsLastTickOfDay()
    {
        var result = new DateTime(2025, 5, 5, 14, 30, 0).EndOfDay();
        result.Date.Should().Be(new DateTime(2025, 5, 5));
        result.TimeOfDay.Should().Be(TimeSpan.FromTicks(TimeSpan.TicksPerDay - 1));
    }
}
