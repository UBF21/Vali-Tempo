using FluentAssertions;
using Vali_Time.Enums;
using Vali_Time.Extensions;
using Xunit;

namespace Vali_Time.Tests.Tests;

/// <summary>
/// Covers <see cref="TimeSpanExtensions"/>: unit conversion, human-readable formatting,
/// best-unit selection, and magnitude comparisons.
/// </summary>
public class TimeSpanExtensionsTests
{
    // ── To ──────────────────────────────────────────────────────────────────

    [Fact]
    public void To_Milliseconds_ReturnsExpected()
    {
        TimeSpan.FromSeconds(1).To(TimeUnit.Milliseconds).Should().Be(1000m);
    }

    [Fact]
    public void To_Seconds_ReturnsExpected()
    {
        TimeSpan.FromMinutes(1).To(TimeUnit.Seconds).Should().Be(60m);
    }

    [Fact]
    public void To_Minutes_ReturnsExpected()
    {
        TimeSpan.FromHours(1).To(TimeUnit.Minutes).Should().Be(60m);
    }

    [Fact]
    public void To_Hours_ReturnsExpected()
    {
        TimeSpan.FromDays(1).To(TimeUnit.Hours).Should().Be(24m);
    }

    [Fact]
    public void To_Days_ReturnsExpected()
    {
        TimeSpan.FromDays(7).To(TimeUnit.Days).Should().Be(7m);
    }

    [Fact]
    public void To_Weeks_ReturnsExpected()
    {
        TimeSpan.FromDays(14).To(TimeUnit.Weeks).Should().Be(2m);
    }

    [Fact]
    public void To_Months_ReturnsApproximate()
    {
        decimal result = TimeSpan.FromDays(30.4375).To(TimeUnit.Months);
        result.Should().BeApproximately(1m, 0.0001m);
    }

    [Fact]
    public void To_Years_ReturnsApproximate()
    {
        decimal result = TimeSpan.FromDays(365.25).To(TimeUnit.Years);
        result.Should().BeApproximately(1m, 0.0001m);
    }

    [Fact]
    public void To_InvalidUnit_ThrowsNotSupportedException()
    {
        Action act = () => TimeSpan.FromSeconds(1).To((TimeUnit)999);
        act.Should().Throw<NotSupportedException>();
    }

    // ── Format ──────────────────────────────────────────────────────────────

    [Fact]
    public void Format_Hours_ReturnsExpectedString()
    {
        TimeSpan.FromHours(1.5).Format(TimeUnit.Hours).Should().Be("1.50 h");
    }

    [Theory]
    [InlineData(TimeUnit.Milliseconds, "ms")]
    [InlineData(TimeUnit.Seconds, "s")]
    [InlineData(TimeUnit.Minutes, "min")]
    [InlineData(TimeUnit.Hours, "h")]
    [InlineData(TimeUnit.Days, "d")]
    [InlineData(TimeUnit.Weeks, "w")]
    [InlineData(TimeUnit.Months, "mo")]
    [InlineData(TimeUnit.Years, "y")]
    public void Format_AllUnits_ContainsCorrectAbbreviation(TimeUnit unit, string abbreviation)
    {
        string result = TimeSpan.FromDays(1).Format(unit);
        result.Should().EndWith(abbreviation);
    }

    [Fact]
    public void Format_CustomDecimalPlaces_RoundsCorrectly()
    {
        TimeSpan.FromMinutes(90).Format(TimeUnit.Hours, 0).Should().Be("2 h");
    }

    [Fact]
    public void Format_NegativeDecimalPlaces_ThrowsArgumentOutOfRangeException()
    {
        Action act = () => TimeSpan.FromHours(1).Format(TimeUnit.Hours, -1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Format_InvalidUnit_ThrowsNotSupportedException()
    {
        Action act = () => TimeSpan.FromHours(1).Format((TimeUnit)999);
        act.Should().Throw<NotSupportedException>();
    }

    // ── BestUnit ────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(0.0005, TimeUnit.Milliseconds)]
    [InlineData(0.5, TimeUnit.Milliseconds)]
    [InlineData(1, TimeUnit.Seconds)]
    [InlineData(90, TimeUnit.Minutes)]
    [InlineData(3700, TimeUnit.Hours)]
    [InlineData(90000, TimeUnit.Days)]
    [InlineData(700000, TimeUnit.Weeks)]
    public void BestUnit_VariousSpans_SelectsExpectedUnit(double seconds, TimeUnit expectedUnit)
    {
        var (_, unit) = TimeSpan.FromSeconds(seconds).BestUnit();
        unit.Should().Be(expectedUnit);
    }

    [Fact]
    public void BestUnit_NegativeSpan_UsesAbsoluteValueForThreshold()
    {
        var (value, unit) = TimeSpan.FromDays(-2).BestUnit();
        unit.Should().Be(TimeUnit.Days);
        value.Should().Be(-2m);
    }

    // ── IsGreaterThan / IsLessThan ──────────────────────────────────────────

    [Fact]
    public void IsGreaterThan_WhenGreater_ReturnsTrue()
    {
        TimeSpan.FromHours(2).IsGreaterThan(1, TimeUnit.Hours).Should().BeTrue();
    }

    [Fact]
    public void IsGreaterThan_WhenNotGreater_ReturnsFalse()
    {
        TimeSpan.FromHours(1).IsGreaterThan(1, TimeUnit.Hours).Should().BeFalse();
    }

    [Fact]
    public void IsLessThan_WhenLess_ReturnsTrue()
    {
        TimeSpan.FromMinutes(30).IsLessThan(1, TimeUnit.Hours).Should().BeTrue();
    }

    [Fact]
    public void IsLessThan_WhenNotLess_ReturnsFalse()
    {
        TimeSpan.FromHours(1).IsLessThan(1, TimeUnit.Hours).Should().BeFalse();
    }
}
