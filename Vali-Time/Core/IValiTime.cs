using System.Globalization;
using Vali_Time.Enums;

namespace Vali_Time.Core;

/// <summary>
/// Defines the contract for time unit conversion, formatting, and breakdown operations.
/// </summary>
public interface IValiTime
{
    /// <summary>
    /// Converts a time value from one unit to another with full precision, optionally applying rounding.
    /// </summary>
    /// <param name="time">The time value to convert.</param>
    /// <param name="fromUnit">The source unit of the time.</param>
    /// <param name="toUnit">The target unit to convert the time to.</param>
    /// <param name="decimalPlaces">The number of decimal places to round to; if null, no rounding is applied.</param>
    /// <param name="rounding">The rounding strategy to apply if rounding is requested (default is ToEven).</param>
    /// <returns>The converted time in the target unit with full precision unless rounding is specified.</returns>
    decimal Convert(decimal time, TimeUnit fromUnit, TimeUnit toUnit, int? decimalPlaces = null, MidpointRounding rounding = MidpointRounding.ToEven);

    /// <summary>
    /// Adds multiple time values in different units and returns the result in a specified unit with full precision, optionally applying rounding.
    /// </summary>
    /// <param name="resultUnit">The unit for the result.</param>
    /// <param name="times">List of tuples containing time values and their units to add.</param>
    /// <param name="decimalPlaces">The number of decimal places to round the result to; if null, no rounding is applied.</param>
    /// <param name="rounding">The rounding strategy to apply if rounding is requested (default is ToEven).</param>
    /// <returns>The sum of all times converted to the specified unit with full precision unless rounding is specified.</returns>
    decimal SumTimes(TimeUnit resultUnit, List<(decimal time, TimeUnit unit)> times, int? decimalPlaces = null, MidpointRounding rounding = MidpointRounding.ToEven);

    /// <summary>
    /// Formats a time value into a human-readable string with customizable precision.
    /// </summary>
    /// <param name="time">The time value to format.</param>
    /// <param name="unit">The unit in which to express the time.</param>
    /// <param name="decimalPlaces">Number of decimal places to display (default is 2).</param>
    /// <param name="culture">Culture for numeric formatting (optional, defaults to current culture).</param>
    /// <returns>A formatted string representing the time (e.g., "1.25 h").</returns>
    string FormatTime(decimal time, TimeUnit unit, int decimalPlaces = 2, CultureInfo? culture = null);

    /// <summary>
    /// Determines the most appropriate unit for a given time in seconds with full precision.
    /// </summary>
    /// <param name="seconds">The time in seconds.</param>
    /// <returns>A tuple with the converted time and the best unit, preserving full precision.</returns>
    (decimal time, TimeUnit unit) GetBestUnit(decimal seconds);

    /// <summary>
    /// Converts a time value to a <see cref="TimeSpan"/> object with full precision.
    /// </summary>
    /// <param name="time">The time value to convert.</param>
    /// <param name="unit">The unit of the time value.</param>
    /// <returns>A <see cref="TimeSpan"/> representing the time.</returns>
    TimeSpan ToTimeSpan(decimal time, TimeUnit unit);

    /// <summary>
    /// Breaks down a time in seconds into a dictionary of units with full precision.
    /// </summary>
    /// <param name="seconds">The time in seconds to break down.</param>
    /// <returns>A dictionary with the time distributed across units, preserving full precision.</returns>
    Dictionary<TimeUnit, decimal> Breakdown(decimal seconds);

    /// <summary>
    /// Subtracts multiple time values and returns the result in the specified unit with full precision, optionally applying rounding.
    /// The first element of <paramref name="times"/> acts as the minuend; all subsequent elements are subtracted from it.
    /// </summary>
    /// <param name="resultUnit">The unit for the result.</param>
    /// <param name="times">
    /// List of tuples containing time values and their units. The first element is the minuend;
    /// the remaining elements are the subtrahends.
    /// </param>
    /// <param name="allowNegative">
    /// When <c>true</c>, a negative result is returned as-is.
    /// When <c>false</c> (default), an <see cref="ArgumentException"/> is thrown if the result is negative.
    /// </param>
    /// <param name="decimalPlaces">The number of decimal places to round the result to; if null, no rounding is applied.</param>
    /// <param name="rounding">The rounding strategy to apply if rounding is requested (default is ToEven).</param>
    /// <returns>The subtraction result converted to the specified unit with full precision unless rounding is specified.</returns>
    decimal SubtractTimes(TimeUnit resultUnit, List<(decimal time, TimeUnit unit)> times,
        bool allowNegative = false, int? decimalPlaces = null, MidpointRounding rounding = MidpointRounding.ToEven);

    /// <summary>
    /// Converts a <see cref="TimeSpan"/> to the specified <see cref="TimeUnit"/> with maximum decimal precision using ticks.
    /// </summary>
    /// <param name="ts">The <see cref="TimeSpan"/> to convert.</param>
    /// <param name="unit">The target unit to express the result in.</param>
    /// <returns>
    /// The value of <paramref name="ts"/> expressed in <paramref name="unit"/> as a <see cref="decimal"/>,
    /// calculated directly from ticks for maximum precision.
    /// </returns>
    decimal FromTimeSpan(TimeSpan ts, TimeUnit unit);

    /// <summary>
    /// Attempts to convert a time value from one unit to another without throwing on invalid input.
    /// </summary>
    /// <param name="time">The time value to convert.</param>
    /// <param name="fromUnit">The source unit of the time.</param>
    /// <param name="toUnit">The target unit to convert the time to.</param>
    /// <param name="result">
    /// When this method returns <c>true</c>, contains the converted value; otherwise, contains <c>0</c>.
    /// </param>
    /// <param name="decimalPlaces">The number of decimal places to round to; if null, no rounding is applied.</param>
    /// <returns>
    /// <c>true</c> if the conversion succeeded; <c>false</c> if any parameter was invalid.
    /// </returns>
    bool TryConvert(decimal time, TimeUnit fromUnit, TimeUnit toUnit, out decimal result, int? decimalPlaces = null);

    /// <summary>
    /// Parses a human-readable time string and returns the total duration in seconds as a <see cref="decimal"/>.
    /// </summary>
    /// <param name="input">
    /// A time string in one of the following formats:
    /// <list type="bullet">
    ///   <item><description>Colon-separated: <c>"hh:mm:ss"</c> or <c>"mm:ss"</c> (e.g., <c>"1:30:00"</c>, <c>"45:30"</c>).</description></item>
    ///   <item><description>
    ///     Unit-labelled tokens (combinable, e.g., <c>"2d 4h 30m 15s"</c>, <c>"1h 30m"</c>, <c>"90 minutes"</c>, <c>"500ms"</c>):
    ///     <c>ms</c> / <c>milliseconds</c>, <c>s</c> / <c>sec</c> / <c>seconds</c>,
    ///     <c>m</c> / <c>min</c> / <c>minutes</c>, <c>h</c> / <c>hr</c> / <c>hours</c>,
    ///     <c>d</c> / <c>days</c>, <c>w</c> / <c>weeks</c>.
    ///   </description></item>
    /// </list>
    /// </param>
    /// <returns>The total time represented by <paramref name="input"/>, expressed in seconds.</returns>
    /// <exception cref="FormatException">Thrown if the string cannot be parsed into a recognised time pattern.</exception>
    decimal ParseTime(string input);

    /// <summary>
    /// Attempts to parse a human-readable time string into total seconds without throwing on invalid input.
    /// </summary>
    /// <param name="input">The time string to parse (see <see cref="ParseTime"/> for supported formats).</param>
    /// <param name="seconds">
    /// When this method returns <c>true</c>, contains the parsed value in seconds;
    /// otherwise, contains <c>0</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the string was parsed successfully; <c>false</c> if the format was not recognised.
    /// </returns>
    bool TryParseTime(string input, out decimal seconds);

    /// <summary>
    /// Constrains a time value between <paramref name="min"/> and <paramref name="max"/> in the same unit.
    /// Returns <paramref name="min"/> if the value is below it, <paramref name="max"/> if above it, or the
    /// original value when it is already within the range.
    /// </summary>
    /// <param name="time">The time value to clamp.</param>
    /// <param name="unit">The unit shared by <paramref name="time"/>, <paramref name="min"/>, and <paramref name="max"/>.</param>
    /// <param name="min">The lower bound.</param>
    /// <param name="max">The upper bound.</param>
    /// <returns>The clamped value in <paramref name="unit"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="min"/> is greater than <paramref name="max"/>.</exception>
    decimal Clamp(decimal time, TimeUnit unit, decimal min, decimal max);

    /// <summary>
    /// Compares two time values that may be expressed in different units.
    /// </summary>
    /// <param name="time1">The first time value.</param>
    /// <param name="unit1">The unit of <paramref name="time1"/>.</param>
    /// <param name="time2">The second time value.</param>
    /// <param name="unit2">The unit of <paramref name="time2"/>.</param>
    /// <returns>
    /// <c>-1</c> if <paramref name="time1"/> &lt; <paramref name="time2"/>;
    /// <c>0</c> if they are equal;
    /// <c>1</c> if <paramref name="time1"/> &gt; <paramref name="time2"/>.
    /// </returns>
    int Compare(decimal time1, TimeUnit unit1, decimal time2, TimeUnit unit2);

    /// <summary>
    /// Converts a time value to multiple target units in a single call.
    /// </summary>
    /// <param name="time">The source time value.</param>
    /// <param name="fromUnit">The source unit.</param>
    /// <param name="toUnits">One or more target units to convert to.</param>
    /// <returns>
    /// A <see cref="Dictionary{TKey,TValue}"/> mapping each requested <see cref="TimeUnit"/> to its converted value.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="toUnits"/> is null or empty.</exception>
    Dictionary<TimeUnit, decimal> MultiConvert(decimal time, TimeUnit fromUnit, params TimeUnit[] toUnits);
}
