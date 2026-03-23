namespace Vali_TimeZone.Models;

/// <summary>
/// Represents a curated timezone entry with IANA-compatible identification,
/// display metadata, UTC offset, DST support flag, and country information.
/// </summary>
public sealed class ValiZoneInfo
{
    /// <summary>
    /// Gets the IANA-style timezone identifier, e.g. <c>"America/Lima"</c>.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets a human-readable display name including the UTC offset,
    /// e.g. <c>"(UTC-05:00) Lima, Bogota, Quito"</c>.
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// Gets the Windows/BCL standard timezone name used by
    /// <see cref="TimeZoneInfo.FindSystemTimeZoneById"/>,
    /// e.g. <c>"SA Pacific Standard Time"</c>.
    /// </summary>
    public string StandardName { get; }

    /// <summary>
    /// Gets the base UTC offset without DST adjustments,
    /// e.g. <c>TimeSpan.FromHours(-5)</c> for UTC-5.
    /// </summary>
    public TimeSpan BaseOffset { get; }

    /// <summary>
    /// Gets a value indicating whether this timezone observes
    /// Daylight Saving Time (DST).
    /// </summary>
    public bool SupportsDst { get; }

    /// <summary>
    /// Gets the ISO 3166-1 alpha-2 country code, e.g. <c>"PE"</c>.
    /// </summary>
    public string CountryCode { get; }

    /// <summary>
    /// Gets the full English country name, e.g. <c>"Peru"</c>.
    /// </summary>
    public string CountryName { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="ValiZoneInfo"/>.
    /// </summary>
    /// <param name="id">IANA-style timezone identifier.</param>
    /// <param name="displayName">Human-readable display name with UTC offset.</param>
    /// <param name="standardName">Windows/BCL standard timezone identifier.</param>
    /// <param name="baseOffset">Base UTC offset (without DST).</param>
    /// <param name="supportsDst">Whether the timezone observes DST.</param>
    /// <param name="countryCode">ISO 3166-1 alpha-2 country code.</param>
    /// <param name="countryName">Full English country name.</param>
    public ValiZoneInfo(
        string id,
        string displayName,
        string standardName,
        TimeSpan baseOffset,
        bool supportsDst,
        string countryCode,
        string countryName)
    {
        Id = id;
        DisplayName = displayName;
        StandardName = standardName;
        BaseOffset = baseOffset;
        SupportsDst = supportsDst;
        CountryCode = countryCode;
        CountryName = countryName;
    }

    /// <summary>
    /// Returns a concise string representation showing the country code,
    /// timezone ID, and base UTC offset.
    /// </summary>
    /// <returns>
    /// A string in the format <c>"[PE] America/Lima (UTC-05:00)"</c>.
    /// </returns>
    public override string ToString()
    {
        var absOffset = BaseOffset < TimeSpan.Zero ? BaseOffset.Negate() : BaseOffset;
        var sign = BaseOffset >= TimeSpan.Zero ? "+" : "-";
        int hours = (int)absOffset.TotalHours;
        int minutes = absOffset.Minutes;
        return $"[{CountryCode}] {Id} (UTC{sign}{hours:D2}:{minutes:D2})";
    }
}
