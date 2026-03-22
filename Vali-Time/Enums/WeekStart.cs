namespace Vali_Time.Enums;

/// <summary>
/// Defines the first day of the week used in week-based calculations such as <c>StartOf</c>, <c>EndOf</c>, and <c>WeekOfYear</c>.
/// </summary>
public enum WeekStart
{
    /// <summary>
    /// The week starts on Monday. Follows the ISO 8601 standard (used in most of Europe and Latin America).
    /// </summary>
    Monday,

    /// <summary>
    /// The week starts on Sunday. Common in the United States and some other countries.
    /// </summary>
    Sunday
}
