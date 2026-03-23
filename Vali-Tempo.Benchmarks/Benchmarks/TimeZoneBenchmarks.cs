using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Vali_TimeZone.Core;

namespace Vali_Tempo.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks for Vali-TimeZone: covers timezone conversion, offset lookup,
/// DST detection, and zone discovery.
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
public class TimeZoneBenchmarks
{
    private ValiTimeZone _tz = null!;
    private static readonly DateTime SampleDate = new(2025, 7, 15, 10, 0, 0); // summer — DST active in many zones

    [GlobalSetup]
    public void Setup()
    {
        _tz = new ValiTimeZone();
    }

    /// <summary>
    /// Converts a DateTime from Lima (UTC-5) to Madrid (UTC+2 in summer).
    /// Exercises the full Find + TimeZoneInfo.ConvertTime path.
    /// </summary>
    [Benchmark]
    public DateTime Convert_LimaToMadrid()
    {
        return _tz.Convert(SampleDate, "America/Lima", "Europe/Madrid");
    }

    /// <summary>
    /// Converts a local DateTime to UTC for the Lima timezone.
    /// </summary>
    [Benchmark]
    public DateTime ToUtc_Lima()
    {
        return _tz.ToUtc(SampleDate, "America/Lima");
    }

    /// <summary>
    /// Converts a UTC DateTime to local time in São Paulo (UTC-3).
    /// </summary>
    [Benchmark]
    public DateTime FromUtc_SaoPaulo()
    {
        return _tz.FromUtc(SampleDate, "America/Sao_Paulo");
    }

    /// <summary>
    /// Gets the UTC offset for Madrid during summer (DST active → UTC+2).
    /// </summary>
    [Benchmark]
    public TimeSpan GetOffset_Madrid_Summer()
    {
        return _tz.GetOffset("Europe/Madrid", SampleDate);
    }

    /// <summary>
    /// Checks whether DST is active for Madrid on the sample date.
    /// </summary>
    [Benchmark]
    public bool IsDst_Madrid_Summer()
    {
        return _tz.IsDst(SampleDate, "Europe/Madrid");
    }

    /// <summary>
    /// Validates whether an IANA zone id is recognized.
    /// Exercises the linear scan over the curated zone catalog.
    /// </summary>
    [Benchmark]
    public bool IsValidZone_Lima()
    {
        return _tz.IsValidZone("America/Lima");
    }

    /// <summary>
    /// Returns all timezone entries for a given country code.
    /// Exercises the LINQ Where over the full catalog.
    /// </summary>
    [Benchmark]
    public int ZonesForCountry_US()
    {
        int count = 0;
        foreach (var _ in _tz.ZonesForCountry("US"))
            count++;
        return count;
    }

    /// <summary>
    /// Formats a DateTime with timezone offset appended (e.g. "2025-07-15 10:00 -05:00").
    /// Exercises string interpolation + GetOffset.
    /// </summary>
    [Benchmark]
    public string FormatWithZone_Lima()
    {
        return _tz.FormatWithZone(SampleDate, "America/Lima");
    }
}
