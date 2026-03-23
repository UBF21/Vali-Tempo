namespace Vali_Time.Core;

/// <summary>
/// Defines quarter-specific date operations, extracted from <see cref="IValiDate"/> following the
/// Interface Segregation Principle. Consumers that only need quarter utilities can depend on this
/// narrower interface instead of the full <see cref="IValiDate"/>.
/// </summary>
public interface IValiDateQuarter
{
    /// <summary>
    /// Returns the quarter number (1–4) of the year in which the specified date falls.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>An integer between 1 and 4 representing the quarter.</returns>
    int QuarterOf(DateTime date);

    /// <summary>
    /// Returns the first day of the quarter in which the specified date falls.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>A <see cref="DateTime"/> representing the start of the quarter.</returns>
    DateTime QuarterStart(DateTime date);

    /// <summary>
    /// Returns the last day of the quarter in which the specified date falls.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>A <see cref="DateTime"/> representing the end of the quarter.</returns>
    DateTime QuarterEnd(DateTime date);

    /// <summary>
    /// Returns the abbreviated name of the quarter in which the specified date falls (e.g., "Q1").
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>A short string representing the quarter name.</returns>
    string QuarterName(DateTime date);

    /// <summary>
    /// Returns the full descriptive name of the quarter in which the specified date falls (e.g., "Q1 2026").
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>A full string representing the quarter name including the year.</returns>
    string QuarterNameFull(DateTime date);

    /// <summary>
    /// Returns the total number of days in the quarter in which the specified date falls.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>The total number of days in the quarter.</returns>
    int DaysInQuarter(DateTime date);

    /// <summary>
    /// Returns the number of days that have elapsed since the start of the quarter up to and including the specified date.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>The number of days elapsed within the quarter.</returns>
    int DaysElapsedInQuarter(DateTime date);

    /// <summary>
    /// Returns the number of days remaining in the quarter after the specified date.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>The number of days remaining in the quarter.</returns>
    int DaysRemainingInQuarter(DateTime date);

    /// <summary>
    /// Returns the progress through the current quarter as a decimal fraction between 0 and 1.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>A decimal between 0 and 1 representing the proportion of the quarter that has elapsed.</returns>
    decimal ProgressInQuarter(DateTime date);

    /// <summary>
    /// Determines whether the specified date is the first day of its quarter.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns><c>true</c> if the date is the first day of its quarter; otherwise, <c>false</c>.</returns>
    bool IsFirstDayOfQuarter(DateTime date);

    /// <summary>
    /// Determines whether the specified date is the last day of its quarter.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns><c>true</c> if the date is the last day of its quarter; otherwise, <c>false</c>.</returns>
    bool IsLastDayOfQuarter(DateTime date);

    /// <summary>
    /// Determines whether two dates fall within the same quarter of the same year.
    /// </summary>
    /// <param name="a">The first date.</param>
    /// <param name="b">The second date.</param>
    /// <returns><c>true</c> if both dates belong to the same quarter and year; otherwise, <c>false</c>.</returns>
    bool IsInSameQuarter(DateTime a, DateTime b);

    /// <summary>
    /// Returns the number of complete or partial weeks contained in the quarter in which the specified date falls.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>The number of weeks in the quarter.</returns>
    int WeeksInQuarter(DateTime date);

    /// <summary>
    /// Returns the first day of the quarter that immediately follows the quarter containing the specified date.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>A <see cref="DateTime"/> representing the start of the next quarter.</returns>
    DateTime NextQuarterStart(DateTime date);

    /// <summary>
    /// Returns the first day of the quarter that immediately precedes the quarter containing the specified date.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>A <see cref="DateTime"/> representing the start of the previous quarter.</returns>
    DateTime PreviousQuarterStart(DateTime date);
}
