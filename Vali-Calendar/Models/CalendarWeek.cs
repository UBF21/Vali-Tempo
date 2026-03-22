namespace Vali_Calendar.Models;

/// <summary>
/// Represents a calendar week, including its week number, year, and the range of dates it spans.
/// </summary>
public readonly struct CalendarWeek
{
    /// <summary>
    /// The week number within the year, from 1 to 53.
    /// </summary>
    public int WeekNumber { get; }

    /// <summary>
    /// The year this week belongs to.
    /// </summary>
    public int Year { get; }

    /// <summary>
    /// The first day of the week (Monday or Sunday depending on configuration).
    /// </summary>
    public DateTime Start { get; }

    /// <summary>
    /// The last day of the week, always 6 days after <see cref="Start"/>.
    /// </summary>
    public DateTime End { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="CalendarWeek"/>.
    /// </summary>
    /// <param name="weekNumber">The week number within the year (1-53).</param>
    /// <param name="year">The year this week belongs to.</param>
    /// <param name="start">The first day of the week.</param>
    /// <param name="end">The last day of the week (6 days after start).</param>
    public CalendarWeek(int weekNumber, int year, DateTime start, DateTime end)
    {
        WeekNumber = weekNumber;
        Year = year;
        Start = start;
        End = end;
    }

    /// <summary>
    /// Returns a string representation of the calendar week in the format "W{nn} {yyyy}" (e.g., "W12 2025").
    /// </summary>
    public override string ToString() => $"W{WeekNumber:D2} {Year}";
}
