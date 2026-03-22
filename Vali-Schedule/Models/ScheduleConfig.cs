using Vali_Schedule.Enums;

namespace Vali_Schedule.Models;

/// <summary>
/// Holds the configuration parameters for a recurring schedule.
/// All properties are set via the fluent API on <see cref="Vali_Schedule.Core.ValiSchedule"/>
/// and are intentionally read-only from the outside to enforce consistent builder usage.
/// </summary>
public class ScheduleConfig
{
    /// <summary>
    /// Gets the recurrence type that determines how the schedule repeats
    /// (e.g., daily, weekly, monthly, yearly, or custom).
    /// </summary>
    public RecurrenceType Type { get; internal set; }

    /// <summary>
    /// Gets the interval between occurrences expressed in the unit defined by <see cref="Type"/>.
    /// For example, an interval of 2 with <see cref="RecurrenceType.Daily"/> means every 2 days.
    /// Defaults to 1 (every period).
    /// </summary>
    public int Interval { get; internal set; } = 1;

    /// <summary>
    /// Gets the days of the week on which the schedule occurs.
    /// Only relevant when <see cref="Type"/> is <see cref="RecurrenceType.Weekly"/>.
    /// </summary>
    public IReadOnlyList<DayOfWeek> DaysOfWeek { get; internal set; } = [];

    /// <summary>
    /// Gets the specific time of day at which the schedule occurs.
    /// When <c>null</c>, occurrences are returned at midnight (00:00:00).
    /// </summary>
    public TimeOnly? TimeOfDay { get; internal set; }

    /// <summary>
    /// Gets the day of the month on which monthly occurrences fall.
    /// Only relevant when <see cref="Type"/> is <see cref="RecurrenceType.Monthly"/>.
    /// When <c>null</c>, the day of <see cref="StartDate"/> is used.
    /// </summary>
    public int? DayOfMonth { get; internal set; }

    /// <summary>
    /// Gets the condition that determines when the schedule ends.
    /// Defaults to <see cref="RecurrenceEnd.Never"/>.
    /// </summary>
    public RecurrenceEnd EndType { get; internal set; } = RecurrenceEnd.Never;

    /// <summary>
    /// Gets the maximum number of occurrences after which the schedule stops.
    /// Only evaluated when <see cref="EndType"/> is <see cref="RecurrenceEnd.AfterOccurrences"/>.
    /// </summary>
    public int? MaxOccurrences { get; internal set; }

    /// <summary>
    /// Gets the date after which no further occurrences are produced.
    /// Only evaluated when <see cref="EndType"/> is <see cref="RecurrenceEnd.OnDate"/>.
    /// </summary>
    public DateTime? EndDate { get; internal set; }

    /// <summary>
    /// Gets the date from which the schedule begins.
    /// No occurrence is generated before this date.
    /// Defaults to <see cref="DateTime.Today"/> if <c>StartingFrom</c> is not called.
    /// </summary>
    public DateTime StartDate { get; internal set; } = DateTime.Today;

    /// <summary>
    /// Optional predicate used when <see cref="RecurrenceType"/> is <see cref="RecurrenceType.Custom"/>.
    /// Receives a candidate date and returns whether it is a valid occurrence.
    /// </summary>
    public Func<DateTime, bool>? CustomPredicate { get; internal set; }
}
