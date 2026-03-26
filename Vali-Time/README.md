# Vali-Time

[![NuGet](https://img.shields.io/nuget/v/Vali-Time.svg)](https://www.nuget.org/packages/Vali-Time)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-6%20%7C%207%20%7C%208%20%7C%209-purple.svg)](https://dotnet.microsoft.com)

> Precise time unit conversions, date arithmetic, period boundaries, and a full Quarter API for .NET.

## Features

- Convert between all eight time units (Milliseconds, Seconds, Minutes, Hours, Days, Weeks, Months, Years) with full `decimal` precision
- Sum or subtract multiple time values expressed in different units in a single call
- Parse human-readable time strings (`"2h 30m"`, `"1:30:00"`, `"500ms"`) into seconds
- Format any value with a unit-aware suffix (`"1.50 h"`, `"45.00 min"`, `"500.00 ms"`)
- Auto-detect the most appropriate display unit with `GetBestUnit`
- Decompose a duration into hours, minutes, seconds, and milliseconds with `Breakdown`
- Clamp a time value within a min/max range
- Compare two values expressed in different units
- Convert to/from `System.TimeSpan` for .NET interoperability
- Date arithmetic: `Add`, `Subtract`, `Diff` across all time units
- Period boundaries: `StartOf` / `EndOf` for Day, Week, Month, Quarter, Year
- Date validations: `IsBefore`, `IsAfter`, `IsSameDay`, `IsSamePeriod`, `IsWeekend`, `IsWeekday`, `IsLeapYear`
- Complete Quarter API: `QuarterOf`, `QuarterStart`, `QuarterEnd`, `QuarterName`, `DaysInQuarter`, `ProgressInQuarter`, and more

## Installation

```bash
dotnet add package Vali-Time
```

Compiled targets: `net8.0` · `net9.0`. Compatible with **.NET 6 and .NET 7** via NuGet backward compatibility — no separate build needed.

## Quick Start

```csharp
using Vali_Time.Core;
using Vali_Time.Enums;

var valiTime = new ValiTime();
var valiDate = new ValiDate();

// Convert 1.5 hours to minutes
decimal minutes = valiTime.Convert(1.5m, TimeUnit.Hours, TimeUnit.Minutes);
Console.WriteLine(minutes); // 90

// Add 2 weeks to a date
DateTime result = valiDate.Add(new DateTime(2025, 3, 1), 2m, TimeUnit.Weeks);
Console.WriteLine(result.ToString("yyyy-MM-dd")); // 2025-03-15
```

## API Reference

### TimeUnit Enum

```csharp
TimeUnit.Milliseconds   // 1/1000 of a second
TimeUnit.Seconds        // base unit
TimeUnit.Minutes        // 60 seconds
TimeUnit.Hours          // 3 600 seconds
TimeUnit.Days           // 86 400 seconds
TimeUnit.Weeks          // 7 days
TimeUnit.Months         // 30.4375 days average (365.25 / 12)
TimeUnit.Years          // 365.25 days average
```

---

### ValiTime

#### Convert

Converts a value from one unit to another with optional rounding.

```csharp
var valiTime = new ValiTime();

// Hours → Seconds
decimal seconds = valiTime.Convert(2m, TimeUnit.Hours, TimeUnit.Seconds);
Console.WriteLine(seconds); // 7200

// Days → Hours, rounded to 2 decimal places
decimal hours = valiTime.Convert(1.5m, TimeUnit.Days, TimeUnit.Hours, decimalPlaces: 2);
Console.WriteLine(hours); // 36.00

// Weeks → Days
decimal days = valiTime.Convert(2m, TimeUnit.Weeks, TimeUnit.Days);
Console.WriteLine(days); // 14

// Months → Hours (approximate, uses 30.4375 d/mo)
decimal monthHours = valiTime.Convert(1m, TimeUnit.Months, TimeUnit.Hours, decimalPlaces: 2);
Console.WriteLine(monthHours); // 730.50
```

#### TryConvert

Safe conversion that returns `false` instead of throwing on invalid input.

```csharp
if (valiTime.TryConvert(90m, TimeUnit.Minutes, TimeUnit.Hours, out decimal h))
    Console.WriteLine(h); // 1.5
```

#### MultiConvert

Convert one value to several units in a single call.

```csharp
var map = valiTime.MultiConvert(3600m, TimeUnit.Seconds,
    TimeUnit.Minutes, TimeUnit.Hours, TimeUnit.Days);

Console.WriteLine(map[TimeUnit.Minutes]); // 60
Console.WriteLine(map[TimeUnit.Hours]);   // 1
Console.WriteLine(map[TimeUnit.Days]);    // 0.041666...
```

#### SumTimes

Add multiple values in different units; get the result in any unit.

```csharp
var times = new List<(decimal, TimeUnit)>
{
    (1m,  TimeUnit.Hours),
    (30m, TimeUnit.Minutes),
    (45m, TimeUnit.Seconds)
};

decimal totalSeconds = valiTime.SumTimes(TimeUnit.Seconds, times);
Console.WriteLine(totalSeconds); // 5445

decimal totalHours = valiTime.SumTimes(TimeUnit.Hours, times, decimalPlaces: 4);
Console.WriteLine(totalHours); // 1.5125
```

#### SubtractTimes

Subtract from the first value all subsequent values. The first element is the minuend.

```csharp
var times = new List<(decimal, TimeUnit)>
{
    (2m,  TimeUnit.Hours),
    (30m, TimeUnit.Minutes),
    (600m, TimeUnit.Seconds)
};

decimal result = valiTime.SubtractTimes(TimeUnit.Minutes, times);
Console.WriteLine(result); // 80  (120 min - 30 min - 10 min)

// Allow negative result
decimal negative = valiTime.SubtractTimes(TimeUnit.Hours, times, allowNegative: true);
```

#### FormatTime

Format a value with a unit-aware suffix. Supports `CultureInfo`.

```csharp
Console.WriteLine(valiTime.FormatTime(1.5m,    TimeUnit.Hours));        // "1.50 h"
Console.WriteLine(valiTime.FormatTime(90m,     TimeUnit.Minutes));      // "90.00 min"
Console.WriteLine(valiTime.FormatTime(500m,    TimeUnit.Milliseconds)); // "500.00 ms"
Console.WriteLine(valiTime.FormatTime(2.5m,    TimeUnit.Days));         // "2.50 d"
Console.WriteLine(valiTime.FormatTime(3m,      TimeUnit.Weeks));        // "3.00 w"
Console.WriteLine(valiTime.FormatTime(1m,      TimeUnit.Months));       // "1.00 mo"
Console.WriteLine(valiTime.FormatTime(1m,      TimeUnit.Years));        // "1.00 yr"

// German culture (comma as decimal separator)
var de = new System.Globalization.CultureInfo("de-DE");
Console.WriteLine(valiTime.FormatTime(1234.5m, TimeUnit.Seconds, 2, de)); // "1234,50 s"
```

#### GetBestUnit

Returns the most readable unit for a value expressed in seconds.

```csharp
var (time, unit) = valiTime.GetBestUnit(7200m);
Console.WriteLine(valiTime.FormatTime(time, unit)); // "2.00 h"

var (time2, unit2) = valiTime.GetBestUnit(0.5m);
Console.WriteLine(valiTime.FormatTime(time2, unit2)); // "500.00 ms"

var (time3, unit3) = valiTime.GetBestUnit(90000m);
Console.WriteLine(valiTime.FormatTime(time3, unit3)); // "1.50 w"
```

#### ToTimeSpan / FromTimeSpan

Interoperability with `System.TimeSpan`.

```csharp
TimeSpan span = valiTime.ToTimeSpan(90m, TimeUnit.Minutes);
Console.WriteLine(span); // 01:30:00

decimal hours = valiTime.FromTimeSpan(span, TimeUnit.Hours);
Console.WriteLine(hours); // 1.5
```

#### Breakdown

Decompose a total number of seconds into hours, minutes, seconds, and milliseconds.

```csharp
var parts = valiTime.Breakdown(3665.678m);

Console.WriteLine(parts[TimeUnit.Hours]);        // 1
Console.WriteLine(parts[TimeUnit.Minutes]);      // 1
Console.WriteLine(parts[TimeUnit.Seconds]);      // 5
Console.WriteLine(parts[TimeUnit.Milliseconds]); // 678
// Prints: 1h 1m 5s 678ms
```

#### ParseTime / TryParseTime

Parse human-readable strings into total seconds.

```csharp
// Colon-separated
decimal s1 = valiTime.ParseTime("1:30:00"); // 5400
decimal s2 = valiTime.ParseTime("45:30");   // 2730

// Labelled tokens (combinable)
decimal s3 = valiTime.ParseTime("2d 4h 30m 15s"); // 188415
decimal s4 = valiTime.ParseTime("500ms");           // 0.5
decimal s5 = valiTime.ParseTime("1h 30m");          // 5400

// Safe parsing
if (valiTime.TryParseTime("bad input", out decimal parsed))
    Console.WriteLine(parsed);
else
    Console.WriteLine("Could not parse"); // this branch runs
```

#### Clamp

Constrain a value between a minimum and maximum in the same unit.

```csharp
// Clamp 300 minutes between 1 hour and 4 hours → returns 4 hours (in minutes)
decimal clamped = valiTime.Clamp(300m, TimeUnit.Minutes, min: 60m, max: 240m);
Console.WriteLine(clamped); // 240

decimal inRange = valiTime.Clamp(120m, TimeUnit.Minutes, min: 60m, max: 240m);
Console.WriteLine(inRange); // 120
```

#### Compare

Compare two values that may be in different units.

```csharp
int cmp = valiTime.Compare(1m, TimeUnit.Hours, 60m, TimeUnit.Minutes);
Console.WriteLine(cmp); // 0  (equal)

int cmp2 = valiTime.Compare(2m, TimeUnit.Hours, 90m, TimeUnit.Minutes);
Console.WriteLine(cmp2); // 1  (2 h > 90 min)
```

---

### ValiDate

`ValiDate` accepts an optional `WeekStart` to control Monday vs Sunday-based week logic.

```csharp
var valiDate = new ValiDate();                         // Monday as week start (default)
var valiDateSun = new ValiDate(WeekStart.Sunday);      // Sunday as week start
```

#### Diff

Calculate the difference between two dates in any time unit.

```csharp
var from = new DateTime(2025, 1, 1);
var to   = new DateTime(2025, 4, 1);

decimal days   = valiDate.Diff(from, to, TimeUnit.Days);    // 90
decimal weeks  = valiDate.Diff(from, to, TimeUnit.Weeks, decimalPlaces: 2);  // 12.86
decimal months = valiDate.Diff(from, to, TimeUnit.Months, decimalPlaces: 2); // 3.00
decimal hours  = valiDate.Diff(from, to, TimeUnit.Hours);  // 2160

// Negative when 'to' is before 'from'
decimal negative = valiDate.Diff(to, from, TimeUnit.Days); // -90
```

#### Add / Subtract

Add or subtract any time unit from a date.

```csharp
var date = new DateTime(2025, 3, 15);

DateTime plusMonth  = valiDate.Add(date, 1m,  TimeUnit.Months);      // 2025-04-15
DateTime plusWeeks  = valiDate.Add(date, 2m,  TimeUnit.Weeks);       // 2025-03-29
DateTime plusHours  = valiDate.Add(date, 8m,  TimeUnit.Hours);       // 2025-03-15 08:00:00
DateTime minusYear  = valiDate.Subtract(date, 1m, TimeUnit.Years);   // 2024-03-15
DateTime minusDays  = valiDate.Subtract(date, 5m, TimeUnit.Days);    // 2025-03-10
```

#### StartOf / EndOf

Get the first or last instant of any period granularity.

```csharp
var date = new DateTime(2025, 5, 20, 14, 30, 0);

DateTime startDay     = valiDate.StartOf(date, DatePart.Day);     // 2025-05-20 00:00:00
DateTime endDay       = valiDate.EndOf(date, DatePart.Day);       // 2025-05-20 23:59:59.999
DateTime startWeek    = valiDate.StartOf(date, DatePart.Week);    // 2025-05-19 (Monday)
DateTime endWeek      = valiDate.EndOf(date, DatePart.Week);      // 2025-05-25 23:59:59.999
DateTime startMonth   = valiDate.StartOf(date, DatePart.Month);   // 2025-05-01 00:00:00
DateTime endMonth     = valiDate.EndOf(date, DatePart.Month);     // 2025-05-31 23:59:59.999
DateTime startQuarter = valiDate.StartOf(date, DatePart.Quarter); // 2025-04-01 00:00:00
DateTime endQuarter   = valiDate.EndOf(date, DatePart.Quarter);   // 2025-06-30 23:59:59.999
DateTime startYear    = valiDate.StartOf(date, DatePart.Year);    // 2025-01-01 00:00:00
DateTime endYear      = valiDate.EndOf(date, DatePart.Year);      // 2025-12-31 23:59:59.999
```

#### Validations

```csharp
var monday   = new DateTime(2025, 5, 19);
var saturday = new DateTime(2025, 5, 17);

valiDate.IsWeekend(saturday);  // true
valiDate.IsWeekday(monday);    // true
valiDate.IsBefore(monday, saturday);  // false
valiDate.IsAfter(monday, saturday);   // true
valiDate.IsSameDay(monday, monday);   // true
valiDate.IsLeapYear(2024);     // true
valiDate.DaysInMonth(2025, 2); // 28
valiDate.WeekOfYear(monday);   // 21 (ISO 8601)
valiDate.DayOfYear(monday);    // 139
```

#### IsSamePeriod

```csharp
var a = new DateTime(2025, 5, 1);
var b = new DateTime(2025, 5, 31);
var c = new DateTime(2025, 7, 15);

valiDate.IsSamePeriod(a, b, DatePart.Month);   // true
valiDate.IsSamePeriod(a, c, DatePart.Quarter); // false  (Q2 vs Q3)
valiDate.IsSamePeriod(a, c, DatePart.Year);    // true
```

#### Quarter API

```csharp
var date = new DateTime(2025, 5, 20);

int quarter    = valiDate.QuarterOf(date);             // 2
string name    = valiDate.QuarterName(date);           // "Q2 2025"
string full    = valiDate.QuarterNameFull(date);       // "Quarter 2 - 2025"
DateTime start = valiDate.QuarterStart(date);          // 2025-04-01 00:00:00
DateTime end   = valiDate.QuarterEnd(date);            // 2025-06-30 23:59:59.999
int totalDays  = valiDate.DaysInQuarter(date);         // 91
int elapsed    = valiDate.DaysElapsedInQuarter(date);  // 50
int remaining  = valiDate.DaysRemainingInQuarter(date);// 42
decimal prog   = valiDate.ProgressInQuarter(date);     // ~0.5494... (54.9%)
int weeks      = valiDate.WeeksInQuarter(date);        // 13
DateTime nextQ = valiDate.NextQuarterStart(date);      // 2025-07-01
DateTime prevQ = valiDate.PreviousQuarterStart(date);  // 2025-01-01
bool isFirst   = valiDate.IsFirstDayOfQuarter(new DateTime(2025, 4, 1));  // true
bool isLast    = valiDate.IsLastDayOfQuarter(new DateTime(2025, 6, 30));  // true
bool sameQ     = valiDate.IsInSameQuarter(new DateTime(2025, 4, 1), new DateTime(2025, 6, 30)); // true

Console.WriteLine($"{name}: {prog:P1} complete, {remaining} days remaining");
// "Q2 2025: 54.9% complete, 42 days remaining"
```

#### WeekYear

Returns the ISO 8601 week-numbering year for the date. Near year boundaries, this can differ from `date.Year`.

```csharp
var dec30 = new DateTime(2024, 12, 30); // ISO week 1 of 2025

int weekYear   = valiDate.WeekYear(dec30);   // 2025
int weekNumber = valiDate.WeekOfYear(dec30); // 1

// With Sunday-start the calendar year is always returned
var valiDateSun = new ValiDate(WeekStart.Sunday);
int yearSun = valiDateSun.WeekYear(dec30); // 2024
```

#### DayOfQuarter

Returns the 1-based day number within the current quarter.

```csharp
var date = new DateTime(2025, 5, 20);

int day = valiDate.DayOfQuarter(date); // 50 (50th day of Q2)
```

#### TryNextQuarterStart

Safely returns the first day of the next quarter. Returns `false` for Q4 of year 9999.

```csharp
var date = new DateTime(2025, 5, 20);
if (valiDate.TryNextQuarterStart(date, out var nextQ))
    Console.WriteLine(nextQ); // 2025-07-01

var endOfTime = new DateTime(9999, 10, 1);
bool hasNext = valiDate.TryNextQuarterStart(endOfTime, out _); // false
```

#### TryPreviousQuarterStart

Safely returns the first day of the previous quarter. Returns `false` when the previous quarter would underflow below `DateTime.MinValue` (i.e., when the date is in Q1 of year 1).

```csharp
var valiDate = new ValiDate();
if (valiDate.TryPreviousQuarterStart(new DateTime(2025, 5, 1), out var prev))
    Console.WriteLine(prev); // 2025-01-01

var earliest = new DateTime(1, 1, 15);
bool hasPrev = valiDate.TryPreviousQuarterStart(earliest, out _); // false
```

#### ProgressInYear

Returns the fraction of the year elapsed (0.0 = Jan 1, approaches 1.0 on Dec 31).

```csharp
decimal p1 = valiDate.ProgressInYear(new DateTime(2025, 1, 1));   // 0.0
decimal p2 = valiDate.ProgressInYear(new DateTime(2025, 7, 1));   // ~0.4959
decimal p3 = valiDate.ProgressInYear(new DateTime(2025, 12, 31)); // ~0.9973
```

#### ProgressInMonth

Returns the fraction of the month elapsed (0.0 = 1st, approaches 1.0 on the last day).

```csharp
decimal p1 = valiDate.ProgressInMonth(new DateTime(2025, 5, 1));  // 0.0
decimal p2 = valiDate.ProgressInMonth(new DateTime(2025, 5, 15)); // ~0.4516
decimal p3 = valiDate.ProgressInMonth(new DateTime(2025, 5, 31)); // ~0.9677
```

## Dependency Injection

```csharp
using Vali_Time.Core;

// Register with default week start (Monday)
services.AddSingleton<IValiTime, ValiTime>();
services.AddSingleton<IValiDate, ValiDate>();

// Or with explicit week start
services.AddSingleton<IValiDate>(_ => new ValiDate(WeekStart.Sunday));
```

## License

MIT

## Donations

If this package is useful to you, consider supporting its development:

- **Latin America** — [MercadoPago](https://link.mercadopago.com.pe/felipermm)
- **International** — [PayPal](https://paypal.me/felipeRMM?country.x=PE&locale.x=es_XC)

---
