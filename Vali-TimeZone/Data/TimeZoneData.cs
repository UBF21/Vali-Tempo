using Vali_TimeZone.Models;

namespace Vali_TimeZone.Data;

/// <summary>
/// Provides a curated, read-only dictionary of <see cref="ValiZoneInfo"/> entries
/// keyed by their IANA-style timezone identifier.
/// </summary>
/// <remarks>
/// The keys use IANA identifiers (e.g. <c>"America/Lima"</c>).
/// On Linux/macOS, .NET resolves IANA IDs natively via
/// <see cref="TimeZoneInfo.FindSystemTimeZoneById"/>. On Windows (net8+),
/// the runtime automatically maps IANA IDs to their Windows equivalents
/// through the ICU / CLDR mapping tables bundled with the SDK.
/// </remarks>
internal static class TimeZoneData
{
    /// <summary>
    /// Read-only map of IANA timezone ID to its <see cref="ValiZoneInfo"/> descriptor.
    /// </summary>
    internal static readonly IReadOnlyDictionary<string, ValiZoneInfo> Zones =
        new Dictionary<string, ValiZoneInfo>
        {
            // ================================================================
            // LATINOAMERICA
            // ================================================================
            ["America/Lima"] = new(
                "America/Lima",
                "(UTC-05:00) Lima, Bogota, Quito",
                "SA Pacific Standard Time",
                TimeSpan.FromHours(-5), false, "PE", "Peru"),

            ["America/Bogota"] = new(
                "America/Bogota",
                "(UTC-05:00) Bogota, Lima, Quito",
                "SA Pacific Standard Time",
                TimeSpan.FromHours(-5), false, "CO", "Colombia"),

            ["America/Guayaquil"] = new(
                "America/Guayaquil",
                "(UTC-05:00) Quito",
                "SA Pacific Standard Time",
                TimeSpan.FromHours(-5), false, "EC", "Ecuador"),

            ["America/Santiago"] = new(
                "America/Santiago",
                "(UTC-04:00) Santiago",
                "Pacific SA Standard Time",
                TimeSpan.FromHours(-4), true, "CL", "Chile"),

            ["America/Argentina/Buenos_Aires"] = new(
                "America/Argentina/Buenos_Aires",
                "(UTC-03:00) Buenos Aires",
                "Argentina Standard Time",
                TimeSpan.FromHours(-3), false, "AR", "Argentina"),

            ["America/Sao_Paulo"] = new(
                "America/Sao_Paulo",
                "(UTC-03:00) Brasilia",
                "E. South America Standard Time",
                TimeSpan.FromHours(-3), true, "BR", "Brazil"),

            ["America/Mexico_City"] = new(
                "America/Mexico_City",
                "(UTC-06:00) Guadalajara, Mexico City, Monterrey",
                "Central Standard Time (Mexico)",
                TimeSpan.FromHours(-6), true, "MX", "Mexico"),

            ["America/Caracas"] = new(
                "America/Caracas",
                "(UTC-04:00) Caracas",
                "Venezuela Standard Time",
                TimeSpan.FromHours(-4), false, "VE", "Venezuela"),

            ["America/La_Paz"] = new(
                "America/La_Paz",
                "(UTC-04:00) Georgetown, La Paz, Manaus, San Juan",
                "SA Western Standard Time",
                TimeSpan.FromHours(-4), false, "BO", "Bolivia"),

            ["America/Asuncion"] = new(
                "America/Asuncion",
                "(UTC-04:00) Asuncion",
                "Paraguay Standard Time",
                TimeSpan.FromHours(-4), true, "PY", "Paraguay"),

            ["America/Montevideo"] = new(
                "America/Montevideo",
                "(UTC-03:00) Montevideo",
                "Montevideo Standard Time",
                TimeSpan.FromHours(-3), false, "UY", "Uruguay"),

            ["America/Panama"] = new(
                "America/Panama",
                "(UTC-05:00) Eastern Time (US & Canada)",
                "SA Pacific Standard Time",
                TimeSpan.FromHours(-5), false, "PA", "Panama"),

            ["America/Costa_Rica"] = new(
                "America/Costa_Rica",
                "(UTC-06:00) Central America",
                "Central America Standard Time",
                TimeSpan.FromHours(-6), false, "CR", "Costa Rica"),

            ["America/Guatemala"] = new(
                "America/Guatemala",
                "(UTC-06:00) Central America",
                "Central America Standard Time",
                TimeSpan.FromHours(-6), false, "GT", "Guatemala"),

            ["America/Tegucigalpa"] = new(
                "America/Tegucigalpa",
                "(UTC-06:00) Central America",
                "Central America Standard Time",
                TimeSpan.FromHours(-6), false, "HN", "Honduras"),

            ["America/El_Salvador"] = new(
                "America/El_Salvador",
                "(UTC-06:00) Central America",
                "Central America Standard Time",
                TimeSpan.FromHours(-6), false, "SV", "El Salvador"),

            ["America/Managua"] = new(
                "America/Managua",
                "(UTC-06:00) Central America",
                "Central America Standard Time",
                TimeSpan.FromHours(-6), false, "NI", "Nicaragua"),

            ["America/Santo_Domingo"] = new(
                "America/Santo_Domingo",
                "(UTC-04:00) Atlantic Time (Canada)",
                "SA Western Standard Time",
                TimeSpan.FromHours(-4), false, "DO", "Dominican Republic"),

            ["America/Havana"] = new(
                "America/Havana",
                "(UTC-05:00) Havana",
                "Cuba Standard Time",
                TimeSpan.FromHours(-5), true, "CU", "Cuba"),

            // ================================================================
            // NORTEAMERICA
            // ================================================================
            ["America/New_York"] = new(
                "America/New_York",
                "(UTC-05:00) Eastern Time (US & Canada)",
                "Eastern Standard Time",
                TimeSpan.FromHours(-5), true, "US", "United States"),

            ["America/Chicago"] = new(
                "America/Chicago",
                "(UTC-06:00) Central Time (US & Canada)",
                "Central Standard Time",
                TimeSpan.FromHours(-6), true, "US", "United States"),

            ["America/Denver"] = new(
                "America/Denver",
                "(UTC-07:00) Mountain Time (US & Canada)",
                "Mountain Standard Time",
                TimeSpan.FromHours(-7), true, "US", "United States"),

            ["America/Los_Angeles"] = new(
                "America/Los_Angeles",
                "(UTC-08:00) Pacific Time (US & Canada)",
                "Pacific Standard Time",
                TimeSpan.FromHours(-8), true, "US", "United States"),

            ["America/Toronto"] = new(
                "America/Toronto",
                "(UTC-05:00) Eastern Time (US & Canada)",
                "Eastern Standard Time",
                TimeSpan.FromHours(-5), true, "CA", "Canada"),

            ["America/Vancouver"] = new(
                "America/Vancouver",
                "(UTC-08:00) Pacific Time (US & Canada)",
                "Pacific Standard Time",
                TimeSpan.FromHours(-8), true, "CA", "Canada"),

            // ================================================================
            // EUROPA
            // ================================================================
            ["Europe/Madrid"] = new(
                "Europe/Madrid",
                "(UTC+01:00) Madrid, Barcelona",
                "Romance Standard Time",
                TimeSpan.FromHours(1), true, "ES", "Spain"),

            ["Europe/London"] = new(
                "Europe/London",
                "(UTC+00:00) London, Edinburgh, Lisbon",
                "GMT Standard Time",
                TimeSpan.Zero, true, "GB", "United Kingdom"),

            ["Europe/Paris"] = new(
                "Europe/Paris",
                "(UTC+01:00) Brussels, Copenhagen, Madrid, Paris",
                "Romance Standard Time",
                TimeSpan.FromHours(1), true, "FR", "France"),

            ["Europe/Berlin"] = new(
                "Europe/Berlin",
                "(UTC+01:00) Amsterdam, Berlin, Bern, Rome, Vienna",
                "W. Europe Standard Time",
                TimeSpan.FromHours(1), true, "DE", "Germany"),

            ["Europe/Rome"] = new(
                "Europe/Rome",
                "(UTC+01:00) Amsterdam, Berlin, Bern, Rome, Vienna",
                "W. Europe Standard Time",
                TimeSpan.FromHours(1), true, "IT", "Italy"),

            ["Europe/Lisbon"] = new(
                "Europe/Lisbon",
                "(UTC+00:00) Lisbon",
                "GMT Standard Time",
                TimeSpan.Zero, true, "PT", "Portugal"),

            ["Europe/Amsterdam"] = new(
                "Europe/Amsterdam",
                "(UTC+01:00) Amsterdam, Berlin",
                "W. Europe Standard Time",
                TimeSpan.FromHours(1), true, "NL", "Netherlands"),

            ["Europe/Brussels"] = new(
                "Europe/Brussels",
                "(UTC+01:00) Brussels, Copenhagen",
                "Romance Standard Time",
                TimeSpan.FromHours(1), true, "BE", "Belgium"),

            ["Europe/Zurich"] = new(
                "Europe/Zurich",
                "(UTC+01:00) Zurich",
                "W. Europe Standard Time",
                TimeSpan.FromHours(1), true, "CH", "Switzerland"),

            ["Europe/Vienna"] = new(
                "Europe/Vienna",
                "(UTC+01:00) Vienna",
                "W. Europe Standard Time",
                TimeSpan.FromHours(1), true, "AT", "Austria"),

            ["Europe/Warsaw"] = new(
                "Europe/Warsaw",
                "(UTC+01:00) Warsaw",
                "Central European Standard Time",
                TimeSpan.FromHours(1), true, "PL", "Poland"),

            ["Europe/Stockholm"] = new(
                "Europe/Stockholm",
                "(UTC+01:00) Stockholm, Oslo, Copenhagen",
                "W. Europe Standard Time",
                TimeSpan.FromHours(1), true, "SE", "Sweden"),

            ["Europe/Oslo"] = new(
                "Europe/Oslo",
                "(UTC+01:00) Oslo",
                "W. Europe Standard Time",
                TimeSpan.FromHours(1), true, "NO", "Norway"),

            ["Europe/Copenhagen"] = new(
                "Europe/Copenhagen",
                "(UTC+01:00) Copenhagen",
                "Romance Standard Time",
                TimeSpan.FromHours(1), true, "DK", "Denmark"),

            ["Europe/Helsinki"] = new(
                "Europe/Helsinki",
                "(UTC+02:00) Helsinki, Kyiv, Riga, Sofia",
                "FLE Standard Time",
                TimeSpan.FromHours(2), true, "FI", "Finland"),

            ["Europe/Dublin"] = new(
                "Europe/Dublin",
                "(UTC+00:00) Dublin",
                "GMT Standard Time",
                TimeSpan.Zero, true, "IE", "Ireland"),

            // ================================================================
            // ASIA / OCEANIA
            // ================================================================
            ["Asia/Tokyo"] = new(
                "Asia/Tokyo",
                "(UTC+09:00) Osaka, Sapporo, Tokyo",
                "Tokyo Standard Time",
                TimeSpan.FromHours(9), false, "JP", "Japan"),

            ["Asia/Seoul"] = new(
                "Asia/Seoul",
                "(UTC+09:00) Seoul",
                "Korea Standard Time",
                TimeSpan.FromHours(9), false, "KR", "South Korea"),

            ["Asia/Shanghai"] = new(
                "Asia/Shanghai",
                "(UTC+08:00) Beijing, Chongqing, Hong Kong, Urumqi",
                "China Standard Time",
                TimeSpan.FromHours(8), false, "CN", "China"),

            ["Asia/Kolkata"] = new(
                "Asia/Kolkata",
                "(UTC+05:30) Chennai, Kolkata, Mumbai, New Delhi",
                "India Standard Time",
                new TimeSpan(5, 30, 0), false, "IN", "India"),

            ["Asia/Dubai"] = new(
                "Asia/Dubai",
                "(UTC+04:00) Abu Dhabi, Muscat",
                "Arabian Standard Time",
                TimeSpan.FromHours(4), false, "AE", "UAE"),

            ["Australia/Sydney"] = new(
                "Australia/Sydney",
                "(UTC+10:00) Canberra, Melbourne, Sydney",
                "AUS Eastern Standard Time",
                TimeSpan.FromHours(10), true, "AU", "Australia"),

            // ================================================================
            // UTC
            // ================================================================
            ["UTC"] = new(
                "UTC",
                "(UTC+00:00) Coordinated Universal Time",
                "UTC",
                TimeSpan.Zero, false, "XX", "Universal"),
        };
}
