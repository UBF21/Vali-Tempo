using System.Globalization;
using System.Text.RegularExpressions;
using Vali_Time.Enums;
using Vali_Time.Utils;

namespace Vali_Time.Core;

/// <summary>
/// Helper class for converting time units between seconds, minutes, and hours with maximum precision.
/// </summary>
public class ValiTime : IValiTime
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
    /// <exception cref="ArgumentException">Thrown if the time is negative or decimalPlaces is negative.</exception>
    /// <exception cref="NotSupportedException">Thrown if an unsupported unit is provided.</exception>
    public decimal Convert(decimal time, TimeUnit fromUnit, TimeUnit toUnit, int? decimalPlaces = null, MidpointRounding rounding = MidpointRounding.ToEven)
    {
        if (time < 0) throw new ArgumentException("Time cannot be negative.", nameof(time));
        if (decimalPlaces is < 0) throw new ArgumentException("Decimal places cannot be negative.", nameof(decimalPlaces));

        decimal timeInSeconds = fromUnit switch
        {
            TimeUnit.Milliseconds => MillisecondsToSeconds(time),
            TimeUnit.Seconds => time,
            TimeUnit.Minutes => MinutesToSeconds(time),
            TimeUnit.Hours => HoursToSeconds(time),
            TimeUnit.Days => DaysToSeconds(time),
            TimeUnit.Weeks => WeeksToSeconds(time),
            TimeUnit.Months => MonthsToSeconds(time),
            TimeUnit.Years => YearsToSeconds(time),
            _ => throw new NotSupportedException("TimeUnit not supported.")
        };

        decimal result = toUnit switch
        {
            TimeUnit.Milliseconds => SecondsToMilliseconds(timeInSeconds),
            TimeUnit.Seconds => timeInSeconds,
            TimeUnit.Minutes => SecondsToMinutes(timeInSeconds),
            TimeUnit.Hours => SecondsToHours(timeInSeconds),
            TimeUnit.Days => SecondsToDays(timeInSeconds),
            TimeUnit.Weeks => SecondsToWeeks(timeInSeconds),
            TimeUnit.Months => SecondsToMonths(timeInSeconds),
            TimeUnit.Years => SecondsToYears(timeInSeconds),
            _ => throw new NotSupportedException("TimeUnit not supported.")
        };

        return decimalPlaces.HasValue ? decimal.Round(result, decimalPlaces.Value, rounding) : result;
    }

    /// <summary>
    /// Adds multiple time values in different units and returns the result in a specified unit with full precision, optionally applying rounding.
    /// </summary>
    /// <param name="resultUnit">The unit for the result.</param>
    /// <param name="times">List of tuples containing time values and their units to add.</param>
    /// <param name="decimalPlaces">The number of decimal places to round the result to; if null, no rounding is applied.</param>
    /// <param name="rounding">The rounding strategy to apply if rounding is requested (default is ToEven).</param>
    /// <returns>The sum of all times converted to the specified unit with full precision unless rounding is specified.</returns>
    /// <exception cref="ArgumentException">Thrown if the times array is null or empty, or if decimalPlaces is negative.</exception>
    /// <exception cref="NotSupportedException">Thrown if an unsupported unit is provided.</exception>
    public decimal SumTimes(TimeUnit resultUnit, List<(decimal time, TimeUnit unit)> times, int? decimalPlaces = null, MidpointRounding rounding = MidpointRounding.ToEven)
    {
        if (times == null || !times.Any())
            throw new ArgumentException("At least one time value must be provided.", nameof(times));
        if (decimalPlaces is < 0)
            throw new ArgumentException("Decimal places cannot be negative.", nameof(decimalPlaces));

        decimal totalSeconds = times.Sum(t => t.time < 0
            ? -ConvertToSeconds(-t.time, t.unit)
            : ConvertToSeconds(t.time, t.unit));
        if (totalSeconds < 0m)
            throw new ArgumentException("The sum of the provided times is negative. Use SubtractTimes with allowNegative=true for signed results.", nameof(times));
        return Convert(totalSeconds, TimeUnit.Seconds, resultUnit, decimalPlaces, rounding);
    }

    /// <summary>
    /// Formats a time value into a human-readable string with customizable precision.
    /// </summary>
    /// <param name="time">The time value to format.</param>
    /// <param name="unit">The unit in which to express the time.</param>
    /// <param name="decimalPlaces">Number of decimal places to display (default is 2).</param>
    /// <param name="culture">Culture for numeric formatting (optional, defaults to current culture).</param>
    /// <returns>A formatted string representing the time (e.g., "1.25 h").</returns>
    /// <exception cref="NotSupportedException">Thrown if an unsupported unit is provided.</exception>
    public string FormatTime(decimal time, TimeUnit unit, int decimalPlaces = 2, CultureInfo? culture = null)
    {
        culture ??= CultureInfo.CurrentCulture;
        string suffix = unit switch
        {
            TimeUnit.Milliseconds => Constants.PrefixMilliseconds,
            TimeUnit.Seconds => Constants.PrefixSeconds,
            TimeUnit.Minutes => Constants.PrefixMinutes,
            TimeUnit.Hours => Constants.PrefixHours,
            TimeUnit.Days => Constants.PrefixDays,
            TimeUnit.Weeks => Constants.PrefixWeeks,
            TimeUnit.Months => Constants.PrefixMonths,
            TimeUnit.Years => Constants.PrefixYears,
            _ => throw new NotSupportedException("TimeUnit not supported.")
        };
        return $"{time.ToString($"F{decimalPlaces}", culture)} {suffix}";
    }

    /// <summary>
    /// Determines the most appropriate unit for a given time in seconds with full precision.
    /// </summary>
    /// <param name="seconds">The time in seconds.</param>
    /// <returns>A tuple with the converted time and the best unit, preserving full precision.</returns>
    /// <exception cref="ArgumentException">Thrown if the time is negative.</exception>
    public (decimal time, TimeUnit unit) GetBestUnit(decimal seconds)
    {
        if (seconds < 0) throw new ArgumentException("Time cannot be negative.", nameof(seconds));
        if (seconds == 0m) return (0m, TimeUnit.Seconds);
        if (seconds >= Constants.SecondsInYear) return (SecondsToYears(seconds), TimeUnit.Years);
        if (seconds >= Constants.SecondsInMonth) return (SecondsToMonths(seconds), TimeUnit.Months);
        if (seconds >= Constants.SecondsInWeek) return (SecondsToWeeks(seconds), TimeUnit.Weeks);
        if (seconds >= Constants.SecondsInDay) return (SecondsToDays(seconds), TimeUnit.Days);
        if (seconds >= Constants.SecondsInHour) return (SecondsToHours(seconds), TimeUnit.Hours);
        if (seconds >= Constants.SecondsInMinute) return (SecondsToMinutes(seconds), TimeUnit.Minutes);
        if (seconds >= 1m) return (seconds, TimeUnit.Seconds);
        return (SecondsToMilliseconds(seconds), TimeUnit.Milliseconds);
    }

    /// <summary>
    /// Converts a time value to a TimeSpan object with full precision.
    /// </summary>
    /// <param name="time">The time value to convert.</param>
    /// <param name="unit">The unit of the time value.</param>
    /// <returns>A TimeSpan representing the time.</returns>
    /// <exception cref="ArgumentException">Thrown if the time is negative.</exception>
    /// <exception cref="NotSupportedException">Thrown if an unsupported unit is provided.</exception>
    public TimeSpan ToTimeSpan(decimal time, TimeUnit unit)
    {
        if (time < 0) throw new ArgumentException("Time cannot be negative.", nameof(time));
        decimal seconds = Convert(time, unit, TimeUnit.Seconds);
        const decimal maxTicks = (decimal)long.MaxValue;
        decimal ticks = seconds * TimeSpan.TicksPerSecond;
        if (ticks > maxTicks)
            throw new OverflowException(
                $"The value {seconds} seconds is too large to represent as a TimeSpan (max ~29,227 years).");
        return TimeSpan.FromTicks((long)ticks);
    }

    /// <summary>
    /// Breaks down a time in seconds into a dictionary of units with full precision.
    /// </summary>
    /// <param name="seconds">The time in seconds to break down.</param>
    /// <returns>A dictionary with the time distributed across units, preserving full precision.</returns>
    /// <exception cref="ArgumentException">Thrown if the time is negative.</exception>
    /// <remarks>
    /// Note: this method decomposes seconds into hours, minutes, seconds, and milliseconds only.
    /// Values representing days or larger are accumulated into the hours component.
    /// </remarks>
    public Dictionary<TimeUnit, decimal> Breakdown(decimal seconds)
    {
        if (seconds < 0) throw new ArgumentException("Time cannot be negative.", nameof(seconds));

        var breakdown = new Dictionary<TimeUnit, decimal>
        {
            [TimeUnit.Hours] = SecondsToHours(seconds),
            [TimeUnit.Minutes] = 0m,
            [TimeUnit.Seconds] = 0m,
            [TimeUnit.Milliseconds] = 0m
        };

        decimal hoursInteger = decimal.Floor(breakdown[TimeUnit.Hours]);
        decimal remainingSeconds = seconds - HoursToSeconds(hoursInteger);
        breakdown[TimeUnit.Hours] = hoursInteger;

        breakdown[TimeUnit.Minutes] = SecondsToMinutes(remainingSeconds);
        decimal minutesInteger = decimal.Floor(breakdown[TimeUnit.Minutes]);
        remainingSeconds -= MinutesToSeconds(minutesInteger);
        breakdown[TimeUnit.Minutes] = minutesInteger;

        breakdown[TimeUnit.Seconds] = remainingSeconds;
        decimal secondsInteger = decimal.Floor(breakdown[TimeUnit.Seconds]);
        decimal remainingMilliseconds = (remainingSeconds - secondsInteger) * Constants.MillisecondsInSecond;
        breakdown[TimeUnit.Seconds] = secondsInteger;

        breakdown[TimeUnit.Milliseconds] = remainingMilliseconds;

        return breakdown;
    }

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
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="times"/> is null or empty, if <paramref name="decimalPlaces"/> is negative,
    /// or if the result is negative and <paramref name="allowNegative"/> is <c>false</c>.
    /// </exception>
    /// <exception cref="NotSupportedException">Thrown if an unsupported unit is provided.</exception>
    public decimal SubtractTimes(TimeUnit resultUnit, List<(decimal time, TimeUnit unit)> times,
        bool allowNegative = false, int? decimalPlaces = null, MidpointRounding rounding = MidpointRounding.ToEven)
    {
        if (times == null || !times.Any())
            throw new ArgumentException("At least one time value must be provided.", nameof(times));
        if (decimalPlaces is < 0)
            throw new ArgumentException("Decimal places cannot be negative.", nameof(decimalPlaces));

        decimal firstSeconds = times[0].time < 0
            ? -ConvertToSeconds(-times[0].time, times[0].unit)
            : ConvertToSeconds(times[0].time, times[0].unit);
        decimal restSeconds = times.Skip(1).Sum(t => t.time < 0
            ? -ConvertToSeconds(-t.time, t.unit)
            : ConvertToSeconds(t.time, t.unit));
        decimal totalSeconds = firstSeconds - restSeconds;

        if (totalSeconds < 0m && !allowNegative)
            throw new ArgumentException("The result of the subtraction is negative. Set allowNegative to true to allow negative results.", nameof(times));

        if (totalSeconds < 0m)
            return -Convert(-totalSeconds, TimeUnit.Seconds, resultUnit, decimalPlaces, rounding);

        return Convert(totalSeconds, TimeUnit.Seconds, resultUnit, decimalPlaces, rounding);
    }

    /// <summary>
    /// Converts a <see cref="TimeSpan"/> to the specified <see cref="TimeUnit"/> with maximum decimal precision using ticks.
    /// </summary>
    /// <param name="ts">The <see cref="TimeSpan"/> to convert.</param>
    /// <param name="unit">The target unit to express the result in.</param>
    /// <returns>
    /// The value of <paramref name="ts"/> expressed in <paramref name="unit"/> as a <see cref="decimal"/>,
    /// calculated directly from ticks for maximum precision.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="ts"/> is negative.</exception>
    /// <exception cref="NotSupportedException">Thrown if an unsupported unit is provided.</exception>
    public decimal FromTimeSpan(TimeSpan ts, TimeUnit unit)
    {
        if (ts < TimeSpan.Zero)
            throw new ArgumentException("TimeSpan cannot be negative.", nameof(ts));
        return unit switch
        {
            TimeUnit.Milliseconds => (decimal)ts.Ticks / TimeSpan.TicksPerMillisecond,
            TimeUnit.Seconds      => (decimal)ts.Ticks / TimeSpan.TicksPerSecond,
            TimeUnit.Minutes      => (decimal)ts.Ticks / TimeSpan.TicksPerMinute,
            TimeUnit.Hours        => (decimal)ts.Ticks / TimeSpan.TicksPerHour,
            TimeUnit.Days         => (decimal)ts.Ticks / TimeSpan.TicksPerDay,
            TimeUnit.Weeks        => (decimal)ts.Ticks / (TimeSpan.TicksPerDay * 7L),
            TimeUnit.Months       => (decimal)ts.Ticks / (TimeSpan.TicksPerDay * Constants.DaysInMonthAvg),  // approximate
            TimeUnit.Years        => (decimal)ts.Ticks / (TimeSpan.TicksPerDay * Constants.DaysInYearAvg),
            _                     => throw new NotSupportedException("TimeUnit not supported.")
        };
    }

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
    /// <c>true</c> if the conversion succeeded; <c>false</c> if any parameter was invalid
    /// (e.g., negative time, negative decimalPlaces, or unsupported unit).
    /// </returns>
    public bool TryConvert(decimal time, TimeUnit fromUnit, TimeUnit toUnit, out decimal result, int? decimalPlaces = null)
    {
        try
        {
            result = Convert(time, fromUnit, toUnit, decimalPlaces);
            return true;
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException or FormatException)
        {
            result = 0;
            return false;
        }
    }

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
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="input"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="input"/> is empty or whitespace.</exception>
    /// <exception cref="FormatException">Thrown if the string cannot be parsed into a recognised time pattern.</exception>
    public decimal ParseTime(string input)
    {
        if (input == null) throw new ArgumentNullException(nameof(input));
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Input cannot be empty or whitespace.", nameof(input));

        string trimmed = input.Trim().ToLowerInvariant();

        if (trimmed.Contains(':'))
            return ParseColonSeparated(trimmed);

        return ParseLabelledTokens(trimmed);
    }

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
    public bool TryParseTime(string input, out decimal seconds)
    {
        try
        {
            seconds = ParseTime(input);
            return true;
        }
        catch (Exception ex) when (ex is ArgumentException or ArgumentNullException or NotSupportedException or FormatException)
        {
            seconds = 0;
            return false;
        }
    }

    // ── ParseTime helpers ────────────────────────────────────────────────────

    private static decimal ParseColonSeparated(string input)
    {
        string[] parts = input.Split(':');

        if (parts.Length == 2)
        {
            if (!decimal.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal minutes) ||
                !decimal.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal secs))
                throw new FormatException($"Unable to parse time string: '{input}'.");

            return minutes * Constants.SecondsInMinute + secs;
        }

        if (parts.Length == 3)
        {
            if (!decimal.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal hours) ||
                !decimal.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal minutes) ||
                !decimal.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal secs))
                throw new FormatException($"Unable to parse time string: '{input}'.");

            return hours * Constants.SecondsInHour + minutes * Constants.SecondsInMinute + secs;
        }

        throw new FormatException($"Unable to parse time string: '{input}'. Expected 'mm:ss' or 'hh:mm:ss' format.");
    }

    private static readonly Regex TokenRegex = new(
        @"(\d+(?:\.\d+)?)\s*(ms|milliseconds?|mo|months?|s|sec(?:onds)?|m|min(?:utes)?|h|hr|hours?|d|days?|w|weeks?|yr|years?)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static decimal ParseLabelledTokens(string input)
    {
        var matches = TokenRegex.Matches(input);

        if (matches.Count == 0)
            throw new FormatException($"Unable to parse time string: '{input}'. No recognised time tokens found.");

        decimal totalSeconds = 0m;

        foreach (Match match in matches)
        {
            decimal value = decimal.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            string unitToken = match.Groups[2].Value.ToLowerInvariant();

            totalSeconds += unitToken switch
            {
                "ms" or "millisecond" or "milliseconds"           => value / Constants.MillisecondsInSecond,
                "s" or "sec" or "second" or "seconds"             => value,
                "m" or "min" or "minute" or "minutes"             => value * Constants.SecondsInMinute,
                "h" or "hr" or "hour" or "hours"                  => value * Constants.SecondsInHour,
                "d" or "day" or "days"                            => value * Constants.SecondsInDay,
                "w" or "week" or "weeks"                          => value * Constants.SecondsInWeek,
                "mo" or "month" or "months"                       => value * Constants.SecondsInMonth,
                "yr" or "year" or "years"                         => value * Constants.SecondsInYear,
                _ => throw new FormatException($"Unrecognised time unit token: '{unitToken}'.")
            };
        }

        return totalSeconds;
    }

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
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="min"/> is negative, <paramref name="max"/> is negative,
    /// or <paramref name="min"/> is greater than <paramref name="max"/>.
    /// </exception>
    public decimal Clamp(decimal time, TimeUnit unit, decimal min, decimal max)
    {
        if (min < 0m) throw new ArgumentException("min cannot be negative.", nameof(min));
        if (max < 0m) throw new ArgumentException("max cannot be negative.", nameof(max));
        if (min > max) throw new ArgumentException("min cannot be greater than max.");
        decimal inSeconds = ConvertAny(time, unit, TimeUnit.Seconds);
        decimal minSeconds = ConvertAny(min, unit, TimeUnit.Seconds);
        decimal maxSeconds = ConvertAny(max, unit, TimeUnit.Seconds);
        decimal clamped = Math.Max(minSeconds, Math.Min(maxSeconds, inSeconds));
        return ConvertAny(clamped, TimeUnit.Seconds, unit);
    }

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
    public int Compare(decimal time1, TimeUnit unit1, decimal time2, TimeUnit unit2)
    {
        decimal s1 = ConvertAny(time1, unit1, TimeUnit.Seconds);
        decimal s2 = ConvertAny(time2, unit2, TimeUnit.Seconds);
        return s1.CompareTo(s2);
    }

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
    public Dictionary<TimeUnit, decimal> MultiConvert(decimal time, TimeUnit fromUnit, params TimeUnit[] toUnits)
    {
        if (toUnits == null || toUnits.Length == 0)
            throw new ArgumentException("At least one target unit must be provided.", nameof(toUnits));
        return toUnits.ToDictionary(u => u, u => Convert(time, fromUnit, u));
    }

    // ── Private conversion helpers ───────────────────────────────────────────

    /// <summary>
    /// Converts a non-negative time value to seconds using only the unit-factor multiplication,
    /// without the negative-value guard. Used internally by <see cref="SumTimes"/> and
    /// <see cref="SubtractTimes"/> to allow negative individual inputs.
    /// </summary>
    private static decimal ConvertToSeconds(decimal time, TimeUnit unit) => unit switch
    {
        TimeUnit.Milliseconds => MillisecondsToSeconds(time),
        TimeUnit.Seconds      => time,
        TimeUnit.Minutes      => MinutesToSeconds(time),
        TimeUnit.Hours        => HoursToSeconds(time),
        TimeUnit.Days         => DaysToSeconds(time),
        TimeUnit.Weeks        => WeeksToSeconds(time),
        TimeUnit.Months       => MonthsToSeconds(time),
        TimeUnit.Years        => YearsToSeconds(time),
        _                     => throw new NotSupportedException("TimeUnit not supported.")
    };

    /// <summary>
    /// Like <see cref="Convert"/> but accepts negative values (used internally by
    /// <see cref="Clamp"/> and <see cref="Compare"/> where negative times are valid).
    /// </summary>
    private decimal ConvertAny(decimal time, TimeUnit fromUnit, TimeUnit toUnit)
    {
        // Convert magnitude, then restore sign.
        if (time < 0m)
            return -Convert(-time, fromUnit, toUnit);
        return Convert(time, fromUnit, toUnit);
    }

    private static decimal MillisecondsToSeconds(decimal milliseconds) => milliseconds / Constants.MillisecondsInSecond;
    private static decimal SecondsToMilliseconds(decimal seconds) => seconds * Constants.MillisecondsInSecond;
    private static decimal MinutesToSeconds(decimal minutes) => minutes * Constants.SecondsInMinute;
    private static decimal HoursToSeconds(decimal hours) => hours * Constants.SecondsInHour;
    private static decimal SecondsToMinutes(decimal seconds) => seconds / Constants.SecondsInMinute;
    private static decimal SecondsToHours(decimal seconds) => seconds / Constants.SecondsInHour;
    private static decimal DaysToSeconds(decimal days) => days * Constants.SecondsInDay;
    private static decimal WeeksToSeconds(decimal weeks) => weeks * Constants.SecondsInWeek;
    private static decimal MonthsToSeconds(decimal months) => months * Constants.SecondsInMonth;
    private static decimal YearsToSeconds(decimal years) => years * Constants.SecondsInYear;
    private static decimal SecondsToDays(decimal seconds) => seconds / Constants.SecondsInDay;
    private static decimal SecondsToWeeks(decimal seconds) => seconds / Constants.SecondsInWeek;
    private static decimal SecondsToMonths(decimal seconds) => seconds / Constants.SecondsInMonth;
    private static decimal SecondsToYears(decimal seconds) => seconds / Constants.SecondsInYear;
}
