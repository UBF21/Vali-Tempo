# Vali-Tempo

[![NuGet](https://img.shields.io/nuget/v/Vali-Tempo.svg)](https://www.nuget.org/packages/Vali-Tempo)
[![License](https://img.shields.io/badge/license-Apache--2.0-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8%20%7C%209-purple.svg)](https://dotnet.microsoft.com)

**Vali-Tempo** is the meta-package for the Vali-Tempo ecosystem. Installing it gives you every module with a single NuGet reference and a single DI registration call.

## Installation

```bash
dotnet add package Vali-Tempo
```

## What's Included

| Package | Class | Description |
|---------|-------|-------------|
| `Vali-Time` | `ValiTime`, `ValiDate` | Core time conversion, date arithmetic |
| `Vali-Range` | `ValiRange` | Date range creation and manipulation |
| `Vali-Calendar` | `ValiCalendar` | Workday and week calculations |
| `Vali-Duration` | `ValiDuration` | High-precision decimal duration struct |
| `Vali-CountDown` | `ValiCountdown` | Countdown and deadline tracking |
| `Vali-Age` | `ValiAge` | Age calculation from birthdate |
| `Vali-Schedule` | `ValiSchedule` | Recurring event scheduling |
| `Vali-Holiday` | `ValiHoliday` | Holiday providers for 35+ countries |
| `Vali-TimeZone` | `ValiTimeZone` | Timezone conversion and discovery |

## Dependency Injection

```csharp
// Program.cs — register everything in one call
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

## Usage Example

```csharp
public class TimeService(
    IValiTime time,
    IValiDate date,
    IValiAge age,
    IValiTimeZone timeZone,
    IValiHoliday holiday)
{
    public string GetSummary(DateTime birthDate)
    {
        var ageResult = age.Exact(birthDate);
        var nowInLima = timeZone.Now("America/Lima");
        var isHoliday = holiday.IsHoliday(nowInLima, "PE");

        return $"Age: {age.Format(birthDate)} | " +
               $"Lima: {nowInLima:HH:mm} | " +
               $"Holiday: {isHoliday}";
    }
}
```

## Individual Packages

You can install only the packages you need instead of the full meta-package:

```bash
dotnet add package Vali-Time
dotnet add package Vali-Range
dotnet add package Vali-Calendar
dotnet add package Vali-Duration
dotnet add package Vali-CountDown
dotnet add package Vali-Age
dotnet add package Vali-Schedule
dotnet add package Vali-Holiday
dotnet add package Vali-TimeZone
```

## Ecosystem Overview

```
Vali-Tempo Ecosystem
│
├── Vali-Time        — IValiTime, IValiDate
├── Vali-Range       — IValiRange, DateRange
├── Vali-Calendar    — IValiCalendar, CalendarWeek
├── Vali-Duration    — ValiDuration (struct)
├── Vali-CountDown   — IValiCountdown
├── Vali-Age         — IValiAge, AgeResult
├── Vali-Schedule    — IValiSchedule, ScheduleConfig
├── Vali-Holiday     — IValiHoliday, IHolidayProvider (35+ countries)
├── Vali-TimeZone    — IValiTimeZone, ValiZoneInfo
└── Vali-Tempo       — Meta-package (this package)
```

## License

Apache-2.0 © 2025 Felipe Rafael Montenegro Morriberon
