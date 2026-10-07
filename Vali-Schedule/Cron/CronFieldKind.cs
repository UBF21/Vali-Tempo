namespace Vali_Schedule.Cron;

/// <summary>
/// Identifies one of the 5 fields of a cron expression, fixing its valid numeric range and
/// whether it accepts name aliases.
/// </summary>
internal enum CronFieldKind
{
    /// <summary>Minute field. Valid range: 0–59. No name aliases.</summary>
    Minute,

    /// <summary>Hour field. Valid range: 0–23. No name aliases.</summary>
    Hour,

    /// <summary>Day-of-month field. Valid range: 1–31. No name aliases.</summary>
    DayOfMonth,

    /// <summary>Month field. Valid range: 1–12. Accepts <c>JAN</c>–<c>DEC</c> aliases.</summary>
    Month,

    /// <summary>
    /// Day-of-week field. Valid range: 0–6 (0 = Sunday). Accepts <c>SUN</c>–<c>SAT</c> aliases.
    /// A raw value of <c>7</c> is also accepted as an alias for Sunday (0), per standard cron.
    /// </summary>
    DayOfWeek
}
