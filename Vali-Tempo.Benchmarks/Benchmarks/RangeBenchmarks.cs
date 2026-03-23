using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Vali_Range.Core;
using Vali_Range.Models;
using Vali_Time.Enums;

namespace Vali_Tempo.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks for Vali-Range: covers creation, set-theory operations, and iteration paths.
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
public class RangeBenchmarks
{
    private ValiRange _range = null!;
    private DateRange _rangeA;
    private DateRange _rangeB;
    private DateRange _quarterRange;
    private List<DateRange> _fragmentedRanges = null!;

    [GlobalSetup]
    public void Setup()
    {
        _range = new ValiRange();
        _rangeA = _range.Create(new DateTime(2025, 1, 1), new DateTime(2025, 6, 30));
        _rangeB = _range.Create(new DateTime(2025, 4, 1), new DateTime(2025, 9, 30));
        _quarterRange = _range.ThisQuarter();

        // 12 non-overlapping monthly ranges for Merge benchmark
        _fragmentedRanges = Enumerable.Range(0, 12)
            .Select(i => _range.Create(
                new DateTime(2025, 1, 1).AddMonths(i),
                new DateTime(2025, 1, 31).AddMonths(i)))
            .ToList();
    }

    /// <summary>
    /// Creates a DateRange from two explicit DateTime values.
    /// Measures struct allocation overhead of the factory method.
    /// </summary>
    [Benchmark]
    public DateRange Create_ExplicitRange()
    {
        return _range.Create(new DateTime(2025, 1, 1), new DateTime(2025, 12, 31));
    }

    /// <summary>
    /// Returns a DateRange representing the current calendar quarter.
    /// Exercises StartOf/EndOf quarter logic.
    /// </summary>
    [Benchmark]
    public DateRange ThisQuarter()
    {
        return _range.ThisQuarter();
    }

    /// <summary>
    /// Calculates the intersection of two overlapping date ranges.
    /// </summary>
    [Benchmark]
    public DateRange? Intersection_Overlapping()
    {
        return _range.Intersection(_rangeA, _rangeB);
    }

    /// <summary>
    /// Checks whether rangeA overlaps with rangeB.
    /// </summary>
    [Benchmark]
    public bool Overlaps()
    {
        return _range.Overlaps(_rangeA, _rangeB);
    }

    /// <summary>
    /// Iterates every day in a full quarter (~90 days).
    /// Measures IEnumerable yield overhead for EachDay.
    /// </summary>
    [Benchmark]
    public int EachDay_Quarter()
    {
        int count = 0;
        foreach (var _ in _range.EachDay(_quarterRange))
            count++;
        return count;
    }

    /// <summary>
    /// Iterates each month boundary in a full-year range (12 iterations).
    /// </summary>
    [Benchmark]
    public int EachMonth_FullYear()
    {
        var fullYear = _range.Create(new DateTime(2025, 1, 1), new DateTime(2025, 12, 31));
        int count = 0;
        foreach (var _ in _range.EachMonth(fullYear))
            count++;
        return count;
    }

    /// <summary>
    /// Merges 12 non-overlapping monthly ranges into one contiguous range.
    /// Exercises the sort + sequential merge path.
    /// </summary>
    [Benchmark]
    public int Merge_12MonthlyRanges()
    {
        int count = 0;
        foreach (var _ in _range.Merge(_fragmentedRanges))
            count++;
        return count;
    }

    /// <summary>
    /// Calculates the duration of a range in days.
    /// </summary>
    [Benchmark]
    public decimal Duration_InDays()
    {
        return _rangeA.Duration(TimeUnit.Days);
    }
}
