using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Vali_CountDown.Core;
using Vali_Time.Enums;

namespace Vali_Tempo.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks for Vali-CountDown: covers expiry detection, progress, breakdown,
/// and formatting. Fixed deadlines are used so results are deterministic.
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
public class CountdownBenchmarks
{
    private ValiCountdown _countdown = null!;

    // Fixed future deadline relative to a reference date well in the past
    private static readonly DateTime FutureDeadline = new(2099, 12, 31, 23, 59, 59);
    private static readonly DateTime PastDeadline   = new(2000, 1, 1, 0, 0, 0);
    private static readonly DateTime StartDate      = new(2025, 1, 1);

    [GlobalSetup]
    public void Setup()
    {
        _countdown = new ValiCountdown();
    }

    /// <summary>
    /// Checks whether a future deadline has expired (uses DateTime.Now internally).
    /// </summary>
    [Benchmark]
    public bool IsExpired_Future()
    {
        return _countdown.IsExpired(FutureDeadline);
    }

    /// <summary>
    /// Checks whether a past deadline has expired.
    /// </summary>
    [Benchmark]
    public bool IsExpired_Past()
    {
        return _countdown.IsExpired(PastDeadline);
    }

    /// <summary>
    /// Calculates time remaining until a future deadline in hours.
    /// </summary>
    [Benchmark]
    public decimal TimeUntil_Hours()
    {
        return _countdown.TimeUntil(FutureDeadline, TimeUnit.Hours);
    }

    /// <summary>
    /// Calculates time elapsed since a past start date in days.
    /// </summary>
    [Benchmark]
    public decimal TimeElapsed_Days()
    {
        return _countdown.TimeElapsed(StartDate, TimeUnit.Days);
    }

    /// <summary>
    /// Calculates fractional progress (0.0–1.0) between a start and a future end date.
    /// </summary>
    [Benchmark]
    public decimal Progress()
    {
        return _countdown.Progress(StartDate, FutureDeadline);
    }

    /// <summary>
    /// Decomposes remaining time into hours, minutes, seconds, and milliseconds.
    /// Allocates one Dictionary per call.
    /// </summary>
    [Benchmark]
    public Dictionary<TimeUnit, decimal> Breakdown()
    {
        return _countdown.Breakdown(FutureDeadline);
    }

    /// <summary>
    /// Formats remaining time as a human-readable string (e.g. "27393d 23h 59m").
    /// Exercises the string allocation and unit-selection path.
    /// </summary>
    [Benchmark]
    public string Format_FutureDeadline()
    {
        return _countdown.Format(FutureDeadline);
    }

    /// <summary>
    /// Checks whether the deadline is within a 7-day threshold.
    /// </summary>
    [Benchmark]
    public bool IsWithin_7Days()
    {
        return _countdown.IsWithin(FutureDeadline, 7m, TimeUnit.Days);
    }
}
