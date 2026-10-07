namespace Vali_Schedule.Enums;

/// <summary>
/// Defines how a recurring schedule resolves an occurrence that lands on an
/// excluded date (weekend and/or holiday).
/// </summary>
public enum HolidaySkipStrategy
{
    /// <summary>
    /// Moves the occurrence forward, day by day, until it lands on a date that is
    /// not excluded by any active skip rule (weekend and/or holiday).
    /// If no valid date is found within 366 days, the occurrence is dropped.
    /// </summary>
    NextWorkday = 0,

    /// <summary>
    /// Moves the occurrence backward, day by day, until it lands on a date that is
    /// not excluded by any active skip rule (weekend and/or holiday).
    /// If the search crosses the schedule's start date, or no valid date is found
    /// within 366 days, the occurrence is dropped.
    /// </summary>
    PreviousWorkday = 1,

    /// <summary>
    /// Drops the occurrence entirely — it is omitted from <c>Occurrences</c>,
    /// <c>OccurrencesInRange</c>, <c>NextOccurrence</c> and <c>PreviousOccurrence</c>
    /// results, with no replacement date generated for it. The dropped occurrence
    /// still consumes one slot of an <c>EndsAfter</c> budget, consistent with how
    /// <c>WithCustomPredicate</c> rejections are counted.
    /// </summary>
    Skip = 2
}
