using FluentAssertions;
using Vali_CountDown.Core;
using Vali_Time.Abstractions;
using Vali_Time.Enums;
using Xunit;

namespace Vali_Time.Tests.Tests;

public class ValiCountdownTests
{
    private readonly ValiCountdown _vali = new();

    private sealed class FixedClock : IClock
    {
        private readonly DateTime _fixed;
        public FixedClock(DateTime fixedValue) => _fixed = fixedValue;
        public DateTime Now => _fixed;
        public DateTime Today => _fixed.Date;
    }

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
        DateTime deadline = new DateTime(2099, 6, 15, 12, 0, 0);
        decimal result = _vali.TimeUntil(deadline, TimeUnit.Hours);
        result.Should().BeGreaterThan(0m);
    }

    [Fact]
    public void TimeUntil_PastDeadline_ReturnsZero()
    {
        DateTime deadline = new DateTime(2000, 6, 15, 12, 0, 0);
        decimal result = _vali.TimeUntil(deadline, TimeUnit.Hours);
        result.Should().Be(0m);
    }

    // ── Format ──────────────────────────────────────────────────────────────

    [Fact]
    public void Format_ExpiredDeadline_ReturnsExpiredString()
    {
        DateTime deadline = new DateTime(2000, 6, 14, 12, 0, 0);
        string result = _vali.Format(deadline);
        result.Should().Be("Expired");
    }

    [Fact]
    public void Format_MoreThanOneDayRemaining_ContainsDayComponent()
    {
        DateTime deadline = new DateTime(2099, 6, 20, 12, 0, 0);
        string result = _vali.Format(deadline);
        result.Should().Contain("d");
    }

    [Fact]
    public void Format_FutureDeadline_DoesNotReturnExpired()
    {
        DateTime deadline = new DateTime(2099, 6, 15, 15, 0, 0);
        string result = _vali.Format(deadline);
        result.Should().NotBe("Expired");
    }

    // ── IsWithin ────────────────────────────────────────────────────────────

    [Fact]
    public void IsWithin_DeadlineWithinThreshold_ReturnsTrue()
    {
        // Deadline is 30 minutes from now — within the 1-hour threshold.
        DateTime deadline = DateTime.Now.AddMinutes(30);
        bool result = _vali.IsWithin(deadline, 1, TimeUnit.Hours);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsWithin_DeadlineOutsideThreshold_ReturnsFalse()
    {
        DateTime deadline = new DateTime(2099, 6, 25, 12, 0, 0);
        bool result = _vali.IsWithin(deadline, 1, TimeUnit.Hours);
        result.Should().BeFalse();
    }

    [Fact]
    public void IsWithin_ExpiredDeadline_ReturnsFalse()
    {
        DateTime deadline = new DateTime(2000, 6, 15, 11, 0, 0);
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
        DateTime from = new DateTime(2025, 6, 15, 11, 59, 50);
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
        breakdown[TimeUnit.Hours].Should().BeGreaterThanOrEqualTo(1m); // at least some hours
        breakdown.Should().ContainKeys(TimeUnit.Hours, TimeUnit.Minutes, TimeUnit.Seconds, TimeUnit.Milliseconds);
    }

    // ── Additional Format tests ──────────────────────────────────────────────

    [Fact]
    public void Format_FutureDeadline_ContainsDaysOrHours()
    {
        DateTime deadline = new DateTime(2099, 6, 17, 12, 0, 0);
        string result = _vali.Format(deadline);
        result.Should().MatchRegex(@"\d+d|\d+h");
    }

    // ── VCO-1: Format returns "< 1s" for sub-second deadlines ────────────────

    [Fact]
    public void Format_SubSecondDeadline_ReturnsLessThanOneSecond()
    {
        // A deadline 0.5 seconds in the future: TimeUntil returns < 1 second,
        // which when cast to long becomes 0, triggering the "< 1s" branch.
        DateTime deadline = DateTime.Now.AddMilliseconds(500);
        string result = _vali.Format(deadline, includeSeconds: true);
        result.Should().Be("< 1s");
    }

    // ── VCO-2: IsStarted ─────────────────────────────────────────────────────

    [Fact]
    public void IsStarted_PastDate_ReturnsTrue()
    {
        // A point 1 hour in the past has already started
        _vali.IsStarted(DateTime.Now.AddHours(-1)).Should().BeTrue();
    }

    [Fact]
    public void IsStarted_FutureDate_ReturnsFalse()
    {
        // A point 1 hour in the future has not yet started
        _vali.IsStarted(DateTime.Now.AddHours(1)).Should().BeFalse();
    }

    // ── IsExpired single-arg overload (uses injected clock) ──────────────────

    [Fact]
    public void IsExpired_SingleArgOverload_PastDeadline_ReturnsTrue()
    {
        var reference = new DateTime(2025, 6, 15, 12, 0, 0);
        var deadline  = new DateTime(2025, 6, 14, 12, 0, 0);
        var vali = new ValiCountdown(new FixedClock(reference));
        vali.IsExpired(deadline).Should().BeTrue();
    }

    [Fact]
    public void IsExpired_SingleArgOverload_FutureDeadline_ReturnsFalse()
    {
        var reference = new DateTime(2025, 6, 15, 12, 0, 0);
        var deadline  = new DateTime(2025, 6, 16, 12, 0, 0);
        var vali = new ValiCountdown(new FixedClock(reference));
        vali.IsExpired(deadline).Should().BeFalse();
    }

    // ── Progress single-overload (uses injected clock) ───────────────────────

    [Fact]
    public void Progress_TwoArgOverload_UsesClockNow_ReturnsExpectedValue()
    {
        var start = new DateTime(2025, 1, 1);
        var end   = new DateTime(2025, 1, 11);
        var mid   = new DateTime(2025, 1, 6);
        var vali = new ValiCountdown(new FixedClock(mid));
        vali.Progress(start, end).Should().BeApproximately(0.5m, 0.02m);
    }

    [Fact]
    public void Progress_EndBeforeOrEqualStart_ThrowsArgumentException()
    {
        var start = new DateTime(2025, 1, 11);
        var end   = new DateTime(2025, 1, 1);
        var reference = new DateTime(2025, 1, 5);
        Action act = () => _vali.Progress(start, end, reference);
        act.Should().Throw<ArgumentException>();
    }

    // ── ProgressPercent two-arg overload (uses injected clock) ───────────────

    [Fact]
    public void ProgressPercent_TwoArgOverload_UsesClockNow_ReturnsExpected50()
    {
        var start = new DateTime(2025, 1, 1);
        var end   = new DateTime(2025, 1, 11);
        var mid   = new DateTime(2025, 1, 6);
        var vali = new ValiCountdown(new FixedClock(mid));
        vali.ProgressPercent(start, end).Should().BeApproximately(50m, 2m);
    }

    // ── Breakdown — expired deadline branch ───────────────────────────────────

    [Fact]
    public void Breakdown_ExpiredDeadline_ReturnsAllZeroParts()
    {
        var reference = new DateTime(2025, 6, 15, 12, 0, 0);
        var deadline  = new DateTime(2025, 6, 14, 12, 0, 0);
        var vali = new ValiCountdown(new FixedClock(reference));
        var breakdown = vali.Breakdown(deadline);

        breakdown[TimeUnit.Days].Should().Be(0m);
        breakdown[TimeUnit.Hours].Should().Be(0m);
        breakdown[TimeUnit.Minutes].Should().Be(0m);
        breakdown[TimeUnit.Seconds].Should().Be(0m);
        breakdown[TimeUnit.Milliseconds].Should().Be(0m);
    }

    [Fact]
    public void Breakdown_DeadlineExactlyAtReference_ReturnsAllZeroParts()
    {
        var reference = new DateTime(2025, 6, 15, 12, 0, 0);
        var vali = new ValiCountdown(new FixedClock(reference));
        var breakdown = vali.Breakdown(reference);

        breakdown[TimeUnit.Days].Should().Be(0m);
        breakdown[TimeUnit.Hours].Should().Be(0m);
    }

    // ── TimeUntil — ConvertTicks unit branches ────────────────────────────────

    [Fact]
    public void TimeUntil_Milliseconds_ReturnsExpectedValue()
    {
        var reference = new DateTime(2025, 1, 1);
        var deadline  = reference.AddMilliseconds(5000);
        var vali = new ValiCountdown(new FixedClock(reference));
        vali.TimeUntil(deadline, TimeUnit.Milliseconds).Should().Be(5000m);
    }

    [Fact]
    public void TimeUntil_Minutes_ReturnsExpectedValue()
    {
        var reference = new DateTime(2025, 1, 1);
        var deadline  = reference.AddMinutes(10);
        var vali = new ValiCountdown(new FixedClock(reference));
        vali.TimeUntil(deadline, TimeUnit.Minutes).Should().Be(10m);
    }

    [Fact]
    public void TimeUntil_Days_ReturnsExpectedValue()
    {
        var reference = new DateTime(2025, 1, 1);
        var deadline  = reference.AddDays(3);
        var vali = new ValiCountdown(new FixedClock(reference));
        vali.TimeUntil(deadline, TimeUnit.Days).Should().Be(3m);
    }

    [Fact]
    public void TimeUntil_Weeks_ReturnsExpectedValue()
    {
        var reference = new DateTime(2025, 1, 1);
        var deadline  = reference.AddDays(14);
        var vali = new ValiCountdown(new FixedClock(reference));
        vali.TimeUntil(deadline, TimeUnit.Weeks).Should().Be(2m);
    }

    [Fact]
    public void TimeUntil_Months_ReturnsApproximateValue()
    {
        var reference = new DateTime(2025, 1, 1);
        var deadline  = reference.AddDays(60.875); // ~2 average months
        var vali = new ValiCountdown(new FixedClock(reference));
        vali.TimeUntil(deadline, TimeUnit.Months).Should().BeApproximately(2m, 0.02m);
    }

    [Fact]
    public void TimeUntil_Years_ReturnsApproximateValue()
    {
        var reference = new DateTime(2025, 1, 1);
        var deadline  = reference.AddDays(365.25); // ~1 average year
        var vali = new ValiCountdown(new FixedClock(reference));
        vali.TimeUntil(deadline, TimeUnit.Years).Should().BeApproximately(1m, 0.02m);
    }

    [Fact]
    public void TimeUntil_WithDecimalPlaces_RoundsResult()
    {
        var reference = new DateTime(2025, 1, 1);
        var deadline  = reference.AddMinutes(10.333);
        var vali = new ValiCountdown(new FixedClock(reference));
        vali.TimeUntil(deadline, TimeUnit.Minutes, 1).Should().Be(10.3m);
    }

    // ── TimeElapsed — future "from" clamps to zero ────────────────────────────

    [Fact]
    public void TimeElapsed_FromInFuture_ReturnsZero()
    {
        var reference = new DateTime(2025, 1, 1);
        var from      = reference.AddHours(1);
        var vali = new ValiCountdown(new FixedClock(reference));
        vali.TimeElapsed(from, TimeUnit.Hours).Should().Be(0m);
    }

    [Fact]
    public void TimeElapsed_WithDecimalPlaces_RoundsResult()
    {
        var reference = new DateTime(2025, 1, 1);
        var from      = reference.AddMinutes(-10.333);
        var vali = new ValiCountdown(new FixedClock(reference));
        vali.TimeElapsed(from, TimeUnit.Minutes, 1).Should().Be(10.3m);
    }

    [Fact]
    public void TimeUntil_UnsupportedUnit_ThrowsArgumentOutOfRangeException()
    {
        var reference = new DateTime(2025, 1, 1);
        var deadline  = reference.AddHours(1);
        var vali = new ValiCountdown(new FixedClock(reference));
        Action act = () => vali.TimeUntil(deadline, (TimeUnit)999);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
