using System.Globalization;
using System.Text.RegularExpressions;

namespace Vali_Time.Utils;

/// <summary>
/// Stateless parser for human-readable duration strings, shared by
/// <see cref="Vali_Time.Core.IValiTime.ParseTime"/> and
/// <see cref="Vali_Duration.Models.ValiDuration"/> parsing so both packages
/// agree on exactly one grammar.
/// </summary>
/// <remarks>
/// Supported formats:
/// <list type="bullet">
///   <item><description>Colon-separated: <c>"hh:mm:ss"</c> or <c>"mm:ss"</c> (e.g., <c>"1:30:00"</c>, <c>"45:30"</c>).</description></item>
///   <item><description>
///     Unit-labelled tokens (combinable, e.g., <c>"2d 4h 30m 15s"</c>, <c>"1h 30m"</c>, <c>"90 minutes"</c>, <c>"500ms"</c>):
///     <c>ms</c> / <c>milliseconds</c>, <c>s</c> / <c>sec</c> / <c>seconds</c>,
///     <c>m</c> / <c>min</c> / <c>minutes</c>, <c>h</c> / <c>hr</c> / <c>hours</c>,
///     <c>d</c> / <c>days</c>, <c>w</c> / <c>weeks</c>.
///   </description></item>
/// </list>
/// </remarks>
public static class DurationStringParser
{
    /// <summary>
    /// Parses a human-readable duration string into total seconds. See the type-level
    /// remarks for the supported grammar.
    /// </summary>
    /// <param name="input">The duration string to parse.</param>
    /// <returns>The total duration represented by <paramref name="input"/>, expressed in seconds.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="input"/> is null.</exception>
    /// <exception cref="FormatException">Thrown if <paramref name="input"/> is empty, whitespace, or not a recognised duration pattern.</exception>
    public static decimal Parse(string input)
    {
        if (input == null) throw new ArgumentNullException(nameof(input));
        if (string.IsNullOrWhiteSpace(input))
            throw new FormatException("Input cannot be empty or whitespace.");

        string trimmed = input.Trim().ToLowerInvariant();

        return trimmed.Contains(':') ? ParseColonSeparated(trimmed) : ParseLabelledTokens(trimmed);
    }

    /// <summary>
    /// Attempts to parse a duration string into total seconds without throwing on invalid input.
    /// </summary>
    /// <param name="input">The duration string to parse, or <c>null</c>.</param>
    /// <param name="seconds">When this method returns <c>true</c>, the parsed value in seconds; otherwise <c>0</c>.</param>
    /// <returns><c>true</c> if <paramref name="input"/> was parsed successfully.</returns>
    public static bool TryParse(string? input, out decimal seconds)
    {
        if (input != null)
        {
            try
            {
                seconds = Parse(input);
                return true;
            }
            catch (FormatException)
            {
                // Fall through to the failure case below.
            }
        }

        seconds = 0m;
        return false;
    }

    private static decimal ParseColonSeparated(string input)
    {
        string[] parts = input.Split(':');

        if (parts.Length == 2)
        {
            if (!decimal.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal minutes) ||
                !decimal.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal secs))
                throw new FormatException($"Unable to parse time string: '{input}'.");

            if (minutes < 0 || minutes > 59 || secs < 0 || secs > 59)
                throw new FormatException($"Invalid time format: minutes and seconds must be between 0 and 59 in '{input}'.");

            return minutes * Constants.SecondsInMinute + secs;
        }

        if (parts.Length == 3)
        {
            if (!decimal.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal hours) ||
                !decimal.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal minutes) ||
                !decimal.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal secs))
                throw new FormatException($"Unable to parse time string: '{input}'.");

            if (minutes < 0 || minutes > 59 || secs < 0 || secs > 59)
                throw new FormatException($"Invalid time format: minutes and seconds must be between 0 and 59 in '{input}'.");

            return hours * Constants.SecondsInHour + minutes * Constants.SecondsInMinute + secs;
        }

        throw new FormatException($"Unable to parse time string: '{input}'. Expected 'mm:ss' or 'hh:mm:ss' format.");
    }

    private static readonly Regex TokenRegex = new(
        @"(\d+(?:\.\d+)?)\s*(milliseconds?|ms|months?|mo|sec(?:onds)?|min(?:utes)?|hours?|hr|days?|weeks?|years?|yr|[smhdw])",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static decimal ParseLabelledTokens(string input)
    {
        string trimmed = input.Trim();
        var matches = TokenRegex.Matches(trimmed);

        if (matches.Count == 0)
            throw new FormatException($"Unable to parse time string: '{input}'. No recognised time tokens found.");

        string matched = string.Concat(matches.Select(m => m.Value)).Replace(" ", "");
        string expected = trimmed.Replace(" ", "");
        if (matched != expected)
            throw new FormatException($"Unrecognised tokens in time string: '{input}'.");

        decimal totalSeconds = 0m;

        foreach (Match match in matches)
        {
            decimal value;
            try
            {
                value = decimal.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            }
            catch (OverflowException ex)
            {
                throw new FormatException($"Numeric value out of range in time string: '{input}'.", ex);
            }

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
}
