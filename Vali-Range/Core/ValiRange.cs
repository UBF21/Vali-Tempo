using Vali_Range.Models;
using Vali_Time.Abstractions;
using Vali_Time.Enums;

namespace Vali_Range.Core;

/// <summary>
/// Provides a comprehensive set of utilities for creating, querying, transforming, and iterating
/// over <see cref="DateRange"/> values. Implements <see cref="IValiRange"/>.
/// </summary>
public class ValiRange : IValiRange
{
    private readonly IClock _clock;

    /// <summary>
    /// Initializes a new instance of <see cref="ValiRange"/>.
    /// </summary>
    /// <param name="clock">
    /// Optional clock abstraction. Defaults to <see cref="SystemClock.Instance"/> when <c>null</c>.
    /// Inject a test double to control time in unit tests.
    /// </param>
    public ValiRange(IClock? clock = null)
    {
        _clock = clock ?? SystemClock.Instance;
    }

    // =========================================================================
    // PRIVATE HELPERS
    // =========================================================================

    /// <summary>
    /// Maximum representable <see cref="TimeSpan"/> value as a <see cref="decimal"/> tick count.
    /// Used to guard against overflow when converting large durations.
    /// </summary>
    private const decimal MaxTicksDecimal = (decimal)long.MaxValue;

    private static long ClampTicks(decimal ticks)
    {
        if (ticks > MaxTicksDecimal) return long.MaxValue;
        if (ticks < 0m) return 0L;
        return (long)ticks;
    }

    private static TimeSpan ToTimeSpan(decimal amount, TimeUnit unit) =>
        unit switch
        {
            TimeUnit.Milliseconds => TimeSpan.FromTicks(ClampTicks(amount * TimeSpan.TicksPerMillisecond)),
            TimeUnit.Seconds      => TimeSpan.FromTicks(ClampTicks(amount * TimeSpan.TicksPerSecond)),
            TimeUnit.Minutes      => TimeSpan.FromTicks(ClampTicks(amount * TimeSpan.TicksPerMinute)),
            TimeUnit.Hours        => TimeSpan.FromTicks(ClampTicks(amount * TimeSpan.TicksPerHour)),
            TimeUnit.Days         => TimeSpan.FromTicks(ClampTicks(amount * TimeSpan.TicksPerDay)),
            TimeUnit.Weeks        => TimeSpan.FromTicks(ClampTicks(amount * TimeSpan.TicksPerDay * 7)),
            TimeUnit.Months       => TimeSpan.FromTicks(ClampTicks(amount * TimeSpan.TicksPerDay * 30.4375m)),
            TimeUnit.Years        => TimeSpan.FromTicks(ClampTicks(amount * TimeSpan.TicksPerDay * 365.25m)),
            _                     => throw new ArgumentOutOfRangeException(nameof(unit), unit, "Unrecognised TimeUnit value.")
        };

    /// <summary>
    /// Subtracts <paramref name="amount"/> in the given <paramref name="unit"/> from <paramref name="date"/>,
    /// using calendar-accurate arithmetic for Months and Years.
    /// </summary>
    private static DateTime SubtractUnit(DateTime date, decimal amount, TimeUnit unit) =>
        unit switch
        {
            TimeUnit.Months => date.AddMonths(-(int)amount),  // fractional values are truncated toward zero
            TimeUnit.Years  => date.AddYears(-(int)amount),    // fractional values are truncated toward zero
            _               => date - ToTimeSpan(amount, unit)
        };

    /// <summary>
    /// Adds <paramref name="amount"/> in the given <paramref name="unit"/> to <paramref name="date"/>,
    /// using calendar-accurate arithmetic for Months and Years.
    /// </summary>
    private static DateTime AddUnit(DateTime date, decimal amount, TimeUnit unit) =>
        unit switch
        {
            TimeUnit.Months => date.AddMonths((int)amount),  // fractional values are truncated toward zero
            TimeUnit.Years  => date.AddYears((int)amount),    // fractional values are truncated toward zero
            _               => date + ToTimeSpan(amount, unit)
        };

    /// <summary>
    /// Returns the first day of the week that contains <paramref name="date"/>,
    /// according to the specified <paramref name="weekStart"/> convention.
    /// </summary>
    private static DateTime StartOfWeek(DateTime date, WeekStart weekStart)
    {
        DayOfWeek firstDay = weekStart == WeekStart.Monday ? DayOfWeek.Monday : DayOfWeek.Sunday;
        int diff = (7 + (date.DayOfWeek - firstDay)) % 7;
        return date.AddDays(-diff).Date;
    }

    /// <summary>
    /// Returns the first month of the quarter that contains month number <paramref name="month"/>.
    /// </summary>
    private static int FirstMonthOfQuarter(int month) => ((month - 1) / 3) * 3 + 1;

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
    public DateRange Create(DateTime start, DateTime end)
    {
        if (start > end)
            throw new ArgumentException($"start ({start:O}) must be less than or equal to end ({end:O}).", nameof(start));

        return new DateRange(start, end);
    }

    /// <summary>
    /// Creates a <see cref="DateRange"/> from <paramref name="amount"/> units ago up to the current instant.
    /// Uses <see cref="DateTime.AddDays"/>, <see cref="DateTime.AddMonths"/>, or <see cref="DateTime.AddYears"/>
    /// as appropriate for the given <paramref name="unit"/>.
    /// </summary>
    /// <remarks>For Months and Years units, fractional amounts are truncated toward zero.</remarks>
    /// <param name="amount">The number of units to look back.</param>
    /// <param name="unit">The unit of time to apply.</param>
    /// <returns>A <see cref="DateRange"/> whose <c>End</c> is <see cref="DateTime.Now"/> and <c>Start</c> is <paramref name="amount"/> units earlier.</returns>
    public DateRange LastUnits(decimal amount, TimeUnit unit)
    {
        DateTime now   = _clock.Now;
        DateTime start = SubtractUnit(now, amount, unit);
        return new DateRange(start, now);
    }

    /// <summary>
    /// Creates a <see cref="DateRange"/> from the current instant forward by <paramref name="amount"/> units.
    /// Uses <see cref="DateTime.AddDays"/>, <see cref="DateTime.AddMonths"/>, or <see cref="DateTime.AddYears"/>
    /// as appropriate for the given <paramref name="unit"/>.
    /// </summary>
    /// <remarks>For Months and Years units, fractional amounts are truncated toward zero.</remarks>
    /// <param name="amount">The number of units to project into the future.</param>
    /// <param name="unit">The unit of time to apply.</param>
    /// <returns>A <see cref="DateRange"/> whose <c>Start</c> is <see cref="DateTime.Now"/> and <c>End</c> is <paramref name="amount"/> units later.</returns>
    public DateRange NextUnits(decimal amount, TimeUnit unit)
    {
        DateTime now = _clock.Now;
        DateTime end = AddUnit(now, amount, unit);
        return new DateRange(now, end);
    }

    /// <summary>
    /// Returns a <see cref="DateRange"/> spanning the complete current calendar month,
    /// from the 1st at midnight to the last day of the month at midnight.
    /// </summary>
    /// <returns>A <see cref="DateRange"/> for the current month.</returns>
    public DateRange ThisMonth()
    {
        DateTime today = _clock.Today;
        DateTime start = new DateTime(today.Year, today.Month, 1);
        DateTime end   = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
        return new DateRange(start, end);
    }

    /// <summary>
    /// Returns a <see cref="DateRange"/> spanning the current calendar week,
    /// from the first day of the week (determined by <paramref name="weekStart"/>) to the last day.
    /// </summary>
    /// <param name="weekStart">Defines whether the week starts on Monday (ISO 8601) or Sunday. Defaults to <see cref="WeekStart.Monday"/>.</param>
    /// <returns>A <see cref="DateRange"/> for the current week.</returns>
    public DateRange ThisWeek(WeekStart weekStart = WeekStart.Monday)
    {
        DateTime start = StartOfWeek(_clock.Today, weekStart);
        DateTime end   = start.AddDays(6);
        return new DateRange(start, end);
    }

    /// <summary>
    /// Returns a <see cref="DateRange"/> spanning the current calendar quarter
    /// (Q1 = Jan–Mar, Q2 = Apr–Jun, Q3 = Jul–Sep, Q4 = Oct–Dec).
    /// </summary>
    /// <returns>A <see cref="DateRange"/> for the current quarter.</returns>
    public DateRange ThisQuarter()
    {
        DateTime today      = _clock.Today;
        int      firstMonth = FirstMonthOfQuarter(today.Month);
        DateTime start      = new DateTime(today.Year, firstMonth, 1);
        DateTime end        = new DateTime(today.Year, firstMonth + 2, DateTime.DaysInMonth(today.Year, firstMonth + 2));
        return new DateRange(start, end);
    }

    /// <summary>
    /// Returns a <see cref="DateRange"/> spanning the entire current calendar year,
    /// from January 1st to December 31st.
    /// </summary>
    /// <returns>A <see cref="DateRange"/> for the current year.</returns>
    public DateRange ThisYear()
    {
        int year = _clock.Today.Year;
        return new DateRange(new DateTime(year, 1, 1), new DateTime(year, 12, 31));
    }

    // =========================================================================
    // QUERIES
    // =========================================================================

    /// <summary>
    /// Determines whether <paramref name="date"/> falls within <paramref name="range"/> (inclusive on both ends).
    /// </summary>
    /// <param name="range">The range to test against.</param>
    /// <param name="date">The date to look for.</param>
    /// <returns><c>true</c> if the date is inside the range; otherwise <c>false</c>.</returns>
    public bool Contains(DateRange range, DateTime date) => range.Contains(date);

    /// <summary>
    /// Determines whether two ranges share at least one common point in time.
    /// </summary>
    /// <param name="a">The first range.</param>
    /// <param name="b">The second range.</param>
    /// <returns><c>true</c> if the ranges overlap; otherwise <c>false</c>.</returns>
    public bool Overlaps(DateRange a, DateRange b) => a.Start <= b.End && b.Start <= a.End;

    /// <summary>
    /// Determines whether <paramref name="inner"/> is completely contained within <paramref name="outer"/>.
    /// </summary>
    /// <param name="inner">The range that must fit inside <paramref name="outer"/>.</param>
    /// <param name="outer">The enclosing range.</param>
    /// <returns><c>true</c> if every point of <paramref name="inner"/> is also in <paramref name="outer"/>; otherwise <c>false</c>.</returns>
    public bool IsContainedBy(DateRange inner, DateRange outer) =>
        inner.Start >= outer.Start && inner.End <= outer.End;

    // =========================================================================
    // OPERATIONS
    // =========================================================================

    /// <summary>
    /// Returns the intersection of two ranges, or <c>null</c> if they do not overlap.
    /// </summary>
    /// <param name="a">The first range.</param>
    /// <param name="b">The second range.</param>
    /// <returns>A <see cref="DateRange"/> representing the overlapping period, or <c>null</c> when there is no overlap.</returns>
    public DateRange? Intersection(DateRange a, DateRange b)
    {
        DateTime start = a.Start > b.Start ? a.Start : b.Start;
        DateTime end   = a.End   < b.End   ? a.End   : b.End;

        if (start > end)
            return null;

        return new DateRange(start, end);
    }

    /// <summary>
    /// Returns the minimum <see cref="DateRange"/> that completely covers both <paramref name="a"/> and <paramref name="b"/>.
    /// </summary>
    /// <param name="a">The first range.</param>
    /// <param name="b">The second range.</param>
    /// <returns>A <see cref="DateRange"/> from the earlier start to the later end of the two ranges.</returns>
    public DateRange Union(DateRange a, DateRange b)
    {
        DateTime start = a.Start < b.Start ? a.Start : b.Start;
        DateTime end   = a.End   > b.End   ? a.End   : b.End;
        return new DateRange(start, end);
    }

    /// <summary>
    /// Expands <paramref name="range"/> by subtracting <paramref name="amount"/> from its <c>Start</c>
    /// and adding <paramref name="amount"/> to its <c>End</c>.
    /// </summary>
    /// <remarks>For Months and Years units, fractional amounts are truncated toward zero.</remarks>
    /// <param name="range">The range to expand.</param>
    /// <param name="amount">The amount to expand by in each direction.</param>
    /// <param name="unit">The unit of time for <paramref name="amount"/>.</param>
    /// <returns>A new, wider <see cref="DateRange"/>.</returns>
    public DateRange Expand(DateRange range, decimal amount, TimeUnit unit) =>
        new DateRange(SubtractUnit(range.Start, amount, unit), AddUnit(range.End, amount, unit));

    /// <summary>
    /// Contracts <paramref name="range"/> by adding <paramref name="amount"/> to its <c>Start</c>
    /// and subtracting <paramref name="amount"/> from its <c>End</c>.
    /// </summary>
    /// <remarks>For Months and Years units, fractional amounts are truncated toward zero.</remarks>
    /// <param name="range">The range to shrink.</param>
    /// <param name="amount">The amount to remove from each end.</param>
    /// <param name="unit">The unit of time for <paramref name="amount"/>.</param>
    /// <returns>A new, narrower <see cref="DateRange"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when the resulting range would be invalid (<c>Start</c> &gt; <c>End</c>).</exception>
    public DateRange Shrink(DateRange range, decimal amount, TimeUnit unit)
    {
        DateTime newStart = AddUnit(range.Start, amount, unit);
        DateTime newEnd   = SubtractUnit(range.End, amount, unit);

        if (newStart > newEnd)
            throw new ArgumentException(
                $"Shrinking the range by {amount} {unit} produces an invalid range (Start > End).", nameof(amount));

        return new DateRange(newStart, newEnd);
    }

    /// <summary>
    /// Shifts <paramref name="range"/> forward (positive <paramref name="amount"/>) or backward (negative)
    /// by the specified duration, keeping its length unchanged.
    /// </summary>
    /// <remarks>For Months and Years units, fractional amounts are truncated toward zero.</remarks>
    /// <param name="range">The range to shift.</param>
    /// <param name="amount">The amount to shift; positive values move toward the future.</param>
    /// <param name="unit">The unit of time for <paramref name="amount"/>.</param>
    /// <returns>A new <see cref="DateRange"/> displaced by the given offset.</returns>
    public DateRange Shift(DateRange range, decimal amount, TimeUnit unit) =>
        new DateRange(AddUnit(range.Start, amount, unit), AddUnit(range.End, amount, unit));

    // =========================================================================
    // ITERATION
    // =========================================================================

    /// <summary>
    /// Enumerates each calendar day within <paramref name="range"/>, inclusive on both boundaries.
    /// Each yielded value has its time component set to midnight via <see cref="DateTime.Date"/>.
    /// </summary>
    /// <param name="range">The range to iterate.</param>
    /// <returns>A sequence of <see cref="DateTime"/> values, one per day.</returns>
    public IEnumerable<DateTime> EachDay(DateRange range)
    {
        DateTime current = range.Start.Date;
        DateTime end     = range.End.Date;

        while (current <= end)
        {
            yield return current;
            if (current == DateTime.MaxValue) yield break;
            current = current.AddDays(1);
        }
    }

    /// <summary>
    /// Enumerates the first day of each calendar week that falls within <paramref name="range"/>.
    /// The first returned value is the start of the week that contains <c>range.Start</c>.
    /// </summary>
    /// <param name="range">The range to iterate.</param>
    /// <param name="weekStart">Defines whether the week starts on Monday or Sunday. Defaults to <see cref="WeekStart.Monday"/>.</param>
    /// <returns>A sequence of <see cref="DateTime"/> values representing the start of each week.</returns>
    public IEnumerable<DateTime> EachWeek(DateRange range, WeekStart weekStart = WeekStart.Monday)
    {
        DateTime current = StartOfWeek(range.Start.Date, weekStart);
        DateTime end     = range.End.Date;

        while (current <= end)
        {
            yield return current;
            if (current > DateTime.MaxValue.AddDays(-7)) yield break;
            current = current.AddDays(7);
        }
    }

    /// <summary>
    /// Enumerates the first day of each calendar month that falls within <paramref name="range"/>.
    /// </summary>
    /// <param name="range">The range to iterate.</param>
    /// <returns>A sequence of <see cref="DateTime"/> values representing the 1st of each month covered by the range.</returns>
    public IEnumerable<DateTime> EachMonth(DateRange range)
    {
        DateTime current = new DateTime(range.Start.Year, range.Start.Month, 1);
        DateTime end     = range.End.Date;

        while (current <= end)
        {
            yield return current;
            if (current.Year == 9999 && current.Month == 12) yield break;
            current = current.AddMonths(1);
        }
    }

    /// <summary>
    /// Splits <paramref name="range"/> into a sequence of single-day <see cref="DateRange"/> sub-ranges.
    /// Each sub-range spans exactly one calendar day from midnight to midnight of the same date.
    /// </summary>
    /// <param name="range">The range to split.</param>
    /// <returns>A sequence of <see cref="DateRange"/> values, each spanning exactly one calendar day.</returns>
    public IEnumerable<DateRange> SplitByDay(DateRange range)
    {
        foreach (DateTime day in EachDay(range))
            yield return new DateRange(day, day);
    }

    /// <summary>
    /// Splits <paramref name="range"/> into weekly sub-ranges, each starting on the configured first day of the week.
    /// The first and last sub-ranges are clipped to the boundaries of <paramref name="range"/>.
    /// </summary>
    /// <param name="range">The range to split.</param>
    /// <param name="weekStart">Defines whether the week starts on Monday or Sunday. Defaults to <see cref="WeekStart.Monday"/>.</param>
    /// <returns>A sequence of <see cref="DateRange"/> values covering the range week by week.</returns>
    public IEnumerable<DateRange> SplitByWeek(DateRange range, WeekStart weekStart = WeekStart.Monday)
    {
        DateTime rangeEnd      = range.End.Date;
        DateTime weekBeginning = StartOfWeek(range.Start.Date, weekStart);

        while (weekBeginning <= rangeEnd)
        {
            DateTime chunkStart = weekBeginning < range.Start.Date ? range.Start.Date : weekBeginning;
            DateTime chunkEnd   = weekBeginning.AddDays(6);

            if (chunkEnd > rangeEnd)
                chunkEnd = rangeEnd;

            yield return new DateRange(chunkStart, chunkEnd);
            if (weekBeginning > DateTime.MaxValue.AddDays(-7)) yield break;
            weekBeginning = weekBeginning.AddDays(7);
        }
    }

    /// <summary>
    /// Splits <paramref name="range"/> into monthly sub-ranges, each spanning from the 1st to the last day
    /// of a calendar month, clipped to the boundaries of <paramref name="range"/> where necessary.
    /// </summary>
    /// <param name="range">The range to split.</param>
    /// <returns>A sequence of <see cref="DateRange"/> values, one per calendar month covered by the range.</returns>
    public IEnumerable<DateRange> SplitByMonth(DateRange range)
    {
        DateTime rangeEnd    = range.End.Date;
        DateTime monthStart  = new DateTime(range.Start.Year, range.Start.Month, 1);

        while (monthStart <= rangeEnd)
        {
            DateTime monthEnd = new DateTime(
                monthStart.Year,
                monthStart.Month,
                DateTime.DaysInMonth(monthStart.Year, monthStart.Month));

            DateTime chunkStart = monthStart < range.Start.Date ? range.Start.Date : monthStart;
            DateTime chunkEnd   = monthEnd   > rangeEnd         ? rangeEnd         : monthEnd;

            yield return new DateRange(chunkStart, chunkEnd);
            if (monthStart.Year == 9999 && monthStart.Month == 12) yield break;
            monthStart = monthStart.AddMonths(1);
        }
    }

    /// <summary>
    /// Splits <paramref name="range"/> into quarterly sub-ranges (Q1–Q4), each clipped to the boundaries
    /// of <paramref name="range"/> where necessary.
    /// </summary>
    /// <param name="range">The range to split.</param>
    /// <returns>A sequence of <see cref="DateRange"/> values, one per calendar quarter covered by the range.</returns>
    public IEnumerable<DateRange> SplitByQuarter(DateRange range)
    {
        DateTime rangeEnd      = range.End.Date;
        int      firstMonth    = FirstMonthOfQuarter(range.Start.Month);
        DateTime quarterStart  = new DateTime(range.Start.Year, firstMonth, 1);

        while (quarterStart <= rangeEnd)
        {
            int      lastMonthOfQuarter = firstMonth + 2;
            DateTime quarterEnd         = new DateTime(
                quarterStart.Year,
                lastMonthOfQuarter,
                DateTime.DaysInMonth(quarterStart.Year, lastMonthOfQuarter));

            DateTime chunkStart = quarterStart < range.Start.Date ? range.Start.Date : quarterStart;
            DateTime chunkEnd   = quarterEnd   > rangeEnd         ? rangeEnd         : quarterEnd;

            yield return new DateRange(chunkStart, chunkEnd);

            if (quarterStart.Year == 9999 && quarterStart.Month >= 10) yield break;
            quarterStart = quarterStart.AddMonths(3);
            firstMonth   = quarterStart.Month;
        }
    }

    // =========================================================================
    // ADVANCED OPERATIONS
    // =========================================================================

    /// <summary>
    /// Determines whether two ranges share exactly one boundary point (touching but not overlapping).
    /// </summary>
    /// <remarks>
    /// Two ranges are considered adjacent when one ends on the same date the other begins (shared boundary date).
    /// Specifically, one range's <c>End</c> date equals the other's <c>Start</c> date (compared at day
    /// granularity via <see cref="DateTime.Date"/>). Ranges separated by one or more calendar days are NOT
    /// adjacent per this method. Note: this definition is stricter than <see cref="Merge"/>, which also
    /// merges ranges separated by exactly one calendar day.
    /// </remarks>
    /// <param name="a">The first range.</param>
    /// <param name="b">The second range.</param>
    /// <returns><c>true</c> if the ranges are adjacent; otherwise <c>false</c>.</returns>
    public bool IsAdjacent(DateRange a, DateRange b)
        => a.End.Date == b.Start.Date || b.End.Date == a.Start.Date;

    /// <summary>
    /// Merges a list of overlapping or adjacent ranges into the minimum set of non-overlapping ranges,
    /// returned sorted by start date.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Two ranges are merged when they overlap OR when one ends on the day immediately before the other
    /// starts (next calendar day). Note: this definition is stricter than <see cref="IsAdjacent"/> which
    /// only checks shared boundary dates.
    /// </para>
    /// <para>
    /// This method operates at day granularity. Boundary calculations use <see cref="DateTime.Date"/>
    /// (midnight), so sub-day precision in range <c>Start</c> or <c>End</c> values is not preserved
    /// in the merged boundaries. Ranges whose boundaries differ only by time-of-day may be merged
    /// into a single range.
    /// </para>
    /// </remarks>
    /// <param name="ranges">The ranges to merge.</param>
    /// <returns>A sorted sequence of merged, non-overlapping <see cref="DateRange"/> values.</returns>
    public IEnumerable<DateRange> Merge(IEnumerable<DateRange> ranges)
    {
        var sorted = ranges.Where(r => r.IsValid).OrderBy(r => r.Start).ToList();
        if (sorted.Count == 0) yield break;
        var current = sorted[0];
        foreach (var next in sorted.Skip(1))
        {
            if (next.Start <= (current.End == DateTime.MaxValue ? DateTime.MaxValue : current.End.AddDays(1)))
                current = new DateRange(current.Start, next.End > current.End ? next.End : current.End);
            else { yield return current; current = next; }
        }
        yield return current;
    }

    /// <summary>
    /// Returns the gaps (date ranges not covered by any of the given ranges) within a container range.
    /// </summary>
    /// <remarks>
    /// This method operates at day granularity. Gap boundaries are computed using
    /// <see cref="DateTime.Date"/>-level arithmetic (whole-day steps via <c>AddDays</c>), so
    /// sub-day precision in range <c>Start</c> or <c>End</c> values is not preserved in the
    /// returned gap boundaries.
    /// </remarks>
    /// <param name="ranges">The ranges to check for gaps between.</param>
    /// <param name="container">The enclosing container range.</param>
    /// <returns>A sequence of <see cref="DateRange"/> values representing uncovered periods.</returns>
    public IEnumerable<DateRange> Gaps(IEnumerable<DateRange> ranges, DateRange container)
    {
        var merged = Merge(ranges).ToList();
        var cursor = container.Start;
        foreach (var r in merged.Where(r => r.End >= container.Start && r.Start <= container.End))
        {
            var start = r.Start > container.Start ? r.Start : container.Start;
            if (cursor < start) yield return new DateRange(cursor, start.AddDays(-1));
            if (r.End > cursor) cursor = r.End == DateTime.MaxValue ? DateTime.MaxValue : r.End.AddDays(1);
        }
        if (cursor <= container.End) yield return new DateRange(cursor, container.End);
    }

    /// <summary>
    /// Iterates each workday (Monday–Friday) within the range.
    /// </summary>
    /// <param name="range">The range to iterate.</param>
    /// <returns>A sequence of <see cref="DateTime"/> values, one per workday.</returns>
    public IEnumerable<DateTime> EachWorkday(DateRange range)
        => EachDay(range).Where(d => d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday);
}
