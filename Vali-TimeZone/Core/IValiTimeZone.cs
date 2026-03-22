using Vali_TimeZone.Models;

namespace Vali_TimeZone.Core;

/// <summary>
/// Defines the contract for timezone-aware date and time operations, including
/// conversion between timezones, UTC offset calculations, DST detection,
/// timezone discovery, and formatting utilities.
/// </summary>
public interface IValiTimeZone
{
    // ====================================================================
    // Conversion
    // ====================================================================

    /// <summary>
    /// Converts a <see cref="DateTime"/> from one timezone to another.
    /// </summary>
    /// <param name="dateTime">The source date and time value (treated as unspecified kind).</param>
    /// <param name="fromZoneId">The IANA timezone ID of the source timezone, e.g. <c>"America/Lima"</c>.</param>
    /// <param name="toZoneId">The IANA timezone ID of the target timezone, e.g. <c>"Europe/Madrid"</c>.</param>
    /// <returns>The equivalent <see cref="DateTime"/> in the target timezone.</returns>
    /// <exception cref="TimeZoneNotFoundException">
    /// Thrown when <paramref name="fromZoneId"/> or <paramref name="toZoneId"/> cannot be resolved.
    /// </exception>
    DateTime Convert(DateTime dateTime, string fromZoneId, string toZoneId);

    /// <summary>
    /// Converts a <see cref="DateTimeOffset"/> to the specified timezone,
    /// preserving the absolute instant in time.
    /// </summary>
    /// <param name="dateTimeOffset">The source <see cref="DateTimeOffset"/> value.</param>
    /// <param name="toZoneId">The IANA timezone ID of the target timezone.</param>
    /// <returns>A <see cref="DateTimeOffset"/> adjusted to the target timezone's offset.</returns>
    /// <exception cref="TimeZoneNotFoundException">
    /// Thrown when <paramref name="toZoneId"/> cannot be resolved.
    /// </exception>
    DateTimeOffset ConvertOffset(DateTimeOffset dateTimeOffset, string toZoneId);

    /// <summary>
    /// Converts a local <see cref="DateTime"/> in the given timezone to UTC.
    /// </summary>
    /// <param name="localDateTime">The local date and time to convert (treated as unspecified kind).</param>
    /// <param name="fromZoneId">The IANA timezone ID of the source timezone.</param>
    /// <returns>The equivalent UTC <see cref="DateTime"/> with <see cref="DateTimeKind.Utc"/>.</returns>
    /// <exception cref="TimeZoneNotFoundException">
    /// Thrown when <paramref name="fromZoneId"/> cannot be resolved.
    /// </exception>
    DateTime ToUtc(DateTime localDateTime, string fromZoneId);

    /// <summary>
    /// Converts a UTC <see cref="DateTime"/> to local time in the specified timezone.
    /// </summary>
    /// <param name="utcDateTime">The UTC date and time to convert (treated as UTC).</param>
    /// <param name="toZoneId">The IANA timezone ID of the target timezone.</param>
    /// <returns>The equivalent local <see cref="DateTime"/> in the target timezone.</returns>
    /// <exception cref="TimeZoneNotFoundException">
    /// Thrown when <paramref name="toZoneId"/> cannot be resolved.
    /// </exception>
    DateTime FromUtc(DateTime utcDateTime, string toZoneId);

    /// <summary>
    /// Creates a <see cref="DateTimeOffset"/> from a <see cref="DateTime"/> and a timezone,
    /// applying the zone's UTC offset at that instant (including any DST adjustment).
    /// </summary>
    /// <param name="dateTime">The date and time value.</param>
    /// <param name="zoneId">The IANA timezone ID used to determine the UTC offset.</param>
    /// <returns>A <see cref="DateTimeOffset"/> with the resolved offset applied.</returns>
    /// <exception cref="TimeZoneNotFoundException">
    /// Thrown when <paramref name="zoneId"/> cannot be resolved.
    /// </exception>
    DateTimeOffset ToDateTimeOffset(DateTime dateTime, string zoneId);

    // ====================================================================
    // Timezone info
    // ====================================================================

    /// <summary>
    /// Returns the UTC offset of a timezone at a specific instant,
    /// taking Daylight Saving Time into account.
    /// </summary>
    /// <param name="zoneId">The IANA timezone ID.</param>
    /// <param name="at">
    /// The UTC instant at which to evaluate the offset.
    /// Defaults to <see cref="DateTime.UtcNow"/> when <see langword="null"/>.
    /// </param>
    /// <returns>The UTC offset as a <see cref="TimeSpan"/> at the given instant.</returns>
    /// <exception cref="TimeZoneNotFoundException">
    /// Thrown when <paramref name="zoneId"/> cannot be resolved.
    /// </exception>
    TimeSpan GetOffset(string zoneId, DateTime? at = null);

    /// <summary>
    /// Returns the base (standard) UTC offset of a timezone,
    /// without any Daylight Saving Time adjustment.
    /// </summary>
    /// <param name="zoneId">The IANA timezone ID.</param>
    /// <returns>The base UTC offset as a <see cref="TimeSpan"/>.</returns>
    /// <exception cref="TimeZoneNotFoundException">
    /// Thrown when <paramref name="zoneId"/> cannot be resolved.
    /// </exception>
    TimeSpan GetBaseOffset(string zoneId);

    /// <summary>
    /// Determines whether a given local date and time falls within
    /// a Daylight Saving Time period in the specified timezone.
    /// </summary>
    /// <param name="dateTime">The local date and time to check.</param>
    /// <param name="zoneId">The IANA timezone ID.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="dateTime"/> is in DST;
    /// otherwise <see langword="false"/>.
    /// </returns>
    /// <exception cref="TimeZoneNotFoundException">
    /// Thrown when <paramref name="zoneId"/> cannot be resolved.
    /// </exception>
    bool IsDst(DateTime dateTime, string zoneId);

    /// <summary>
    /// Calculates the difference in hours between the UTC offsets of two timezones
    /// at a specific instant.
    /// </summary>
    /// <param name="zoneId1">The IANA timezone ID of the first timezone.</param>
    /// <param name="zoneId2">The IANA timezone ID of the second timezone.</param>
    /// <param name="at">
    /// The UTC instant at which to evaluate the offsets.
    /// Defaults to <see cref="DateTime.UtcNow"/> when <see langword="null"/>.
    /// </param>
    /// <returns>
    /// The difference <c>offset(zoneId1) - offset(zoneId2)</c> expressed in decimal hours.
    /// A positive value means <paramref name="zoneId1"/> is ahead of <paramref name="zoneId2"/>.
    /// </returns>
    /// <exception cref="TimeZoneNotFoundException">
    /// Thrown when either timezone ID cannot be resolved.
    /// </exception>
    decimal OffsetDiff(string zoneId1, string zoneId2, DateTime? at = null);

    // ====================================================================
    // Discovery
    // ====================================================================

    /// <summary>
    /// Looks up a <see cref="ValiZoneInfo"/> by its IANA timezone ID.
    /// </summary>
    /// <param name="zoneId">The IANA timezone ID to search for.</param>
    /// <returns>
    /// The matching <see cref="ValiZoneInfo"/>, or <see langword="null"/>
    /// if the ID is not in the curated dataset.
    /// </returns>
    ValiZoneInfo? FindZone(string zoneId);

    /// <summary>
    /// Returns all timezone entries in the curated dataset.
    /// </summary>
    /// <returns>An enumerable of all <see cref="ValiZoneInfo"/> entries.</returns>
    IEnumerable<ValiZoneInfo> AllZones();

    /// <summary>
    /// Returns all timezone entries associated with a specific country.
    /// </summary>
    /// <param name="countryCode">
    /// The ISO 3166-1 alpha-2 country code, e.g. <c>"US"</c>, <c>"PE"</c>.
    /// The comparison is case-insensitive.
    /// </param>
    /// <returns>
    /// An enumerable of <see cref="ValiZoneInfo"/> entries for the given country.
    /// Returns an empty sequence if no timezones are found for the country code.
    /// </returns>
    IEnumerable<ValiZoneInfo> ZonesForCountry(string countryCode);

    /// <summary>
    /// Determines whether a timezone ID is valid — either in the curated dataset
    /// or resolvable by the host system's timezone database.
    /// </summary>
    /// <param name="zoneId">The IANA timezone ID to validate.</param>
    /// <returns>
    /// <see langword="true"/> if the ID is recognized; otherwise <see langword="false"/>.
    /// </returns>
    bool IsValidZone(string zoneId);

    // ====================================================================
    // Utilities
    // ====================================================================

    /// <summary>
    /// Returns the current date and time in the specified timezone.
    /// </summary>
    /// <param name="zoneId">The IANA timezone ID.</param>
    /// <returns>
    /// A <see cref="DateTime"/> representing "now" in the given timezone,
    /// derived from <see cref="DateTime.UtcNow"/>.
    /// </returns>
    /// <exception cref="TimeZoneNotFoundException">
    /// Thrown when <paramref name="zoneId"/> cannot be resolved.
    /// </exception>
    DateTime Now(string zoneId);

    /// <summary>
    /// Returns the current calendar date (without time component) in the specified timezone.
    /// </summary>
    /// <param name="zoneId">The IANA timezone ID.</param>
    /// <returns>
    /// A <see cref="DateTime"/> with the time set to midnight (<c>00:00:00</c>),
    /// representing today's date in the given timezone.
    /// </returns>
    /// <exception cref="TimeZoneNotFoundException">
    /// Thrown when <paramref name="zoneId"/> cannot be resolved.
    /// </exception>
    DateTime Today(string zoneId);

    /// <summary>
    /// Determines whether two date-time values in different timezones represent
    /// the exact same instant in time (i.e. the same UTC moment).
    /// </summary>
    /// <param name="a">The first date and time value.</param>
    /// <param name="zoneA">The IANA timezone ID of <paramref name="a"/>.</param>
    /// <param name="b">The second date and time value.</param>
    /// <param name="zoneB">The IANA timezone ID of <paramref name="b"/>.</param>
    /// <returns>
    /// <see langword="true"/> if both values resolve to the same UTC instant;
    /// otherwise <see langword="false"/>.
    /// </returns>
    /// <exception cref="TimeZoneNotFoundException">
    /// Thrown when either timezone ID cannot be resolved.
    /// </exception>
    bool IsSameInstant(DateTime a, string zoneA, DateTime b, string zoneB);

    /// <summary>
    /// Formats a <see cref="DateTime"/> as a string in the context of the specified timezone,
    /// applying the zone's UTC offset and using the given format pattern.
    /// </summary>
    /// <param name="dateTime">The date and time value to format.</param>
    /// <param name="zoneId">The IANA timezone ID used to resolve the UTC offset.</param>
    /// <param name="format">
    /// A standard or custom date-time format string.
    /// Defaults to <c>"yyyy-MM-dd HH:mm:ss zzz"</c>.
    /// </param>
    /// <returns>
    /// A formatted string representation of the date and time with timezone offset,
    /// e.g. <c>"2025-07-15 10:00:00 -05:00"</c>.
    /// </returns>
    /// <exception cref="TimeZoneNotFoundException">
    /// Thrown when <paramref name="zoneId"/> cannot be resolved.
    /// </exception>
    string FormatWithZone(DateTime dateTime, string zoneId, string format = "yyyy-MM-dd HH:mm:ss zzz");
}
