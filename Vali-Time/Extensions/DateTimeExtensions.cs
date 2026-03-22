using System.Globalization;
using Vali_Time.Enums;

namespace Vali_Time.Extensions;

/// <summary>
/// Extension methods for <see cref="DateTime"/> that provide period boundaries, calendar
/// utilities, and common date comparisons.
/// </summary>
public static class DateTimeExtensions
{
    // =========================================================================
    // WEEK BOUNDARIES
    // =========================================================================

    /// <summary>
    /// Returns the first instant (midnight) of the week that contains <paramref name="date"/>.
    /// </summary>
    /// <param name="date">The reference date.</param>
    /// <param name="weekStart">
    /// The day considered the first day of the week. Defaults to <see cref="WeekStart.Monday"/>.
    /// </param>
    /// <returns>
    /// A <see cref="DateTime"/> set to 00:00:00.000 on the first day of the week.
    /// </returns>
    public static DateTime StartOfWeek(this DateTime date, WeekStart weekStart = WeekStart.Monday)
    {
        DayOfWeek firstDay = weekStart == WeekStart.Monday ? DayOfWeek.Monday : DayOfWeek.Sunday;
        int diff = (7 + (date.DayOfWeek - firstDay)) % 7;
        return date.Date.AddDays(-diff);
    }

    /// <summary>
    /// Returns the last instant of the week that contains <paramref name="date"/>.
    /// </summary>
    /// <param name="date">The reference date.</param>
    /// <param name="weekStart">
    /// The day considered the first day of the week. Defaults to <see cref="WeekStart.Monday"/>.
    /// </param>
    /// <returns>
    /// A <see cref="DateTime"/> set to 23:59:59.999 on the last day of the week.
    /// </returns>
    public static DateTime EndOfWeek(this DateTime date, WeekStart weekStart = WeekStart.Monday)
    {
        return date.StartOfWeek(weekStart).AddDays(6) + new TimeSpan(0, 23, 59, 59, 999);
    }

    // =========================================================================
    // MONTH BOUNDARIES
    // =========================================================================

    /// <summary>
    /// Returns the first instant (midnight) of the month that contains <paramref name="date"/>.
    /// </summary>
    /// <param name="date">The reference date.</param>
    /// <returns>A <see cref="DateTime"/> set to 00:00:00.000 on the first day of the month.</returns>
    public static DateTime StartOfMonth(this DateTime date)
    {
        return new DateTime(date.Year, date.Month, 1);
    }

    /// <summary>
    /// Returns the last instant of the month that contains <paramref name="date"/>.
    /// </summary>
    /// <param name="date">The reference date.</param>
    /// <returns>
    /// A <see cref="DateTime"/> set to 23:59:59.999 on the last day of the month.
    /// </returns>
    public static DateTime EndOfMonth(this DateTime date)
    {
        int lastDay = DateTime.DaysInMonth(date.Year, date.Month);
        return new DateTime(date.Year, date.Month, lastDay) + new TimeSpan(0, 23, 59, 59, 999);
    }

    // =========================================================================
    // YEAR BOUNDARIES
    // =========================================================================

    /// <summary>
    /// Returns the first instant (midnight) of the year that contains <paramref name="date"/>.
    /// </summary>
    /// <param name="date">The reference date.</param>
    /// <returns>A <see cref="DateTime"/> set to 00:00:00.000 on January 1st of the year.</returns>
    public static DateTime StartOfYear(this DateTime date)
    {
        return new DateTime(date.Year, 1, 1);
    }

    /// <summary>
    /// Returns the last instant of the year that contains <paramref name="date"/>.
    /// </summary>
    /// <param name="date">The reference date.</param>
    /// <returns>
    /// A <see cref="DateTime"/> set to 23:59:59.999 on December 31st of the year.
    /// </returns>
    public static DateTime EndOfYear(this DateTime date)
    {
        return new DateTime(date.Year, 12, 31) + new TimeSpan(0, 23, 59, 59, 999);
    }

    // =========================================================================
    // QUARTER BOUNDARIES
    // =========================================================================

    /// <summary>
    /// Returns the first instant (midnight) of the quarter that contains <paramref name="date"/>.
    /// </summary>
    /// <param name="date">The reference date.</param>
    /// <returns>
    /// A <see cref="DateTime"/> set to 00:00:00.000 on the first day of the quarter
    /// (January 1, April 1, July 1, or October 1).
    /// </returns>
    public static DateTime StartOfQuarter(this DateTime date)
    {
        int firstMonth = (date.QuarterOfYear() - 1) * 3 + 1;
        return new DateTime(date.Year, firstMonth, 1);
    }

    /// <summary>
    /// Returns the last instant of the quarter that contains <paramref name="date"/>.
    /// </summary>
    /// <param name="date">The reference date.</param>
    /// <returns>
    /// A <see cref="DateTime"/> set to 23:59:59.999 on the last day of the quarter
    /// (March 31, June 30, September 30, or December 31).
    /// </returns>
    public static DateTime EndOfQuarter(this DateTime date)
    {
        int lastMonth = date.QuarterOfYear() * 3;
        int lastDay   = DateTime.DaysInMonth(date.Year, lastMonth);
        return new DateTime(date.Year, lastMonth, lastDay) + new TimeSpan(0, 23, 59, 59, 999);
    }

    // =========================================================================
    // WEEKDAY / WEEKEND
    // =========================================================================

    /// <summary>
    /// Determines whether <paramref name="date"/> falls on a weekend (Saturday or Sunday).
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns><c>true</c> if the day is Saturday or Sunday.</returns>
    public static bool IsWeekend(this DateTime date)
    {
        return date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
    }

    /// <summary>
    /// Determines whether <paramref name="date"/> falls on a weekday (Monday through Friday).
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns><c>true</c> if the day is Monday through Friday.</returns>
    public static bool IsWeekday(this DateTime date)
    {
        return !date.IsWeekend();
    }

    // =========================================================================
    // CALENDAR INFO
    // =========================================================================

    /// <summary>
    /// Returns the ISO 8601 or calendar week number (1–53) for <paramref name="date"/>.
    /// </summary>
    /// <remarks>
    /// When <paramref name="weekStart"/> is <see cref="WeekStart.Monday"/>,
    /// <see cref="ISOWeek.GetWeekOfYear"/> is used, which follows ISO 8601.
    /// When <paramref name="weekStart"/> is <see cref="WeekStart.Sunday"/>, the
    /// Gregorian calendar with <see cref="CalendarWeekRule.FirstDay"/> is used instead.
    /// </remarks>
    /// <param name="date">The date whose week number is required.</param>
    /// <param name="weekStart">
    /// The day considered the first day of the week. Defaults to <see cref="WeekStart.Monday"/>.
    /// </param>
    /// <returns>The week-of-year number (1-based).</returns>
    public static int WeekOfYear(this DateTime date, WeekStart weekStart = WeekStart.Monday)
    {
        if (weekStart == WeekStart.Monday)
            return ISOWeek.GetWeekOfYear(date);

        Calendar calendar = new GregorianCalendar();
        return calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
    }

    /// <summary>
    /// Returns the quarter (1–4) that <paramref name="date"/> belongs to.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>An integer from 1 to 4 representing the calendar quarter.</returns>
    public static int QuarterOfYear(this DateTime date)
    {
        return (date.Month - 1) / 3 + 1;
    }

    // =========================================================================
    // SAME-PERIOD COMPARISONS
    // =========================================================================

    /// <summary>
    /// Determines whether <paramref name="date"/> and <paramref name="other"/> fall on the
    /// same calendar day.
    /// </summary>
    /// <param name="date">The first date.</param>
    /// <param name="other">The second date.</param>
    /// <returns><c>true</c> if both dates share the same year, month, and day.</returns>
    public static bool IsSameDay(this DateTime date, DateTime other)
    {
        return date.Date == other.Date;
    }

    /// <summary>
    /// Determines whether <paramref name="date"/> and <paramref name="other"/> fall within
    /// the same calendar month and year.
    /// </summary>
    /// <param name="date">The first date.</param>
    /// <param name="other">The second date.</param>
    /// <returns><c>true</c> if both dates share the same year and month.</returns>
    public static bool IsSameMonth(this DateTime date, DateTime other)
    {
        return date.Year == other.Year && date.Month == other.Month;
    }

    /// <summary>
    /// Determines whether <paramref name="date"/> and <paramref name="other"/> fall within
    /// the same calendar year.
    /// </summary>
    /// <param name="date">The first date.</param>
    /// <param name="other">The second date.</param>
    /// <returns><c>true</c> if both dates share the same year.</returns>
    public static bool IsSameYear(this DateTime date, DateTime other)
    {
        return date.Year == other.Year;
    }

    // =========================================================================
    // DAY BOUNDARIES
    // =========================================================================

    /// <summary>
    /// Returns the first instant (midnight) of the day represented by <paramref name="date"/>.
    /// </summary>
    /// <param name="date">The reference date.</param>
    /// <returns>A <see cref="DateTime"/> set to 00:00:00.000 on the same calendar day.</returns>
    public static DateTime StartOfDay(this DateTime date)
    {
        return date.Date;
    }

    /// <summary>
    /// Returns the last instant of the day represented by <paramref name="date"/>.
    /// </summary>
    /// <param name="date">The reference date.</param>
    /// <returns>A <see cref="DateTime"/> set to 23:59:59.999 on the same calendar day.</returns>
    public static DateTime EndOfDay(this DateTime date)
    {
        return date.Date + new TimeSpan(0, 23, 59, 59, 999);
    }
}
