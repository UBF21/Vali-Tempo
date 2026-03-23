using Vali_Holiday.Models;

namespace Vali_Holiday.Core;

/// <summary>
/// Defines the public contract for holiday query operations across multiple countries.
/// </summary>
public interface IValiHoliday
{
    /// <summary>
    /// Registers a provider, replacing any existing registration for the same country code.
    /// </summary>
    /// <param name="provider">The provider to register.</param>
    /// <returns>The current <see cref="IValiHoliday"/> instance for fluent chaining.</returns>
    IValiHoliday Register(IHolidayProvider provider);

    /// <summary>
    /// Returns the registered <see cref="IHolidayProvider"/> for the given country code.
    /// </summary>
    /// <param name="countryCode">
    /// ISO 3166-1 alpha-2 country code (case-insensitive, e.g., "ES", "FR", "DE").
    /// </param>
    /// <returns>The registered provider for the country.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no provider has been registered for <paramref name="countryCode"/>.
    /// </exception>
    IHolidayProvider For(string countryCode);

    /// <summary>
    /// Returns <see langword="true"/> if a provider is registered for the given country code.
    /// </summary>
    /// <param name="countryCode">
    /// ISO 3166-1 alpha-2 country code (case-insensitive).
    /// </param>
    /// <returns>
    /// <see langword="true"/> if a provider exists; otherwise <see langword="false"/>.
    /// </returns>
    bool Supports(string countryCode);

    /// <summary>
    /// Gets the ISO 3166-1 alpha-2 codes of all currently registered countries.
    /// </summary>
    IEnumerable<string> SupportedCountries { get; }

    /// <summary>
    /// Determines whether the given <paramref name="date"/> is a public holiday
    /// in the specified country.
    /// </summary>
    /// <param name="date">The date to check (time component is ignored).</param>
    /// <param name="countryCode">
    /// ISO 3166-1 alpha-2 country code (case-insensitive).
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the date is a registered public holiday;
    /// otherwise <see langword="false"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no provider is registered for <paramref name="countryCode"/>.
    /// </exception>
    bool IsHoliday(DateTime date, string countryCode);

    /// <summary>
    /// Returns all public holidays for the given <paramref name="year"/> in the
    /// specified country, ordered by month and day.
    /// </summary>
    /// <param name="year">The four-digit calendar year.</param>
    /// <param name="countryCode">
    /// ISO 3166-1 alpha-2 country code (case-insensitive).
    /// </param>
    /// <returns>
    /// An ordered sequence of <see cref="HolidayInfo"/> for the year.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no provider is registered for <paramref name="countryCode"/>.
    /// </exception>
    IEnumerable<HolidayInfo> GetHolidays(int year, string countryCode);

    /// <summary>
    /// Returns all public holidays for the given <paramref name="year"/> across
    /// multiple countries. Results are not sorted across countries.
    /// </summary>
    /// <param name="year">The four-digit calendar year.</param>
    /// <param name="countryCodes">
    /// One or more ISO 3166-1 alpha-2 country codes (case-insensitive).
    /// </param>
    /// <returns>
    /// A concatenated sequence of <see cref="HolidayInfo"/> for all specified countries.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no provider is registered for any of the specified country codes.
    /// </exception>
    IEnumerable<HolidayInfo> GetHolidays(int year, params string[] countryCodes);

    /// <summary>
    /// Returns the next holiday on or after the given date for the specified country.
    /// Searches up to one year ahead and returns <see langword="null"/> when no holiday is found.
    /// </summary>
    /// <param name="date">The reference date (time component is ignored).</param>
    /// <param name="countryCode">ISO 3166-1 alpha-2 country code (case-insensitive).</param>
    /// <returns>
    /// The next <see cref="HolidayInfo"/> on or after <paramref name="date"/>,
    /// or <see langword="null"/> if none is found within the next year.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no provider is registered for <paramref name="countryCode"/>.
    /// </exception>
    /// <remarks>
    /// This overload does not include the resolved year in its return value. For movable holidays
    /// (e.g. Easter-based), the same <see cref="HolidayInfo"/> object may represent different
    /// calendar dates in different years. Use <see cref="GetNextHolidayWithYear"/> to obtain
    /// both the holiday and the year in which it falls.
    /// </remarks>
    [Obsolete("Use GetNextHolidayWithYear to obtain the resolved year together with the holiday. " +
              "This overload will be removed in a future version.")]
    HolidayInfo? GetNextHoliday(DateTime date, string countryCode);

    /// <summary>
    /// Returns the next holiday on or after the given date for the specified country,
    /// together with the calendar year in which that holiday falls.
    /// Searches up to one year ahead and returns <see langword="null"/> when no holiday is found.
    /// </summary>
    /// <param name="date">The reference date (time component is ignored).</param>
    /// <param name="countryCode">ISO 3166-1 alpha-2 country code (case-insensitive).</param>
    /// <returns>
    /// A tuple of (<see cref="HolidayInfo"/> Holiday, <see cref="int"/> Year) representing the
    /// next holiday and the year it occurs in, or <see langword="null"/> if none is found.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="countryCode"/> is null, empty, or whitespace.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no provider is registered for <paramref name="countryCode"/>.
    /// </exception>
    (HolidayInfo Holiday, int Year)? GetNextHolidayWithYear(DateTime date, string countryCode);

    /// <summary>
    /// Returns the previous holiday strictly before the given date for the specified country.
    /// Searches up to one year back and returns <see langword="null"/> when no holiday is found.
    /// </summary>
    /// <param name="date">The reference date (time component is ignored).</param>
    /// <param name="countryCode">ISO 3166-1 alpha-2 country code (case-insensitive).</param>
    /// <returns>
    /// The most recent <see cref="HolidayInfo"/> before <paramref name="date"/>,
    /// or <see langword="null"/> if none is found within the previous year.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no provider is registered for <paramref name="countryCode"/>.
    /// </exception>
    /// <remarks>
    /// This overload does not include the resolved year in its return value. For movable holidays
    /// (e.g. Easter-based), the same <see cref="HolidayInfo"/> object may represent different
    /// calendar dates in different years. Use <see cref="GetPreviousHolidayWithYear"/> to obtain
    /// both the holiday and the year in which it falls.
    /// </remarks>
    [Obsolete("Use GetPreviousHolidayWithYear to obtain the resolved year together with the holiday. " +
              "This overload will be removed in a future version.")]
    HolidayInfo? GetPreviousHoliday(DateTime date, string countryCode);

    /// <summary>
    /// Returns the previous holiday strictly before the given date for the specified country,
    /// together with the calendar year in which that holiday falls.
    /// Searches up to one year back and returns <see langword="null"/> when no holiday is found.
    /// </summary>
    /// <param name="date">The reference date (time component is ignored).</param>
    /// <param name="countryCode">ISO 3166-1 alpha-2 country code (case-insensitive).</param>
    /// <returns>
    /// A tuple of (<see cref="HolidayInfo"/> Holiday, <see cref="int"/> Year) representing the
    /// most recent holiday before <paramref name="date"/> and the year it occurs in,
    /// or <see langword="null"/> if none is found.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="countryCode"/> is null, empty, or whitespace.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no provider is registered for <paramref name="countryCode"/>.
    /// </exception>
    (HolidayInfo Holiday, int Year)? GetPreviousHolidayWithYear(DateTime date, string countryCode);

    /// <summary>
    /// Determines whether the given date both falls on a public holiday and is adjacent to a weekend
    /// (i.e., it is a Monday or Friday), creating a long weekend.
    /// </summary>
    /// <param name="date">The date to check (time component is ignored).</param>
    /// <param name="countryCode">ISO 3166-1 alpha-2 country code (case-insensitive).</param>
    /// <returns>
    /// <see langword="true"/> if the date is a holiday that falls on a Monday or Friday;
    /// otherwise <see langword="false"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no provider is registered for <paramref name="countryCode"/>.
    /// </exception>
    bool IsLongWeekend(DateTime date, string countryCode);

    /// <summary>
    /// Returns all public holidays in the given month and country.
    /// </summary>
    /// <param name="year">The four-digit calendar year.</param>
    /// <param name="month">The month number (1-12).</param>
    /// <param name="countryCode">ISO 3166-1 alpha-2 country code (case-insensitive).</param>
    /// <returns>
    /// A filtered sequence of <see cref="HolidayInfo"/> for the specified month.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no provider is registered for <paramref name="countryCode"/>.
    /// </exception>
    IEnumerable<HolidayInfo> HolidaysThisMonth(int year, int month, string countryCode);
}
