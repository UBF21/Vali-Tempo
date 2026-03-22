namespace Vali_Schedule.Enums;

/// <summary>
/// Specifies the type of recurrence pattern for a schedule.
/// Controls how the schedule repeats over time.
/// </summary>
public enum RecurrenceType
{
    /// <summary>
    /// The schedule repeats on a daily basis, with an optional interval of N days.
    /// </summary>
    Daily,

    /// <summary>
    /// The schedule repeats on a weekly basis, on specific days of the week,
    /// with an optional interval of N weeks.
    /// </summary>
    Weekly,

    /// <summary>
    /// The schedule repeats on a monthly basis, on a specific day of the month,
    /// with an optional interval of N months.
    /// </summary>
    Monthly,

    /// <summary>
    /// The schedule repeats on a yearly basis, anchored to the same day and month
    /// as the start date, with an optional interval of N years.
    /// </summary>
    Yearly,

    /// <summary>
    /// Reserved for custom recurrence logic not covered by the built-in patterns.
    /// </summary>
    Custom
}

/// <summary>
/// Specifies how a recurring schedule terminates.
/// </summary>
public enum RecurrenceEnd
{
    /// <summary>
    /// The schedule repeats indefinitely with no end condition.
    /// </summary>
    Never,

    /// <summary>
    /// The schedule ends after a fixed number of occurrences.
    /// Use <see cref="Vali_Schedule.Models.ScheduleConfig.MaxOccurrences"/> to configure the limit.
    /// </summary>
    AfterOccurrences,

    /// <summary>
    /// The schedule ends on a specific calendar date.
    /// Use <see cref="Vali_Schedule.Models.ScheduleConfig.EndDate"/> to configure the cutoff.
    /// </summary>
    OnDate
}
