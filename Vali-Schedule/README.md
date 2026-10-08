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

#### `SkipWeekends()`

Excludes Saturday/Sunday. An occurrence that lands on a weekend is moved forward to the
next non-excluded day.

```csharp
var schedule = new ValiSchedule()
    .Every(1, TimeUnit.Days)
    .StartingFrom(new DateTime(2025, 1, 1))
    .SkipWeekends();
```

#### `SkipHolidays(IHolidayProvider provider, HolidaySkipStrategy strategy = HolidaySkipStrategy.NextWorkday)`

Excludes dates that are public holidays according to a `Vali-Holiday` provider. Requires
adding `Vali-Holiday` as a dependency (installing `Vali-Schedule` alone is enough for
`SkipWeekends()`, but `SkipHolidays()` needs a concrete `IHolidayProvider`, e.g.
`HolidayProviderFactory.CreateAll().For("PE")`).

```csharp
using Vali_Holiday.Core;
using Vali_Schedule.Enums;

IHolidayProvider peru = HolidayProviderFactory.CreateAll().For("PE");

var schedule = new ValiSchedule()
    .Every(1, TimeUnit.Days)
    .StartingFrom(new DateTime(2025, 1, 1))
    .SkipWeekends()
    .SkipHolidays(peru, HolidaySkipStrategy.NextWorkday);
```

`HolidaySkipStrategy` controls how a collision is resolved:

| Value | Behavior |
|---|---|
| `NextWorkday` (default) | Moves forward until a non-excluded date is found. |
| `PreviousWorkday` | Moves backward; drops the occurrence if it crosses `StartingFrom`. |
| `Skip` | Omits the occurrence with no replacement. Still consumes one `EndsAfter` slot. |

`SkipWeekends()` and `SkipHolidays()` combine into a single unified exclusion set — a
Friday holiday followed by a weekend resolves to the next real business day in one pass,
not two independent shifts.

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

#### `OccurrencePeriods(DateTime from, DateTime to)`

Pairs each occurrence with the schedule's next real occurrence, returning closed
`Vali_Range.Models.DateRange` periods instead of single instants — useful for billing
cycles, maintenance windows, or any "time between occurrences" use case. Requires
`Vali-Range` (added automatically as a dependency).

```csharp
using Vali_Range.Models;

var schedule = new ValiSchedule()
    .Every(1, TimeUnit.Months).OnDayOfMonth(1)
    .StartingFrom(new DateTime(2026, 1, 1));

IEnumerable<DateRange> periods = schedule.OccurrencePeriods(
    new DateTime(2026, 1, 1), new DateTime(2026, 3, 15));
// [2026-01-01 → 2026-01-31 23:59:59.9999999]
// [2026-02-01 → 2026-02-28 23:59:59.9999999]
// [2026-03-01 → 2026-03-31 23:59:59.9999999]  ← closes with the real next occurrence,
//                                                even though it falls beyond `to`.
```

A period is never truncated at `to`, and the last occurrence in range is excluded (no
period emitted) if the schedule has no further real occurrence — the method never
synthesizes `End = to`.

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

## Cron expressions

`CronSchedule` parses standard 5-field cron syntax (minute, hour, day-of-month, month,
day-of-week) into an `IValiSchedule` — no external cron library, no new dependency.

| Syntax | Meaning | Example |
|---|---|---|
| `*` | every value | `* * * * *` |
| `N` | exact value | `0 9 * * *` |
| `N,M,...` | list | `0,15,30,45 * * * *` |
| `N-M` | inclusive range | `9-17 * * * *` |
| `*/S` | step from the field minimum | `*/5 * * * *` |
| `N-M/S` | step within a range | `9-17/2 * * * *` |
| `N/S` | step from `N` through the field maximum | `10/15 * * * *` |
| `JAN`–`DEC`, `SUN`–`SAT` | 3-letter name aliases, case-insensitive | `MON-FRI`, `JAN,JUL` |

```csharp
using Vali_Schedule.Cron;

var businessOpen = CronSchedule.Parse("0 9 * * MON-FRI");     // every weekday at 9:00 AM
var healthCheck  = CronSchedule.Parse("*/15 9-17 * * MON-FRI"); // every 15 min, business hours
var quarterly    = CronSchedule.Parse("0 0 1 1,4,7,10 *");      // midnight, 1st of each quarter

DateTime? next = businessOpen.NextOccurrence(DateTime.Today);
IEnumerable<DateTime> nextTen = healthCheck.Occurrences(DateTime.Now, limit: 10);

// Safe parsing at the edge (e.g. a cron string from user-supplied config):
if (CronSchedule.TryParse(userSuppliedExpression, out var schedule, out var error))
{
    // use `schedule`
}
```

Compared to `NCrontab`/`Cronos`: same 5-field syntax, zero added dependency (ships inside
`Vali-Schedule`), and `CronSchedule` implements the same `IValiSchedule` query surface used by
every other schedule type in the ecosystem — including `WithCustomPredicate` to layer a
holiday-skip filter on top of a cron expression.

**Caveats, documented rather than silently papered over:**
- **DST / wall-clock**: `CronSchedule` evaluates plain `DateTime` wall-clock values with no
  timezone awareness. A cron hour that doesn't exist (or is ambiguous) on a DST transition day is
  not specially resolved — convert to UTC first if that matters for your use case.
- **Feb 29**: `0 0 29 2 *` only matches in leap years; `NextOccurrence` correctly skips to the next
  one. `30 2 30 2 *` never matches (Feb never has 30 days) and returns `null`/empty rather than
  looping.
- **`SkipWeekends`/`SkipHolidays`**: unlike `ValiSchedule`, these *drop* excluded occurrences
  instead of shifting them to the next/previous workday. A cron schedule can fire more than once a
  day, so "shift to the next workday" is not well-defined — it would relocate every occurrence of
  the excluded day onto the next day's own occurrences at the same times.
- **Not supported**: `Every`, `On`, `At`, `OnDayOfMonth` all throw `NotSupportedException` on
  `CronSchedule` — express those concerns directly in the cron string instead.

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
