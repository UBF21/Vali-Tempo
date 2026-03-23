using Vali_Age.Models;
using Vali_Time.Abstractions;
using Vali_Time.Enums;
using Vali_Time.Utils;

namespace Vali_Age.Core;

/// <summary>
/// Core class providing age calculations, relative time formatting, and birthday utilities.
/// </summary>
public class ValiAge : IValiAge
{
    private readonly IClock _clock;

    /// <summary>
    /// Initializes a new instance of <see cref="ValiAge"/>.
    /// </summary>
    /// <param name="clock">
    /// Optional clock abstraction. Defaults to <see cref="SystemClock.Instance"/> when <c>null</c>.
    /// Inject a test double to control time in unit tests.
    /// </param>
    public ValiAge(IClock? clock = null)
    {
        _clock = clock ?? SystemClock.Instance;
    }

    // =========================================================================
    // YEARS
    // =========================================================================

    /// <summary>
    /// Returns the number of complete years elapsed from <paramref name="birthDate"/> to today.
    /// </summary>
    /// <param name="birthDate">The date of birth.</param>
    /// <returns>The number of whole years of age as of <see cref="DateTime.Today"/>.</returns>
    public int Years(DateTime birthDate)
    {
        return Years(birthDate, _clock.Today);
    }

    /// <summary>
    /// Returns the number of complete years elapsed from <paramref name="birthDate"/> to
    /// <paramref name="reference"/>.
    /// </summary>
    /// <remarks>
    /// The birthday is considered to have occurred on <paramref name="reference"/> only if
    /// <paramref name="reference"/> is greater than or equal to the date obtained by adding
    /// the computed years to <paramref name="birthDate"/>.
    /// </remarks>
    /// <param name="birthDate">The date of birth.</param>
    /// <param name="reference">The reference date against which the age is measured.</param>
    /// <returns>The number of whole years of age as of <paramref name="reference"/>.</returns>
    public int Years(DateTime birthDate, DateTime reference)
    {
        var today = reference.Date;
        var birth = birthDate.Date;
        int age = today.Year - birth.Year;
        if (today < birth.AddYears(age)) age--;
        return age;
    }

    // =========================================================================
    // EXACT
    // =========================================================================

    /// <summary>
    /// Returns an exact age breakdown (years, months, days, total days) from
    /// <paramref name="birthDate"/> to today.
    /// </summary>
    /// <param name="birthDate">The date of birth.</param>
    /// <returns>An <see cref="AgeResult"/> describing the exact age as of <see cref="DateTime.Today"/>.</returns>
    public AgeResult Exact(DateTime birthDate)
    {
        return Exact(birthDate, _clock.Today);
    }

    /// <summary>
    /// Returns an exact age breakdown (years, months, days, total days) from
    /// <paramref name="birthDate"/> to <paramref name="reference"/>.
    /// </summary>
    /// <remarks>
    /// Whole years are subtracted first, then whole months are counted from the resulting date,
    /// and finally the remaining calendar days are computed. The <c>TotalDays</c> field is the
    /// raw day difference between the two date components.
    /// </remarks>
    /// <param name="birthDate">The date of birth.</param>
    /// <param name="reference">The reference date against which the age is measured.</param>
    /// <returns>An <see cref="AgeResult"/> describing the exact age as of <paramref name="reference"/>.</returns>
    public AgeResult Exact(DateTime birthDate, DateTime reference)
    {
        int years = Years(birthDate, reference);
        DateTime afterYears = birthDate.AddYears(years);

        int months = 0;
        while (afterYears.AddMonths(months + 1) <= reference) months++;

        DateTime afterMonths = afterYears.AddMonths(months);
        int days      = (reference.Date - afterMonths.Date).Days;
        int totalDays = (reference.Date - birthDate.Date).Days;

        return new AgeResult(years, months, days, totalDays);
    }

    // =========================================================================
    // FORMAT
    // =========================================================================

    /// <summary>
    /// Formats the age as a human-readable string from <paramref name="birthDate"/> to today.
    /// </summary>
    /// <param name="birthDate">The date of birth.</param>
    /// <returns>A string in the format <c>"32 years, 4 months, 12 days"</c>.</returns>
    public string Format(DateTime birthDate)
    {
        return Format(birthDate, _clock.Today);
    }

    /// <summary>
    /// Formats the age as a human-readable string from <paramref name="birthDate"/> to
    /// <paramref name="reference"/>.
    /// </summary>
    /// <param name="birthDate">The date of birth.</param>
    /// <param name="reference">The reference date against which the age is measured.</param>
    /// <returns>A string in the format <c>"32 years, 4 months, 12 days"</c>.</returns>
    public string Format(DateTime birthDate, DateTime reference)
    {
        return Exact(birthDate, reference).ToString();
    }

    // =========================================================================
    // RELATIVE
    // =========================================================================

    /// <summary>
    /// Returns a relative time description from <paramref name="date"/> to
    /// <see cref="DateTime.Now"/>.
    /// </summary>
    /// <remarks>
    /// Thresholds applied (absolute difference):
    /// <list type="bullet">
    ///   <item><description>Less than 60 seconds: <c>"just now"</c></description></item>
    ///   <item><description>Less than 60 minutes: <c>"X minutes ago"</c> / <c>"in X minutes"</c></description></item>
    ///   <item><description>Less than 24 hours: <c>"X hours ago"</c> / <c>"in X hours"</c></description></item>
    ///   <item><description>Less than 7 days: <c>"X days ago"</c> / <c>"in X days"</c></description></item>
    ///   <item><description>Less than 30 days: <c>"X weeks ago"</c> / <c>"in X weeks"</c></description></item>
    ///   <item><description>Less than 12 months: <c>"X months ago"</c> / <c>"in X months"</c></description></item>
    ///   <item><description>12 months or more: <c>"X years ago"</c> / <c>"in X years"</c></description></item>
    /// </list>
    /// </remarks>
    /// <param name="date">The date to describe relative to now.</param>
    /// <returns>A human-readable relative time string.</returns>
    public string Relative(DateTime date)
    {
        return Relative(date, _clock.Now);
    }

    /// <summary>
    /// Returns a relative time description from <paramref name="date"/> to
    /// <paramref name="reference"/>.
    /// </summary>
    /// <remarks>
    /// Thresholds applied (absolute difference):
    /// <list type="bullet">
    ///   <item><description>Less than 60 seconds: <c>"just now"</c></description></item>
    ///   <item><description>Less than 60 minutes: <c>"X minutes ago"</c> / <c>"in X minutes"</c></description></item>
    ///   <item><description>Less than 24 hours: <c>"X hours ago"</c> / <c>"in X hours"</c></description></item>
    ///   <item><description>Less than 7 days: <c>"X days ago"</c> / <c>"in X days"</c></description></item>
    ///   <item><description>Less than 30 days: <c>"X weeks ago"</c> / <c>"in X weeks"</c></description></item>
    ///   <item><description>Less than 12 months: <c>"X months ago"</c> / <c>"in X months"</c></description></item>
    ///   <item><description>12 months or more: <c>"X years ago"</c> / <c>"in X years"</c></description></item>
    /// </list>
    /// </remarks>
    /// <param name="date">The date to describe relative to <paramref name="reference"/>.</param>
    /// <param name="reference">The reference point in time.</param>
    /// <returns>A human-readable relative time string.</returns>
    public string Relative(DateTime date, DateTime reference)
    {
        TimeSpan diff    = reference - date;
        bool     isPast  = diff.Ticks >= 0;
        double   seconds = Math.Abs(diff.TotalSeconds);

        if (seconds < 60)
            return "just now";

        (string label, int value) = seconds switch
        {
            < (double)Constants.SecondsInHour    => ("minute", (int)(seconds / (double)Constants.SecondsInMinute)),
            < (double)Constants.SecondsInDay     => ("hour",   (int)(seconds / (double)Constants.SecondsInHour)),
            < (double)Constants.SecondsInWeek    => ("day",    (int)(seconds / (double)Constants.SecondsInDay)),
            < (double)Constants.SecondsInMonth   => ("week",   (int)(seconds / (double)Constants.SecondsInWeek)),
            < (double)Constants.SecondsInYear    => ("month",  (int)(seconds / (double)Constants.SecondsInMonth)),
            _                                    => ("year",   (int)(seconds / (double)Constants.SecondsInYear))
        };

        string plural = value == 1 ? label : label + "s";

        return isPast
            ? $"{value} {plural} ago"
            : $"in {value} {plural}";
    }

    // =========================================================================
    // IS AT LEAST
    // =========================================================================

    /// <summary>
    /// Determines whether the person born on <paramref name="birthDate"/> has reached at least
    /// <paramref name="amount"/> of the specified <paramref name="part"/> as of today.
    /// </summary>
    /// <param name="birthDate">The date of birth.</param>
    /// <param name="amount">The minimum age threshold to check.</param>
    /// <param name="part">
    /// The calendar unit in which the threshold is expressed. Supported values are
    /// <see cref="DatePart.Year"/>, <see cref="DatePart.Month"/>, and <see cref="DatePart.Day"/>.
    /// </param>
    /// <returns><c>true</c> if the age satisfies the minimum threshold as of today.</returns>
    public bool IsAtLeast(DateTime birthDate, int amount, DatePart part)
    {
        return IsAtLeast(birthDate, amount, part, _clock.Today);
    }

    /// <summary>
    /// Determines whether the person born on <paramref name="birthDate"/> has reached at least
    /// <paramref name="amount"/> of the specified <paramref name="part"/> as of
    /// <paramref name="reference"/>.
    /// </summary>
    /// <param name="birthDate">The date of birth.</param>
    /// <param name="amount">The minimum age threshold to check.</param>
    /// <param name="part">
    /// The calendar unit in which the threshold is expressed. Supported values are
    /// <see cref="DatePart.Year"/>, <see cref="DatePart.Month"/>, and <see cref="DatePart.Day"/>.
    /// </param>
    /// <param name="reference">The reference date against which the check is performed.</param>
    /// <returns><c>true</c> if the age satisfies the minimum threshold as of <paramref name="reference"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="part"/> is not <see cref="DatePart.Year"/>,
    /// <see cref="DatePart.Month"/>, or <see cref="DatePart.Day"/>.
    /// </exception>
    public bool IsAtLeast(DateTime birthDate, int amount, DatePart part, DateTime reference)
    {
        AgeResult age = Exact(birthDate, reference);

        return part switch
        {
            DatePart.Year  => age.Years >= amount,
            DatePart.Month => age.Years * 12 + age.Months >= amount,
            DatePart.Day   => age.TotalDays >= amount,
            _ => throw new ArgumentOutOfRangeException(nameof(part), part,
                     "Supported DatePart values are Year, Month, and Day.")
        };
    }

    // =========================================================================
    // IS BIRTHDAY
    // =========================================================================

    /// <summary>
    /// Determines whether today is the birthday of the person born on <paramref name="birthDate"/>
    /// (same day and month).
    /// </summary>
    /// <param name="birthDate">The date of birth.</param>
    /// <returns><c>true</c> if today shares the same day and month as <paramref name="birthDate"/>.</returns>
    public bool IsBirthday(DateTime birthDate)
    {
        return IsBirthday(birthDate, _clock.Today);
    }

    /// <summary>
    /// Determines whether <paramref name="reference"/> is the birthday of the person born on
    /// <paramref name="birthDate"/> (same day and month).
    /// </summary>
    /// <param name="birthDate">The date of birth.</param>
    /// <param name="reference">The date to check against.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="reference"/> shares the same day and month as
    /// <paramref name="birthDate"/>.
    /// </returns>
    public bool IsBirthday(DateTime birthDate, DateTime reference)
    {
        DateTime effectiveBirthday = BirthdayInYear(birthDate, reference.Year);
        return reference.Date == effectiveBirthday;
    }

    // =========================================================================
    // NEXT BIRTHDAY
    // =========================================================================

    /// <summary>
    /// Returns the date of the next occurrence of the birthday after today.
    /// </summary>
    /// <remarks>
    /// If the birthday falls on today, the following year's date is returned.
    /// </remarks>
    /// <param name="birthDate">The date of birth.</param>
    /// <returns>A <see cref="DateTime"/> representing the next upcoming birthday.</returns>
    public DateTime NextBirthday(DateTime birthDate)
    {
        return NextBirthday(birthDate, _clock.Today);
    }

    /// <summary>
    /// Returns the date of the next occurrence of the birthday after <paramref name="reference"/>.
    /// </summary>
    /// <remarks>
    /// If the birthday of the current year is strictly after <paramref name="reference"/>, that
    /// date is returned; otherwise the birthday of the following year is returned.
    /// </remarks>
    /// <param name="birthDate">The date of birth.</param>
    /// <param name="reference">The reference date from which to search for the next birthday.</param>
    /// <returns>A <see cref="DateTime"/> representing the next birthday after <paramref name="reference"/>.</returns>
    public DateTime NextBirthday(DateTime birthDate, DateTime reference)
    {
        var thisYear = BirthdayInYear(birthDate, reference.Year);
        return thisYear > reference.Date ? thisYear : BirthdayInYear(birthDate, reference.Year + 1);
    }

    // =========================================================================
    // DAYS UNTIL BIRTHDAY
    // =========================================================================

    /// <summary>
    /// Returns the number of days remaining until the next birthday from today.
    /// </summary>
    /// <param name="birthDate">The date of birth.</param>
    /// <returns>The number of whole days from today until the next birthday.</returns>
    public int DaysUntilBirthday(DateTime birthDate)
    {
        return DaysUntilBirthday(birthDate, _clock.Today);
    }

    /// <summary>
    /// Returns the number of days remaining until the next birthday from <paramref name="reference"/>.
    /// </summary>
    /// <param name="birthDate">The date of birth.</param>
    /// <param name="reference">The reference date from which to count.</param>
    /// <returns>The number of whole days from <paramref name="reference"/> until the next birthday.</returns>
    public int DaysUntilBirthday(DateTime birthDate, DateTime reference)
    {
        DateTime next = NextBirthday(birthDate, reference);
        return (next - reference.Date).Days;
    }

    // =========================================================================
    // PRIVATE HELPERS
    // =========================================================================

    /// <summary>
    /// Returns the date on which <paramref name="birthDate"/>'s anniversary falls in the given
    /// <paramref name="year"/>, applying the Feb 28 convention for leap-day birthdays in
    /// non-leap years.
    /// </summary>
    /// <param name="birthDate">The original date of birth.</param>
    /// <param name="year">The target year.</param>
    /// <returns>
    /// A <see cref="DateTime"/> representing the birthday in <paramref name="year"/>.
    /// When <paramref name="birthDate"/> is February 29 and <paramref name="year"/> is not a
    /// leap year, February 28 is returned instead.
    /// </returns>
    private static DateTime BirthdayInYear(DateTime birthDate, int year)
    {
        int day = (birthDate.Month == 2 && birthDate.Day == 29 && !DateTime.IsLeapYear(year))
            ? 28
            : birthDate.Day;
        return new DateTime(year, birthDate.Month, day);
    }
}
