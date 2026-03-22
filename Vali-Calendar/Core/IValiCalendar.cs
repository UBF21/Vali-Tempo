using Vali_Calendar.Models;
using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Time.Enums;

namespace Vali_Calendar.Core;

/// <summary>
/// Defines calendar-related operations including week, month, workday, and year calculations.
/// Optionally integrates with an <see cref="IHolidayProvider"/> to exclude public holidays from workday logic.
/// </summary>
public interface IValiCalendar
{
    // === WEEKS ===

    /// <summary>
    /// Returns the <see cref="CalendarWeek"/> that contains the specified date.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <param name="weekStart">The day the week starts on. Uses the instance default if not specified.</param>
    CalendarWeek WeekOf(DateTime date, WeekStart? weekStart = null);

    /// <summary>
    /// Returns the <see cref="CalendarWeek"/> for the current date.
    /// </summary>
    /// <param name="weekStart">The day the week starts on. Uses the instance default if not specified.</param>
    CalendarWeek CurrentWeek(WeekStart? weekStart = null);

    /// <summary>
    /// Returns all calendar weeks that overlap with the specified month.
    /// </summary>
    /// <param name="year">The year of the month.</param>
    /// <param name="month">The month number (1-12).</param>
    /// <param name="weekStart">The day the week starts on. Uses the instance default if not specified.</param>
    IEnumerable<CalendarWeek> WeeksInMonth(int year, int month, WeekStart? weekStart = null);

    /// <summary>
    /// Returns the number of calendar weeks (complete or partial) that overlap with the specified month.
    /// </summary>
    /// <param name="year">The year of the month.</param>
    /// <param name="month">The month number (1-12).</param>
    /// <param name="weekStart">The day the week starts on. Uses the instance default if not specified.</param>
    int WeekCountInMonth(int year, int month, WeekStart? weekStart = null);

    // === MONTHS ===

    /// <summary>
    /// Returns <c>true</c> if the specified date is the first day of its month.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    bool IsFirstDayOfMonth(DateTime date);

    /// <summary>
    /// Returns <c>true</c> if the specified date is the last day of its month.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    bool IsLastDayOfMonth(DateTime date);

    /// <summary>
    /// Returns the total number of days in the month of the specified date.
    /// </summary>
    /// <param name="date">The date whose month is evaluated.</param>
    int DaysInMonth(DateTime date);

    /// <summary>
    /// Returns all dates within the specified month.
    /// </summary>
    /// <param name="year">The year of the month.</param>
    /// <param name="month">The month number (1-12).</param>
    IEnumerable<DateTime> DaysOfMonth(int year, int month);

    // === WORKDAYS (weekends excluded; holidays excluded when provider is configured) ===

    /// <summary>
    /// Returns <c>true</c> if the specified date is a workday.
    /// A workday is a weekday (Monday–Friday) that is not a public holiday
    /// (when an <see cref="IHolidayProvider"/> is configured).
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    bool IsWorkday(DateTime date);

    /// <summary>
    /// Returns the number of workdays between two dates, inclusive of both endpoints.
    /// </summary>
    /// <param name="from">The start date.</param>
    /// <param name="to">The end date.</param>
    int WorkdaysBetween(DateTime from, DateTime to);

    /// <summary>
    /// Adds the specified number of workdays to a date, skipping weekends and public holidays.
    /// </summary>
    /// <param name="date">The starting date.</param>
    /// <param name="workdays">The number of workdays to add.</param>
    DateTime AddWorkdays(DateTime date, int workdays);

    /// <summary>
    /// Returns the number of workdays in the specified month.
    /// </summary>
    /// <param name="year">The year of the month.</param>
    /// <param name="month">The month number (1-12).</param>
    int WorkdaysInMonth(int year, int month);

    /// <summary>
    /// Returns the total number of workdays in the specified year.
    /// </summary>
    /// <param name="year">The year to evaluate.</param>
    int WorkdaysInYear(int year);

    /// <summary>
    /// Returns the next workday on or after the specified date.
    /// If the date is already a workday, it is returned as-is.
    /// </summary>
    /// <param name="date">The reference date.</param>
    DateTime NextWorkday(DateTime date);

    /// <summary>
    /// Returns the most recent workday on or before the specified date.
    /// If the date is already a workday, it is returned as-is.
    /// </summary>
    /// <param name="date">The reference date.</param>
    DateTime PreviousWorkday(DateTime date);

    // === HOLIDAY INTEGRATION ===

    /// <summary>
    /// Gets a value indicating whether this instance has an <see cref="IHolidayProvider"/> configured.
    /// </summary>
    bool HasHolidayProvider { get; }

    /// <summary>
    /// Returns all public holidays in the specified month.
    /// </summary>
    /// <param name="year">The four-digit calendar year.</param>
    /// <param name="month">The month number (1-12).</param>
    /// <returns>A sequence of <see cref="HolidayInfo"/> for the given month.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no <see cref="IHolidayProvider"/> has been configured.
    /// </exception>
    IEnumerable<HolidayInfo> HolidaysInMonth(int year, int month);

    /// <summary>
    /// Returns all public holidays in the specified year.
    /// </summary>
    /// <param name="year">The four-digit calendar year.</param>
    /// <returns>A sequence of <see cref="HolidayInfo"/> for the given year.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no <see cref="IHolidayProvider"/> has been configured.
    /// </exception>
    IEnumerable<HolidayInfo> HolidaysInYear(int year);

    // === YEARS ===

    /// <summary>
    /// Returns <c>true</c> if the specified year is a leap year.
    /// </summary>
    /// <param name="year">The year to evaluate.</param>
    bool IsLeapYear(int year);

    /// <summary>
    /// Returns the total number of days in the specified year (365 or 366).
    /// </summary>
    /// <param name="year">The year to evaluate.</param>
    int DaysInYear(int year);
}
