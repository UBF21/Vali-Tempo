using System.Globalization;
using Vali_Calendar.Models;
using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Time.Abstractions;
using Vali_Time.Enums;

namespace Vali_Calendar.Core;

/// <summary>
/// Provides calendar-related operations including week, month, workday, and year calculations.
/// Optionally integrates with an <see cref="IHolidayProvider"/> to exclude public holidays from workday logic.
/// </summary>
public class ValiCalendar : IValiCalendar
{
    private readonly WeekStart _defaultWeekStart;
    private readonly IHolidayProvider? _holidayProvider;
    private readonly IClock _clock;

    /// <summary>
    /// Initializes a new instance of <see cref="ValiCalendar"/> with an optional default week start day,
    /// an optional holiday provider, and an optional clock abstraction.
    /// </summary>
    /// <param name="defaultWeekStart">The default first day of the week. Defaults to <see cref="WeekStart.Monday"/>.</param>
    /// <param name="holidayProvider">
    /// An optional <see cref="IHolidayProvider"/> used to exclude public holidays from workday calculations.
    /// When <see langword="null"/>, workday logic is based solely on weekdays (Monday–Friday).
    /// </param>
    /// <param name="clock">
    /// Optional clock abstraction. Defaults to <see cref="SystemClock.Instance"/> when <c>null</c>.
    /// Inject a test double to control time in unit tests.
    /// </param>
    public ValiCalendar(WeekStart defaultWeekStart = WeekStart.Monday, IHolidayProvider? holidayProvider = null, IClock? clock = null)
    {
        _defaultWeekStart = defaultWeekStart;
        _holidayProvider = holidayProvider;
        _clock = clock ?? SystemClock.Instance;
    }

    // === WEEKS ===

    /// <summary>
    /// Returns the <see cref="CalendarWeek"/> that contains the specified date.
    /// The week number follows ISO 8601 when <paramref name="weekStart"/> is <see cref="WeekStart.Monday"/>,
    /// or uses <see cref="GregorianCalendar"/> rules when it is <see cref="WeekStart.Sunday"/>.
    /// </summary>
    /// <remarks>
    /// For Sunday-based weeks, the year always equals the calendar year of the date
    /// (<see cref="CalendarWeekRule.FirstDay"/> is used, which starts a new week 1 on every January 1st).
    /// </remarks>
    /// <param name="date">The date to evaluate.</param>
    /// <param name="weekStart">The day the week starts on. Uses the instance default if not specified.</param>
    public CalendarWeek WeekOf(DateTime date, WeekStart? weekStart = null)
    {
        var ws = weekStart ?? _defaultWeekStart;
        var firstDay = ws == WeekStart.Sunday ? DayOfWeek.Sunday : DayOfWeek.Monday;

        int diff = ((7 + (int)date.DayOfWeek - (int)firstDay) % 7);
        DateTime start = date.Date.AddDays(-diff);
        DateTime end = start.AddDays(6);

        int weekNumber;
        int year;

        if (ws == WeekStart.Monday)
        {
            weekNumber = ISOWeek.GetWeekOfYear(date);
            year = ISOWeek.GetYear(date);
        }
        else
        {
            weekNumber = new System.Globalization.GregorianCalendar().GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
            year = date.Year;
        }

        return new CalendarWeek(weekNumber, year, start, end);
    }

    /// <summary>
    /// Returns the <see cref="CalendarWeek"/> for the current date.
    /// </summary>
    /// <param name="weekStart">The day the week starts on. Uses the instance default if not specified.</param>
    public CalendarWeek CurrentWeek(WeekStart? weekStart = null)
        => WeekOf(_clock.Today, weekStart);

    /// <summary>
    /// Returns all calendar weeks that overlap with the specified month.
    /// </summary>
    /// <param name="year">The year of the month.</param>
    /// <param name="month">The month number (1-12).</param>
    /// <param name="weekStart">The day the week starts on. Uses the instance default if not specified.</param>
    public IEnumerable<CalendarWeek> WeeksInMonth(int year, int month, WeekStart? weekStart = null)
    {
        var firstDay = new DateTime(year, month, 1);
        var lastDay = new DateTime(year, month, DateTime.DaysInMonth(year, month));

        var seen = new HashSet<(int, int)>();
        var result = new List<CalendarWeek>();

        for (var d = firstDay; d <= lastDay; d = d.AddDays(1))
        {
            var week = WeekOf(d, weekStart);
            var key = (week.WeekNumber, week.Year);
            if (seen.Add(key))
                result.Add(week);
        }

        return result;
    }

    /// <summary>
    /// Returns the number of calendar weeks (complete or partial) that overlap with the specified month.
    /// </summary>
    /// <param name="year">The year of the month.</param>
    /// <param name="month">The month number (1-12).</param>
    /// <param name="weekStart">The day the week starts on. Uses the instance default if not specified.</param>
    public int WeekCountInMonth(int year, int month, WeekStart? weekStart = null)
        => WeeksInMonth(year, month, weekStart).Count();

    // === MONTHS ===

    /// <summary>
    /// Returns <c>true</c> if the specified date is the first day of its month.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    public bool IsFirstDayOfMonth(DateTime date)
        => date.Day == 1;

    /// <summary>
    /// Returns <c>true</c> if the specified date is the last day of its month.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    public bool IsLastDayOfMonth(DateTime date)
        => date.Day == DateTime.DaysInMonth(date.Year, date.Month);

    /// <summary>
    /// Returns the total number of days in the month of the specified date.
    /// </summary>
    /// <param name="date">The date whose month is evaluated.</param>
    public int DaysInMonth(DateTime date)
        => DateTime.DaysInMonth(date.Year, date.Month);

    /// <summary>
    /// Returns all dates within the specified month, from the first to the last day.
    /// </summary>
    /// <param name="year">The year of the month.</param>
    /// <param name="month">The month number (1-12).</param>
    public IEnumerable<DateTime> DaysOfMonth(int year, int month)
    {
        int totalDays = DateTime.DaysInMonth(year, month);
        for (int d = 1; d <= totalDays; d++)
            yield return new DateTime(year, month, d);
    }

    // === WORKDAYS (weekends excluded; holidays excluded when provider is configured) ===

    private const int MaxWorkdayScan = 3660; // 10 years

    /// <summary>
    /// Returns <c>true</c> if the specified date is a workday.
    /// A workday is a weekday (Monday–Friday) that is not a public holiday
    /// (when an <see cref="IHolidayProvider"/> is configured).
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    public bool IsWorkday(DateTime date)
    {
        if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) return false;
        if (_holidayProvider != null && _holidayProvider.IsHoliday(date)) return false;
        return true;
    }

    /// <summary>
    /// Returns the number of workdays between two dates, inclusive of both endpoints.
    /// Iterates day by day between <paramref name="from"/> and <paramref name="to"/>.
    /// </summary>
    /// <param name="from">The start date.</param>
    /// <param name="to">The end date.</param>
    public int WorkdaysBetween(DateTime from, DateTime to)
    {
        if (from.Date > to.Date)
            throw new ArgumentException("from must be less than or equal to to.", nameof(from));

        if ((to.Date - from.Date).Days > MaxWorkdayScan)
            throw new InvalidOperationException($"Date span exceeds maximum scan limit of {MaxWorkdayScan} days.");

        int count = 0;
        for (var d = from.Date; d <= to.Date; d = d.AddDays(1))
        {
            if (IsWorkday(d))
                count++;
        }
        return count;
    }

    /// <summary>
    /// Adds the specified number of workdays to a date, skipping weekends and public holidays.
    /// Iterates one day at a time until the required number of workdays has been accumulated.
    /// </summary>
    /// <param name="date">The starting date.</param>
    /// <param name="workdays">The number of workdays to add.</param>
    public DateTime AddWorkdays(DateTime date, int workdays)
    {
        var result = date.Date;
        int added = 0;
        int step = workdays >= 0 ? 1 : -1;
        int target = Math.Abs(workdays);
        int iterations = 0;

        while (added < target)
        {
            if (++iterations > MaxWorkdayScan)
                throw new InvalidOperationException(
                    $"AddWorkdays exceeded the maximum scan limit of {MaxWorkdayScan} days. " +
                    "This may indicate an unbounded holiday configuration with no workdays in the scan window.");
            result = result.AddDays(step);
            if (IsWorkday(result))
                added++;
        }

        return result;
    }

    /// <summary>
    /// Returns the number of workdays in the specified month.
    /// </summary>
    /// <param name="year">The year of the month.</param>
    /// <param name="month">The month number (1-12).</param>
    public int WorkdaysInMonth(int year, int month)
    {
        var first = new DateTime(year, month, 1);
        var last = new DateTime(year, month, DateTime.DaysInMonth(year, month));
        return WorkdaysBetween(first, last);
    }

    /// <summary>
    /// Returns the total number of workdays in the specified year.
    /// </summary>
    /// <param name="year">The year to evaluate.</param>
    public int WorkdaysInYear(int year)
    {
        var first = new DateTime(year, 1, 1);
        var last = new DateTime(year, 12, 31);
        return WorkdaysBetween(first, last);
    }

    /// <summary>
    /// Returns the next workday on or after the specified date.
    /// If the date is already a workday, it is returned as-is.
    /// </summary>
    /// <param name="date">The reference date.</param>
    public DateTime NextWorkday(DateTime date)
    {
        var result = date.Date;
        int iterations = 0;
        while (!IsWorkday(result))
        {
            if (++iterations > MaxWorkdayScan)
                throw new InvalidOperationException(
                    $"NextWorkday exceeded the maximum scan limit of {MaxWorkdayScan} days. " +
                    "This may indicate an unbounded holiday configuration with no workdays in the scan window.");
            result = result.AddDays(1);
        }
        return result;
    }

    /// <summary>
    /// Returns the most recent workday on or before the specified date.
    /// If the date is already a workday, it is returned as-is.
    /// </summary>
    /// <param name="date">The reference date.</param>
    public DateTime PreviousWorkday(DateTime date)
    {
        var result = date.Date;
        int iterations = 0;
        while (!IsWorkday(result))
        {
            if (++iterations > MaxWorkdayScan)
                throw new InvalidOperationException(
                    $"PreviousWorkday exceeded the maximum scan limit of {MaxWorkdayScan} days. " +
                    "This may indicate an unbounded holiday configuration with no workdays in the scan window.");
            result = result.AddDays(-1);
        }
        return result;
    }

    // === HOLIDAY INTEGRATION ===

    /// <summary>
    /// Gets a value indicating whether this instance has an <see cref="IHolidayProvider"/> configured.
    /// </summary>
    public bool HasHolidayProvider => _holidayProvider != null;

    /// <summary>
    /// Returns all public holidays in the specified month.
    /// </summary>
    /// <param name="year">The four-digit calendar year.</param>
    /// <param name="month">The month number (1-12).</param>
    /// <returns>A sequence of <see cref="HolidayInfo"/> for the given month.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no <see cref="IHolidayProvider"/> has been configured.
    /// Use the <see cref="ValiCalendar(WeekStart, IHolidayProvider?, IClock?)"/> constructor overload to supply one.
    /// </exception>
    public IEnumerable<HolidayInfo> HolidaysInMonth(int year, int month)
    {
        if (_holidayProvider == null)
            throw new InvalidOperationException(
                "No holiday provider has been configured. Pass an IHolidayProvider to the ValiCalendar constructor.");

        return _holidayProvider.GetHolidays(year).Where(h => h.Month == month);
    }

    /// <summary>
    /// Returns all public holidays in the specified year.
    /// </summary>
    /// <param name="year">The four-digit calendar year.</param>
    /// <returns>A sequence of <see cref="HolidayInfo"/> for the given year.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no <see cref="IHolidayProvider"/> has been configured.
    /// Use the <see cref="ValiCalendar(WeekStart, IHolidayProvider?, IClock?)"/> constructor overload to supply one.
    /// </exception>
    public IEnumerable<HolidayInfo> HolidaysInYear(int year)
    {
        if (_holidayProvider == null)
            throw new InvalidOperationException(
                "No holiday provider has been configured. Pass an IHolidayProvider to the ValiCalendar constructor.");

        return _holidayProvider.GetHolidays(year);
    }

    // === YEARS ===

    /// <summary>
    /// Returns <c>true</c> if the specified year is a leap year.
    /// </summary>
    /// <param name="year">The year to evaluate.</param>
    public bool IsLeapYear(int year)
        => DateTime.IsLeapYear(year);

    /// <summary>
    /// Returns the total number of days in the specified year (365 for regular years, 366 for leap years).
    /// </summary>
    /// <param name="year">The year to evaluate.</param>
    public int DaysInYear(int year)
        => DateTime.IsLeapYear(year) ? 366 : 365;
}
