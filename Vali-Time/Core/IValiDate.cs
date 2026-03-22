using Vali_Time.Enums;

namespace Vali_Time.Core;

/// <summary>
/// Defines the contract for date operations including difference calculation, arithmetic, period boundaries, validations, and quarter utilities.
/// </summary>
public interface IValiDate
{
    // -------------------------------------------------------------------------
    // Date difference
    // -------------------------------------------------------------------------

    /// <summary>
    /// Calculates the difference between two dates expressed in the specified time unit.
    /// </summary>
    /// <param name="from">The start date.</param>
    /// <param name="to">The end date.</param>
    /// <param name="unit">The unit in which to express the difference.</param>
    /// <param name="decimalPlaces">The number of decimal places to round the result to; if null, no rounding is applied.</param>
    /// <returns>The difference between the two dates expressed in the specified unit.</returns>
    decimal Diff(DateTime from, DateTime to, TimeUnit unit, int? decimalPlaces = null);

    // -------------------------------------------------------------------------
    // Date arithmetic
    // -------------------------------------------------------------------------

    /// <summary>
    /// Adds the specified amount of the given time unit to a date.
    /// </summary>
    /// <param name="date">The base date to add to.</param>
    /// <param name="amount">The amount to add.</param>
    /// <param name="unit">The unit of the amount to add.</param>
    /// <returns>A new <see cref="DateTime"/> representing the date after the addition.</returns>
    DateTime Add(DateTime date, decimal amount, TimeUnit unit);

    /// <summary>
    /// Subtracts the specified amount of the given time unit from a date.
    /// </summary>
    /// <param name="date">The base date to subtract from.</param>
    /// <param name="amount">The amount to subtract.</param>
    /// <param name="unit">The unit of the amount to subtract.</param>
    /// <returns>A new <see cref="DateTime"/> representing the date after the subtraction.</returns>
    DateTime Subtract(DateTime date, decimal amount, TimeUnit unit);

    // -------------------------------------------------------------------------
    // Period boundaries
    // -------------------------------------------------------------------------

    /// <summary>
    /// Returns the start of the specified date part period containing the given date.
    /// </summary>
    /// <param name="date">The reference date.</param>
    /// <param name="part">The date part that defines the period boundary.</param>
    /// <param name="weekStart">The day considered the start of the week; only used when <paramref name="part"/> is <see cref="DatePart.Week"/>.</param>
    /// <returns>A <see cref="DateTime"/> representing the first moment of the period.</returns>
    DateTime StartOf(DateTime date, DatePart part, WeekStart? weekStart = null);

    /// <summary>
    /// Returns the end of the specified date part period containing the given date.
    /// </summary>
    /// <param name="date">The reference date.</param>
    /// <param name="part">The date part that defines the period boundary.</param>
    /// <param name="weekStart">The day considered the start of the week; only used when <paramref name="part"/> is <see cref="DatePart.Week"/>.</param>
    /// <returns>A <see cref="DateTime"/> representing the last moment of the period.</returns>
    DateTime EndOf(DateTime date, DatePart part, WeekStart? weekStart = null);

    // -------------------------------------------------------------------------
    // Validations
    // -------------------------------------------------------------------------

    /// <summary>
    /// Determines whether a date is strictly before the reference date.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <param name="reference">The reference date to compare against.</param>
    /// <returns><c>true</c> if <paramref name="date"/> is earlier than <paramref name="reference"/>; otherwise, <c>false</c>.</returns>
    bool IsBefore(DateTime date, DateTime reference);

    /// <summary>
    /// Determines whether a date is strictly after the reference date.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <param name="reference">The reference date to compare against.</param>
    /// <returns><c>true</c> if <paramref name="date"/> is later than <paramref name="reference"/>; otherwise, <c>false</c>.</returns>
    bool IsAfter(DateTime date, DateTime reference);

    /// <summary>
    /// Determines whether two dates fall on the same calendar day.
    /// </summary>
    /// <param name="a">The first date.</param>
    /// <param name="b">The second date.</param>
    /// <returns><c>true</c> if both dates share the same year, month, and day; otherwise, <c>false</c>.</returns>
    bool IsSameDay(DateTime a, DateTime b);

    /// <summary>
    /// Determines whether two dates fall within the same period defined by the specified date part.
    /// </summary>
    /// <param name="a">The first date.</param>
    /// <param name="b">The second date.</param>
    /// <param name="part">The date part that defines the period granularity.</param>
    /// <param name="weekStart">The day considered the start of the week; only used when <paramref name="part"/> is <see cref="DatePart.Week"/>.</param>
    /// <returns><c>true</c> if both dates belong to the same period; otherwise, <c>false</c>.</returns>
    bool IsSamePeriod(DateTime a, DateTime b, DatePart part, WeekStart? weekStart = null);

    /// <summary>
    /// Determines whether the specified date falls on a weekend (Saturday or Sunday).
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns><c>true</c> if the date is Saturday or Sunday; otherwise, <c>false</c>.</returns>
    bool IsWeekend(DateTime date);

    /// <summary>
    /// Determines whether the specified date falls on a weekday (Monday through Friday).
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns><c>true</c> if the date is Monday through Friday; otherwise, <c>false</c>.</returns>
    bool IsWeekday(DateTime date);

    /// <summary>
    /// Determines whether the specified year is a leap year.
    /// </summary>
    /// <param name="year">The four-digit year to evaluate.</param>
    /// <returns><c>true</c> if the year is a leap year; otherwise, <c>false</c>.</returns>
    bool IsLeapYear(int year);

    // -------------------------------------------------------------------------
    // Date information
    // -------------------------------------------------------------------------

    /// <summary>
    /// Returns the number of days in the specified month of the specified year.
    /// </summary>
    /// <param name="year">The four-digit year.</param>
    /// <param name="month">The month number (1–12).</param>
    /// <returns>The number of days in the given month.</returns>
    int DaysInMonth(int year, int month);

    /// <summary>
    /// Returns the ISO or locale week number of the year for the specified date.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <param name="weekStart">The day considered the start of the week; defaults to Monday when null.</param>
    /// <returns>The week number within the year.</returns>
    int WeekOfYear(DateTime date, WeekStart? weekStart = null);

    /// <summary>
    /// Returns the one-based day number within the year for the specified date.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>A value between 1 and 366 representing the day of the year.</returns>
    int DayOfYear(DateTime date);

    // -------------------------------------------------------------------------
    // Quarter - basic
    // -------------------------------------------------------------------------

    /// <summary>
    /// Returns the quarter number (1–4) of the year in which the specified date falls.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>An integer between 1 and 4 representing the quarter.</returns>
    int QuarterOf(DateTime date);

    // -------------------------------------------------------------------------
    // Quarter - extended
    // -------------------------------------------------------------------------

    /// <summary>
    /// Returns the first day of the quarter in which the specified date falls.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>A <see cref="DateTime"/> representing the start of the quarter.</returns>
    DateTime QuarterStart(DateTime date);

    /// <summary>
    /// Returns the last day of the quarter in which the specified date falls.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>A <see cref="DateTime"/> representing the end of the quarter.</returns>
    DateTime QuarterEnd(DateTime date);

    /// <summary>
    /// Returns the abbreviated name of the quarter in which the specified date falls (e.g., "Q1").
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>A short string representing the quarter name.</returns>
    string QuarterName(DateTime date);

    /// <summary>
    /// Returns the full descriptive name of the quarter in which the specified date falls (e.g., "Q1 2026").
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>A full string representing the quarter name including the year.</returns>
    string QuarterNameFull(DateTime date);

    /// <summary>
    /// Returns the total number of days in the quarter in which the specified date falls.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>The total number of days in the quarter.</returns>
    int DaysInQuarter(DateTime date);

    /// <summary>
    /// Returns the number of days that have elapsed since the start of the quarter up to and including the specified date.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>The number of days elapsed within the quarter.</returns>
    int DaysElapsedInQuarter(DateTime date);

    /// <summary>
    /// Returns the number of days remaining in the quarter after the specified date.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>The number of days remaining in the quarter.</returns>
    int DaysRemainingInQuarter(DateTime date);

    /// <summary>
    /// Returns the progress through the current quarter as a decimal fraction between 0 and 1.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>A decimal between 0 and 1 representing the proportion of the quarter that has elapsed.</returns>
    decimal ProgressInQuarter(DateTime date);

    /// <summary>
    /// Determines whether the specified date is the first day of its quarter.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns><c>true</c> if the date is the first day of its quarter; otherwise, <c>false</c>.</returns>
    bool IsFirstDayOfQuarter(DateTime date);

    /// <summary>
    /// Determines whether the specified date is the last day of its quarter.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns><c>true</c> if the date is the last day of its quarter; otherwise, <c>false</c>.</returns>
    bool IsLastDayOfQuarter(DateTime date);

    /// <summary>
    /// Determines whether two dates fall within the same quarter of the same year.
    /// </summary>
    /// <param name="a">The first date.</param>
    /// <param name="b">The second date.</param>
    /// <returns><c>true</c> if both dates belong to the same quarter and year; otherwise, <c>false</c>.</returns>
    bool IsInSameQuarter(DateTime a, DateTime b);

    /// <summary>
    /// Returns the number of complete or partial weeks contained in the quarter in which the specified date falls.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>The number of weeks in the quarter.</returns>
    int WeeksInQuarter(DateTime date);

    /// <summary>
    /// Returns the first day of the quarter that immediately follows the quarter containing the specified date.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>A <see cref="DateTime"/> representing the start of the next quarter.</returns>
    DateTime NextQuarterStart(DateTime date);

    /// <summary>
    /// Returns the first day of the quarter that immediately precedes the quarter containing the specified date.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>A <see cref="DateTime"/> representing the start of the previous quarter.</returns>
    DateTime PreviousQuarterStart(DateTime date);
}
