using Vali_TimeZone.Data;
using Vali_TimeZone.Models;

namespace Vali_TimeZone.Core;

/// <summary>
/// Provides timezone-aware date and time operations using IANA-compatible timezone identifiers.
/// Internally delegates to <see cref="TimeZoneInfo.FindSystemTimeZoneById"/> for timezone resolution,
/// which on .NET 8+ supports IANA IDs natively on all platforms.
/// </summary>
public sealed class ValiTimeZone : IValiTimeZone
{
    // ====================================================================
    // Conversion
    // ====================================================================

    /// <inheritdoc />
    public DateTime Convert(DateTime dateTime, string fromZoneId, string toZoneId)
    {
        var fromZone = ResolveZone(fromZoneId);
        var toZone = ResolveZone(toZoneId);
        var utc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified), fromZone);
        return TimeZoneInfo.ConvertTimeFromUtc(utc, toZone);
    }

    /// <inheritdoc />
    public DateTimeOffset ConvertOffset(DateTimeOffset dateTimeOffset, string toZoneId)
    {
        var zone = ResolveZone(toZoneId);
        return TimeZoneInfo.ConvertTime(dateTimeOffset, zone);
    }

    /// <inheritdoc />
    public DateTime ToUtc(DateTime localDateTime, string fromZoneId)
    {
        var zone = ResolveZone(fromZoneId);
        return TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(localDateTime, DateTimeKind.Unspecified), zone);
    }

    /// <inheritdoc />
    public DateTime FromUtc(DateTime utcDateTime, string toZoneId)
    {
        var zone = ResolveZone(toZoneId);
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc), zone);
    }

    /// <inheritdoc />
    public DateTimeOffset ToDateTimeOffset(DateTime dateTime, string zoneId)
    {
        var zone = ResolveZone(zoneId);
        var unspecified = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
        var offset = zone.GetUtcOffset(unspecified);
        return new DateTimeOffset(unspecified, offset);
    }

    // ====================================================================
    // Timezone info
    // ====================================================================

    /// <inheritdoc />
    public TimeSpan GetOffset(string zoneId, DateTime? at = null)
    {
        var zone = ResolveZone(zoneId);
        var dt = DateTime.SpecifyKind(at ?? DateTime.UtcNow, DateTimeKind.Unspecified);
        return zone.GetUtcOffset(dt);
    }

    /// <inheritdoc />
    public TimeSpan GetBaseOffset(string zoneId)
    {
        var zone = ResolveZone(zoneId);
        return zone.BaseUtcOffset;
    }

    /// <inheritdoc />
    public bool IsDst(DateTime dateTime, string zoneId)
    {
        var zone = ResolveZone(zoneId);
        return zone.IsDaylightSavingTime(DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified));
    }

    /// <inheritdoc />
    public decimal OffsetDiff(string zoneId1, string zoneId2, DateTime? at = null)
    {
        var instant = DateTime.SpecifyKind(at ?? DateTime.UtcNow, DateTimeKind.Unspecified);
        var offset1 = ResolveZone(zoneId1).GetUtcOffset(instant).TotalHours;
        var offset2 = ResolveZone(zoneId2).GetUtcOffset(instant).TotalHours;
        return (decimal)(offset1 - offset2);
    }

    // ====================================================================
    // Discovery
    // ====================================================================

    /// <inheritdoc />
    public ValiZoneInfo? FindZone(string zoneId) =>
        TimeZoneData.Zones.TryGetValue(zoneId, out var info) ? info : null;

    /// <inheritdoc />
    public IEnumerable<ValiZoneInfo> AllZones() => TimeZoneData.Zones.Values;

    /// <inheritdoc />
    public IEnumerable<ValiZoneInfo> ZonesForCountry(string countryCode) =>
        TimeZoneData.Zones.Values
            .Where(z => z.CountryCode.Equals(countryCode, StringComparison.OrdinalIgnoreCase));

    /// <inheritdoc />
    public bool IsValidZone(string zoneId)
    {
        if (TimeZoneData.Zones.ContainsKey(zoneId)) return true;
        try { TimeZoneInfo.FindSystemTimeZoneById(zoneId); return true; }
        catch (TimeZoneNotFoundException) { return false; }
    }

    // ====================================================================
    // Utilities
    // ====================================================================

    /// <inheritdoc />
    public DateTime Now(string zoneId) => FromUtc(DateTime.UtcNow, zoneId);

    /// <inheritdoc />
    public DateTime Today(string zoneId) => Now(zoneId).Date;

    /// <inheritdoc />
    public bool IsSameInstant(DateTime a, string zoneA, DateTime b, string zoneB) =>
        ToUtc(a, zoneA) == ToUtc(b, zoneB);

    /// <inheritdoc />
    public string FormatWithZone(DateTime dateTime, string zoneId, string format = "yyyy-MM-dd HH:mm:ss zzz") =>
        ToDateTimeOffset(dateTime, zoneId).ToString(format);

    // ====================================================================
    // Private helpers
    // ====================================================================

    private static TimeZoneInfo ResolveZone(string zoneId)
    {
        if (TimeZoneData.Zones.TryGetValue(zoneId, out var info))
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById(zoneId); }
            catch (TimeZoneNotFoundException)
            {
                return TimeZoneInfo.FindSystemTimeZoneById(info.StandardName);
            }
        }
        return TimeZoneInfo.FindSystemTimeZoneById(zoneId);
    }
}
