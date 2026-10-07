using Vali_Holiday.Models;

namespace Vali_Holiday.Core;

/// <summary>
/// Defines the contract for country-specific public holiday providers.
/// Implement this interface (or extend <see cref="BaseHolidayProvider"/>) to supply
/// holiday data for a new country or region.
/// </summary>
public interface IHolidayProvider
{
    /// <summary>
    /// ISO 3166-1 alpha-2 country code (e.g., "PE", "CL", "AR").
    /// </summary>
    string CountryCode { get; }

    /// <summary>
    /// English name of the country (e.g., "Peru", "Chile", "Argentina").
    /// </summary>
    string CountryName { get; }

    /// <summary>
    /// BCP 47 language tag for the primary official language used in holiday names
    /// (e.g., "es" for Spanish, "pt" for Portuguese, "en" for English).
    /// </summary>
    string PrimaryLanguage { get; }

    /// <summary>
    /// Returns all public holidays for the given <paramref name="year"/>,
    /// including both fixed-date and movable (Easter-based) holidays.
    /// </summary>
    /// <param name="year">The four-digit calendar year.</param>
    /// <returns>
    /// An ordered sequence of <see cref="HolidayInfo"/> sorted by month and day.
    /// </returns>
    IEnumerable<HolidayInfo> GetHolidays(int year);

    /// <summary>
    /// Determines whether the given <paramref name="date"/> falls on a public holiday.
    /// </summary>
    /// <param name="date">The date and time to check (time component is ignored).</param>
    /// <returns><see langword="true"/> if the date is a public holiday; otherwise <see langword="false"/>.</returns>
    bool IsHoliday(DateTime date);

    /// <summary>
    /// Determines whether the given <paramref name="date"/> falls on a public holiday.
    /// </summary>
    /// <param name="date">The date-only value to check.</param>
    /// <returns><see langword="true"/> if the date is a public holiday; otherwise <see langword="false"/>.</returns>
    bool IsHoliday(DateOnly date);

    /// <summary>
    /// Returns the <see cref="HolidayInfo"/> for the given <paramref name="date"/>,
    /// or <see langword="null"/> if the date is not a public holiday.
    /// </summary>
    /// <param name="date">The date and time to look up (time component is ignored).</param>
    /// <returns>
    /// The matching <see cref="HolidayInfo"/>, or <see langword="null"/> if none exists.
    /// </returns>
    HolidayInfo? GetHoliday(DateTime date);

    /// <summary>
    /// Returns all holidays that fall within the inclusive date range
    /// [<paramref name="from"/>, <paramref name="to"/>].
    /// </summary>
    /// <param name="from">The start of the range (inclusive).</param>
    /// <param name="to">The end of the range (inclusive).</param>
    /// <returns>A sequence of matching <see cref="HolidayInfo"/> entries.</returns>
    IEnumerable<HolidayInfo> GetHolidaysInRange(DateTime from, DateTime to);

    /// <summary>
    /// Returns the name of the given <paramref name="holiday"/> in the requested
    /// <paramref name="languageCode"/>. Falls back to the primary language name if the
    /// requested language is not available.
    /// </summary>
    /// <param name="holiday">The holiday whose name is requested.</param>
    /// <param name="languageCode">BCP 47 language tag (e.g., "es", "en", "pt", "fr", "de").</param>
    /// <returns>The holiday name in the requested language, or the primary-language name as fallback.</returns>
    string GetName(HolidayInfo holiday, string languageCode);

    /// <summary>
    /// Returns the official "observed"/"substitute" <see cref="DateTime"/> for
    /// <paramref name="holiday"/> in the given <paramref name="year"/>, applying the weekend-shift
    /// rule declared on <see cref="HolidayInfo.ObservedRule"/>, if any.
    /// </summary>
    /// <param name="holiday">The holiday to resolve an observed date for.</param>
    /// <param name="year">The four-digit calendar year.</param>
    /// <returns>
    /// The observed <see cref="DateTime"/>, or <see langword="null"/> when the holiday declares
    /// no shift rule, its literal date does not fall on a weekend, or its literal date cannot be
    /// represented for <paramref name="year"/> (e.g. Feb 29 in a non-leap year).
    /// </returns>
    DateTime? GetObservedDate(HolidayInfo holiday, int year);
}
