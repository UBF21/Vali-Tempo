namespace Vali_Schedule.Core;

using Vali_Time.Enums;

/// <summary>
/// Defines the contract for building and querying a recurring schedule.
/// Implementations expose a fluent configuration API together with
/// methods to enumerate or test occurrences.
/// </summary>
/// <remarks>
/// <b>Thread safety:</b> A single <see cref="IValiSchedule"/> instance is NOT thread-safe.
/// The fluent builder methods mutate internal state. Create a new instance per schedule definition.
/// Do not register as a singleton in a DI container.
/// </remarks>
public interface IValiSchedule
{
    /// <summary>
    /// Sets the recurrence interval and its time unit.
    /// For example, <c>Every(2, TimeUnit.Weeks)</c> produces a biweekly schedule.
    /// </summary>
    /// <param name="interval">The number of units between occurrences. Must be greater than zero.</param>
    /// <param name="unit">
    /// The time unit that qualifies <paramref name="interval"/>.
    /// Supported values: <see cref="TimeUnit.Days"/>, <see cref="TimeUnit.Weeks"/>,
    /// <see cref="TimeUnit.Months"/>, <see cref="TimeUnit.Years"/>.
    /// </param>
    /// <returns>The current <see cref="IValiSchedule"/> instance for fluent chaining.</returns>
    IValiSchedule Every(int interval, TimeUnit unit);

    /// <summary>
    /// Restricts a weekly schedule to the specified days of the week.
    /// Only relevant when the recurrence type is <c>Weekly</c>.
    /// </summary>
    /// <param name="days">One or more days of the week on which the schedule occurs.</param>
    /// <returns>The current <see cref="IValiSchedule"/> instance for fluent chaining.</returns>
    IValiSchedule On(params DayOfWeek[] days);

    /// <summary>
    /// Sets the time of day at which each occurrence takes place.
    /// </summary>
    /// <param name="time">The time of day for each occurrence.</param>
    /// <returns>The current <see cref="IValiSchedule"/> instance for fluent chaining.</returns>
    IValiSchedule At(TimeOnly time);

    /// <summary>
    /// Sets the date from which the schedule starts generating occurrences.
    /// No occurrence is produced before this date.
    /// </summary>
    /// <param name="date">The inclusive start date of the schedule.</param>
    /// <returns>The current <see cref="IValiSchedule"/> instance for fluent chaining.</returns>
    IValiSchedule StartingFrom(DateTime date);

    /// <summary>
    /// Configures the schedule to end after a fixed number of occurrences.
    /// </summary>
    /// <param name="occurrences">The maximum number of occurrences to generate.</param>
    /// <returns>The current <see cref="IValiSchedule"/> instance for fluent chaining.</returns>
    IValiSchedule EndsAfter(int occurrences);

    /// <summary>
    /// Configures the schedule to end on a specific date.
    /// No occurrence is produced after this date.
    /// </summary>
    /// <param name="date">The inclusive end date of the schedule.</param>
    /// <returns>The current <see cref="IValiSchedule"/> instance for fluent chaining.</returns>
    IValiSchedule EndsOn(DateTime date);

    /// <summary>
    /// Sets the day of the month on which monthly occurrences fall.
    /// Only relevant when the recurrence type is <c>Monthly</c>.
    /// </summary>
    /// <param name="day">The day of the month (1–31).</param>
    /// <returns>The current <see cref="IValiSchedule"/> instance for fluent chaining.</returns>
    IValiSchedule OnDayOfMonth(int day);

    /// <summary>
    /// Returns the next occurrence on or after the given reference date.
    /// </summary>
    /// <param name="reference">The date from which to search forward.</param>
    /// <returns>
    /// The next occurrence as a <see cref="DateTime"/>, or <c>null</c> if the schedule
    /// produces no further occurrences within the configured end conditions.
    /// </returns>
    DateTime? NextOccurrence(DateTime reference);

    /// <summary>
    /// Returns the most recent occurrence strictly before the given reference date.
    /// </summary>
    /// <param name="reference">The date from which to search backward.</param>
    /// <returns>
    /// The previous occurrence as a <see cref="DateTime"/>, or <c>null</c> if no
    /// occurrence exists before <paramref name="reference"/>.
    /// </returns>
    DateTime? PreviousOccurrence(DateTime reference);

    /// <summary>
    /// Determines whether the schedule produces an occurrence on the given date,
    /// regardless of the configured time of day.
    /// </summary>
    /// <param name="date">The date to test.</param>
    /// <returns><c>true</c> if the schedule occurs on <paramref name="date"/>; otherwise, <c>false</c>.</returns>
    bool OccursOn(DateTime date);

    /// <summary>
    /// Enumerates up to <paramref name="limit"/> occurrences starting on or after the given reference date.
    /// </summary>
    /// <param name="reference">The date from which to start enumerating occurrences.</param>
    /// <param name="limit">The maximum number of occurrences to return. Defaults to 10.</param>
    /// <returns>A sequence of up to <paramref name="limit"/> occurrence dates.</returns>
    IEnumerable<DateTime> Occurrences(DateTime reference, int limit = 10);

    /// <summary>
    /// Enumerates all occurrences within the inclusive date range
    /// [<paramref name="from"/>, <paramref name="to"/>].
    /// </summary>
    /// <param name="from">The inclusive start of the date range.</param>
    /// <param name="to">The inclusive end of the date range.</param>
    /// <returns>A sequence of all occurrences within the specified range.</returns>
    IEnumerable<DateTime> OccurrencesInRange(DateTime from, DateTime to);
}
