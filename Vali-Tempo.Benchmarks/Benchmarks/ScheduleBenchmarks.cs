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
    private static readonly DateTime Start = new(2025, 1, 1);

    /// <summary>
    /// Generates 100 occurrences of a daily schedule starting on 2025-01-01.
    /// Tests the inner hot path of IsValidOccurrence for RecurrenceType.Daily.
    /// </summary>
    [Benchmark]
    public int Occurrences_Daily_100()
    {
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Days)
            .StartingFrom(Start);

        int count = 0;
        foreach (var _ in schedule.Occurrences(Start, 100))
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
        // 2025-01-06 is the first Monday of 2025
        var start = new DateTime(2025, 1, 6);
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Weeks)
            .On(DayOfWeek.Monday)
            .StartingFrom(start);

        int count = 0;
        foreach (var _ in schedule.Occurrences(start, 52))
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
        var start = new DateTime(2025, 1, 6); // first Monday of 2025
        var end = new DateTime(2025, 12, 31);

        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Weeks)
            .On(DayOfWeek.Monday, DayOfWeek.Thursday)
            .StartingFrom(start);

        int count = 0;
        foreach (var _ in schedule.OccurrencesInRange(start, end))
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
        var schedule = new ValiSchedule()
            .StartingFrom(Start)
            .WithCustomPredicate(d => d.DayOfWeek == DayOfWeek.Wednesday);

        int count = 0;
        foreach (var _ in schedule.Occurrences(Start, 50))
            count++;
        return count;
    }
}
