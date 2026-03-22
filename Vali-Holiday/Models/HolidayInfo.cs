namespace Vali_Holiday.Models;

/// <summary>
/// Immutable record describing a single public holiday entry for a country.
/// </summary>
public sealed class HolidayInfo
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
        string? description = null)
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
    }

    /// <summary>
    /// Converts this holiday to a <see cref="DateTime"/> for the specified <paramref name="year"/>.
    /// </summary>
    /// <param name="year">The calendar year to use.</param>
    /// <returns>A <see cref="DateTime"/> at midnight on the holiday date.</returns>
    public DateTime ToDateTime(int year) => new DateTime(year, Month, Day);

    /// <inheritdoc/>
    public override string ToString() => $"[{CountryCode}] {Month:D2}/{Day:D2} - {Name}";
}
