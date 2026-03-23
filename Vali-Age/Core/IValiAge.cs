using Vali_Age.Models;
using Vali_Time.Enums;

namespace Vali_Age.Core;

/// <summary>
/// Defines the public contract for age-related calculations provided by <see cref="ValiAge"/>.
/// </summary>
public interface IValiAge
{
    /// <summary>
    /// Returns the number of complete years elapsed from <paramref name="birthDate"/> to today.
    /// </summary>
    /// <param name="birthDate">The date of birth.</param>
    /// <returns>The number of whole years of age as of today.</returns>
    int Years(DateTime birthDate);

    /// <summary>
    /// Returns the number of complete years elapsed from <paramref name="birthDate"/> to
    /// <paramref name="reference"/>.
    /// </summary>
    /// <param name="birthDate">The date of birth.</param>
    /// <param name="reference">The reference date against which the age is measured.</param>
    /// <returns>The number of whole years of age as of <paramref name="reference"/>.</returns>
    int Years(DateTime birthDate, DateTime reference);

    /// <summary>
    /// Returns an exact age breakdown (years, months, days, total days) from
    /// <paramref name="birthDate"/> to today.
    /// </summary>
    /// <param name="birthDate">The date of birth.</param>
    /// <returns>An <see cref="AgeResult"/> describing the exact age as of today.</returns>
    AgeResult Exact(DateTime birthDate);

    /// <summary>
    /// Returns an exact age breakdown (years, months, days, total days) from
    /// <paramref name="birthDate"/> to <paramref name="reference"/>.
    /// </summary>
    /// <param name="birthDate">The date of birth.</param>
    /// <param name="reference">The reference date against which the age is measured.</param>
    /// <returns>An <see cref="AgeResult"/> describing the exact age as of <paramref name="reference"/>.</returns>
    AgeResult Exact(DateTime birthDate, DateTime reference);

    /// <summary>
    /// Formats the age as a human-readable string from <paramref name="birthDate"/> to today.
    /// </summary>
    /// <param name="birthDate">The date of birth.</param>
    /// <returns>A string in the format <c>"32 years, 4 months, 12 days"</c>.</returns>
    string Format(DateTime birthDate);

    /// <summary>
    /// Formats the age as a human-readable string from <paramref name="birthDate"/> to
    /// <paramref name="reference"/>.
    /// </summary>
    /// <param name="birthDate">The date of birth.</param>
    /// <param name="reference">The reference date against which the age is measured.</param>
    /// <returns>A string in the format <c>"32 years, 4 months, 12 days"</c>.</returns>
    string Format(DateTime birthDate, DateTime reference);

    /// <summary>
    /// Returns a relative time description from <paramref name="date"/> to
    /// the current time as provided by the injected clock, such as <c>"3 days ago"</c>, <c>"in 2 weeks"</c>,
    /// or <c>"just now"</c>.
    /// </summary>
    /// <param name="date">The date to describe relative to now.</param>
    /// <returns>A human-readable relative time string.</returns>
    string Relative(DateTime date);

    /// <summary>
    /// Returns a relative time description from <paramref name="date"/> to
    /// <paramref name="reference"/>, such as <c>"3 days ago"</c>, <c>"in 2 weeks"</c>,
    /// or <c>"just now"</c>.
    /// </summary>
    /// <param name="date">The date to describe relative to <paramref name="reference"/>.</param>
    /// <param name="reference">The reference point in time.</param>
    /// <returns>A human-readable relative time string.</returns>
    string Relative(DateTime date, DateTime reference);

    /// <summary>
    /// Determines whether the person born on <paramref name="birthDate"/> has reached at least
    /// <paramref name="amount"/> of the specified <paramref name="part"/> as of today.
    /// </summary>
    /// <param name="birthDate">The date of birth.</param>
    /// <param name="amount">The minimum age threshold to check.</param>
    /// <param name="part">The calendar unit (<see cref="DatePart.Year"/>, <see cref="DatePart.Month"/>,
    /// or <see cref="DatePart.Day"/>) in which the threshold is expressed.</param>
    /// <returns><c>true</c> if the age satisfies the minimum threshold as of today.</returns>
    bool IsAtLeast(DateTime birthDate, int amount, DatePart part);

    /// <summary>
    /// Determines whether the person born on <paramref name="birthDate"/> has reached at least
    /// <paramref name="amount"/> of the specified <paramref name="part"/> as of
    /// <paramref name="reference"/>.
    /// </summary>
    /// <param name="birthDate">The date of birth.</param>
    /// <param name="amount">The minimum age threshold to check.</param>
    /// <param name="part">The calendar unit (<see cref="DatePart.Year"/>, <see cref="DatePart.Month"/>,
    /// or <see cref="DatePart.Day"/>) in which the threshold is expressed.</param>
    /// <param name="reference">The reference date against which the check is performed.</param>
    /// <returns><c>true</c> if the age satisfies the minimum threshold as of <paramref name="reference"/>.</returns>
    bool IsAtLeast(DateTime birthDate, int amount, DatePart part, DateTime reference);

    /// <summary>
    /// Determines whether today is the birthday of the person born on <paramref name="birthDate"/>
    /// (same day and month).
    /// </summary>
    /// <param name="birthDate">The date of birth.</param>
    /// <returns><c>true</c> if today shares the same day and month as <paramref name="birthDate"/>.</returns>
    bool IsBirthday(DateTime birthDate);

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
    bool IsBirthday(DateTime birthDate, DateTime reference);

    /// <summary>
    /// Returns the date of the next occurrence of the birthday after today.
    /// </summary>
    /// <param name="birthDate">The date of birth.</param>
    /// <returns>
    /// A <see cref="DateTime"/> representing the next birthday. If today is the birthday,
    /// the next year's date is returned.
    /// </returns>
    DateTime NextBirthday(DateTime birthDate);

    /// <summary>
    /// Returns the date of the next occurrence of the birthday after <paramref name="reference"/>.
    /// </summary>
    /// <param name="birthDate">The date of birth.</param>
    /// <param name="reference">The reference date from which to search for the next birthday.</param>
    /// <returns>
    /// A <see cref="DateTime"/> representing the next birthday after <paramref name="reference"/>.
    /// If <paramref name="reference"/> is the birthday, the next year's date is returned.
    /// </returns>
    DateTime NextBirthday(DateTime birthDate, DateTime reference);

    /// <summary>
    /// Returns the date of the most recent birthday on or before today.
    /// </summary>
    /// <param name="birthDate">The date of birth.</param>
    /// <returns>
    /// A <see cref="DateTime"/> representing the most recent birthday.
    /// If today is the birthday, today's date is returned.
    /// </returns>
    DateTime PreviousBirthday(DateTime birthDate);

    /// <summary>
    /// Returns the date of the most recent birthday on or before <paramref name="reference"/>.
    /// </summary>
    /// <param name="birthDate">The date of birth.</param>
    /// <param name="reference">The reference date.</param>
    /// <returns>
    /// A <see cref="DateTime"/> representing the most recent birthday on or before
    /// <paramref name="reference"/>.
    /// If <paramref name="reference"/> is the birthday, that date is returned.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="reference"/> is before the first birthday.
    /// </exception>
    DateTime PreviousBirthday(DateTime birthDate, DateTime reference);

    /// <summary>
    /// Returns the number of days remaining until the next birthday from today.
    /// </summary>
    /// <param name="birthDate">The date of birth.</param>
    /// <returns>The number of days until the next birthday.</returns>
    int DaysUntilBirthday(DateTime birthDate);

    /// <summary>
    /// Returns the number of days remaining until the next birthday from <paramref name="reference"/>.
    /// </summary>
    /// <param name="birthDate">The date of birth.</param>
    /// <param name="reference">The reference date from which to count.</param>
    /// <returns>The number of days from <paramref name="reference"/> until the next birthday.</returns>
    int DaysUntilBirthday(DateTime birthDate, DateTime reference);
}
