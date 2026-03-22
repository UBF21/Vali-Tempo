namespace Vali_Time.Enums;

/// <summary>
/// Represents the supported units of time for conversions and calculations in the Vali-Time library.
/// These units define the scale of time values, ranging from milliseconds to years.
/// </summary>
public enum TimeUnit
{
    /// <summary>
    /// Represents time in milliseconds (1/1000 of a second). Suitable for high-precision measurements.
    /// </summary>
    Milliseconds,

    /// <summary>
    /// Represents time in seconds. A base unit for many time-related calculations.
    /// </summary>
    Seconds,

    /// <summary>
    /// Represents time in minutes (60 seconds). Commonly used for short durations.
    /// </summary>
    Minutes,

    /// <summary>
    /// Represents time in hours (3600 seconds). Used for longer time spans.
    /// </summary>
    Hours,

    /// <summary>
    /// Represents time in days (86,400 seconds). Used for multi-day durations and date arithmetic.
    /// </summary>
    Days,

    /// <summary>
    /// Represents time in weeks (7 days). Used for scheduling and calendar calculations.
    /// </summary>
    Weeks,

    /// <summary>
    /// Represents time in months. Uses an average of 30.4375 days per month when a concrete calendar
    /// date is not available (365.25 / 12). For calendar-accurate month arithmetic, use <see cref="ValiDate"/>.
    /// </summary>
    Months,

    /// <summary>
    /// Represents time in years. Uses an average of 365.25 days per year (accounting for leap years)
    /// when a concrete calendar date is not available. For calendar-accurate year arithmetic, use <see cref="ValiDate"/>.
    /// </summary>
    Years
}