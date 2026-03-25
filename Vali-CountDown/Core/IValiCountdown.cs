using Vali_Time.Enums;

namespace Vali_CountDown.Core;

/// <summary>
/// Defines countdown and elapsed-time operations based on a reference point in time.
/// </summary>
public interface IValiCountdown
{
    /// <summary>
    /// Returns <c>true</c> if the deadline has already passed relative to <see cref="DateTime.Now"/>.
    /// </summary>
    /// <param name="deadline">The deadline to evaluate.</param>
    bool IsExpired(DateTime deadline);

    /// <summary>
    /// Returns <c>true</c> if the deadline has already passed relative to a custom reference time.
    /// </summary>
    /// <param name="deadline">The deadline to evaluate.</param>
    /// <param name="reference">The reference point in time used for comparison.</param>
    bool IsExpired(DateTime deadline, DateTime reference);

    /// <summary>
    /// Returns the amount of time remaining until the deadline in the specified unit.
    /// Returns 0 if the deadline has already expired.
    /// </summary>
    /// <param name="deadline">The target deadline.</param>
    /// <param name="unit">The time unit in which to express the result.</param>
    /// <param name="decimalPlaces">Optional number of decimal places to round the result to.</param>
    decimal TimeUntil(DateTime deadline, TimeUnit unit, int? decimalPlaces = null);

    /// <summary>
    /// Returns the amount of time that has elapsed since the specified point in the given unit.
    /// When <paramref name="from"/> is in the future, the method returns 0.
    /// </summary>
    /// <param name="from">The starting point in time.</param>
    /// <param name="unit">The time unit in which to express the result.</param>
    /// <param name="decimalPlaces">Optional number of decimal places to round the result to.</param>
    decimal TimeElapsed(DateTime from, TimeUnit unit, int? decimalPlaces = null);

    /// <summary>
    /// Returns a progress value between 0.0 and 1.0 based on the current time relative to a start and end.
    /// Returns 0 if the current time is before or at start; returns 1 if at or past end.
    /// </summary>
    /// <param name="start">The beginning of the time range.</param>
    /// <param name="end">The end of the time range.</param>
    decimal Progress(DateTime start, DateTime end);

    /// <summary>
    /// Returns a progress value between 0.0 and 1.0 using a custom reference time.
    /// Returns 0 if reference is before or at start; returns 1 if at or past end.
    /// </summary>
    /// <param name="start">The beginning of the time range.</param>
    /// <param name="end">The end of the time range.</param>
    /// <param name="reference">The reference point in time used to calculate progress.</param>
    decimal Progress(DateTime start, DateTime end, DateTime reference);

    /// <summary>
    /// Returns the progress as a percentage between 0 and 100.
    /// </summary>
    /// <param name="start">The beginning of the time range.</param>
    /// <param name="end">The end of the time range.</param>
    decimal ProgressPercent(DateTime start, DateTime end);

    /// <summary>
    /// Breaks down the time remaining until the deadline into hours, minutes, seconds, and milliseconds.
    /// Returns a dictionary keyed by <see cref="TimeUnit"/>. Returns all zeros if expired.
    /// </summary>
    /// <param name="deadline">The target deadline.</param>
    Dictionary<TimeUnit, decimal> Breakdown(DateTime deadline);

    /// <summary>
    /// Formats the time remaining until the deadline as a human-readable string (e.g., "5d 3h 20m").
    /// Returns "Expired" if the deadline has already passed.
    /// </summary>
    /// <param name="deadline">The target deadline.</param>
    /// <param name="includeSeconds">If <c>true</c>, seconds are included in the output string.</param>
    string Format(DateTime deadline, bool includeSeconds = false);

    /// <summary>
    /// Returns <c>true</c> if the deadline is within the specified amount of the given time unit from now.
    /// </summary>
    /// <param name="deadline">The target deadline.</param>
    /// <param name="amount">The threshold amount.</param>
    /// <param name="unit">The time unit for the threshold.</param>
    bool IsWithin(DateTime deadline, decimal amount, TimeUnit unit);

    /// <summary>
    /// Returns <c>true</c> if <paramref name="from"/> is in the past or present.
    /// </summary>
    /// <param name="from">The start instant to check.</param>
    bool IsStarted(DateTime from);
}
