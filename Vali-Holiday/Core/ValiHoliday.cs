using Vali_Holiday.Models;

namespace Vali_Holiday.Core;

/// <summary>
/// The main entry point for the Vali-Holiday library. Maintains a registry of
/// <see cref="IHolidayProvider"/> instances keyed by ISO 3166-1 alpha-2 country code
/// and delegates all holiday queries to the appropriate provider.
/// </summary>
/// <remarks>
/// Typical usage:
/// <code>
/// // Use the factory for a pre-configured instance:
/// var holidays = HolidayProviderFactory.CreateAll();
/// bool isHoliday = holidays.IsHoliday(DateTime.Today, "ES");
///
/// // Or build a custom instance:
/// var holidays = new ValiHoliday()
///     .Register(new SpainHolidayProvider())
///     .Register(new FranceHolidayProvider());
/// </code>
/// </remarks>
public class ValiHoliday : IValiHoliday
{
    private readonly Dictionary<string, IHolidayProvider> _providers = new();

    /// <summary>
    /// Initialises an empty <see cref="ValiHoliday"/> instance with no providers registered.
    /// Use <see cref="Register"/> to add providers, or use
    /// <see cref="HolidayProviderFactory"/> for pre-configured instances.
    /// </summary>
    public ValiHoliday() { }

    /// <summary>
    /// Initialises a <see cref="ValiHoliday"/> instance and registers all providers
    /// in the supplied sequence.
    /// </summary>
    /// <param name="providers">
    /// The providers to register. If two providers share the same
    /// <see cref="IHolidayProvider.CountryCode"/>, the last one wins.
    /// </param>
    public ValiHoliday(IEnumerable<IHolidayProvider> providers)
    {
        foreach (var p in providers) Register(p);
    }

    /// <summary>
    /// Registers a provider, replacing any existing registration for the same country code.
    /// </summary>
    /// <param name="provider">The provider to register.</param>
    /// <returns>The current <see cref="ValiHoliday"/> instance for fluent chaining.</returns>
    public ValiHoliday Register(IHolidayProvider provider)
    {
        _providers[provider.CountryCode.ToUpperInvariant()] = provider;
        return this;
    }

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
    public IHolidayProvider For(string countryCode)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
            throw new ArgumentNullException(nameof(countryCode));

        if (_providers.TryGetValue(countryCode.ToUpperInvariant(), out var p)) return p;
        throw new InvalidOperationException(
            $"No holiday provider registered for country code '{countryCode}'.");
    }

    /// <summary>
    /// Returns <see langword="true"/> if a provider is registered for the given country code.
    /// </summary>
    /// <param name="countryCode">
    /// ISO 3166-1 alpha-2 country code (case-insensitive).
    /// </param>
    /// <returns>
    /// <see langword="true"/> if a provider exists; otherwise <see langword="false"/>.
    /// </returns>
    public bool Supports(string countryCode)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
            throw new ArgumentNullException(nameof(countryCode));

        return _providers.ContainsKey(countryCode.ToUpperInvariant());
    }

    /// <summary>
    /// Gets the ISO 3166-1 alpha-2 codes of all currently registered countries.
    /// </summary>
    public IEnumerable<string> SupportedCountries => _providers.Keys;

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
    public bool IsHoliday(DateTime date, string countryCode)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
            throw new ArgumentNullException(nameof(countryCode));

        return For(countryCode).IsHoliday(date);
    }

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
    public IEnumerable<HolidayInfo> GetHolidays(int year, string countryCode)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
            throw new ArgumentNullException(nameof(countryCode));

        return For(countryCode).GetHolidays(year);
    }

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
    public IEnumerable<HolidayInfo> GetHolidays(int year, params string[] countryCodes)
        => countryCodes.SelectMany(c => GetHolidays(year, c));

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
    public HolidayInfo? GetNextHoliday(DateTime date, string countryCode)
        => GetNextHolidayWithYear(date, countryCode)?.Holiday;

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
    public (HolidayInfo Holiday, int Year)? GetNextHolidayWithYear(DateTime date, string countryCode)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
            throw new ArgumentNullException(nameof(countryCode));

        var provider = For(countryCode);
        for (int year = date.Year; year <= date.Year + 1; year++)
        {
            var holiday = provider.GetHolidays(year)
                .Where(h => !(h.Month == 2 && h.Day == 29 && !DateTime.IsLeapYear(year)))
                .Where(h => new DateTime(year, h.Month, h.Day) >= date.Date)
                .OrderBy(h => h.Month).ThenBy(h => h.Day)
                .FirstOrDefault();
            if (holiday != null) return (holiday, year);
        }
        return null;
    }

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
    public HolidayInfo? GetPreviousHoliday(DateTime date, string countryCode)
        => GetPreviousHolidayWithYear(date, countryCode)?.Holiday;

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
    public (HolidayInfo Holiday, int Year)? GetPreviousHolidayWithYear(DateTime date, string countryCode)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
            throw new ArgumentNullException(nameof(countryCode));

        var provider = For(countryCode);
        for (int year = date.Year; year >= date.Year - 1; year--)
        {
            var holiday = provider.GetHolidays(year)
                .Where(h => !(h.Month == 2 && h.Day == 29 && !DateTime.IsLeapYear(year)))
                .Where(h => new DateTime(year, h.Month, h.Day) < date.Date)
                .OrderByDescending(h => h.Month).ThenByDescending(h => h.Day)
                .FirstOrDefault();
            if (holiday != null) return (holiday, year);
        }
        return null;
    }

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
    public bool IsLongWeekend(DateTime date, string countryCode)
    {
        if (!IsHoliday(date, countryCode)) return false;
        return date.DayOfWeek == DayOfWeek.Monday || date.DayOfWeek == DayOfWeek.Friday;
    }

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
    public IEnumerable<HolidayInfo> HolidaysThisMonth(int year, int month, string countryCode)
        => For(countryCode).GetHolidays(year).Where(h => h.Month == month);
}
