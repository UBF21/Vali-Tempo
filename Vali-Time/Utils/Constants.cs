namespace Vali_Time.Utils;

/// <summary>
/// Provides a collection of constant values used throughout the Vali-Time library for time unit conversions and formatting.
/// These constants ensure consistency and precision in calculations and string representations.
/// </summary>
public  static class Constants
{
   /// <summary>
    /// Represents the number of seconds in a minute (60). Used for converting between seconds and minutes.
    /// </summary>
    public const decimal SecondsInMinute = 60m;

    /// <summary>
    /// Represents the number of milliseconds in a second (1000). Used for converting between milliseconds and seconds.
    /// </summary>
    public const decimal MillisecondsInSecond = 1000m;

    /// <summary>
    /// Represents the number of seconds in an hour (3600). Used for converting between seconds and hours.
    /// </summary>
    public const decimal SecondsInHour = 3600m;

    /// <summary>
    /// Represents the string prefix for milliseconds ("ms"). Used in time formatting.
    /// </summary>
    public const string PrefixMilliseconds = "ms";

    /// <summary>
    /// Represents the string prefix for seconds ("s"). Used in time formatting.
    /// </summary>
    public const string PrefixSeconds = "s";

    /// <summary>
    /// Represents the string prefix for minutes ("min"). Used in time formatting.
    /// </summary>
    public const string PrefixMinutes = "min";

    /// <summary>
    /// Represents the string prefix for hours ("h"). Used in time formatting.
    /// </summary>
    public const string PrefixHours = "h";

    // Conversiones temporales nuevas

    /// <summary>
    /// Represents the number of seconds in a day (86400). Used for converting between seconds and days.
    /// </summary>
    public const decimal SecondsInDay = 86400m;

    /// <summary>
    /// Represents the number of seconds in a week (604800). Used for converting between seconds and weeks.
    /// </summary>
    public const decimal SecondsInWeek = 604800m;

    /// <summary>
    /// Represents the average number of seconds in a month (2629800), calculated as 365.25 / 12 * 86400.
    /// Used for converting between seconds and months using the Gregorian calendar average.
    /// </summary>
    public const decimal SecondsInMonth = 2629800m;   // 365.25 / 12 * 86400

    /// <summary>
    /// Represents the average number of seconds in a year (31557600), calculated as 365.25 * 86400.
    /// Used for converting between seconds and years using the Gregorian calendar average.
    /// </summary>
    public const decimal SecondsInYear = 31557600m;    // 365.25 * 86400

    /// <summary>
    /// Represents the number of days in a week (7). Used for converting between days and weeks.
    /// </summary>
    public const decimal DaysInWeek = 7m;

    /// <summary>
    /// Represents the number of months in a year (12). Used for converting between months and years.
    /// </summary>
    public const decimal MonthsInYear = 12m;

    /// <summary>
    /// Represents the average number of days in a year (365.25), accounting for leap years in the Gregorian calendar.
    /// Used for precise year-based calculations.
    /// </summary>
    public const decimal DaysInYearAvg = 365.25m;

    /// <summary>
    /// Represents the average number of days in a month (30.4375), calculated as 365.25 / 12.
    /// Used for precise month-based calculations using the Gregorian calendar average.
    /// </summary>
    public const decimal DaysInMonthAvg = 30.4375m;   // 365.25 / 12

    // Sufijos de formato para nuevas unidades

    /// <summary>
    /// Represents the string suffix for days ("d"). Used in time formatting for day values.
    /// </summary>
    public const string PrefixDays = "d";

    /// <summary>
    /// Represents the string suffix for weeks ("w"). Used in time formatting for week values.
    /// </summary>
    public const string PrefixWeeks = "w";

    /// <summary>
    /// Represents the string suffix for months ("mo"). Used in time formatting for month values.
    /// </summary>
    public const string PrefixMonths = "mo";

    /// <summary>
    /// Represents the string suffix for years ("yr"). Used in time formatting for year values.
    /// </summary>
    public const string PrefixYears = "yr";

}
