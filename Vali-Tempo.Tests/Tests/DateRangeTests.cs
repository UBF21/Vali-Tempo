using FluentAssertions;
using Vali_Range.Models;
using Vali_Time.Enums;
using Xunit;

namespace Vali_Time.Tests.Tests;

public class DateRangeTests
{
    // ── Duration — remaining TimeUnit branches ──────────────────────────────

    [Fact]
    public void Duration_Milliseconds_ReturnsCorrectValue()
    {
        var range = new DateRange(new DateTime(2025, 1, 1, 0, 0, 0), new DateTime(2025, 1, 1, 0, 0, 1));
        decimal ms = range.Duration(TimeUnit.Milliseconds);
        ms.Should().Be(1000m);
    }

    [Fact]
    public void Duration_Seconds_ReturnsCorrectValue()
    {
        var range = new DateRange(new DateTime(2025, 1, 1, 0, 0, 0), new DateTime(2025, 1, 1, 0, 1, 0));
        decimal seconds = range.Duration(TimeUnit.Seconds);
        seconds.Should().Be(60m);
    }

    [Fact]
    public void Duration_Minutes_ReturnsCorrectValue()
    {
        var range = new DateRange(new DateTime(2025, 1, 1, 0, 0, 0), new DateTime(2025, 1, 1, 1, 0, 0));
        decimal minutes = range.Duration(TimeUnit.Minutes);
        minutes.Should().Be(60m);
    }

    [Fact]
    public void Duration_Weeks_ReturnsCorrectValue()
    {
        var range = new DateRange(new DateTime(2025, 1, 1), new DateTime(2025, 1, 15));
        decimal weeks = range.Duration(TimeUnit.Weeks);
        weeks.Should().Be(2m);
    }

    [Fact]
    public void Duration_Months_ReturnsApproximateValue()
    {
        var range = new DateRange(new DateTime(2025, 1, 1), new DateTime(2025, 1, 31));
        decimal months = range.Duration(TimeUnit.Months);
        months.Should().BeGreaterThan(0m);
    }

    [Fact]
    public void Duration_Years_ReturnsApproximateValue()
    {
        var range = new DateRange(new DateTime(2023, 1, 1), new DateTime(2025, 1, 1));
        decimal years = range.Duration(TimeUnit.Years);
        years.Should().BeApproximately(2m, 0.1m);
    }

    [Fact]
    public void Duration_UnrecognisedUnit_ThrowsArgumentOutOfRangeException()
    {
        var range = new DateRange(new DateTime(2025, 1, 1), new DateTime(2025, 1, 2));
        Action act = () => range.Duration((TimeUnit)9999);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    // ── ToString ─────────────────────────────────────────────────────────────

    [Fact]
    public void ToString_ReturnsFormattedRange()
    {
        var range = new DateRange(new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        range.ToString().Should().Be("2025-01-01 → 2025-03-31");
    }
}
