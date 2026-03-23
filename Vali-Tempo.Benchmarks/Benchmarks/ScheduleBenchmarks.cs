using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Vali_Schedule.Core;
using Vali_Time.Enums;

namespace Vali_Tempo.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks for Vali-Schedule: measures Occurrences generation speed and
/// OccurrencesInRange performance, documenting the O(n) incremental counter fix.
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
public class ScheduleBenchmarks
{
    private static readonly DateTime Start        = new(2025, 1, 1);
    private static readonly DateTime WeeklyStart  = new(2025, 1, 6); // first Monday of 2025
    private static readonly DateTime RangeEnd     = new(2025, 12, 31);

    private ValiSchedule _dailySchedule      = null!;
    private ValiSchedule _weeklySchedule     = null!;
    private ValiSchedule _rangeSchedule      = null!;
    private ValiSchedule _customSchedule     = null!;

    [GlobalSetup]
    public void Setup()
    {
        _dailySchedule = new ValiSchedule()
            .Every(1, TimeUnit.Days)
            .StartingFrom(Start);

        _weeklySchedule = new ValiSchedule()
            .Every(1, TimeUnit.Weeks)
            .On(DayOfWeek.Monday)
            .StartingFrom(WeeklyStart);

        _rangeSchedule = new ValiSchedule()
            .Every(1, TimeUnit.Weeks)
            .On(DayOfWeek.Monday, DayOfWeek.Thursday)
            .StartingFrom(WeeklyStart);

        _customSchedule = new ValiSchedule()
            .StartingFrom(Start)
            .WithCustomPredicate(d => d.DayOfWeek == DayOfWeek.Wednesday);
    }

    /// <summary>
    /// Generates 100 occurrences of a daily schedule starting on 2025-01-01.
    /// Tests the inner hot path of IsValidOccurrence for RecurrenceType.Daily.
    /// </summary>
    [Benchmark]
    public int Occurrences_Daily_100()
    {
        int count = 0;
        foreach (var _ in _dailySchedule.Occurrences(Start, 100))
            count++;
        return count;
    }

    /// <summary>
    /// Generates 52 weekly occurrences (roughly one year) on Mondays starting 2025-01-06.
    /// Tests RecurrenceType.Weekly with a DaysOfWeek filter.
    /// </summary>
    [Benchmark]
    public int Occurrences_Weekly_52()
    {
        int count = 0;
        foreach (var _ in _weeklySchedule.Occurrences(WeeklyStart, 52))
            count++;
        return count;
    }

    /// <summary>
    /// Enumerates all weekly occurrences in a full year range (2025-01-01 to 2025-12-31)
    /// for a schedule that fires every Monday and Thursday.
    /// Exercises OccurrencesInRange without a max-occurrence cap.
    /// </summary>
    [Benchmark]
    public int OccurrencesInRange_OneYear()
    {
        int count = 0;
        foreach (var _ in _rangeSchedule.OccurrencesInRange(WeeklyStart, RangeEnd))
            count++;
        return count;
    }

    /// <summary>
    /// Generates 50 occurrences using a custom predicate that selects only Wednesdays.
    /// Tests the RecurrenceType.Custom code path and delegate invocation overhead.
    /// </summary>
    [Benchmark]
    public int CustomPredicate_Wednesdays_50()
    {
        int count = 0;
        foreach (var _ in _customSchedule.Occurrences(Start, 50))
            count++;
        return count;
    }
}
