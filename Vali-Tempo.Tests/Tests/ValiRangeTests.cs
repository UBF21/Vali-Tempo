using FluentAssertions;
using Vali_Range.Core;
using Vali_Range.Models;
using Vali_Time.Abstractions;
using Vali_Time.Enums;
using Xunit;

namespace Vali_Time.Tests.Tests;

public class ValiRangeTests
{
    private readonly ValiRange _vali = new();

    private sealed class FixedClock : IClock
    {
        private readonly DateTime _fixed;
        public FixedClock(DateTime fixedValue) => _fixed = fixedValue;
        public DateTime Now => _fixed;
        public DateTime Today => _fixed.Date;
    }

    // ── Create ──────────────────────────────────────────────────────────────

    [Fact]
    public void Create_ValidDates_IsValidTrue()
    {
        var start = new DateTime(2025, 1, 1);
        var end   = new DateTime(2025, 12, 31);
        DateRange range = _vali.Create(start, end);
        range.IsValid.Should().BeTrue();
        range.Start.Should().Be(start);
        range.End.Should().Be(end);
    }

    [Fact]
    public void Create_SameDate_IsValidTrue()
    {
        var date = new DateTime(2025, 6, 15);
        DateRange range = _vali.Create(date, date);
        range.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Create_StartAfterEnd_ThrowsArgumentException()
    {
        var start = new DateTime(2025, 12, 31);
        var end   = new DateTime(2025, 1, 1);
        Action act = () => _vali.Create(start, end);
        act.Should().Throw<ArgumentException>();
    }

    // ── Contains ────────────────────────────────────────────────────────────

    [Fact]
    public void Contains_DateInsideRange_ReturnsTrue()
    {
        DateRange range = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 12, 31));
        _vali.Contains(range, new DateTime(2025, 6, 15)).Should().BeTrue();
    }

    [Fact]
    public void Contains_DateOutsideRange_ReturnsFalse()
    {
        DateRange range = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 6, 30));
        _vali.Contains(range, new DateTime(2025, 7, 1)).Should().BeFalse();
    }

    [Fact]
    public void Contains_DateOnStartBoundary_ReturnsTrue()
    {
        var start = new DateTime(2025, 1, 1);
        DateRange range = _vali.Create(start, new DateTime(2025, 12, 31));
        _vali.Contains(range, start).Should().BeTrue();
    }

    [Fact]
    public void Contains_DateOnEndBoundary_ReturnsTrue()
    {
        var end = new DateTime(2025, 12, 31);
        DateRange range = _vali.Create(new DateTime(2025, 1, 1), end);
        _vali.Contains(range, end).Should().BeTrue();
    }

    // ── Overlaps ────────────────────────────────────────────────────────────

    [Fact]
    public void Overlaps_OverlappingRanges_ReturnsTrue()
    {
        DateRange a = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 6, 30));
        DateRange b = _vali.Create(new DateTime(2025, 4, 1), new DateTime(2025, 12, 31));
        _vali.Overlaps(a, b).Should().BeTrue();
    }

    [Fact]
    public void Overlaps_DisjointRanges_ReturnsFalse()
    {
        DateRange a = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        DateRange b = _vali.Create(new DateTime(2025, 7, 1), new DateTime(2025, 12, 31));
        _vali.Overlaps(a, b).Should().BeFalse();
    }

    [Fact]
    public void Overlaps_AdjacentRanges_ReturnsFalse()
    {
        // Adjacent: end of a == day before start of b — they don't share any instant
        DateRange a = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        DateRange b = _vali.Create(new DateTime(2025, 4, 1), new DateTime(2025, 6, 30));
        // The implementation uses start <= end && start <= end (inclusive), so sharing a
        // single point counts as overlap. Here end of a != start of b so no overlap.
        _vali.Overlaps(a, b).Should().BeFalse();
    }

    // ── Intersection ────────────────────────────────────────────────────────

    [Fact]
    public void Intersection_OverlappingRanges_ReturnsCorrectRange()
    {
        DateRange a = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 6, 30));
        DateRange b = _vali.Create(new DateTime(2025, 4, 1), new DateTime(2025, 12, 31));

        DateRange? result = _vali.Intersection(a, b);

        result.Should().NotBeNull();
        result!.Value.Start.Should().Be(new DateTime(2025, 4, 1));
        result.Value.End.Should().Be(new DateTime(2025, 6, 30));
    }

    [Fact]
    public void Intersection_NonOverlappingRanges_ReturnsNull()
    {
        DateRange a = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        DateRange b = _vali.Create(new DateTime(2025, 7, 1), new DateTime(2025, 12, 31));

        DateRange? result = _vali.Intersection(a, b);

        result.Should().BeNull();
    }

    // ── Subtract ────────────────────────────────────────────────────────────

    [Fact]
    public void Subtract_NoOverlap_ReturnsOriginalRangeUnchanged()
    {
        DateRange a = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 1, 9));
        DateRange b = _vali.Create(new DateTime(2025, 2, 1), new DateTime(2025, 2, 10));

        var result = _vali.Subtract(a, b).ToList();

        result.Should().ContainSingle();
        result[0].Start.Should().Be(a.Start);
        result[0].End.Should().Be(a.End);
    }

    [Fact]
    public void Subtract_BContainsA_ReturnsEmpty()
    {
        DateRange a = _vali.Create(new DateTime(2025, 2, 1), new DateTime(2025, 2, 10));
        DateRange b = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 3, 1));

        var result = _vali.Subtract(a, b).ToList();

        result.Should().BeEmpty();
    }

    [Fact]
    public void Subtract_OverlapAtStart_ReturnsRightRemainder()
    {
        // b covers the start of a; the right remainder of a survives.
        DateRange a = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 1, 31));
        DateRange b = _vali.Create(new DateTime(2024, 12, 15), new DateTime(2025, 1, 10));

        var result = _vali.Subtract(a, b).ToList();

        result.Should().ContainSingle();
        result[0].Start.Should().Be(new DateTime(2025, 1, 10).AddTicks(1));
        result[0].End.Should().Be(a.End);
    }

    [Fact]
    public void Subtract_OverlapAtEnd_ReturnsLeftRemainder()
    {
        // b covers the end of a; the left remainder of a survives.
        DateRange a = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 1, 31));
        DateRange b = _vali.Create(new DateTime(2025, 1, 20), new DateTime(2025, 2, 15));

        var result = _vali.Subtract(a, b).ToList();

        result.Should().ContainSingle();
        result[0].Start.Should().Be(a.Start);
        result[0].End.Should().Be(new DateTime(2025, 1, 20).AddTicks(-1));
    }

    [Fact]
    public void Subtract_BInsideA_ReturnsTwoSubRanges()
    {
        DateRange a = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 1, 31));
        DateRange b = _vali.Create(new DateTime(2025, 1, 10), new DateTime(2025, 1, 20));

        var result = _vali.Subtract(a, b).ToList();

        result.Should().HaveCount(2);
        result[0].Start.Should().Be(a.Start);
        result[0].End.Should().Be(new DateTime(2025, 1, 10).AddTicks(-1));
        result[1].Start.Should().Be(new DateTime(2025, 1, 20).AddTicks(1));
        result[1].End.Should().Be(a.End);
    }

    [Fact]
    public void Subtract_IdenticalRanges_ReturnsEmpty()
    {
        DateRange a = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 1, 31));
        DateRange b = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 1, 31));

        var result = _vali.Subtract(a, b).ToList();

        result.Should().BeEmpty();
    }

    [Fact]
    public void Subtract_SinglePointOverlapAtBoundary_TrimsThatInstant()
    {
        // Inclusive boundaries: a.End == b.Start is a one-instant overlap, not "no overlap".
        DateRange a = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 1, 10));
        DateRange b = _vali.Create(new DateTime(2025, 1, 10), new DateTime(2025, 1, 15));

        var result = _vali.Subtract(a, b).ToList();

        result.Should().ContainSingle();
        result[0].Start.Should().Be(a.Start);
        result[0].End.Should().Be(new DateTime(2025, 1, 10).AddTicks(-1));
    }

    [Fact]
    public void Subtract_AdjacentNoOverlap_ReturnsOriginalRangeUnchanged()
    {
        // Adjacent but disjoint: no shared instant, unlike the boundary-overlap case above.
        DateRange a = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 1, 9));
        DateRange b = _vali.Create(new DateTime(2025, 1, 10), new DateTime(2025, 1, 15));

        var result = _vali.Subtract(a, b).ToList();

        result.Should().ContainSingle();
        result[0].Start.Should().Be(a.Start);
        result[0].End.Should().Be(a.End);
    }

    [Fact]
    public void Subtract_IntersectionTouchesMinOrMaxValue_DoesNotThrow()
    {
        DateRange a = _vali.Create(DateTime.MinValue, new DateTime(2025, 1, 31));
        DateRange b = _vali.Create(DateTime.MinValue, new DateTime(2025, 1, 10));

        var act = () => _vali.Subtract(a, b).ToList();

        act.Should().NotThrow();
        var result = act();
        result.Should().ContainSingle();
        result[0].Start.Should().Be(new DateTime(2025, 1, 10).AddTicks(1));
        result[0].End.Should().Be(a.End);
    }

    // ── Union ───────────────────────────────────────────────────────────────

    [Fact]
    public void Union_TwoRanges_CoversBoth()
    {
        DateRange a = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 6, 30));
        DateRange b = _vali.Create(new DateTime(2025, 4, 1), new DateTime(2025, 12, 31));

        DateRange result = _vali.Union(a, b);

        result.Start.Should().Be(new DateTime(2025, 1, 1));
        result.End.Should().Be(new DateTime(2025, 12, 31));
    }

    // ── EachDay ─────────────────────────────────────────────────────────────

    [Fact]
    public void EachDay_ThreeDayRange_ReturnsThreeDays()
    {
        DateRange range = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 1, 3));
        var days = _vali.EachDay(range).ToList();
        days.Should().HaveCount(3);
    }

    [Fact]
    public void EachDay_SingleDay_ReturnsOneDays()
    {
        var date = new DateTime(2025, 6, 15);
        DateRange range = _vali.Create(date, date);
        var days = _vali.EachDay(range).ToList();
        days.Should().HaveCount(1);
    }

    // ── SplitByMonth ────────────────────────────────────────────────────────

    [Fact]
    public void SplitByMonth_ThreeMonthRange_ReturnsThreeSubRanges()
    {
        DateRange range = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var months = _vali.SplitByMonth(range).ToList();
        months.Should().HaveCount(3);
    }

    [Fact]
    public void SplitByMonth_SingleMonth_ReturnsOneSubRange()
    {
        DateRange range = _vali.Create(new DateTime(2025, 6, 1), new DateTime(2025, 6, 30));
        var months = _vali.SplitByMonth(range).ToList();
        months.Should().HaveCount(1);
    }

    // ── DateRange.Duration ──────────────────────────────────────────────────

    [Fact]
    public void Duration_Days_ReturnsCorrectValue()
    {
        var range = new DateRange(new DateTime(2025, 1, 1), new DateTime(2025, 1, 11));
        decimal days = range.Duration(TimeUnit.Days);
        days.Should().Be(10m);
    }

    [Fact]
    public void Duration_Hours_ReturnsCorrectValue()
    {
        var range = new DateRange(new DateTime(2025, 1, 1, 0, 0, 0), new DateTime(2025, 1, 1, 12, 0, 0));
        decimal hours = range.Duration(TimeUnit.Hours);
        hours.Should().Be(12m);
    }

    // ── ThisMonth ───────────────────────────────────────────────────────────

    [Fact]
    public void ThisMonth_IsValidAndCoherent()
    {
        DateRange range = _vali.ThisMonth();
        range.IsValid.Should().BeTrue();
        range.Start.Day.Should().Be(1);
        range.End.Month.Should().Be(range.Start.Month);
    }

    // ── ThisWeek ────────────────────────────────────────────────────────────

    [Fact]
    public void ThisWeek_IsValidAndSpansSevenDays()
    {
        DateRange range = _vali.ThisWeek();
        range.IsValid.Should().BeTrue();
        decimal days = range.Duration(TimeUnit.Days);
        days.Should().Be(6m); // 7 days inclusive = 6 days span
    }

    [Theory]
    [InlineData(WeekStart.Monday)]
    [InlineData(WeekStart.Sunday)]
    public void ThisWeek_WhenTodayIsNearDateTimeMinValue_DoesNotThrow(WeekStart weekStart)
    {
        // Regression: StartOfWeek used to lack the anti-underflow guard that ValiDate already
        // had, so a "today" close to DateTime.MinValue could throw ArgumentOutOfRangeException.
        var vali = new ValiRange(new FixedClock(DateTime.MinValue.AddDays(2)));

        Action act = () => vali.ThisWeek(weekStart);

        act.Should().NotThrow();
        DateRange range = vali.ThisWeek(weekStart);
        range.Start.Should().BeOnOrAfter(DateTime.MinValue);
        range.IsValid.Should().BeTrue();
    }

    // ── LastUnits ───────────────────────────────────────────────────────────

    [Fact]
    public void LastUnits_30Days_IsValidAndEndIsNow()
    {
        DateRange range = _vali.LastUnits(30, TimeUnit.Days);
        range.IsValid.Should().BeTrue();
        range.End.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(2));
    }

    // ── NextUnits ───────────────────────────────────────────────────────────

    [Fact]
    public void NextUnits_7Days_IsValidAndStartIsNow()
    {
        DateRange range = _vali.NextUnits(7, TimeUnit.Days);
        range.IsValid.Should().BeTrue();
        range.Start.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(2));
    }

    // ── Additional Union tests ───────────────────────────────────────────────

    [Fact]
    public void Union_TwoRanges_CoversAll()
    {
        DateRange a = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        DateRange b = _vali.Create(new DateTime(2025, 6, 1), new DateTime(2025, 12, 31));

        DateRange result = _vali.Union(a, b);

        result.Start.Should().Be(new DateTime(2025, 1, 1));
        result.End.Should().Be(new DateTime(2025, 12, 31));
    }

    // ── Additional Overlaps tests ────────────────────────────────────────────

    [Fact]
    public void Overlaps_NonOverlapping_ReturnsFalse()
    {
        DateRange a = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 2, 28));
        DateRange b = _vali.Create(new DateTime(2025, 4, 1), new DateTime(2025, 6, 30));
        _vali.Overlaps(a, b).Should().BeFalse();
    }

    // ── IsAdjacent ───────────────────────────────────────────────────────────

    [Fact]
    public void IsAdjacent_ConsecutiveRanges_ReturnsTrue()
    {
        DateRange a = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        DateRange b = _vali.Create(new DateTime(2025, 3, 31), new DateTime(2025, 6, 30));
        _vali.IsAdjacent(a, b).Should().BeTrue();
    }

    // ── EachWorkday ──────────────────────────────────────────────────────────

    [Fact]
    public void EachWorkday_OneWorkweek_ReturnsFiveWorkdays()
    {
        // 2025-01-06 is a Monday, 2025-01-10 is a Friday — one full work week
        DateRange range = _vali.Create(new DateTime(2025, 1, 6), new DateTime(2025, 1, 10));
        var workdays = _vali.EachWorkday(range).ToList();
        workdays.Should().HaveCount(5);
    }

    // ── Shift ────────────────────────────────────────────────────────────────

    [Fact]
    public void Shift_PositiveDays_MovesRangeForward()
    {
        DateRange range = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 1, 31));
        DateRange shifted = _vali.Shift(range, 31m, TimeUnit.Days);
        shifted.Start.Should().Be(new DateTime(2025, 2, 1));
        shifted.End.Should().Be(new DateTime(2025, 3, 3));
    }

    // ── Expand ───────────────────────────────────────────────────────────────

    [Fact]
    public void Expand_Days_IncreasesRange()
    {
        DateRange range = _vali.Create(new DateTime(2025, 3, 10), new DateTime(2025, 3, 20));
        DateRange expanded = _vali.Expand(range, 5m, TimeUnit.Days);
        expanded.Start.Should().Be(new DateTime(2025, 3, 5));
        expanded.End.Should().Be(new DateTime(2025, 3, 25));
    }

    // ── Shrink ───────────────────────────────────────────────────────────────

    [Fact]
    public void Shrink_Days_DecreasesRange()
    {
        DateRange range = _vali.Create(new DateTime(2025, 3, 1), new DateTime(2025, 3, 31));
        DateRange shrunk = _vali.Shrink(range, 5m, TimeUnit.Days);
        shrunk.Start.Should().Be(new DateTime(2025, 3, 6));
        shrunk.End.Should().Be(new DateTime(2025, 3, 26));
    }

    // ── SplitByMonth ─────────────────────────────────────────────────────────

    [Fact]
    public void SplitByMonth_TwoMonthRange_ReturnsTwoSegments()
    {
        DateRange range = _vali.Create(new DateTime(2025, 5, 1), new DateTime(2025, 6, 30));
        var segments = _vali.SplitByMonth(range).ToList();
        segments.Should().HaveCount(2);
    }

    // ── Merge ─────────────────────────────────────────────────────────────────

    [Fact]
    public void Merge_TwoOverlappingRanges_ReturnsSingleRange()
    {
        DateRange a = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 6, 30));
        DateRange b = _vali.Create(new DateTime(2025, 4, 1), new DateTime(2025, 12, 31));

        var merged = _vali.Merge(new[] { a, b }).ToList();

        merged.Should().HaveCount(1);
        merged[0].Start.Should().Be(new DateTime(2025, 1, 1));
        merged[0].End.Should().Be(new DateTime(2025, 12, 31));
    }

    // ── R-1: SplitByQuarter overflow guard ───────────────────────────────────

    [Fact]
    public void SplitByQuarter_RangeEndingInMaxYear_DoesNotThrow()
    {
        // Q4 9999 starts Oct 1 — AddMonths(3) would overflow; guard must yield break
        var range = new DateRange(new DateTime(9999, 10, 1), DateTime.MaxValue);
        Action act = () => _vali.SplitByQuarter(range).ToList();
        act.Should().NotThrow();
    }

    [Fact]
    public void SplitByQuarter_RangeEndingInMaxYear_YieldsExactlyOneQuarter()
    {
        // Only Q4 9999 exists in the range — exactly one result expected
        var range = new DateRange(new DateTime(9999, 10, 1), DateTime.MaxValue);
        var quarters = _vali.SplitByQuarter(range).ToList();
        quarters.Should().HaveCount(1);
    }

    [Fact]
    public void SplitByQuarter_RangeSpanningQ3Q4MaxYear_YieldsCorrectly()
    {
        // Q3 9999 = Jul 1 – Sep 30; Q4 9999 = Oct 1 – Dec 31 — two quarters expected
        var range = new DateRange(new DateTime(9999, 7, 1), new DateTime(9999, 12, 31));
        var quarters = _vali.SplitByQuarter(range).ToList();
        quarters.Should().HaveCount(2);
        quarters[0].Start.Should().Be(new DateTime(9999, 7, 1));
        quarters[1].Start.Should().Be(new DateTime(9999, 10, 1));
    }

    // ── VR-1: Gaps no spurious gap at DateTime.MaxValue ──────────────────────

    [Fact]
    public void Gaps_ContainerEndIsMaxValue_NoSpuriousZeroLengthGap()
    {
        // When the covered range exactly matches the container (both ending at MaxValue),
        // there should be no gaps reported.
        var container = new DateRange(new DateTime(2025, 1, 1), DateTime.MaxValue);
        var covered   = new[] { new DateRange(new DateTime(2025, 1, 1), DateTime.MaxValue) };
        _vali.Gaps(covered, container).Should().BeEmpty();
    }

    // ── VR-2: LastUnits/NextUnits reject non-positive amount ──────────────────

    [Fact]
    public void LastUnits_NegativeAmount_ThrowsArgumentException()
    {
        Action act = () => _vali.LastUnits(-1, TimeUnit.Days);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void LastUnits_ZeroAmount_ThrowsArgumentException()
    {
        Action act = () => _vali.LastUnits(0, TimeUnit.Days);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void NextUnits_NegativeAmount_ThrowsArgumentException()
    {
        Action act = () => _vali.NextUnits(-5, TimeUnit.Hours);
        act.Should().Throw<ArgumentException>();
    }

    // ── ThisQuarter ──────────────────────────────────────────────────────────

    [Fact]
    public void ThisQuarter_FixedDateInQ3_ReturnsJulyToSeptember()
    {
        var vali = new ValiRange(new FixedClock(new DateTime(2025, 8, 15)));
        DateRange range = vali.ThisQuarter();
        range.Start.Should().Be(new DateTime(2025, 7, 1));
        range.End.Should().Be(new DateTime(2025, 9, 30));
    }

    // ── ThisYear ─────────────────────────────────────────────────────────────

    [Fact]
    public void ThisYear_FixedDate_ReturnsJanToDecOfSameYear()
    {
        var vali = new ValiRange(new FixedClock(new DateTime(2025, 5, 10)));
        DateRange range = vali.ThisYear();
        range.Start.Should().Be(new DateTime(2025, 1, 1));
        range.End.Should().Be(new DateTime(2025, 12, 31));
    }

    // ── IsContainedBy ────────────────────────────────────────────────────────

    [Fact]
    public void IsContainedBy_InnerWithinOuter_ReturnsTrue()
    {
        DateRange outer = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 12, 31));
        DateRange inner = _vali.Create(new DateTime(2025, 3, 1), new DateTime(2025, 6, 30));
        _vali.IsContainedBy(inner, outer).Should().BeTrue();
    }

    [Fact]
    public void IsContainedBy_InnerExtendsBeyondOuter_ReturnsFalse()
    {
        DateRange outer = _vali.Create(new DateTime(2025, 3, 1), new DateTime(2025, 6, 30));
        DateRange inner = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 12, 31));
        _vali.IsContainedBy(inner, outer).Should().BeFalse();
    }

    // ── Shrink ───────────────────────────────────────────────────────────────

    [Fact]
    public void Shrink_AmountLargerThanRange_ThrowsArgumentException()
    {
        DateRange range = _vali.Create(new DateTime(2025, 3, 1), new DateTime(2025, 3, 5));
        Action act = () => _vali.Shrink(range, 10m, TimeUnit.Days);
        act.Should().Throw<ArgumentException>();
    }

    // ── ToTimeSpan unit coverage (via Shift) ────────────────────────────────

    [Fact]
    public void Shift_Milliseconds_MovesRangeForward()
    {
        DateRange range = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 1, 2));
        DateRange shifted = _vali.Shift(range, 500m, TimeUnit.Milliseconds);
        shifted.Start.Should().Be(new DateTime(2025, 1, 1).AddMilliseconds(500));
    }

    [Fact]
    public void Shift_Seconds_MovesRangeForward()
    {
        DateRange range = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 1, 2));
        DateRange shifted = _vali.Shift(range, 30m, TimeUnit.Seconds);
        shifted.Start.Should().Be(new DateTime(2025, 1, 1).AddSeconds(30));
    }

    [Fact]
    public void Shift_Minutes_MovesRangeForward()
    {
        DateRange range = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 1, 2));
        DateRange shifted = _vali.Shift(range, 15m, TimeUnit.Minutes);
        shifted.Start.Should().Be(new DateTime(2025, 1, 1).AddMinutes(15));
    }

    [Fact]
    public void Shift_Hours_MovesRangeForward()
    {
        DateRange range = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 1, 2));
        DateRange shifted = _vali.Shift(range, 6m, TimeUnit.Hours);
        shifted.Start.Should().Be(new DateTime(2025, 1, 1).AddHours(6));
    }

    [Fact]
    public void Shift_Weeks_MovesRangeForward()
    {
        DateRange range = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 1, 2));
        DateRange shifted = _vali.Shift(range, 2m, TimeUnit.Weeks);
        shifted.Start.Should().Be(new DateTime(2025, 1, 15));
    }

    [Fact]
    public void Shift_UnrecognisedUnit_ThrowsArgumentOutOfRangeException()
    {
        DateRange range = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 1, 2));
        Action act = () => _vali.Shift(range, 1m, (TimeUnit)9999);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    // ── SubtractUnit/AddUnit Months/Years coverage ──────────────────────────

    [Fact]
    public void LastUnits_Months_SubtractsCalendarMonths()
    {
        var vali = new ValiRange(new FixedClock(new DateTime(2025, 6, 15)));
        DateRange range = vali.LastUnits(3, TimeUnit.Months);
        range.Start.Should().Be(new DateTime(2025, 3, 15));
    }

    [Fact]
    public void LastUnits_Years_SubtractsCalendarYears()
    {
        var vali = new ValiRange(new FixedClock(new DateTime(2025, 6, 15)));
        DateRange range = vali.LastUnits(2, TimeUnit.Years);
        range.Start.Should().Be(new DateTime(2023, 6, 15));
    }

    [Fact]
    public void NextUnits_Months_AddsCalendarMonths()
    {
        var vali = new ValiRange(new FixedClock(new DateTime(2025, 6, 15)));
        DateRange range = vali.NextUnits(3, TimeUnit.Months);
        range.End.Should().Be(new DateTime(2025, 9, 15));
    }

    [Fact]
    public void NextUnits_Years_AddsCalendarYears()
    {
        var vali = new ValiRange(new FixedClock(new DateTime(2025, 6, 15)));
        DateRange range = vali.NextUnits(2, TimeUnit.Years);
        range.End.Should().Be(new DateTime(2027, 6, 15));
    }

    // ── EachWeek ─────────────────────────────────────────────────────────────

    [Fact]
    public void EachWeek_SpanningMonthBoundary_ReturnsExpectedWeekStarts()
    {
        // 2025-01-01 is a Wednesday; week (Monday-start) begins 2024-12-30
        DateRange range = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 1, 20));
        var weeks = _vali.EachWeek(range).ToList();
        weeks.Should().NotBeEmpty();
        weeks[0].Should().Be(new DateTime(2024, 12, 30));
    }

    [Fact]
    public void EachWeek_LeapFebruaryRange_ReturnsExpectedCount()
    {
        DateRange range = _vali.Create(new DateTime(2024, 2, 1), new DateTime(2024, 2, 29));
        var weeks = _vali.EachWeek(range).ToList();
        weeks.Should().HaveCountGreaterThanOrEqualTo(4);
    }

    // ── EachMonth ────────────────────────────────────────────────────────────

    [Fact]
    public void EachMonth_SpanningYearBoundary_ReturnsExpectedMonths()
    {
        DateRange range = _vali.Create(new DateTime(2024, 11, 1), new DateTime(2025, 2, 1));
        var months = _vali.EachMonth(range).ToList();
        months.Should().HaveCount(4);
        months[0].Should().Be(new DateTime(2024, 11, 1));
        months[^1].Should().Be(new DateTime(2025, 2, 1));
    }

    [Fact]
    public void EachMonth_LeapFebruaryRange_ReturnsSingleMonth()
    {
        DateRange range = _vali.Create(new DateTime(2024, 2, 1), new DateTime(2024, 2, 29));
        var months = _vali.EachMonth(range).ToList();
        months.Should().ContainSingle();
        months[0].Should().Be(new DateTime(2024, 2, 1));
    }

    // ── SplitByDay ───────────────────────────────────────────────────────────

    [Fact]
    public void SplitByDay_SpanningMonthBoundary_ReturnsOneRangePerDay()
    {
        DateRange range = _vali.Create(new DateTime(2025, 1, 30), new DateTime(2025, 2, 2));
        var days = _vali.SplitByDay(range).ToList();
        days.Should().HaveCount(4);
        days[0].Start.Should().Be(new DateTime(2025, 1, 30));
        days[0].End.Should().Be(new DateTime(2025, 1, 30));
        days[^1].Start.Should().Be(new DateTime(2025, 2, 2));
    }

    [Fact]
    public void SplitByDay_LeapFebruaryRange_ReturnsTwentyNineDays()
    {
        DateRange range = _vali.Create(new DateTime(2024, 2, 1), new DateTime(2024, 2, 29));
        var days = _vali.SplitByDay(range).ToList();
        days.Should().HaveCount(29);
    }

    // ── SplitByWeek ──────────────────────────────────────────────────────────

    [Fact]
    public void SplitByWeek_SpanningMonthBoundary_ClipsFirstAndLastChunks()
    {
        // 2025-01-01 is Wednesday; week start 2024-12-30, range ends 2025-01-20
        DateRange range = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 1, 20));
        var weeks = _vali.SplitByWeek(range).ToList();
        weeks.Should().NotBeEmpty();
        weeks[0].Start.Should().Be(new DateTime(2025, 1, 1)); // clipped to range start
        weeks[^1].End.Should().Be(new DateTime(2025, 1, 20)); // clipped to range end
    }

    [Fact]
    public void SplitByWeek_LeapFebruaryRange_ReturnsWeeklyChunks()
    {
        DateRange range = _vali.Create(new DateTime(2024, 2, 1), new DateTime(2024, 2, 29));
        var weeks = _vali.SplitByWeek(range).ToList();
        weeks.Should().HaveCountGreaterThanOrEqualTo(4);
        weeks[0].Start.Should().Be(new DateTime(2024, 2, 1));
        weeks[^1].End.Should().Be(new DateTime(2024, 2, 29));
    }

    // ── SplitByQuarter (normal completion, no overflow) ─────────────────────

    [Fact]
    public void SplitByQuarter_TwoQuarterRange_ReturnsTwoSegments()
    {
        DateRange range = _vali.Create(new DateTime(2025, 2, 1), new DateTime(2025, 5, 1));
        var quarters = _vali.SplitByQuarter(range).ToList();
        quarters.Should().HaveCount(2);
    }

    // ── Merge (non-adjacent branch) ──────────────────────────────────────────

    [Fact]
    public void Merge_ThreeRangesWithOneGapLargerThanOneDay_ReturnsTwoMergedRanges()
    {
        DateRange a = _vali.Create(new DateTime(2025, 1, 1), new DateTime(2025, 1, 5));
        DateRange b = _vali.Create(new DateTime(2025, 1, 4), new DateTime(2025, 1, 8)); // overlaps a
        DateRange c = _vali.Create(new DateTime(2025, 2, 1), new DateTime(2025, 2, 10)); // gap > 1 day from b

        var merged = _vali.Merge(new[] { a, b, c }).ToList();

        merged.Should().HaveCount(2);
        merged[0].Start.Should().Be(new DateTime(2025, 1, 1));
        merged[0].End.Should().Be(new DateTime(2025, 1, 8));
        merged[1].Start.Should().Be(new DateTime(2025, 2, 1));
        merged[1].End.Should().Be(new DateTime(2025, 2, 10));
    }
}
