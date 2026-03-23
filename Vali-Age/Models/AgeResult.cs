namespace Vali_Age.Models;

/// <summary>
/// Represents the result of an exact age calculation, broken down into years, months, days,
/// and a total day count.
/// </summary>
public readonly struct AgeResult
{
    /// <summary>
    /// Gets the number of complete years in the age.
    /// </summary>
    public int Years { get; }

    /// <summary>
    /// Gets the number of complete months remaining after subtracting whole years.
    /// </summary>
    public int Months { get; }

    /// <summary>
    /// Gets the number of complete days remaining after subtracting whole years and months.
    /// </summary>
    public int Days { get; }

    /// <summary>
    /// Gets the total number of elapsed days between the birth date and the reference date.
    /// </summary>
    public int TotalDays { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="AgeResult"/> with the specified components.
    /// </summary>
    /// <param name="years">The number of complete years.</param>
    /// <param name="months">The number of remaining complete months after whole years.</param>
    /// <param name="days">The number of remaining complete days after whole years and months.</param>
    /// <param name="totalDays">The total number of elapsed days between birth date and reference.</param>
    public AgeResult(int years, int months, int days, int totalDays)
    {
        Years     = years;
        Months    = months;
        Days      = days;
        TotalDays = totalDays;
    }

    /// <summary>
    /// Returns a human-readable string representing the age breakdown.
    /// </summary>
    /// <returns>A string in the format <c>"32 years, 4 months, 12 days"</c>.</returns>
    public override string ToString() =>
        $"{Years} {(Years == 1 ? "year" : "years")}, " +
        $"{Months} {(Months == 1 ? "month" : "months")}, " +
        $"{Days} {(Days == 1 ? "day" : "days")}";
}
