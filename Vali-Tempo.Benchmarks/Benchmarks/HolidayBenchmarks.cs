using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Vali_Holiday.Core;

namespace Vali_Tempo.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks for Vali-Holiday: measures IsHoliday cache efficiency and GetHolidays throughput.
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
public class HolidayBenchmarks
{
    private ValiHoliday _holidays = null!;
    private DateTime[] _datesYear2025 = null!;
    private DateTime[] _datesMultiYear = null!;

    [GlobalSetup]
    public void Setup()
    {
        _holidays = HolidayProviderFactory.CreateLatinAmerica();

        // All 365 days of 2025
        _datesYear2025 = Enumerable.Range(0, 365)
            .Select(i => new DateTime(2025, 1, 1).AddDays(i))
            .ToArray();

        // Dates spread across 3 different years
        _datesMultiYear = new[]
        {
            new DateTime(2023, 7, 28),
            new DateTime(2023, 12, 25),
            new DateTime(2024, 1, 1),
            new DateTime(2024, 7, 28),
            new DateTime(2024, 12, 25),
            new DateTime(2025, 1, 1),
            new DateTime(2025, 7, 28),
            new DateTime(2025, 12, 25),
            new DateTime(2023, 4, 14),
            new DateTime(2024, 3, 29),
            new DateTime(2025, 4, 18),
        };
    }

    /// <summary>
    /// Calls IsHoliday for all 365 days of 2025 (Peru). After the first call the provider's
    /// internal per-year HashSet cache is warm, so this measures cached lookup throughput.
    /// </summary>
    [Benchmark]
    public int IsHoliday_WithCache()
    {
        int count = 0;
        foreach (var date in _datesYear2025)
        {
            if (_holidays.IsHoliday(date, "PE"))
                count++;
        }
        return count;
    }

    /// <summary>
    /// Retrieves all holidays for Peru for the year 2025 and materialises them into a list.
    /// Exercises the fixed + movable holiday merge and OrderBy path.
    /// </summary>
    [Benchmark]
    public int GetHolidays_Peru_2025()
    {
        var result = _holidays.GetHolidays(2025, "PE");
        int count = 0;
        foreach (var _ in result) count++;
        return count;
    }

    /// <summary>
    /// Calls IsHoliday against dates from 3 different years (2023, 2024, 2025).
    /// Each distinct year triggers a separate HashSet cache entry, measuring multi-year cache behaviour.
    /// </summary>
    [Benchmark]
    public int IsHoliday_MultipleYears()
    {
        int count = 0;
        foreach (var date in _datesMultiYear)
        {
            if (_holidays.IsHoliday(date, "PE"))
                count++;
        }
        return count;
    }
}
