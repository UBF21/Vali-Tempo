namespace Vali_Time.Enums;

/// <summary>
/// Represents the parts of a calendar date used for range operations, period comparisons, and date truncation.
/// </summary>
public enum DatePart
{
    /// <summary>
    /// Represents a single calendar day. Used to truncate or compare at day precision.
    /// </summary>
    Day,

    /// <summary>
    /// Represents a calendar week (Monday–Sunday or Sunday–Saturday depending on <see cref="WeekStart"/>).
    /// </summary>
    Week,

    /// <summary>
    /// Represents a calendar month (1–12).
    /// </summary>
    Month,

    /// <summary>
    /// Represents a calendar quarter (Q1 = Jan–Mar, Q2 = Apr–Jun, Q3 = Jul–Sep, Q4 = Oct–Dec).
    /// </summary>
    Quarter,

    /// <summary>
    /// Represents a calendar year.
    /// </summary>
    Year
}
