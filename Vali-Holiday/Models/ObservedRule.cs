namespace Vali_Holiday.Models;

/// <summary>
/// Identifies the official weekend-shift ("observed"/"substitute" day) rule that applies to a
/// fixed <see cref="HolidayInfo"/> entry, if any. A provider declares this per holiday; the
/// actual date arithmetic is centralized in <c>ObservedDateCalculator</c>.
/// </summary>
public enum ObservedRule
{
    /// <summary>No official observed-day shift applies to this holiday.</summary>
    None,

    /// <summary>
    /// United States federal rule (5 U.S.C. § 6103, per OPM guidance): a holiday falling on
    /// Saturday is observed the preceding Friday; a holiday falling on Sunday is observed the
    /// following Monday.
    /// </summary>
    SaturdayToFriday_SundayToMonday,

    /// <summary>
    /// United Kingdom "in lieu" rule (Banking and Financial Dealings Act 1971): a bank holiday
    /// falling on a weekend is observed on the next available weekday that is not already a
    /// bank holiday (normally the following Monday, or Tuesday if Monday is already taken by
    /// another substitute day).
    /// </summary>
    NextAvailableWeekday
}
