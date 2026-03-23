using System.Globalization;
using Vali_Time.Enums;

namespace Vali_Time.Core;

/// <summary>
/// Core class providing date arithmetic, period boundaries, validations, and quarter utilities.
/// </summary>
public class ValiDate : IValiDate
{
    private readonly WeekStart _defaultWeekStart;

    /// <summary>
    /// Initializes a new instance of <see cref="ValiDate"/> with the specified default week start.
    /// </summary>
    /// <param name="defaultWeekStart">
    /// The default day considered the start of the week. Defaults to <see cref="WeekStart.Monday"/>.
    /// </param>
    public ValiDate(WeekStart defaultWeekStart = WeekStart.Monday)
    {
        _defaultWeekStart = defaultWeekStart;
    }

    // =========================================================================
    // DATE DIFFERENCE
    // =========================================================================

    /// <summary>
    /// Calculates the difference between two dates in the specified time unit.
    /// </summary>
    /// <remarks>
    /// For <see cref="TimeUnit.Months"/>, the result is computed as whole calendar months
    /// plus a fractional part based on the day difference relative to the number of days
    /// in the destination month. For <see cref="TimeUnit.Years"/>, the result is months / 12.
    /// All other units use <see cref="TimeSpan.Ticks"/> for maximum precision without
    /// intermediate floating-point conversion.
    /// </remarks>
    /// <param name="from">The start date.</param>
    /// <param name="to">The end date.</param>
    /// <param name="unit">The <see cref="TimeUnit"/> in which to express the difference.</param>
    /// <param name="decimalPlaces">
    /// Optional number of decimal places to round the result to.
    /// When <c>null</c>, no rounding is applied.
    /// </param>
    /// <returns>
    /// A <see cref="decimal"/> representing the difference from <paramref name="from"/> to
    /// <paramref name="to"/> in the given <paramref name="unit"/>. The value is negative when
    /// <paramref name="to"/> is earlier than <paramref name="from"/>.
    /// </returns>
    public decimal Diff(DateTime from, DateTime to, TimeUnit unit, int? decimalPlaces = null)
    {
        TimeSpan diff = to - from;

        decimal result = unit switch
        {
            TimeUnit.Milliseconds => (decimal)diff.Ticks / TimeSpan.TicksPerMillisecond,
            TimeUnit.Seconds      => (decimal)diff.Ticks / TimeSpan.TicksPerSecond,
            TimeUnit.Minutes      => (decimal)diff.Ticks / TimeSpan.TicksPerMinute,
            TimeUnit.Hours        => (decimal)diff.Ticks / TimeSpan.TicksPerHour,
            TimeUnit.Days         => (decimal)diff.Ticks / TimeSpan.TicksPerDay,
            TimeUnit.Weeks        => (decimal)diff.Ticks / (TimeSpan.TicksPerDay * 7),
            TimeUnit.Months       => MonthsDiff(from, to),
            TimeUnit.Years        => MonthsDiff(from, to) / 12m,
            _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, "Unsupported TimeUnit.")
        };

        return decimalPlaces.HasValue
            ? decimal.Round(result, decimalPlaces.Value)
            : result;
    }

    // =========================================================================
    // DATE ARITHMETIC
    // =========================================================================

    /// <summary>
    /// Adds a specified amount of time to a date.
    /// </summary>
    /// <remarks>
    /// For <see cref="TimeUnit.Months"/> and <see cref="TimeUnit.Years"/>, the integer part of
    /// <paramref name="amount"/> is used via <see cref="DateTime.AddMonths"/> /
    /// <see cref="DateTime.AddYears"/>. For all other units, the full decimal value is cast to
    /// <c>double</c> and passed to the corresponding <c>Add*</c> method.
    /// </remarks>
    /// <param name="date">The base date.</param>
    /// <param name="amount">The amount to add. May be negative to subtract.</param>
    /// <param name="unit">The <see cref="TimeUnit"/> that <paramref name="amount"/> represents.</param>
    /// <returns>A new <see cref="DateTime"/> with the specified amount added.</returns>
    public DateTime Add(DateTime date, decimal amount, TimeUnit unit)
    {
        return unit switch
        {
            TimeUnit.Months       => date.AddMonths((int)amount),
            TimeUnit.Years        => date.AddYears((int)amount),
            TimeUnit.Days         => date.AddDays((double)amount),
            TimeUnit.Hours        => date.AddHours((double)amount),
            TimeUnit.Minutes      => date.AddMinutes((double)amount),
            TimeUnit.Seconds      => date.AddSeconds((double)amount),
            TimeUnit.Milliseconds => date.AddMilliseconds((double)amount),
            TimeUnit.Weeks        => date.AddDays((double)(amount * 7)),
            _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, "Unsupported TimeUnit.")
        };
    }

    /// <summary>
    /// Subtracts a specified amount of time from a date.
    /// </summary>
    /// <param name="date">The base date.</param>
    /// <param name="amount">The amount to subtract. Must be positive.</param>
    /// <param name="unit">The <see cref="TimeUnit"/> that <paramref name="amount"/> represents.</param>
    /// <returns>A new <see cref="DateTime"/> with the specified amount subtracted.</returns>
    public DateTime Subtract(DateTime date, decimal amount, TimeUnit unit)
    {
        return Add(date, -amount, unit);
    }

    // =========================================================================
    // PERIOD BOUNDARIES
    // =========================================================================

    /// <summary>
    /// Returns the first instant of the period that contains the given date.
    /// </summary>
    /// <param name="date">The reference date.</param>
    /// <param name="part">The <see cref="DatePart"/> representing the period granularity.</param>
    /// <param name="weekStart">
    /// The day considered the start of the week. When <c>null</c>, uses
    /// <see cref="_defaultWeekStart"/>.
    /// </param>
    /// <returns>
    /// A <see cref="DateTime"/> set to midnight (00:00:00.000) at the start of the period.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="part"/> is not a recognised <see cref="DatePart"/> value.
    /// </exception>
    public DateTime StartOf(DateTime date, DatePart part, WeekStart? weekStart = null)
    {
        WeekStart ws = weekStart ?? _defaultWeekStart;

        return part switch
        {
            DatePart.Day     => date.Date,
            DatePart.Week    => StartOfWeek(date, ws),
            DatePart.Month   => new DateTime(date.Year, date.Month, 1),
            DatePart.Quarter => QuarterStartDate(date),
            DatePart.Year    => new DateTime(date.Year, 1, 1),
            _ => throw new ArgumentOutOfRangeException(nameof(part), part, "Unsupported DatePart.")
        };
    }

    /// <summary>
    /// Returns the last instant of the period that contains the given date.
    /// </summary>
    /// <param name="date">The reference date.</param>
    /// <param name="part">The <see cref="DatePart"/> representing the period granularity.</param>
    /// <param name="weekStart">
    /// The day considered the start of the week. When <c>null</c>, uses
    /// <see cref="_defaultWeekStart"/>.
    /// </param>
    /// <returns>
    /// A <see cref="DateTime"/> set to 23:59:59.999 on the last day of the period.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="part"/> is not a recognised <see cref="DatePart"/> value.
    /// </exception>
    public DateTime EndOf(DateTime date, DatePart part, WeekStart? weekStart = null)
    {
        WeekStart ws = weekStart ?? _defaultWeekStart;
        TimeSpan endTime = TimeSpan.FromTicks(TimeSpan.TicksPerDay - 1);

        return part switch
        {
            DatePart.Day  => date.Date + endTime,
            DatePart.Week => StartOfWeek(date, ws).AddDays(6) + endTime,
            DatePart.Month =>
                new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month)) + endTime,
            DatePart.Quarter => QuarterEndDate(date) + endTime,
            DatePart.Year    => new DateTime(date.Year, 12, 31) + endTime,
            _ => throw new ArgumentOutOfRangeException(nameof(part), part, "Unsupported DatePart.")
        };
    }

    // =========================================================================
    // VALIDATIONS
    // =========================================================================

    /// <summary>
    /// Determines whether <paramref name="date"/> is strictly before <paramref name="reference"/>.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <param name="reference">The reference date.</param>
    /// <returns><c>true</c> if <paramref name="date"/> &lt; <paramref name="reference"/>.</returns>
    public bool IsBefore(DateTime date, DateTime reference)
    {
        return date < reference;
    }

    /// <summary>
    /// Determines whether <paramref name="date"/> is strictly after <paramref name="reference"/>.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <param name="reference">The reference date.</param>
    /// <returns><c>true</c> if <paramref name="date"/> &gt; <paramref name="reference"/>.</returns>
    public bool IsAfter(DateTime date, DateTime reference)
    {
        return date > reference;
    }

    /// <summary>
    /// Determines whether two dates fall on the same calendar day.
    /// </summary>
    /// <param name="a">The first date.</param>
    /// <param name="b">The second date.</param>
    /// <returns><c>true</c> if <paramref name="a"/> and <paramref name="b"/> share the same date component.</returns>
    public bool IsSameDay(DateTime a, DateTime b)
    {
        return a.Date == b.Date;
    }

    /// <summary>
    /// Determines whether two dates fall within the same period.
    /// </summary>
    /// <param name="a">The first date.</param>
    /// <param name="b">The second date.</param>
    /// <param name="part">The period granularity to compare.</param>
    /// <param name="weekStart">
    /// The day considered the start of the week, used only when <paramref name="part"/> is
    /// <see cref="DatePart.Week"/>. When <c>null</c>, uses <see cref="_defaultWeekStart"/>.
    /// </param>
    /// <returns>
    /// <c>true</c> when both dates belong to the same period defined by <paramref name="part"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="part"/> is not a recognised <see cref="DatePart"/> value.
    /// </exception>
    public bool IsSamePeriod(DateTime a, DateTime b, DatePart part, WeekStart? weekStart = null)
    {
        WeekStart ws = weekStart ?? _defaultWeekStart;

        return part switch
        {
            DatePart.Day     => a.Date == b.Date,
            DatePart.Week    => StartOfWeek(a, ws) == StartOfWeek(b, ws),
            DatePart.Month   => a.Year == b.Year && a.Month == b.Month,
            DatePart.Quarter => a.Year == b.Year && QuarterOf(a) == QuarterOf(b),
            DatePart.Year    => a.Year == b.Year,
            _ => throw new ArgumentOutOfRangeException(nameof(part), part, "Unsupported DatePart.")
        };
    }

    /// <summary>
    /// Determines whether the given date falls on a weekend (Saturday or Sunday).
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns><c>true</c> if the day is Saturday or Sunday.</returns>
    public bool IsWeekend(DateTime date)
    {
        return date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
    }

    /// <summary>
    /// Determines whether the given date falls on a weekday (Monday through Friday).
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns><c>true</c> if the day is Monday through Friday.</returns>
    public bool IsWeekday(DateTime date)
    {
        return !IsWeekend(date);
    }

    /// <summary>
    /// Determines whether the specified year is a leap year.
    /// </summary>
    /// <param name="year">The four-digit year to evaluate.</param>
    /// <returns><c>true</c> if <paramref name="year"/> is a leap year.</returns>
    public bool IsLeapYear(int year)
    {
        return DateTime.IsLeapYear(year);
    }

    // =========================================================================
    // DATE INFORMATION
    // =========================================================================

    /// <summary>
    /// Returns the number of days in the specified month of the given year.
    /// </summary>
    /// <param name="year">The four-digit year.</param>
    /// <param name="month">The month (1–12).</param>
    /// <returns>The number of days in the month.</returns>
    public int DaysInMonth(int year, int month)
    {
        return DateTime.DaysInMonth(year, month);
    }

    /// <summary>
    /// Returns the ISO or calendar week number of the year for the given date.
    /// </summary>
    /// <remarks>
    /// When <paramref name="weekStart"/> (or <see cref="_defaultWeekStart"/>) is
    /// <see cref="WeekStart.Monday"/>, <see cref="ISOWeek.GetWeekOfYear"/> is used, which
    /// follows ISO 8601. When the week starts on <see cref="WeekStart.Sunday"/>, the
    /// Gregorian calendar with <see cref="CalendarWeekRule.FirstDay"/> and
    /// <see cref="DayOfWeek.Sunday"/> is used instead.
    /// </remarks>
    /// <param name="date">The date whose week number is required.</param>
    /// <param name="weekStart">
    /// The day considered the start of the week. When <c>null</c>, uses
    /// <see cref="_defaultWeekStart"/>.
    /// </param>
    /// <returns>The week-of-year number (1-based).</returns>
    public int WeekOfYear(DateTime date, WeekStart? weekStart = null)
    {
        WeekStart ws = weekStart ?? _defaultWeekStart;

        if (ws == WeekStart.Monday)
        {
            return ISOWeek.GetWeekOfYear(date);
        }

        // Sunday-based week using the Gregorian calendar.
        Calendar calendar = new GregorianCalendar();
        return calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
    }

    /// <summary>
    /// Returns the day-of-year number for the given date (1 = January 1st).
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>An integer from 1 to 366 representing the day within the year.</returns>
    public int DayOfYear(DateTime date)
    {
        return date.DayOfYear;
    }

    // =========================================================================
    // QUARTER - BASIC METHODS
    // =========================================================================

    /// <summary>
    /// Returns the quarter (1–4) that the given date belongs to.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>An integer from 1 to 4 representing the fiscal quarter.</returns>
    public int QuarterOf(DateTime date)
    {
        return (date.Month - 1) / 3 + 1;
    }

    // =========================================================================
    // QUARTER - FULL EXTENSION
    // =========================================================================

    /// <summary>
    /// Returns the first day of the quarter that contains the given date, at midnight.
    /// </summary>
    /// <param name="date">The reference date.</param>
    /// <returns>
    /// A <see cref="DateTime"/> set to 00:00:00.000 on the first day of the quarter
    /// (January 1, April 1, July 1, or October 1).
    /// </returns>
    public DateTime QuarterStart(DateTime date)
    {
        return StartOf(date, DatePart.Quarter);
    }

    /// <summary>
    /// Returns the last instant of the quarter that contains the given date.
    /// </summary>
    /// <param name="date">The reference date.</param>
    /// <returns>
    /// A <see cref="DateTime"/> set to 23:59:59.999 on the last day of the quarter
    /// (March 31, June 30, September 30, or December 31).
    /// </returns>
    public DateTime QuarterEnd(DateTime date)
    {
        return EndOf(date, DatePart.Quarter);
    }

    /// <summary>
    /// Returns a short display name for the quarter containing the given date.
    /// </summary>
    /// <param name="date">The reference date.</param>
    /// <returns>A string in the format <c>"Q1 2025"</c>, <c>"Q2 2025"</c>, etc.</returns>
    public string QuarterName(DateTime date)
    {
        return $"Q{QuarterOf(date)} {date.Year}";
    }

    /// <summary>
    /// Returns a long display name for the quarter containing the given date.
    /// </summary>
    /// <param name="date">The reference date.</param>
    /// <returns>A string in the format <c>"Quarter 1 - 2025"</c>, <c>"Quarter 2 - 2025"</c>, etc.</returns>
    public string QuarterNameFull(DateTime date)
    {
        return $"Quarter {QuarterOf(date)} - {date.Year}";
    }

    /// <summary>
    /// Returns the total number of calendar days in the quarter containing the given date.
    /// </summary>
    /// <param name="date">The reference date.</param>
    /// <returns>The number of days from the first to the last day of the quarter (inclusive).</returns>
    public int DaysInQuarter(DateTime date)
    {
        DateTime start = QuarterStartDate(date);
        DateTime end   = QuarterEndDate(date);
        return (end - start).Days + 1;
    }

    /// <summary>
    /// Returns the number of days elapsed since the beginning of the quarter, not counting today.
    /// </summary>
    /// <param name="date">The reference date.</param>
    /// <returns>
    /// The number of days from the first day of the quarter up to (but not including)
    /// <paramref name="date"/>. Returns 0 on the first day of the quarter.
    /// </returns>
    public int DaysElapsedInQuarter(DateTime date)
    {
        DateTime start = QuarterStartDate(date);
        return (date.Date - start).Days;
    }

    /// <summary>
    /// Returns the number of days remaining until the end of the quarter, not counting today.
    /// </summary>
    /// <param name="date">The reference date.</param>
    /// <returns>
    /// The number of days from (but not including) <paramref name="date"/> to the last day of
    /// the quarter. Returns 0 on the last day of the quarter.
    /// </returns>
    public int DaysRemainingInQuarter(DateTime date)
    {
        DateTime end = QuarterEndDate(date);
        return (end - date.Date).Days;
    }

    /// <summary>
    /// Returns the fraction of the quarter that has elapsed as of the given date.
    /// </summary>
    /// <param name="date">The reference date.</param>
    /// <returns>
    /// A <see cref="decimal"/> between 0.0 (start of quarter) and &lt; 1.0 (approaching the last day of the quarter).
    /// The value does not reach exactly 1.0 on the last day of the quarter because today is not counted as elapsed.
    /// </returns>
    public decimal ProgressInQuarter(DateTime date)
    {
        int elapsed = DaysElapsedInQuarter(date);
        int total   = DaysInQuarter(date);
        return (decimal)elapsed / total;
    }

    /// <summary>
    /// Determines whether the given date is the first day of its quarter.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns><c>true</c> if <paramref name="date"/> is January 1, April 1, July 1, or October 1.</returns>
    public bool IsFirstDayOfQuarter(DateTime date)
    {
        return date.Date == QuarterStartDate(date);
    }

    /// <summary>
    /// Determines whether the given date is the last day of its quarter.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns><c>true</c> if <paramref name="date"/> is March 31, June 30, September 30, or December 31.</returns>
    public bool IsLastDayOfQuarter(DateTime date)
    {
        return date.Date == QuarterEndDate(date);
    }

    /// <summary>
    /// Determines whether two dates fall within the same quarter of the same year.
    /// </summary>
    /// <param name="a">The first date.</param>
    /// <param name="b">The second date.</param>
    /// <returns>
    /// <c>true</c> if both dates share the same year and the same quarter number.
    /// </returns>
    public bool IsInSameQuarter(DateTime a, DateTime b)
    {
        return a.Year == b.Year && QuarterOf(a) == QuarterOf(b);
    }

    /// <summary>
    /// Returns the number of complete or partial weeks in the quarter containing the given date.
    /// </summary>
    /// <param name="date">The reference date.</param>
    /// <returns>
    /// The ceiling of <c>DaysInQuarter / 7</c>, representing the total number of
    /// (possibly partial) weeks.
    /// </returns>
    public int WeeksInQuarter(DateTime date)
    {
        return (int)Math.Ceiling((double)DaysInQuarter(date) / 7);
    }

    /// <summary>
    /// Returns the first day of the quarter immediately following the one containing the given date.
    /// </summary>
    /// <param name="date">The reference date.</param>
    /// <returns>
    /// A <see cref="DateTime"/> set to midnight on the first day of the next quarter.
    /// </returns>
    public DateTime NextQuarterStart(DateTime date)
    {
        DateTime currentStart = QuarterStartDate(date);
        return currentStart.AddMonths(3);
    }

    /// <summary>
    /// Returns the first day of the quarter immediately preceding the one containing the given date.
    /// </summary>
    /// <param name="date">The reference date.</param>
    /// <returns>
    /// A <see cref="DateTime"/> set to midnight on the first day of the previous quarter.
    /// </returns>
    public DateTime PreviousQuarterStart(DateTime date)
    {
        DateTime currentStart = QuarterStartDate(date);
        return currentStart.AddMonths(-3);
    }

    // =========================================================================
    // PRIVATE HELPERS
    // =========================================================================

    /// <summary>
    /// Calculates the difference between two dates in fractional calendar months.
    /// </summary>
    /// <param name="from">The start date.</param>
    /// <param name="to">The end date.</param>
    /// <returns>
    /// Whole calendar months plus a fractional day component relative to the number of
    /// days in the destination month.
    /// </returns>
    private decimal MonthsDiff(DateTime from, DateTime to)
    {
        int months = (to.Year - from.Year) * 12 + (to.Month - from.Month);
        int dayDiff = to.Day - from.Day;

        // If dayDiff is negative, borrow one month and express the remainder
        // as a fraction of the previous month's day count.
        if (dayDiff < 0)
        {
            months -= 1;
            // Number of days in the month just before 'to'
            DateTime prevMonth = to.AddMonths(-1);
            int daysInPrevMonth = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);
            dayDiff += daysInPrevMonth;
            decimal dayFraction = (decimal)Math.Max(0, dayDiff) / daysInPrevMonth;
            return months + dayFraction;
        }

        decimal dayFractionNormal = (decimal)dayDiff / DateTime.DaysInMonth(to.Year, to.Month);
        return months + dayFractionNormal;
    }

    /// <summary>
    /// Returns the date of the Monday (or Sunday) that begins the week containing
    /// <paramref name="date"/>, at midnight.
    /// </summary>
    /// <param name="date">The reference date.</param>
    /// <param name="weekStart">The day considered the first day of the week.</param>
    /// <returns>A <see cref="DateTime"/> set to midnight on the first day of the week.</returns>
    private DateTime StartOfWeek(DateTime date, WeekStart weekStart)
    {
        DayOfWeek firstDay = weekStart == WeekStart.Monday ? DayOfWeek.Monday : DayOfWeek.Sunday;
        int diff = (7 + (date.DayOfWeek - firstDay)) % 7;
        return date.Date.AddDays(-diff);
    }

    /// <summary>
    /// Returns the date-only (midnight) of the first day of the quarter containing
    /// <paramref name="date"/>.
    /// </summary>
    /// <param name="date">The reference date.</param>
    /// <returns>A <see cref="DateTime"/> at midnight on the first day of the quarter.</returns>
    private DateTime QuarterStartDate(DateTime date)
    {
        int firstMonth = (QuarterOf(date) - 1) * 3 + 1;
        return new DateTime(date.Year, firstMonth, 1);
    }

    /// <summary>
    /// Returns the date-only (midnight) of the last day of the quarter containing
    /// <paramref name="date"/>.
    /// </summary>
    /// <param name="date">The reference date.</param>
    /// <returns>A <see cref="DateTime"/> at midnight on the last day of the quarter.</returns>
    private DateTime QuarterEndDate(DateTime date)
    {
        int lastMonth = QuarterOf(date) * 3;
        int lastDay   = DateTime.DaysInMonth(date.Year, lastMonth);
        return new DateTime(date.Year, lastMonth, lastDay);
    }
}
