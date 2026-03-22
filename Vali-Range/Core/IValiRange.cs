using Vali_Range.Models;
using Vali_Time.Enums;

namespace Vali_Range.Core;

/// <summary>
/// Defines the public contract for the <see cref="ValiRange"/> date-range utility class,
/// covering construction, querying, set operations, and iteration over <see cref="DateRange"/> values.
/// </summary>
public interface IValiRange
{
    // =========================================================================
    // CONSTRUCTION
    // =========================================================================

    /// <summary>
    /// Creates a <see cref="DateRange"/> between two explicit dates.
    /// </summary>
    /// <param name="start">The inclusive start of the range.</param>
    /// <param name="end">The inclusive end of the range.</param>
    /// <returns>A new <see cref="DateRange"/> spanning from <paramref name="start"/> to <paramref name="end"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="start"/> is greater than <paramref name="end"/>.</exception>
    DateRange Create(DateTime start, DateTime end);

    /// <summary>
    /// Creates a <see cref="DateRange"/> from <paramref name="amount"/> units ago up to the current instant.
    /// </summary>
    /// <param name="amount">The number of units to look back.</param>
    /// <param name="unit">The unit of time to apply.</param>
    /// <returns>A <see cref="DateRange"/> whose <c>End</c> is <see cref="DateTime.Now"/> and whose <c>Start</c> is <paramref name="amount"/> units earlier.</returns>
    DateRange LastUnits(decimal amount, TimeUnit unit);

    /// <summary>
    /// Creates a <see cref="DateRange"/> from the current instant forward by <paramref name="amount"/> units.
    /// </summary>
    /// <param name="amount">The number of units to project into the future.</param>
    /// <param name="unit">The unit of time to apply.</param>
    /// <returns>A <see cref="DateRange"/> whose <c>Start</c> is <see cref="DateTime.Now"/> and whose <c>End</c> is <paramref name="amount"/> units later.</returns>
    DateRange NextUnits(decimal amount, TimeUnit unit);

    /// <summary>
    /// Returns a <see cref="DateRange"/> spanning the complete current calendar month,
    /// from the 1st at midnight to the last day of the month at midnight.
    /// </summary>
    /// <returns>A <see cref="DateRange"/> for the current month.</returns>
    DateRange ThisMonth();

    /// <summary>
    /// Returns a <see cref="DateRange"/> spanning the current calendar week,
    /// from the first day of the week (determined by <paramref name="weekStart"/>) to the last day.
    /// </summary>
    /// <param name="weekStart">Defines whether the week starts on Monday (ISO 8601) or Sunday. Defaults to <see cref="WeekStart.Monday"/>.</param>
    /// <returns>A <see cref="DateRange"/> for the current week.</returns>
    DateRange ThisWeek(WeekStart weekStart = WeekStart.Monday);

    /// <summary>
    /// Returns a <see cref="DateRange"/> spanning the current calendar quarter
    /// (Q1 = Jan–Mar, Q2 = Apr–Jun, Q3 = Jul–Sep, Q4 = Oct–Dec).
    /// </summary>
    /// <returns>A <see cref="DateRange"/> for the current quarter.</returns>
    DateRange ThisQuarter();

    /// <summary>
    /// Returns a <see cref="DateRange"/> spanning the entire current calendar year,
    /// from January 1st to December 31st.
    /// </summary>
    /// <returns>A <see cref="DateRange"/> for the current year.</returns>
    DateRange ThisYear();

    // =========================================================================
    // QUERIES
    // =========================================================================

    /// <summary>
    /// Determines whether <paramref name="date"/> falls within <paramref name="range"/> (inclusive on both ends).
    /// </summary>
    /// <param name="range">The range to test against.</param>
    /// <param name="date">The date to look for.</param>
    /// <returns><c>true</c> if the date is inside the range; otherwise <c>false</c>.</returns>
    bool Contains(DateRange range, DateTime date);

    /// <summary>
    /// Determines whether two ranges share at least one common point in time.
    /// </summary>
    /// <param name="a">The first range.</param>
    /// <param name="b">The second range.</param>
    /// <returns><c>true</c> if the ranges overlap; otherwise <c>false</c>.</returns>
    bool Overlaps(DateRange a, DateRange b);

    /// <summary>
    /// Determines whether <paramref name="inner"/> is completely contained within <paramref name="outer"/>.
    /// </summary>
    /// <param name="inner">The range that must fit inside <paramref name="outer"/>.</param>
    /// <param name="outer">The enclosing range.</param>
    /// <returns><c>true</c> if every point of <paramref name="inner"/> is also in <paramref name="outer"/>; otherwise <c>false</c>.</returns>
    bool IsContainedBy(DateRange inner, DateRange outer);

    // =========================================================================
    // OPERATIONS
    // =========================================================================

    /// <summary>
    /// Returns the intersection of two ranges, or <c>null</c> if they do not overlap.
    /// </summary>
    /// <param name="a">The first range.</param>
    /// <param name="b">The second range.</param>
    /// <returns>A <see cref="DateRange"/> representing the overlapping period, or <c>null</c> when there is no overlap.</returns>
    DateRange? Intersection(DateRange a, DateRange b);

    /// <summary>
    /// Returns the minimum <see cref="DateRange"/> that completely covers both <paramref name="a"/> and <paramref name="b"/>.
    /// </summary>
    /// <param name="a">The first range.</param>
    /// <param name="b">The second range.</param>
    /// <returns>A <see cref="DateRange"/> from the earlier start to the later end of the two ranges.</returns>
    DateRange Union(DateRange a, DateRange b);

    /// <summary>
    /// Expands <paramref name="range"/> by subtracting <paramref name="amount"/> from its <c>Start</c>
    /// and adding <paramref name="amount"/> to its <c>End</c>.
    /// </summary>
    /// <param name="range">The range to expand.</param>
    /// <param name="amount">The amount to expand by in each direction.</param>
    /// <param name="unit">The unit of time for <paramref name="amount"/>.</param>
    /// <returns>A new, wider <see cref="DateRange"/>.</returns>
    DateRange Expand(DateRange range, decimal amount, TimeUnit unit);

    /// <summary>
    /// Contracts <paramref name="range"/> by adding <paramref name="amount"/> to its <c>Start</c>
    /// and subtracting <paramref name="amount"/> from its <c>End</c>.
    /// </summary>
    /// <param name="range">The range to shrink.</param>
    /// <param name="amount">The amount to remove from each end.</param>
    /// <param name="unit">The unit of time for <paramref name="amount"/>.</param>
    /// <returns>A new, narrower <see cref="DateRange"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when the resulting range would be invalid (<c>Start</c> &gt; <c>End</c>).</exception>
    DateRange Shrink(DateRange range, decimal amount, TimeUnit unit);

    /// <summary>
    /// Shifts <paramref name="range"/> forward (positive <paramref name="amount"/>) or backward (negative)
    /// by the specified duration, keeping its length unchanged.
    /// </summary>
    /// <param name="range">The range to shift.</param>
    /// <param name="amount">The amount to shift; positive values move toward the future.</param>
    /// <param name="unit">The unit of time for <paramref name="amount"/>.</param>
    /// <returns>A new <see cref="DateRange"/> displaced by the given offset.</returns>
    DateRange Shift(DateRange range, decimal amount, TimeUnit unit);

    // =========================================================================
    // ITERATION
    // =========================================================================

    /// <summary>
    /// Enumerates each calendar day within <paramref name="range"/>, inclusive on both boundaries.
    /// </summary>
    /// <param name="range">The range to iterate.</param>
    /// <returns>A sequence of <see cref="DateTime"/> values, one per day.</returns>
    IEnumerable<DateTime> EachDay(DateRange range);

    /// <summary>
    /// Enumerates the first day of each calendar week that falls within <paramref name="range"/>.
    /// </summary>
    /// <param name="range">The range to iterate.</param>
    /// <param name="weekStart">Defines whether the week starts on Monday or Sunday. Defaults to <see cref="WeekStart.Monday"/>.</param>
    /// <returns>A sequence of <see cref="DateTime"/> values representing the start of each week.</returns>
    IEnumerable<DateTime> EachWeek(DateRange range, WeekStart weekStart = WeekStart.Monday);

    /// <summary>
    /// Enumerates the first day of each calendar month that falls within <paramref name="range"/>.
    /// </summary>
    /// <param name="range">The range to iterate.</param>
    /// <returns>A sequence of <see cref="DateTime"/> values representing the 1st of each month covered by the range.</returns>
    IEnumerable<DateTime> EachMonth(DateRange range);

    /// <summary>
    /// Splits <paramref name="range"/> into a sequence of single-day <see cref="DateRange"/> sub-ranges.
    /// </summary>
    /// <param name="range">The range to split.</param>
    /// <returns>A sequence of <see cref="DateRange"/> values, each spanning exactly one calendar day.</returns>
    IEnumerable<DateRange> SplitByDay(DateRange range);

    /// <summary>
    /// Splits <paramref name="range"/> into weekly sub-ranges, each starting on the configured first day of the week.
    /// The first and last sub-ranges are clipped to the boundaries of <paramref name="range"/>.
    /// </summary>
    /// <param name="range">The range to split.</param>
    /// <param name="weekStart">Defines whether the week starts on Monday or Sunday. Defaults to <see cref="WeekStart.Monday"/>.</param>
    /// <returns>A sequence of <see cref="DateRange"/> values covering the range week by week.</returns>
    IEnumerable<DateRange> SplitByWeek(DateRange range, WeekStart weekStart = WeekStart.Monday);

    /// <summary>
    /// Splits <paramref name="range"/> into monthly sub-ranges, each spanning from the 1st to the last day of a calendar month,
    /// clipped to the boundaries of <paramref name="range"/> where necessary.
    /// </summary>
    /// <param name="range">The range to split.</param>
    /// <returns>A sequence of <see cref="DateRange"/> values, one per calendar month covered by the range.</returns>
    IEnumerable<DateRange> SplitByMonth(DateRange range);

    /// <summary>
    /// Splits <paramref name="range"/> into quarterly sub-ranges (Q1–Q4), each clipped to the boundaries of <paramref name="range"/>.
    /// </summary>
    /// <param name="range">The range to split.</param>
    /// <returns>A sequence of <see cref="DateRange"/> values, one per calendar quarter covered by the range.</returns>
    IEnumerable<DateRange> SplitByQuarter(DateRange range);

    // =========================================================================
    // ADVANCED OPERATIONS
    // =========================================================================

    /// <summary>
    /// Determines whether two ranges share exactly one boundary point (touching but not overlapping).
    /// </summary>
    /// <param name="a">The first range.</param>
    /// <param name="b">The second range.</param>
    /// <returns><c>true</c> if the ranges are adjacent; otherwise <c>false</c>.</returns>
    bool IsAdjacent(DateRange a, DateRange b);

    /// <summary>
    /// Merges a list of overlapping or adjacent ranges into the minimum set of non-overlapping ranges,
    /// returned sorted by start date.
    /// </summary>
    /// <param name="ranges">The ranges to merge.</param>
    /// <returns>A sorted sequence of merged, non-overlapping <see cref="DateRange"/> values.</returns>
    IEnumerable<DateRange> Merge(IEnumerable<DateRange> ranges);

    /// <summary>
    /// Returns the gaps (date ranges not covered by any of the given ranges) within a container range.
    /// </summary>
    /// <param name="ranges">The ranges to check for gaps between.</param>
    /// <param name="container">The enclosing container range.</param>
    /// <returns>A sequence of <see cref="DateRange"/> values representing uncovered periods.</returns>
    IEnumerable<DateRange> Gaps(IEnumerable<DateRange> ranges, DateRange container);

    /// <summary>
    /// Iterates each workday (Monday–Friday) within the range.
    /// </summary>
    /// <param name="range">The range to iterate.</param>
    /// <returns>A sequence of <see cref="DateTime"/> values, one per workday.</returns>
    IEnumerable<DateTime> EachWorkday(DateRange range);
}
