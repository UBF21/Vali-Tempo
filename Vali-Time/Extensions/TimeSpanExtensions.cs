using Vali_Time.Enums;

namespace Vali_Time.Extensions;

/// <summary>
/// Extension methods for <see cref="TimeSpan"/> that provide unit conversions, formatting,
/// and magnitude comparisons.
/// </summary>
public static class TimeSpanExtensions
{
    // =========================================================================
    // CONVERSION
    // =========================================================================

    /// <summary>
    /// Converts the <see cref="TimeSpan"/> to the specified <see cref="TimeUnit"/> using
    /// <see cref="TimeSpan.Ticks"/> for maximum precision without intermediate
    /// floating-point conversion.
    /// </summary>
    /// <remarks>
    /// Month conversion uses an average of 30.4375 days (365.25 / 12).
    /// Year conversion uses an average of 365.25 days.
    /// </remarks>
    /// <param name="ts">The time span to convert.</param>
    /// <param name="unit">The target unit.</param>
    /// <returns>
    /// A <see cref="decimal"/> representing the duration expressed in <paramref name="unit"/>.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// Thrown when <paramref name="unit"/> is not a recognised <see cref="TimeUnit"/> value.
    /// </exception>
    public static decimal To(this TimeSpan ts, TimeUnit unit)
    {
        return unit switch
        {
            TimeUnit.Milliseconds => (decimal)ts.Ticks / TimeSpan.TicksPerMillisecond,
            TimeUnit.Seconds      => (decimal)ts.Ticks / TimeSpan.TicksPerSecond,
            TimeUnit.Minutes      => (decimal)ts.Ticks / TimeSpan.TicksPerMinute,
            TimeUnit.Hours        => (decimal)ts.Ticks / TimeSpan.TicksPerHour,
            TimeUnit.Days         => (decimal)ts.Ticks / TimeSpan.TicksPerDay,
            TimeUnit.Weeks        => (decimal)ts.Ticks / (TimeSpan.TicksPerDay * 7L),
            TimeUnit.Months       => (decimal)ts.Ticks / (TimeSpan.TicksPerDay * 30.4375m),
            TimeUnit.Years        => (decimal)ts.Ticks / (TimeSpan.TicksPerDay * 365.25m),
            _ => throw new NotSupportedException($"TimeUnit '{unit}' is not supported.")
        };
    }

    // =========================================================================
    // FORMATTING
    // =========================================================================

    /// <summary>
    /// Formats the <see cref="TimeSpan"/> as a human-readable string in the specified unit.
    /// </summary>
    /// <remarks>
    /// The unit abbreviations used are: <c>ms</c> (milliseconds), <c>s</c> (seconds),
    /// <c>min</c> (minutes), <c>h</c> (hours), <c>d</c> (days), <c>w</c> (weeks),
    /// <c>mo</c> (months), <c>y</c> (years).
    /// </remarks>
    /// <param name="ts">The time span to format.</param>
    /// <param name="unit">The unit in which to express the duration.</param>
    /// <param name="decimalPlaces">
    /// The number of decimal places to include in the output. Defaults to <c>2</c>.
    /// </param>
    /// <returns>A string such as <c>"1.50 h"</c> or <c>"90.00 min"</c>.</returns>
    public static string Format(this TimeSpan ts, TimeUnit unit, int decimalPlaces = 2)
    {
        decimal value = decimal.Round(ts.To(unit), decimalPlaces);

        string abbreviation = unit switch
        {
            TimeUnit.Milliseconds => "ms",
            TimeUnit.Seconds      => "s",
            TimeUnit.Minutes      => "min",
            TimeUnit.Hours        => "h",
            TimeUnit.Days         => "d",
            TimeUnit.Weeks        => "w",
            TimeUnit.Months       => "mo",
            TimeUnit.Years        => "y",
            _ => throw new NotSupportedException($"TimeUnit '{unit}' is not supported.")
        };

        return $"{value.ToString($"F{decimalPlaces}")} {abbreviation}";
    }

    // =========================================================================
    // BEST UNIT
    // =========================================================================

    /// <summary>
    /// Returns the most appropriate <see cref="TimeUnit"/> and its corresponding value for
    /// representing this <see cref="TimeSpan"/>.
    /// </summary>
    /// <remarks>
    /// The selection thresholds (using the absolute value of the total seconds) are:
    /// <list type="bullet">
    ///   <item><description>604800 s or more: weeks</description></item>
    ///   <item><description>86400 s or more: days</description></item>
    ///   <item><description>3600 s or more: hours</description></item>
    ///   <item><description>60 s or more: minutes</description></item>
    ///   <item><description>1 s or more: seconds</description></item>
    ///   <item><description>less than 1 s: milliseconds</description></item>
    /// </list>
    /// </remarks>
    /// <param name="ts">The time span to evaluate.</param>
    /// <returns>
    /// A tuple of the converted value and the selected <see cref="TimeUnit"/>.
    /// </returns>
    public static (decimal value, TimeUnit unit) BestUnit(this TimeSpan ts)
    {
        decimal seconds = ts.To(TimeUnit.Seconds);

        if (seconds >= 604800m) return (ts.To(TimeUnit.Weeks),        TimeUnit.Weeks);
        if (seconds >= 86400m)  return (ts.To(TimeUnit.Days),         TimeUnit.Days);
        if (seconds >= 3600m)   return (ts.To(TimeUnit.Hours),        TimeUnit.Hours);
        if (seconds >= 60m)     return (ts.To(TimeUnit.Minutes),      TimeUnit.Minutes);
        if (seconds >= 1m)      return (ts.To(TimeUnit.Seconds),      TimeUnit.Seconds);

        return (ts.To(TimeUnit.Milliseconds), TimeUnit.Milliseconds);
    }

    // =========================================================================
    // COMPARISONS
    // =========================================================================

    /// <summary>
    /// Determines whether the <see cref="TimeSpan"/> is greater than the specified
    /// <paramref name="amount"/> expressed in <paramref name="unit"/>.
    /// </summary>
    /// <param name="ts">The time span to compare.</param>
    /// <param name="amount">The threshold value in the given unit.</param>
    /// <param name="unit">The unit of the threshold.</param>
    /// <returns>
    /// <c>true</c> if the duration represented by <paramref name="ts"/> exceeds
    /// <paramref name="amount"/> <paramref name="unit"/>s.
    /// </returns>
    public static bool IsGreaterThan(this TimeSpan ts, decimal amount, TimeUnit unit)
    {
        return ts.To(unit) > amount;
    }

    /// <summary>
    /// Determines whether the <see cref="TimeSpan"/> is less than the specified
    /// <paramref name="amount"/> expressed in <paramref name="unit"/>.
    /// </summary>
    /// <param name="ts">The time span to compare.</param>
    /// <param name="amount">The threshold value in the given unit.</param>
    /// <param name="unit">The unit of the threshold.</param>
    /// <returns>
    /// <c>true</c> if the duration represented by <paramref name="ts"/> is below
    /// <paramref name="amount"/> <paramref name="unit"/>s.
    /// </returns>
    public static bool IsLessThan(this TimeSpan ts, decimal amount, TimeUnit unit)
    {
        return ts.To(unit) < amount;
    }
}
