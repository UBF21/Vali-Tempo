# Vali-Tempo

[![NuGet](https://img.shields.io/nuget/v/Vali-Tempo.svg)](https://www.nuget.org/packages/Vali-Tempo)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-6%20%7C%207%20%7C%208%20%7C%209-purple.svg)](https://dotnet.microsoft.com)

> A modular .NET ecosystem of utilities for time, dates, timezones, calendars, and more — built for precision and composability.

## Package Index

| Package | Description | Folder |
|---------|-------------|--------|
| [Vali-Time](./Vali-Time) | Time unit conversions, date arithmetic, and quarter utilities | [Vali-Time/](./Vali-Time) |
| [Vali-Range](./Vali-Range) | Date range creation, querying, iteration, and set operations | [Vali-Range/](./Vali-Range) |
| [Vali-Calendar](./Vali-Calendar) | Workday calculations, calendar weeks, and holiday integration | [Vali-Calendar/](./Vali-Calendar) |
| [Vali-Duration](./Vali-Duration) | Immutable duration value type with arithmetic operators | [Vali-Duration/](./Vali-Duration) |
| [Vali-CountDown](./Vali-CountDown) | Countdown timers and deadline tracking | [Vali-CountDown/](./Vali-CountDown) |
| [Vali-Age](./Vali-Age) | Age calculation from birthdates with precision control | [Vali-Age/](./Vali-Age) |
| [Vali-Schedule](./Vali-Schedule) | Recurring schedule definition and occurrence generation | [Vali-Schedule/](./Vali-Schedule) |
| [Vali-Holiday](./Vali-Holiday) | Holiday provider implementations (35+ countries) | [Vali-Holiday/](./Vali-Holiday) |
| [Vali-TimeZone](./Vali-TimeZone) | Timezone conversion and discovery | [Vali-TimeZone/](./Vali-TimeZone) |
| [Vali-BusinessHours](./Vali-BusinessHours) | Cross-timezone business-hours overlap (DST-correct, midnight-crossing aware) | [Vali-BusinessHours/](./Vali-BusinessHours) |
| [Vali-Bucket](./Vali-Bucket) | Fixed-size, anchor-aligned time-series bucketing and aggregation | [Vali-Bucket/](./Vali-Bucket) |
| **Vali-Tempo** | Meta-package — installs the entire Vali-Tempo ecosystem | [Vali-Tempo/](./Vali-Tempo) |

## Installation

```bash
# Meta-package (everything included)
dotnet add package Vali-Tempo

# Or install only what you need:
dotnet add package Vali-Time
dotnet add package Vali-Range
dotnet add package Vali-Calendar
dotnet add package Vali-Duration
dotnet add package Vali-CountDown
dotnet add package Vali-Age
dotnet add package Vali-Schedule
dotnet add package Vali-Holiday
dotnet add package Vali-TimeZone
dotnet add package Vali-BusinessHours
dotnet add package Vali-Bucket
```

## Dependency Injection

Register all packages with a single call using the meta-package:

```csharp
// Program.cs
builder.Services.AddValiTempo();
```

This registers all services with singleton lifetime:
- `IValiTime` → `ValiTime`
- `IValiDate` → `ValiDate`
- `IValiRange` → `ValiRange`
- `IValiCalendar` → `ValiCalendar`
- `IValiCountdown` → `ValiCountdown`
- `IValiAge` → `ValiAge`
- `IValiSchedule` → `ValiSchedule`
- `IValiHoliday` → `ValiHoliday`
- `IValiTimeZone` → `ValiTimeZone`
- `IValiBusinessHours` → `ValiBusinessHours`
- `IValiBucket` → `ValiBucket`

## Quick Example — Using Multiple Modules Together

```csharp
using Vali_Time.Core;
using Vali_Time.Enums;
using Vali_Range.Core;
using Vali_Range.Models;
using Vali_Calendar.Core;
using Vali_Duration.Models;
using Vali_Holiday.Core;
using Vali_TimeZone.Core;

// --- Vali-Time: convert and format a duration ---
var valiTime = new ValiTime();
decimal hours = valiTime.Convert(7200m, TimeUnit.Seconds, TimeUnit.Hours);
string label  = valiTime.FormatTime(hours, TimeUnit.Hours); // "2.00 h"

// --- Vali-Duration: measure elapsed time from a Stopwatch ---
var stopwatch = System.Diagnostics.Stopwatch.StartNew();
// ... work ...
stopwatch.Stop();
ValiDuration elapsed = ValiDuration.FromTimeSpan(stopwatch.Elapsed);
Console.WriteLine(elapsed.Format()); // e.g. "342.00 ms"

// --- Vali-Range: define a reporting window ---
var range = new ValiRange();
DateRange thisQuarter = range.ThisQuarter();
Console.WriteLine(thisQuarter); // "2025-01-01 → 2025-03-31"

// --- Vali-Calendar: find the delivery date in 5 workdays ---
var calendar = new ValiCalendar();
DateTime orderDate    = new DateTime(2025, 3, 20);
DateTime deliveryDate = calendar.AddWorkdays(orderDate, 5);
Console.WriteLine(deliveryDate.ToString("yyyy-MM-dd")); // "2025-03-27"

// --- Vali-TimeZone: get current time in a specific timezone ---
var timeZone = new ValiTimeZone();
DateTime nowInLima = timeZone.Now("America/Lima");
Console.WriteLine(timeZone.FormatWithZone(nowInLima, "America/Lima"));

// --- Vali-Holiday: check if today is a holiday in Peru ---
var holiday = new ValiHoliday();
bool isHoliday = holiday.IsHoliday(nowInLima, "PE");
Console.WriteLine($"Holiday in Peru: {isHoliday}");

// --- Combine: count workdays within the quarter range ---
int workdaysInQuarter = calendar.WorkdaysBetween(thisQuarter.Start, thisQuarter.End);
Console.WriteLine($"Workdays this quarter: {workdaysInQuarter}");
```

## Ecosystem Architecture

```
Vali-Tempo (meta-package)
│
├── Vali-Time        — IValiTime, IValiDate          (foundation)
├── Vali-Range       — IValiRange, DateRange
├── Vali-Calendar    — IValiCalendar, CalendarWeek
├── Vali-Duration    — ValiDuration (struct)
├── Vali-CountDown   — IValiCountdown
├── Vali-Age         — IValiAge, AgeResult
├── Vali-Schedule    — IValiSchedule, ScheduleConfig
├── Vali-Holiday     — IValiHoliday, IHolidayProvider (35+ countries)
├── Vali-TimeZone    — IValiTimeZone, ValiZoneInfo
├── Vali-BusinessHours — IValiBusinessHours (composes Vali-Calendar + Vali-TimeZone)
└── Vali-Bucket      — IValiBucket, TimeBucket
```

- **Vali-Time** is the foundation: `TimeUnit` enum, `ValiTime` (unit conversions), `ValiDate` (date arithmetic).
- **Vali-Range** builds on `TimeUnit` to offer `DateRange` and `ValiRange` with set-theory operations.
- **Vali-Calendar** uses `ValiDate` conventions and accepts an `IHolidayProvider` (from **Vali-Holiday**) to make workday logic country-aware.
- **Vali-Duration** is a standalone value type that uses `TimeUnit` for its `As()` conversion method.
- **Vali-TimeZone** provides IANA-compatible timezone resolution, conversion, and formatting.
- **Vali-Tempo** is a convenience meta-package that pulls in all of the above.

## Compatibility

All packages in the Vali-Tempo ecosystem are compiled against **net8.0** and **net9.0**.
Thanks to NuGet's built-in backward compatibility resolution, they are also fully usable
from **any project targeting .NET 6 or .NET 7** — NuGet automatically selects the closest
available target framework without requiring a separate build.

| Package | .NET 6 | .NET 7 | .NET 8 | .NET 9 |
|---------|:------:|:------:|:------:|:------:|
| Vali-Time | ✅ | ✅ | ✅ | ✅ |
| Vali-Range | ✅ | ✅ | ✅ | ✅ |
| Vali-Calendar | ✅ | ✅ | ✅ | ✅ |
| Vali-Duration | ✅ | ✅ | ✅ | ✅ |
| Vali-CountDown | ✅ | ✅ | ✅ | ✅ |
| Vali-Age | ✅ | ✅ | ✅ | ✅ |
| Vali-Schedule | ✅ | ✅ | ✅ | ✅ |
| Vali-Holiday | ✅ | ✅ | ✅ | ✅ |
| Vali-TimeZone | ✅ | ✅ | ✅ | ✅ |
| Vali-BusinessHours | ✅ | ✅ | ✅ | ✅ |
| Vali-Bucket | ✅ | ✅ | ✅ | ✅ |
| Vali-Tempo | ✅ | ✅ | ✅ | ✅ |

> **Note:** .NET 6 and .NET 7 reached end-of-life in November 2024 and May 2024 respectively.
> While these runtimes are no longer receiving security updates from Microsoft, the packages
> remain compatible for teams that have not yet migrated. For new projects, .NET 8 (LTS) or
> .NET 9 is strongly recommended.

### .NET Version Reference

| Version | Type | Support ends |
|---------|------|-------------|
| .NET 6 | LTS | November 2024 (EOL) |
| .NET 7 | STS | May 2024 (EOL) |
| .NET 8 | LTS | November 2026 ✅ |
| .NET 9 | STS | May 2026 ✅ |

## Individual Package READMEs

- [Vali-Time README](./Vali-Time/README.md)
- [Vali-Range README](./Vali-Range/README.md)
- [Vali-Calendar README](./Vali-Calendar/README.md)
- [Vali-Duration README](./Vali-Duration/README.md)
- [Vali-CountDown README](./Vali-CountDown/README.md)
- [Vali-Age README](./Vali-Age/README.md)
- [Vali-Schedule README](./Vali-Schedule/README.md)
- [Vali-Holiday README](./Vali-Holiday/README.md)
- [Vali-TimeZone README](./Vali-TimeZone/README.md)
- [Vali-BusinessHours README](./Vali-BusinessHours/README.md)
- [Vali-Bucket README](./Vali-Bucket/README.md)
- [Vali-Tempo (meta-package) README](./Vali-Tempo/README.md)

## Estándares de Desarrollo

> Generado por `/flows/onboard` — 2026-10-07. Fuente completa en [CONTRIBUTING.md](CONTRIBUTING.md); valores de tamaño/complejidad no cubiertos ahí están en `.claude/memory/project-standards.md` (local, no versionado).

- `decimal` siempre para tiempo/duración — nunca `double`/`float`
- `readonly struct` para value types (`DateRange`, `ValiDuration`, `AgeResult`, `CalendarWeek`)
- Cada servicio implementa `IValiX` + expone `AddValiX()`
- Cero dependencias externas por paquete salvo `Microsoft.Extensions.DependencyInjection.Abstractions`
- XML `<summary>` obligatorio en miembros públicos
- Tests con `DateTime` fijo (nunca `.Now`), cubriendo Feb 29/bisiestos/DST/negativos
- Commits: Conventional Commits con scope de paquete (`fix(Vali-Age): ...`)

## License

MIT © 2025 Felipe Rafael Montenegro Morriberon

## Donations

If Vali-Tempo is useful to you, consider supporting its development:

- **Latin America** — [MercadoPago](https://link.mercadopago.com.pe/felipermm)
- **International** — [PayPal](https://paypal.me/felipeRMM?country.x=PE&locale.x=es_XC)

---
