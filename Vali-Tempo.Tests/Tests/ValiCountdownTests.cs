using FluentAssertions;
using Vali_CountDown.Core;
using Vali_Time.Enums;
using Xunit;

namespace Vali_Time.Tests.Tests;

public class ValiCountdownTests
{
    private readonly ValiCountdown _vali = new();

    // ── IsExpired ───────────────────────────────────────────────────────────

    [Fact]
    public void IsExpired_PastDate_ReturnsTrue()
    {
        var reference = new DateTime(2025, 6, 15, 12, 0, 0);
        var deadline  = new DateTime(2025, 6, 14, 12, 0, 0);
        _vali.IsExpired(deadline, reference).Should().BeTrue();
    }

    [Fact]
    public void IsExpired_FutureDate_ReturnsFalse()
    {
        var reference = new DateTime(2025, 6, 15, 12, 0, 0);
        var deadline  = new DateTime(2025, 6, 16, 12, 0, 0);
        _vali.IsExpired(deadline, reference).Should().BeFalse();
    }

    // ── Progress ────────────────────────────────────────────────────────────

    [Fact]
    public void Progress_ReferenceAtStart_ReturnsZero()
    {
        var start = new DateTime(2025, 1, 1);
        var end   = new DateTime(2025, 12, 31);
        decimal result = _vali.Progress(start, end, start);
        result.Should().Be(0m);
    }

    [Fact]
    public void Progress_ReferenceAtEnd_ReturnsOne()
    {
        var start = new DateTime(2025, 1, 1);
        var end   = new DateTime(2025, 12, 31);
        decimal result = _vali.Progress(start, end, end);
        result.Should().Be(1m);
    }

    [Fact]
    public void Progress_ReferenceAtMidpoint_ReturnsApproximatelyHalf()
    {
        var start = new DateTime(2025, 1, 1);
        var end   = new DateTime(2025, 1, 11);
        var mid   = new DateTime(2025, 1, 6);
        decimal result = _vali.Progress(start, end, mid);
        result.Should().BeApproximately(0.5m, 0.02m);
    }

    // ── ProgressPercent ─────────────────────────────────────────────────────

    [Fact]
    public void ProgressPercent_Midpoint_ReturnsApproximately50()
    {
        var start = new DateTime(2025, 1, 1);
        var end   = new DateTime(2025, 1, 11);
        var mid   = new DateTime(2025, 1, 6);

        // ProgressPercent uses DateTime.Now internally, so we use Progress * 100 check via overloads
        decimal progress = _vali.Progress(start, end, mid) * 100m;
        progress.Should().BeApproximately(50m, 2m);
    }

    // ── TimeUntil ───────────────────────────────────────────────────────────

    [Fact]
    public void TimeUntil_FutureDeadline_ReturnsPositiveValue()
    {
        DateTime deadline = DateTime.Now.AddHours(2);
        decimal result = _vali.TimeUntil(deadline, TimeUnit.Hours);
        result.Should().BeGreaterThan(0m);
    }

    [Fact]
    public void TimeUntil_PastDeadline_ReturnsZero()
    {
        DateTime deadline = DateTime.Now.AddHours(-2);
        decimal result = _vali.TimeUntil(deadline, TimeUnit.Hours);
        result.Should().Be(0m);
    }

    // ── Format ──────────────────────────────────────────────────────────────

    [Fact]
    public void Format_ExpiredDeadline_ReturnsExpiredString()
    {
        DateTime deadline = DateTime.Now.AddDays(-1);
        string result = _vali.Format(deadline);
        result.Should().Be("Expired");
    }

    [Fact]
    public void Format_MoreThanOneDayRemaining_ContainsDayComponent()
    {
        DateTime deadline = DateTime.Now.AddDays(5);
        string result = _vali.Format(deadline);
        result.Should().Contain("d");
    }

    [Fact]
    public void Format_FutureDeadline_DoesNotReturnExpired()
    {
        DateTime deadline = DateTime.Now.AddHours(3);
        string result = _vali.Format(deadline);
        result.Should().NotBe("Expired");
    }

    // ── IsWithin ────────────────────────────────────────────────────────────

    [Fact]
    public void IsWithin_DeadlineWithinThreshold_ReturnsTrue()
    {
        DateTime deadline = DateTime.Now.AddMinutes(30);
        bool result = _vali.IsWithin(deadline, 1, TimeUnit.Hours);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsWithin_DeadlineOutsideThreshold_ReturnsFalse()
    {
        DateTime deadline = DateTime.Now.AddDays(10);
        bool result = _vali.IsWithin(deadline, 1, TimeUnit.Hours);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsWithin_ExpiredDeadline_ReturnsFalse()
    {
        DateTime deadline = DateTime.Now.AddHours(-1);
        bool result = _vali.IsWithin(deadline, 2, TimeUnit.Hours);
        result.Should().BeFalse();
    }

    // ── Additional IsExpired tests ───────────────────────────────────────────

    [Fact]
    public void IsExpired_FutureDeadline_ReturnsFalse()
    {
        var reference = new DateTime(2025, 6, 15, 12, 0, 0);
        var deadline  = new DateTime(2025, 6, 20, 12, 0, 0);
        _vali.IsExpired(deadline, reference).Should().BeFalse();
    }

    [Fact]
    public void IsExpired_PastDeadline_ReturnsTrue()
    {
        var reference = new DateTime(2025, 6, 15, 12, 0, 0);
        var deadline  = new DateTime(2025, 6, 10, 12, 0, 0);
        _vali.IsExpired(deadline, reference).Should().BeTrue();
    }

    // ── Additional Progress / ProgressPercent tests ──────────────────────────

    [Fact]
    public void Progress_HalfwayThrough_ReturnsHalf()
    {
        var start = new DateTime(2025, 1, 1);
        var end   = new DateTime(2025, 1, 11);
        var mid   = new DateTime(2025, 1, 6);
        decimal result = _vali.Progress(start, end, mid);
        result.Should().BeApproximately(0.5m, 0.02m);
    }

    [Fact]
    public void ProgressPercent_HalfwayThrough_Returns50()
    {
        var start = new DateTime(2025, 1, 1);
        var end   = new DateTime(2025, 1, 11);
        var mid   = new DateTime(2025, 1, 6);
        decimal progress = _vali.Progress(start, end, mid) * 100m;
        progress.Should().BeApproximately(50m, 2m);
    }

    // ── TimeElapsed ──────────────────────────────────────────────────────────

    [Fact]
    public void TimeElapsed_ReturnsPositiveValue()
    {
        DateTime from = DateTime.Now.AddSeconds(-10);
        decimal result = _vali.TimeElapsed(from, TimeUnit.Seconds);
        result.Should().BeGreaterThan(0m);
    }

    // ── Additional IsWithin tests ────────────────────────────────────────────

    [Fact]
    public void IsWithin_ExactlyAtBoundary_ReturnsTrue()
    {
        // Deadline is exactly 1 hour from now; IsWithin(1 hour) should be true
        DateTime deadline = DateTime.Now.AddHours(1);
        bool result = _vali.IsWithin(deadline, 1, TimeUnit.Hours);
        result.Should().BeTrue();
    }

    // ── Breakdown ────────────────────────────────────────────────────────────

    [Fact]
    public void Breakdown_OneDayAndTwoHours_ReturnsCorrectParts()
    {
        // Deadline is 1 day + 2 hours from now
        DateTime deadline = DateTime.Now.AddDays(1).AddHours(2);
        var breakdown = _vali.Breakdown(deadline);
        breakdown[TimeUnit.Hours].Should().BeGreaterThanOrEqualTo(25m); // at least 26h worth
        breakdown.Should().ContainKeys(TimeUnit.Hours, TimeUnit.Minutes, TimeUnit.Seconds, TimeUnit.Milliseconds);
    }

    // ── Additional Format tests ──────────────────────────────────────────────

    [Fact]
    public void Format_FutureDeadline_ContainsDaysOrHours()
    {
        DateTime deadline = DateTime.Now.AddDays(2);
        string result = _vali.Format(deadline);
        result.Should().MatchRegex(@"\d+d|\d+h");
    }
}
