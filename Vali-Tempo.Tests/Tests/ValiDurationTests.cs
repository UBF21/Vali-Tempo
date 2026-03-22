using FluentAssertions;
using Vali_Duration.Models;
using Vali_Time.Enums;
using Xunit;

namespace Vali_Time.Tests.Tests;

public class ValiDurationTests
{
    // ── Factory: FromHours ───────────────────────────────────────────────────

    [Fact]
    public void FromHours_1_5_TotalMinutes_Is90()
    {
        ValiDuration duration = ValiDuration.FromHours(1.5m);
        duration.TotalMinutes.Should().Be(90m);
    }

    // ── Factory: FromMinutes ─────────────────────────────────────────────────

    [Fact]
    public void FromMinutes_90_TotalHours_Is1_5()
    {
        ValiDuration duration = ValiDuration.FromMinutes(90m);
        duration.TotalHours.Should().Be(1.5m);
    }

    // ── Arithmetic: Addition ─────────────────────────────────────────────────

    [Fact]
    public void Addition_TwoHalfHours_EqualOneHour()
    {
        ValiDuration half  = ValiDuration.FromMinutes(30m);
        ValiDuration result = half + half;
        result.TotalHours.Should().Be(1m);
    }

    // ── Arithmetic: Subtraction ──────────────────────────────────────────────

    [Fact]
    public void Subtraction_OneHourMinusThirtyMinutes_Is30Minutes()
    {
        ValiDuration oneHour    = ValiDuration.FromHours(1m);
        ValiDuration thirtyMin  = ValiDuration.FromMinutes(30m);
        ValiDuration result     = oneHour - thirtyMin;
        result.TotalMinutes.Should().Be(30m);
    }

    // ── Arithmetic: Multiplication ───────────────────────────────────────────

    [Fact]
    public void Multiplication_ByTwo_DoublesValue()
    {
        ValiDuration oneHour = ValiDuration.FromHours(1m);
        ValiDuration result  = oneHour * 2m;
        result.TotalHours.Should().Be(2m);
    }

    // ── Arithmetic: Division ─────────────────────────────────────────────────

    [Fact]
    public void Division_ByTwo_HalvesValue()
    {
        ValiDuration oneHour = ValiDuration.FromHours(1m);
        ValiDuration result  = oneHour / 2m;
        result.TotalMinutes.Should().Be(30m);
    }

    // ── Comparison ───────────────────────────────────────────────────────────

    [Fact]
    public void Comparison_LargerIsGreater()
    {
        ValiDuration two  = ValiDuration.FromHours(2m);
        ValiDuration one  = ValiDuration.FromHours(1m);
        (two > one).Should().BeTrue();
        (one < two).Should().BeTrue();
    }

    // ── Equality ─────────────────────────────────────────────────────────────

    [Fact]
    public void Equality_SameDuration_AreEqual()
    {
        ValiDuration a = ValiDuration.FromMinutes(90m);
        ValiDuration b = ValiDuration.FromHours(1.5m);
        (a == b).Should().BeTrue();
        a.Equals(b).Should().BeTrue();
    }

    // ── TimeSpan interop: implicit to TimeSpan ───────────────────────────────

    [Fact]
    public void ImplicitToTimeSpan_IsCorrect()
    {
        ValiDuration duration = ValiDuration.FromHours(2m);
        TimeSpan ts = duration; // implicit operator
        ts.TotalHours.Should().BeApproximately(2.0, 0.0001);
    }

    // ── TimeSpan interop: implicit from TimeSpan ─────────────────────────────

    [Fact]
    public void ImplicitFromTimeSpan_IsCorrect()
    {
        TimeSpan ts = TimeSpan.FromHours(3);
        ValiDuration duration = ts; // implicit operator
        duration.TotalHours.Should().BeApproximately(3m, 0.0001m);
    }

    // ── Decimal precision ────────────────────────────────────────────────────

    [Fact]
    public void DecimalPrecision_NoFloatingPointDrift()
    {
        // With double: 1.1 + 1.2 != 2.3 due to floating-point drift.
        // With decimal-backed ValiDuration: 1.1h + 1.2h = exactly 2.3h.
        ValiDuration a = ValiDuration.FromHours(1.1m);
        ValiDuration b = ValiDuration.FromHours(1.2m);
        ValiDuration result = a + b;
        result.TotalHours.Should().Be(2.3m);
    }

    // ── Format ───────────────────────────────────────────────────────────────

    [Fact]
    public void Format_OneHourThirtyMin_IsCorrectString()
    {
        ValiDuration duration = ValiDuration.FromMinutes(90m);
        string result = duration.Format();
        result.Should().Be("1.50 h");
    }

    // ── As(TimeUnit) ─────────────────────────────────────────────────────────

    [Fact]
    public void As_TimeUnit_Weeks_IsCorrect()
    {
        ValiDuration duration = ValiDuration.FromDays(14m);
        duration.As(TimeUnit.Weeks).Should().Be(2m);
    }

    // ── Factory: FromMilliseconds ────────────────────────────────────────────

    [Fact]
    public void FromMilliseconds_1000_Is1Second()
    {
        ValiDuration duration = ValiDuration.FromMilliseconds(1000m);
        duration.TotalSeconds.Should().Be(1m);
    }

    // ── Factory: FromWeeks ───────────────────────────────────────────────────

    [Fact]
    public void FromWeeks_2_Is14Days()
    {
        ValiDuration duration = ValiDuration.FromWeeks(2m);
        duration.TotalDays.Should().Be(14m);
    }
}
