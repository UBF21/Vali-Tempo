# Vali-Schedule
> Fluent recurring schedule builder for .NET — define daily, weekly, monthly, and yearly patterns and query next/previous occurrences.

## Features

- **Fluent builder API** — `Every().On().At().StartingFrom().EndsAfter().EndsOn()`
- **RecurrenceType** — Daily, Weekly, Monthly, Yearly, Custom
- **RecurrenceEnd** — Never, AfterOccurrences, OnDate
- **NextOccurrence** — next occurrence on or after a reference date (5-year lookahead)
- **PreviousOccurrence** — last occurrence strictly before a reference date (5-year lookback)
- **OccursOn** — check whether the schedule fires on a specific date
- **Occurrences** — enumerate up to N upcoming occurrences
- **OccurrencesInRange** — all occurrences within an inclusive date range
- **OnDayOfMonth** — pin monthly schedules to a fixed calendar day (1–31)
- Compiled targets: `net8.0` · `net9.0` — compatible with **.NET 6+** via NuGet backward compatibility

## Installation

```bash
dotnet add package Vali-Schedule
```

> Compatible with .NET 6, 7, 8, and 9. For new projects, .NET 8 (LTS) or .NET 9 is recommended.

## Quick Start

```csharp
using Vali_Schedule.Core;
using Vali_Time.Enums;

// Weekly schedule — every Monday at 08:00, starting 2025-01-01
var schedule = new ValiSchedule()
    .Every(1, TimeUnit.Weeks)
    .On(DayOfWeek.Monday)
    .At(new TimeOnly(8, 0))
    .StartingFrom(new DateTime(2025, 1, 1));

DateTime? next = schedule.NextOccurrence(DateTime.Today);
Console.WriteLine(next); // next Monday at 08:00:00
```

## API Reference

### Fluent Builder

#### `Every(int interval, TimeUnit unit)`

Sets the recurrence interval. Supported units: `Days`, `Weeks`, `Months`, `Years`.

```csharp
new ValiSchedule().Every(1, TimeUnit.Days)   // daily
new ValiSchedule().Every(2, TimeUnit.Weeks)  // biweekly
new ValiSchedule().Every(1, TimeUnit.Months) // monthly
new ValiSchedule().Every(1, TimeUnit.Years)  // yearly
```

#### `On(params DayOfWeek[] days)`

Restricts a weekly schedule to specific days of the week.

```csharp
.On(DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday)
```

#### `At(TimeOnly time)`

Sets the time of day for each occurrence.

```csharp
.At(new TimeOnly(8, 0))    // 08:00
.At(new TimeOnly(23, 30))  // 23:30
```

#### `StartingFrom(DateTime date)`

Defines the inclusive start date. No occurrence is generated before this date.

```csharp
.StartingFrom(new DateTime(2025, 1, 1))
```

#### `EndsAfter(int occurrences)`

Terminates the schedule after a fixed number of occurrences.

```csharp
.EndsAfter(12) // 12 occurrences then done
```

#### `EndsOn(DateTime date)`

Terminates the schedule on a specific date (inclusive).

```csharp
.EndsOn(new DateTime(2025, 12, 31))
```

#### `OnDayOfMonth(int day)`

Pins a monthly schedule to a specific calendar day.

```csharp
.Every(1, TimeUnit.Months).OnDayOfMonth(1)  // always the 1st
.Every(1, TimeUnit.Months).OnDayOfMonth(15) // always the 15th
```

---

### Query API

#### `NextOccurrence(DateTime reference)`

Returns the next occurrence on or after `reference`. Returns `null` if none exists within end conditions or the 5-year window.

```csharp
DateTime? NextOccurrence(DateTime reference);
```

#### `PreviousOccurrence(DateTime reference)`

Returns the most recent occurrence **strictly before** `reference`. Returns `null` if none is found.

```csharp
DateTime? PreviousOccurrence(DateTime reference);
```

#### `OccursOn(DateTime date)`

Returns `true` if the schedule fires on the given date (time-of-day is ignored).

```csharp
bool OccursOn(DateTime date);
```

#### `Occurrences(DateTime reference, int limit = 10)`

Enumerates up to `limit` occurrences starting from `reference`.

```csharp
IEnumerable<DateTime> Occurrences(DateTime reference, int limit = 10);
```

#### `OccurrencesInRange(DateTime from, DateTime to)`

Returns all occurrences within the inclusive date range.

```csharp
IEnumerable<DateTime> OccurrencesInRange(DateTime from, DateTime to);
```

---

### Practical Examples

#### Weekly report — every Monday at 08:00

```csharp
var report = new ValiSchedule()
    .Every(1, TimeUnit.Weeks)
    .On(DayOfWeek.Monday)
    .At(new TimeOnly(8, 0))
    .StartingFrom(new DateTime(2025, 1, 1));

DateTime? next = report.NextOccurrence(DateTime.Today);
Console.WriteLine($"Next report: {next:dddd, MMMM dd 'at' HH:mm}");
// "Next report: Monday, March 24 at 08:00"

// Check whether a specific date triggers the report
bool firesNextMonday = report.OccursOn(new DateTime(2025, 3, 24));
```

---

#### Monthly billing — on the 1st of every month

```csharp
var billing = new ValiSchedule()
    .Every(1, TimeUnit.Months)
    .OnDayOfMonth(1)
    .At(new TimeOnly(0, 0))
    .StartingFrom(new DateTime(2025, 1, 1));

// Next billing date
DateTime? nextBilling = billing.NextOccurrence(DateTime.Today);
Console.WriteLine($"Next charge: {nextBilling:yyyy-MM-dd}"); // "Next charge: 2025-04-01"

// All billing dates in Q2 2025
var q2Dates = billing.OccurrencesInRange(
    new DateTime(2025, 4, 1),
    new DateTime(2025, 6, 30));

foreach (var date in q2Dates)
    Console.WriteLine(date.ToString("yyyy-MM-dd")); // 2025-04-01, 2025-05-01, 2025-06-01
```

---

#### Sync every 6 hours (daily, 4 times a day)

```csharp
// Model 6-hour sync as daily with 4 occurrences per day at fixed times
var times = new[] { 0, 6, 12, 18 }.Select(h => new TimeOnly(h, 0));

foreach (var time in times)
{
    var sync = new ValiSchedule()
        .Every(1, TimeUnit.Days)
        .At(time)
        .StartingFrom(DateTime.Today);

    DateTime? next = sync.NextOccurrence(DateTime.Now);
    if (next.HasValue)
        Console.WriteLine($"Next sync at: {next:HH:mm}");
}
```

---

#### Annual license renewal reminder

```csharp
var licenseStart = new DateTime(2024, 3, 15);

var renewal = new ValiSchedule()
    .Every(1, TimeUnit.Years)
    .At(new TimeOnly(9, 0))
    .StartingFrom(licenseStart);

DateTime? next = renewal.NextOccurrence(DateTime.Today);
Console.WriteLine($"Renew license by: {next:MMMM dd, yyyy}"); // "Renew license by: March 15, 2026"

// List the next 5 renewal dates
var upcoming = renewal.Occurrences(DateTime.Today, limit: 5);
foreach (var d in upcoming)
    Console.WriteLine(d.ToString("yyyy-MM-dd"));
```

---

#### Schedule with a hard end date

```csharp
var campaign = new ValiSchedule()
    .Every(1, TimeUnit.Weeks)
    .On(DayOfWeek.Friday)
    .At(new TimeOnly(18, 0))
    .StartingFrom(new DateTime(2025, 3, 1))
    .EndsOn(new DateTime(2025, 6, 30));

var allFridays = campaign.OccurrencesInRange(
    new DateTime(2025, 3, 1),
    new DateTime(2025, 6, 30));

Console.WriteLine($"Total campaign sends: {allFridays.Count()}"); // 17
```

## Dependency Injection

`ValiSchedule` is a lightweight builder — instantiate it directly wherever a schedule is configured (e.g., a background service or job scheduler). No DI registration is required.

```csharp
// Inside a hosted service or job handler
var schedule = new ValiSchedule()
    .Every(1, TimeUnit.Days)
    .At(new TimeOnly(2, 0))
    .StartingFrom(DateTime.Today);

DateTime? nextRun = schedule.NextOccurrence(DateTime.Now);
```

## License

Licensed under the [MIT License](../LICENSE).
Copyright © 2025 Felipe Rafael Montenegro Morriberon. All rights reserved.

## Donations

If this package is useful to you, consider supporting its development:

- **Latin America** — [MercadoPago](https://link.mercadopago.com.pe/felipermm)
- **International** — [PayPal](https://paypal.me/felipeRMM?country.x=PE&locale.x=es_XC)

---
