using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Vali_Age.Core;
using Vali_Age.Models;

namespace Vali_Tempo.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks for Vali-Age: covers the common calculation paths and the Feb 29 edge case.
/// All benchmarks use a fixed reference date so results are deterministic across runs.
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
public class AgeBenchmarks
{
    private ValiAge _age = null!;

    // Fixed reference so benchmarks are reproducible regardless of when they run.
    private static readonly DateTime Reference = new(2025, 3, 22);

    // A typical birth date: someone who turned 32 before the reference date.
    private static readonly DateTime BirthDate = new(1992, 6, 15);

    // Feb 29 birth date — exercises the leap-year branching in BirthdayInYear.
    private static readonly DateTime Feb29BirthDate = new(1996, 2, 29);

    // A date close to the reference — exercises the Relative short-path branches.
    private static readonly DateTime RecentDate = Reference.AddDays(-5);

    [GlobalSetup]
    public void Setup()
    {
        _age = new ValiAge();
    }

    /// <summary>
    /// Calculates the number of complete years between a birth date and the fixed reference.
    /// Exercises the simple subtraction + AddYears correction path.
    /// </summary>
    [Benchmark]
    public int Years_Simple()
    {
        return _age.Years(BirthDate, Reference);
    }

    /// <summary>
    /// Calculates the full years/months/days/totalDays breakdown.
    /// Exercises the inner while loop that advances months one at a time.
    /// </summary>
    [Benchmark]
    public AgeResult Exact_Full()
    {
        return _age.Exact(BirthDate, Reference);
    }

    /// <summary>
    /// Calculates the next birthday for a Feb 29 birth date.
    /// Exercises the BirthdayInYear branch that falls back to Feb 28 in non-leap years.
    /// </summary>
    [Benchmark]
    public DateTime NextBirthday_Feb29()
    {
        return _age.NextBirthday(Feb29BirthDate, Reference);
    }

    /// <summary>
    /// Formats a recent date as a relative string (e.g., "5 days ago").
    /// Exercises the short-path branches in the seconds switch expression.
    /// </summary>
    [Benchmark]
    public string Relative_RecentDate()
    {
        return _age.Relative(RecentDate, Reference);
    }
}
