# Vali-TimeZone

[![NuGet](https://img.shields.io/nuget/v/Vali-TimeZone.svg)](https://www.nuget.org/packages/Vali-TimeZone)
[![License](https://img.shields.io/badge/license-Apache--2.0-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8%20%7C%209-purple.svg)](https://dotnet.microsoft.com)

**Vali-TimeZone** is the timezone-aware module of the Vali-Tempo ecosystem. It provides conversion between timezones, UTC offset calculations, DST detection, and a curated catalog of IANA-compatible timezone identifiers for 30+ countries.

## Features

- Convert `DateTime` between any two timezones
- Convert `DateTimeOffset` preserving the absolute UTC instant
- Get current time or today's date in any timezone
- Detect Daylight Saving Time (DST) at a specific moment
- Calculate UTC offset differences between two zones
- Discover timezones by country code
- Validate any IANA or Windows timezone ID
- Format date-times with timezone offset included
- Check if two local times in different zones are the same instant
- 45+ curated timezone entries (LATAM, Europe, North America, Asia, Oceania, UTC)

## Installation

```bash
dotnet add package Vali-TimeZone
```

## Quick Start

```csharp
using Vali_TimeZone.Core;

var tz = new ValiTimeZone();

// Convert Lima → Madrid
var lima   = new DateTime(2025, 7, 15, 10, 0, 0);
var madrid = tz.Convert(lima, "America/Lima", "Europe/Madrid");
// madrid → 2025-07-15 17:00:00

// Get current time in Tokyo
var tokyoNow = tz.Now("Asia/Tokyo");

// Is Lima currently in DST?
var isDst = tz.IsDst(DateTime.Now, "America/Lima"); // false

// Offset difference Lima ↔ New York
var diff = tz.OffsetDiff("America/Lima", "America/New_York"); // 0 (both UTC-5)

// Format with offset
var formatted = tz.FormatWithZone(lima, "America/Lima");
// "2025-07-15 10:00:00 -05:00"

// Find all timezones for Chile
var zones = tz.ZonesForCountry("CL");
```

## Dependency Injection

```csharp
// Program.cs
builder.Services.AddValiTimeZone();

// Service constructor
public class MyService(IValiTimeZone timeZone)
{
    public DateTime GetLondonTime(DateTime utcTime) =>
        timeZone.FromUtc(utcTime, "Europe/London");
}
```

## Core API

### Conversion

| Method | Description |
|--------|-------------|
| `Convert(dt, fromZone, toZone)` | Convert DateTime between two timezones |
| `ConvertOffset(dto, toZone)` | Convert DateTimeOffset to another timezone |
| `ToUtc(local, fromZone)` | Convert local DateTime to UTC |
| `FromUtc(utc, toZone)` | Convert UTC DateTime to local time |
| `ToDateTimeOffset(dt, zone)` | Wrap DateTime as DateTimeOffset with zone offset |

### Timezone Info

| Method | Description |
|--------|-------------|
| `GetOffset(zone, at?)` | UTC offset at a specific instant (DST-aware) |
| `GetBaseOffset(zone)` | Standard UTC offset (no DST) |
| `IsDst(dt, zone)` | Whether the datetime falls in DST |
| `OffsetDiff(zone1, zone2, at?)` | Difference in hours between two zones |

### Discovery

| Method | Description |
|--------|-------------|
| `FindZone(id)` | Lookup a `ValiZoneInfo` by IANA ID |
| `AllZones()` | All curated timezone entries |
| `ZonesForCountry(code)` | Timezones for a country (ISO 3166-1 alpha-2) |
| `IsValidZone(id)` | Validate IANA or Windows timezone ID |

### Utilities

| Method | Description |
|--------|-------------|
| `Now(zone)` | Current time in a timezone |
| `Today(zone)` | Today's date (midnight) in a timezone |
| `IsSameInstant(a, zA, b, zB)` | Whether two local times are the same UTC instant |
| `FormatWithZone(dt, zone, fmt?)` | Format a DateTime with its timezone offset |

## ValiZoneInfo

Each curated entry exposes:

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `string` | IANA identifier (e.g. `"America/Lima"`) |
| `DisplayName` | `string` | Human-readable name with UTC offset |
| `StandardName` | `string` | Windows/BCL name for `TimeZoneInfo` fallback |
| `BaseOffset` | `TimeSpan` | Standard UTC offset (no DST) |
| `SupportsDst` | `bool` | Whether DST is observed |
| `CountryCode` | `string` | ISO 3166-1 alpha-2 code |
| `CountryName` | `string` | Full English country name |

## Supported Timezone Catalog

### Latin America
`America/Lima` · `America/Bogota` · `America/Guayaquil` · `America/Santiago` · `America/Argentina/Buenos_Aires` · `America/Sao_Paulo` · `America/Mexico_City` · `America/Caracas` · `America/La_Paz` · `America/Asuncion` · `America/Montevideo` · `America/Panama` · `America/Costa_Rica` · `America/Santo_Domingo` · `America/Havana` · `America/Guatemala`

### North America
`America/New_York` · `America/Chicago` · `America/Denver` · `America/Los_Angeles` · `America/Phoenix` · `America/Anchorage` · `America/Honolulu` · `America/Toronto` · `America/Vancouver`

### Europe
`Europe/Madrid` · `Europe/London` · `Europe/Paris` · `Europe/Berlin` · `Europe/Rome` · `Europe/Amsterdam` · `Europe/Lisbon` · `Europe/Brussels` · `Europe/Vienna` · `Europe/Warsaw` · `Europe/Stockholm` · `Europe/Oslo` · `Europe/Copenhagen` · `Europe/Helsinki` · `Europe/Dublin` · `Europe/Zurich`

### Asia & Oceania
`Asia/Tokyo` · `Asia/Shanghai` · `Asia/Kolkata` · `Asia/Singapore` · `Asia/Dubai` · `Asia/Seoul` · `Asia/Bangkok` · `Australia/Sydney`

### UTC
`UTC`

## Part of the Vali-Tempo Ecosystem

| Package | Description |
|---------|-------------|
| [Vali-Time](../Vali-Time/README.md) | Core time conversion and formatting |
| [Vali-Date](../Vali-Time/README.md) | Date arithmetic and calendar operations |
| [Vali-Range](../Vali-Range/README.md) | Date range operations |
| [Vali-Calendar](../Vali-Calendar/README.md) | Workday and week calculations |
| [Vali-Duration](../Vali-Duration/README.md) | High-precision duration struct |
| [Vali-CountDown](../Vali-CountDown/README.md) | Countdown and deadline tracking |
| [Vali-Age](../Vali-Age/README.md) | Age calculation utilities |
| [Vali-Schedule](../Vali-Schedule/README.md) | Recurring event scheduling |
| [Vali-Holiday](../Vali-Holiday/README.md) | Holiday providers for 35+ countries |
| **Vali-TimeZone** | Timezone conversion and discovery |
| [Vali-Tempo](../Vali-Tempo/README.md) | Meta-package: all of the above |

## License

Apache-2.0 © 2025 Felipe Rafael Montenegro Morriberon
