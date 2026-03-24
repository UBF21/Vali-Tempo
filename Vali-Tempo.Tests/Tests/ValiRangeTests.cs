using FluentAssertions;
using Vali_Range.Core;
using Vali_Range.Models;
using Vali_Time.Enums;
using Xunit;

namespace Vali_Time.Tests.Tests;

public class ValiRangeTests
{
    private readonly ValiRange _vali = new();

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
}
