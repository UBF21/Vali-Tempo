namespace Vali_Holiday.Models;

/// <summary>
/// Immutable record describing a single public holiday entry for a country.
/// </summary>
/// <remarks>
/// Implemented as a <see langword="record"/> for free structural equality — this type has no
/// identity of its own, it is a value produced by a holiday lookup. Note that <see cref="Names"/>
/// is compared by reference (records do not deep-compare collections), so two instances with
/// equal dictionary contents but different <see cref="IReadOnlyDictionary{TKey,TValue}"/>
/// instances are not equal under the generated <c>Equals</c>.
/// </remarks>
public sealed record HolidayInfo
{
    /// <summary>
    /// Unique identifier for this holiday entry (e.g., "pe_new_year").
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Month component of the holiday date (1–12).
    /// For movable holidays this reflects the computed month for the requested year.
    /// </summary>
    public int Month { get; }

    /// <summary>
    /// Day-of-month component of the holiday date (1–31).
    /// For movable holidays this reflects the computed day for the requested year.
    /// </summary>
    public int Day { get; }

    /// <summary>
    /// ISO 3166-1 alpha-2 country code (e.g., "PE", "CL", "AR").
    /// </summary>
    public string CountryCode { get; }

    /// <summary>
    /// ISO 3166-2 region/state code, or <see langword="null"/> when the holiday is national.
    /// </summary>
    public string? RegionCode { get; }

    /// <summary>
    /// Holiday name in the primary language of the country.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Multilingual name map. Keys are BCP 47 language tags: "es", "en", "pt", "fr", "de".
    /// </summary>
    public IReadOnlyDictionary<string, string> Names { get; }

    /// <summary>
    /// Classification of the holiday (National, Religious, Civic, etc.).
    /// </summary>
    public HolidayType Type { get; }

    /// <summary>
    /// <see langword="true"/> when the holiday date is not fixed (e.g., Easter-based or floating weekday).
    /// </summary>
    public bool IsMovable { get; }

    /// <summary>
    /// Optional cultural or historical context for the holiday.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// The official weekend-shift ("observed"/"substitute" day) rule that applies to this
    /// holiday, or <see cref="ObservedRule.None"/> if no such rule is declared for it. See
    /// <see cref="Core.IHolidayProvider.GetObservedDate"/> for how this is resolved to a date.
    /// </summary>
    public ObservedRule ObservedRule { get; }

    /// <summary>
    /// Initialises a new <see cref="HolidayInfo"/> instance.
    /// </summary>
    /// <param name="id">Unique identifier string.</param>
    /// <param name="month">Month (1–12).</param>
    /// <param name="day">Day of month (1–31).</param>
    /// <param name="countryCode">ISO 3166-1 alpha-2 code.</param>
    /// <param name="name">Primary-language name.</param>
    /// <param name="names">Multilingual name dictionary.</param>
    /// <param name="type">Holiday classification.</param>
    /// <param name="isMovable">Whether the date moves each year.</param>
    /// <param name="regionCode">Optional region/state code.</param>
    /// <param name="description">Optional historical or cultural description.</param>
    /// <param name="observedRule">
    /// Optional official weekend-shift rule for this holiday. Defaults to
    /// <see cref="ObservedRule.None"/> (no shift).
    /// </param>
    public HolidayInfo(
        string id,
        int month,
        int day,
        string countryCode,
        string name,
        IReadOnlyDictionary<string, string> names,
        HolidayType type,
        bool isMovable = false,
        string? regionCode = null,
        string? description = null,
        ObservedRule observedRule = ObservedRule.None)
    {
        Id = id;
        Month = month;
        Day = day;
        CountryCode = countryCode;
        Name = name;
        Names = names;
        Type = type;
        IsMovable = isMovable;
        RegionCode = regionCode;
        Description = description;
        ObservedRule = observedRule;
    }

    /// <summary>
    /// Converts this holiday to a <see cref="DateTime"/> for the specified <paramref name="year"/>.
    /// </summary>
    /// <param name="year">The calendar year to use.</param>
    /// <returns>A <see cref="DateTime"/> at midnight on the holiday date.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when this holiday falls on February 29 and <paramref name="year"/> is not a leap
    /// year. Use <see cref="TryToDateTime"/> to convert without throwing in that case.
    /// </exception>
    public DateTime ToDateTime(int year) => new DateTime(year, Month, Day);

    /// <summary>
    /// Attempts to convert this holiday to a <see cref="DateTime"/> for the specified
    /// <paramref name="year"/>, safely handling the February-29-in-a-non-leap-year edge case.
    /// </summary>
    /// <param name="year">The calendar year to use.</param>
    /// <param name="result">
    /// When this method returns <see langword="true"/>, the resulting <see cref="DateTime"/> at
    /// midnight on the holiday date; otherwise <see cref="DateTime.MinValue"/>.
    /// </param>
    /// <returns>
    /// <see langword="false"/> if this holiday falls on February 29 and <paramref name="year"/> is
    /// not a leap year; otherwise <see langword="true"/>.
    /// </returns>
    public bool TryToDateTime(int year, out DateTime result)
    {
        if (Month == 2 && Day == 29 && !DateTime.IsLeapYear(year))
        {
            result = default;
            return false;
        }

        result = new DateTime(year, Month, Day);
        return true;
    }

    /// <inheritdoc/>
    public override string ToString() => $"[{CountryCode}] {Month:D2}/{Day:D2} - {Name}";
}
