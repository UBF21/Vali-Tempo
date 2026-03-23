using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Vali_Calendar.Core;
using Vali_Calendar.Models;
using Vali_Holiday.Core;
using Vali_Holiday.Providers.LatinAmerica;
using Vali_Time.Enums;

namespace Vali_Tempo.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks for Vali-Calendar: covers workday arithmetic, week calculations,
/// and the optional holiday-provider integration path.
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
public class CalendarBenchmarks
{
    private ValiCalendar _calendar = null!;
    private ValiCalendar _calendarWithHolidays = null!;
    private static readonly DateTime OrderDate = new(2025, 3, 17); // Monday
    private static readonly DateTime YearStart = new(2025, 1, 1);
    private static readonly DateTime YearEnd = new(2025, 12, 31);

    [GlobalSetup]
    public void Setup()
    {
        _calendar = new ValiCalendar();
        _calendarWithHolidays = new ValiCalendar(WeekStart.Monday, new PeruHolidayProvider());
    }

    /// <summary>
    /// Counts workdays between January 1 and December 31, 2025 (no holiday provider).
    /// Exercises the inner date-walk loop across ~260 workdays.
    /// </summary>
    [Benchmark]
    public int WorkdaysBetween_FullYear()
    {
        return _calendar.WorkdaysBetween(YearStart, YearEnd);
    }

    /// <summary>
    /// Counts workdays for the full year with a holiday provider registered (Peru).
    /// Each workday candidate hits the O(1) HashSet lookup path.
    /// </summary>
    [Benchmark]
    public int WorkdaysBetween_FullYear_WithHolidays()
    {
        return _calendarWithHolidays.WorkdaysBetween(YearStart, YearEnd);
    }

    /// <summary>
    /// Advances a date by 10 workdays, skipping weekends.
    /// Exercises the forward-walk loop in AddWorkdays.
    /// </summary>
    [Benchmark]
    public DateTime AddWorkdays_10()
    {
        return _calendar.AddWorkdays(OrderDate, 10);
    }

    /// <summary>
    /// Advances a date by 10 workdays with a holiday provider.
    /// Each candidate day triggers an additional IsHoliday lookup.
    /// </summary>
    [Benchmark]
    public DateTime AddWorkdays_10_WithHolidays()
    {
        return _calendarWithHolidays.AddWorkdays(OrderDate, 10);
    }

    /// <summary>
    /// Returns the CalendarWeek metadata for a given date.
    /// Exercises the GregorianCalendar static instance path.
    /// </summary>
    [Benchmark]
    public CalendarWeek WeekOf()
    {
        return _calendar.WeekOf(OrderDate);
    }

    /// <summary>
    /// Counts total workdays in a specific month (March 2025 = 21 workdays).
    /// </summary>
    [Benchmark]
    public int WorkdaysInMonth_March2025()
    {
        return _calendar.WorkdaysInMonth(2025, 3);
    }

    /// <summary>
    /// Counts total workdays in the full year 2025 without holiday provider.
    /// </summary>
    [Benchmark]
    public int WorkdaysInYear_2025()
    {
        return _calendar.WorkdaysInYear(2025);
    }
}
