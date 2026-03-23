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
        var utcInstant = at.HasValue
            ? DateTime.SpecifyKind(at.Value, DateTimeKind.Utc)
            : DateTime.UtcNow;
        // Convert UTC to zone's local time, then get offset
        var localTime = TimeZoneInfo.ConvertTimeFromUtc(utcInstant, zone);
        return zone.GetUtcOffset(localTime);
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
        var utcInstant = at.HasValue
            ? DateTime.SpecifyKind(at.Value, DateTimeKind.Utc)
            : DateTime.UtcNow;
        var zone1 = ResolveZone(zoneId1);
        var zone2 = ResolveZone(zoneId2);
        var local1 = TimeZoneInfo.ConvertTimeFromUtc(utcInstant, zone1);
        var local2 = TimeZoneInfo.ConvertTimeFromUtc(utcInstant, zone2);
        var offset1 = zone1.GetUtcOffset(local1).TotalHours;
        var offset2 = zone2.GetUtcOffset(local2).TotalHours;
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
                // On Windows fall back to the Windows timezone name
                if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
                        System.Runtime.InteropServices.OSPlatform.Windows))
                {
                    try { return TimeZoneInfo.FindSystemTimeZoneById(info.StandardName); }
                    catch (TimeZoneNotFoundException) { }
                }
                throw new TimeZoneNotFoundException(
                    $"Timezone '{zoneId}' not found on this platform. " +
                    $"Ensure the IANA timezone database is installed.");
            }
        }
        return TimeZoneInfo.FindSystemTimeZoneById(zoneId);
    }
}
