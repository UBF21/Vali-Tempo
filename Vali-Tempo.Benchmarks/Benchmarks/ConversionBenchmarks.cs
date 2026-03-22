using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Vali_Duration.Models;
using Vali_Time.Core;
using Vali_Time.Enums;

namespace Vali_Tempo.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks for ValiTime.Convert and ValiDuration arithmetic.
/// Compares ValiDuration addition against native TimeSpan addition to establish overhead.
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
public class ConversionBenchmarks
{
    private ValiTime _valiTime = null!;

    // Pre-built units array for MultiConvert
    private static readonly TimeUnit[] AllUnits =
    {
        TimeUnit.Milliseconds,
        TimeUnit.Seconds,
        TimeUnit.Minutes,
        TimeUnit.Hours,
        TimeUnit.Days,
        TimeUnit.Weeks,
        TimeUnit.Months,
        TimeUnit.Years,
    };

    [GlobalSetup]
    public void Setup()
    {
        _valiTime = new ValiTime();
    }

    /// <summary>
    /// Simple single-unit conversion: 3 600 seconds -> hours.
    /// Measures the overhead of the Convert switch expression at its most direct.
    /// </summary>
    [Benchmark]
    public decimal Convert_SecondsToHours()
    {
        return _valiTime.Convert(3_600m, TimeUnit.Seconds, TimeUnit.Hours);
    }

    /// <summary>
    /// Simple single-unit conversion: 24 hours -> days.
    /// </summary>
    [Benchmark]
    public decimal Convert_HoursToDays()
    {
        return _valiTime.Convert(24m, TimeUnit.Hours, TimeUnit.Days);
    }

    /// <summary>
    /// Converts 1 day to every supported TimeUnit in a single MultiConvert call.
    /// Allocates one Dictionary per call — good for tracking allocation regression.
    /// </summary>
    [Benchmark]
    public Dictionary<TimeUnit, decimal> MultiConvert_AllUnits()
    {
        return _valiTime.MultiConvert(1m, TimeUnit.Days, AllUnits);
    }

    /// <summary>
    /// Adds 100 ValiDuration values together using the + operator.
    /// Each iteration creates one new ValiDuration (struct on stack) — tests decimal arithmetic path.
    /// </summary>
    [Benchmark]
    public ValiDuration ValiDuration_Addition_100Times()
    {
        var duration = ValiDuration.Zero;
        var increment = ValiDuration.FromMinutes(1m);
        for (int i = 0; i < 100; i++)
        {
            duration = duration + increment;
        }
        return duration;
    }

    /// <summary>
    /// Head-to-head comparison: adds 100 ValiDuration values vs 100 TimeSpan values.
    /// ValiDuration uses decimal arithmetic; TimeSpan uses long ticks.
    /// Both results are returned to prevent dead-code elimination.
    /// </summary>
    [Benchmark]
    public (ValiDuration, TimeSpan) ValiDuration_vs_TimeSpan_Addition()
    {
        // ValiDuration path
        var vd = ValiDuration.Zero;
        var vdStep = ValiDuration.FromMinutes(1m);
        for (int i = 0; i < 100; i++)
            vd = vd + vdStep;

        // TimeSpan path
        var ts = TimeSpan.Zero;
        var tsStep = TimeSpan.FromMinutes(1);
        for (int i = 0; i < 100; i++)
            ts += tsStep;

        return (vd, ts);
    }
}
