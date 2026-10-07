namespace Vali_Holiday.Utils;

/// <summary>
/// Provides static methods for computing the "observed"/"substitute" date of a fixed holiday
/// that falls on a weekend, following the rule in effect for a given country. Pure, side-effect
/// free date arithmetic — mirrors the existing <see cref="EasterCalculator"/> pattern.
/// </summary>
public static class ObservedDateCalculator
{
    /// <summary>
    /// Applies the United States federal shift rule: Saturday moves to the preceding Friday,
    /// Sunday moves to the following Monday. Dates that already fall on a weekday are returned
    /// unchanged.
    /// </summary>
    /// <param name="actualDate">The literal calendar date of the holiday.</param>
    /// <returns>The observed date under the US federal rule.</returns>
    public static DateTime ShiftSaturdayToFridaySundayToMonday(DateTime actualDate) => actualDate.DayOfWeek switch
    {
        DayOfWeek.Saturday => actualDate.AddDays(-1),
        DayOfWeek.Sunday => actualDate.AddDays(1),
        _ => actualDate
    };

    /// <summary>
    /// Applies the United Kingdom "in lieu" rule: a date falling on Saturday or Sunday moves to
    /// the next available weekday that is not present in <paramref name="unavailableDates"/> (used
    /// to resolve collisions such as Christmas Day/Boxing Day both falling on the same weekend).
    /// Dates that already fall on a weekday are returned unchanged.
    /// </summary>
    /// <param name="actualDate">The literal calendar date of the holiday.</param>
    /// <param name="unavailableDates">
    /// Dates already taken by another holiday or substitute day for the same year, which the
    /// result must not collide with.
    /// </param>
    /// <returns>The observed date under the UK "in lieu" rule.</returns>
    public static DateTime NextAvailableWeekday(DateTime actualDate, ISet<DateTime> unavailableDates)
    {
        if (actualDate.DayOfWeek != DayOfWeek.Saturday && actualDate.DayOfWeek != DayOfWeek.Sunday)
            return actualDate;

        var candidate = actualDate.DayOfWeek == DayOfWeek.Saturday
            ? actualDate.AddDays(2)
            : actualDate.AddDays(1);

        while (unavailableDates.Contains(candidate))
            candidate = candidate.AddDays(1);

        return candidate;
    }
}
