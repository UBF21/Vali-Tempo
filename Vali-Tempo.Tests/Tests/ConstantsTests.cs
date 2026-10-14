using FluentAssertions;
using Vali_Time.Utils;
using Xunit;

namespace Vali_Time.Tests.Tests;

/// <summary>
/// Exercises every public constant and helper in <see cref="Constants"/> directly,
/// rather than only indirectly through <see cref="Vali_Time.Core.ValiTime"/>/<see cref="Vali_Time.Core.ValiDate"/>.
/// </summary>
public class ConstantsTests
{
    [Fact]
    public void SecondsInMinute_Is60()
    {
        Constants.SecondsInMinute.Should().Be(60m);
    }

    [Fact]
    public void MillisecondsInSecond_Is1000()
    {
        Constants.MillisecondsInSecond.Should().Be(1000m);
    }

    [Fact]
    public void SecondsInHour_Is3600()
    {
        Constants.SecondsInHour.Should().Be(3600m);
    }

    [Fact]
    public void SecondsInDay_Is86400()
    {
        Constants.SecondsInDay.Should().Be(86400m);
    }

    [Fact]
    public void SecondsInWeek_Is604800()
    {
        Constants.SecondsInWeek.Should().Be(604800m);
    }

    [Fact]
    public void SecondsInMonth_Is2629800()
    {
        Constants.SecondsInMonth.Should().Be(2629800m);
    }

    [Fact]
    public void SecondsInYear_Is31557600()
    {
        Constants.SecondsInYear.Should().Be(31557600m);
    }

    [Fact]
    public void DaysInWeek_Is7()
    {
        Constants.DaysInWeek.Should().Be(7m);
    }

    [Fact]
    public void MonthsInYear_Is12()
    {
        Constants.MonthsInYear.Should().Be(12m);
    }

    [Fact]
    public void DaysInYearAvg_Is365Point25()
    {
        Constants.DaysInYearAvg.Should().Be(365.25m);
    }

    [Fact]
    public void DaysInMonthAvg_Is30Point4375()
    {
        Constants.DaysInMonthAvg.Should().Be(30.4375m);
    }

    [Fact]
    public void PrefixMilliseconds_IsMs()
    {
        Constants.PrefixMilliseconds.Should().Be("ms");
    }

    [Fact]
    public void PrefixSeconds_IsS()
    {
        Constants.PrefixSeconds.Should().Be("s");
    }

    [Fact]
    public void PrefixMinutes_IsMin()
    {
        Constants.PrefixMinutes.Should().Be("min");
    }

    [Fact]
    public void PrefixHours_IsH()
    {
        Constants.PrefixHours.Should().Be("h");
    }

    [Fact]
    public void PrefixDays_IsD()
    {
        Constants.PrefixDays.Should().Be("d");
    }

    [Fact]
    public void PrefixWeeks_IsW()
    {
        Constants.PrefixWeeks.Should().Be("w");
    }

    [Fact]
    public void PrefixMonths_IsMo()
    {
        Constants.PrefixMonths.Should().Be("mo");
    }

    [Fact]
    public void PrefixYears_IsYr()
    {
        Constants.PrefixYears.Should().Be("yr");
    }

    // ── IsClampedMonthEnd ───────────────────────────────────────────────────

    [Fact]
    public void IsClampedMonthEnd_OriginalDay31_CandidateFebruary_ReturnsTrue()
    {
        // Jan 31 + 1 month clamps to Feb 28/29; original day (31) > days in Feb.
        var candidate = new DateTime(2025, 2, 28);
        Constants.IsClampedMonthEnd(31, candidate).Should().BeTrue();
    }

    [Fact]
    public void IsClampedMonthEnd_OriginalDayFitsInCandidateMonth_ReturnsFalse()
    {
        var candidate = new DateTime(2025, 3, 15);
        Constants.IsClampedMonthEnd(15, candidate).Should().BeFalse();
    }

    [Fact]
    public void IsClampedMonthEnd_OriginalDay30_CandidateLeapFebruary_ReturnsTrue()
    {
        // Day 30 does not fit Feb (29 days max in leap year) -> still clamped.
        var candidate = new DateTime(2024, 2, 29);
        Constants.IsClampedMonthEnd(30, candidate).Should().BeTrue();
    }

    [Fact]
    public void IsClampedMonthEnd_OriginalDayEqualsDaysInMonth_ReturnsFalse()
    {
        var candidate = new DateTime(2025, 4, 10); // April has 30 days
        Constants.IsClampedMonthEnd(30, candidate).Should().BeFalse();
    }
}
