using Vali_Time.Abstractions;
using Vali_Time.Enums;

namespace Vali_CountDown.Core;

/// <summary>
/// Provides countdown and elapsed-time operations based on a reference point in time.
/// All methods that do not accept a <c>reference</c> parameter use the injected <see cref="IClock"/> internally.
/// </summary>
public class ValiCountdown : IValiCountdown
{
    private readonly IClock _clock;

    /// <summary>
    /// Initializes a new instance of <see cref="ValiCountdown"/>.
    /// </summary>
    /// <param name="clock">
    /// Optional clock abstraction. Defaults to <see cref="SystemClock.Instance"/> when <c>null</c>.
    /// Inject a test double to control time in unit tests.
    /// </param>
    public ValiCountdown(IClock? clock = null)
    {
        _clock = clock ?? SystemClock.Instance;
    }

    private const decimal TicksPerMillisecond = 10_000m;
    private const decimal TicksPerSecond      = TicksPerMillisecond * 1_000m;
    private const decimal TicksPerMinute      = TicksPerSecond * 60m;
    private const decimal TicksPerHour        = TicksPerMinute * 60m;
    private const decimal TicksPerDay         = TicksPerHour * 24m;
    private const decimal TicksPerWeek        = TicksPerDay * 7m;
    private const decimal AvgDaysPerMonth     = 30.4375m;
    private const decimal AvgDaysPerYear      = 365.25m;

    // === EXPIRY ===

    /// <summary>
    /// Returns <c>true</c> if the deadline has already passed relative to <see cref="DateTime.Now"/>.
    /// </summary>
    /// <param name="deadline">The deadline to evaluate.</param>
    public bool IsExpired(DateTime deadline)
        => IsExpired(deadline, _clock.Now);

    /// <summary>
    /// Returns <c>true</c> if the deadline has already passed relative to a custom reference time.
    /// </summary>
    /// <param name="deadline">The deadline to evaluate.</param>
    /// <param name="reference">The reference point in time used for comparison.</param>
    public bool IsExpired(DateTime deadline, DateTime reference)
        => deadline < reference;

    // === TIME UNTIL / ELAPSED ===

    /// <summary>
    /// Returns the amount of time remaining until the deadline in the specified unit.
    /// Returns 0 if the deadline has already expired.
    /// </summary>
    /// <param name="deadline">The target deadline.</param>
    /// <param name="unit">The time unit in which to express the result.</param>
    /// <param name="decimalPlaces">Optional number of decimal places to round the result to.</param>
    public decimal TimeUntil(DateTime deadline, TimeUnit unit, int? decimalPlaces = null)
    {
        var now = _clock.Now;
        if (deadline <= now)
            return 0m;

        decimal ticks = (decimal)(deadline - now).Ticks;
        decimal result = ConvertTicks(ticks, unit);
        return decimalPlaces.HasValue ? Math.Round(result, decimalPlaces.Value) : result;
    }

    /// <summary>
    /// Returns the amount of time that has elapsed since the specified point in the given unit.
    /// </summary>
    /// <param name="from">The starting point in time.</param>
    /// <param name="unit">The time unit in which to express the result.</param>
    /// <param name="decimalPlaces">Optional number of decimal places to round the result to.</param>
    public decimal TimeElapsed(DateTime from, TimeUnit unit, int? decimalPlaces = null)
    {
        decimal ticks = (decimal)(_clock.Now - from).Ticks;
        decimal result = ConvertTicks(ticks, unit);
        return decimalPlaces.HasValue ? Math.Round(Math.Max(0m, result), decimalPlaces.Value) : Math.Max(0m, result);
    }

    // === PROGRESS ===

    /// <summary>
    /// Returns a progress value between 0.0 and 1.0 based on the current time relative to a start and end.
    /// Returns 0 if the current time is before or at <paramref name="start"/>; returns 1 if at or past <paramref name="end"/>.
    /// </summary>
    /// <param name="start">The beginning of the time range.</param>
    /// <param name="end">The end of the time range.</param>
    public decimal Progress(DateTime start, DateTime end)
        => Progress(start, end, _clock.Now);

    /// <summary>
    /// Returns a progress value between 0.0 and 1.0 using a custom reference time.
    /// Returns 0 if <paramref name="reference"/> is before or at <paramref name="start"/>;
    /// returns 1 if at or past <paramref name="end"/>.
    /// </summary>
    /// <param name="start">The beginning of the time range.</param>
    /// <param name="end">The end of the time range.</param>
    /// <param name="reference">The reference point in time used to calculate progress.</param>
    public decimal Progress(DateTime start, DateTime end, DateTime reference)
    {
        if (end <= start)
            throw new ArgumentException("end must be strictly after start.", nameof(end));
        if (reference <= start) return 0m;
        if (reference >= end)   return 1m;

        decimal total   = (decimal)(end - start).Ticks;
        decimal elapsed = (decimal)(reference - start).Ticks;
        return elapsed / total;
    }

    /// <summary>
    /// Returns the progress as a percentage between 0 and 100, based on <see cref="DateTime.Now"/>.
    /// </summary>
    /// <param name="start">The beginning of the time range.</param>
    /// <param name="end">The end of the time range.</param>
    public decimal ProgressPercent(DateTime start, DateTime end)
        => Progress(start, end) * 100m;

    // === BREAKDOWN ===

    /// <summary>
    /// Breaks down the time remaining until the deadline into hours, minutes, seconds, and milliseconds.
    /// Returns a dictionary keyed by <see cref="TimeUnit"/>. All values are 0 if the deadline has expired.
    /// </summary>
    /// <param name="deadline">The target deadline.</param>
    public Dictionary<TimeUnit, decimal> Breakdown(DateTime deadline)
    {
        var span = deadline - _clock.Now;

        if (span.Ticks <= 0)
        {
            return new Dictionary<TimeUnit, decimal>
            {
                [TimeUnit.Days]         = 0m,
                [TimeUnit.Hours]        = 0m,
                [TimeUnit.Minutes]      = 0m,
                [TimeUnit.Seconds]      = 0m,
                [TimeUnit.Milliseconds] = 0m
            };
        }

        long remaining = span.Ticks;
        long days    = remaining / TimeSpan.TicksPerDay;    remaining %= TimeSpan.TicksPerDay;
        long hours   = remaining / TimeSpan.TicksPerHour;   remaining %= TimeSpan.TicksPerHour;
        long minutes = remaining / TimeSpan.TicksPerMinute; remaining %= TimeSpan.TicksPerMinute;
        long seconds = remaining / TimeSpan.TicksPerSecond; remaining %= TimeSpan.TicksPerSecond;
        long ms      = remaining / TimeSpan.TicksPerMillisecond;

        return new Dictionary<TimeUnit, decimal>
        {
            [TimeUnit.Days]         = days,
            [TimeUnit.Hours]        = hours,
            [TimeUnit.Minutes]      = minutes,
            [TimeUnit.Seconds]      = seconds,
            [TimeUnit.Milliseconds] = ms
        };
    }

    // === FORMAT ===

    /// <summary>
    /// Formats the time remaining until the deadline as a human-readable string (e.g., "5d 3h 20m").
    /// Returns "Expired" if the deadline has already passed.
    /// Only non-zero components are included in the output.
    /// </summary>
    /// <param name="deadline">The target deadline.</param>
    /// <param name="includeSeconds">If <c>true</c>, seconds are included in the output string.</param>
    public string Format(DateTime deadline, bool includeSeconds = false)
    {
        decimal totalSeconds = TimeUntil(deadline, TimeUnit.Seconds);

        if (totalSeconds <= 0m)
            return "Expired";

        long total   = (long)totalSeconds;
        long days    = total / 86_400L;
        long rem     = total % 86_400L;
        long hours   = rem / 3_600L;
        rem          = rem % 3_600L;
        long minutes = rem / 60L;
        long seconds = rem % 60L;

        var parts = new List<string>();

        if (days    > 0) parts.Add($"{days}d");
        if (hours   > 0) parts.Add($"{hours}h");
        if (minutes > 0) parts.Add($"{minutes}m");
        if (includeSeconds && seconds > 0) parts.Add($"{seconds}s");

        return parts.Count > 0 ? string.Join(" ", parts) : (includeSeconds ? "0s" : "0m");
    }

    // === IS WITHIN ===

    /// <summary>
    /// Returns <c>true</c> if the deadline is within the specified amount of the given time unit from now.
    /// Returns <c>false</c> if the deadline has already expired.
    /// </summary>
    /// <param name="deadline">The target deadline.</param>
    /// <param name="amount">The threshold amount.</param>
    /// <param name="unit">The time unit for the threshold.</param>
    public bool IsWithin(DateTime deadline, decimal amount, TimeUnit unit)
    {
        decimal remaining = TimeUntil(deadline, unit);
        return remaining > 0m && remaining <= amount;
    }

    // === PRIVATE HELPERS ===

    private static decimal ConvertTicks(decimal ticks, TimeUnit unit) => unit switch
    {
        TimeUnit.Milliseconds => ticks / TicksPerMillisecond,
        TimeUnit.Seconds      => ticks / TicksPerSecond,
        TimeUnit.Minutes      => ticks / TicksPerMinute,
        TimeUnit.Hours        => ticks / TicksPerHour,
        TimeUnit.Days         => ticks / TicksPerDay,
        TimeUnit.Weeks        => ticks / TicksPerWeek,
        TimeUnit.Months       => ticks / TicksPerDay / AvgDaysPerMonth,
        TimeUnit.Years        => ticks / TicksPerDay / AvgDaysPerYear,
        _                     => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
    };
}
