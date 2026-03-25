# Vali-CountDown
> Deadline tracking and countdown calculations for .NET — expiry detection, progress, breakdowns, and human-readable formatting.

## Features

- **IsExpired** — check whether a deadline has already passed (vs. `DateTime.Now` or a custom reference)
- **TimeUntil** — remaining time until a deadline in any `TimeUnit` (milliseconds through years)
- **TimeElapsed** — time elapsed since a given point, in any `TimeUnit`
- **Progress** — fractional progress (0.0–1.0) between a start and end date
- **ProgressPercent** — same as `Progress` but scaled to 0–100
- **Breakdown** — remaining time split into hours, minutes, seconds, and milliseconds as a `Dictionary<TimeUnit, decimal>`
- **Format** — human-readable string such as `"5d 3h 20m"` or `"Expired"`
- **IsWithin** — returns `true` if the deadline is within a given threshold (e.g., within 1 hour)
- **IsStarted** — returns `true` if a given point in time is in the past or present (elapsed time > 0)
- Compiled targets: `net8.0` · `net9.0` — compatible with **.NET 6+** via NuGet backward compatibility
- Registered as a **singleton** via `AddValiCountdown()`

## Installation

```bash
dotnet add package Vali-CountDown
```

> Compatible with .NET 6, 7, 8, and 9. For new projects, .NET 8 (LTS) or .NET 9 is recommended.

## Quick Start

```csharp
using Vali_CountDown.Core;
using Vali_Time.Enums;

var countdown = new ValiCountdown();

var deadline = new DateTime(2025, 12, 31, 23, 59, 59);

// Is the deadline still in the future?
bool expired = countdown.IsExpired(deadline);                    // false (while in 2025)

// How many days remain?
decimal days = countdown.TimeUntil(deadline, TimeUnit.Days, 1); // e.g. 45.3

// Human-readable label
string label = countdown.Format(deadline);                       // "45d 7h 12m"
```

## API Reference

### `IsExpired`

```csharp
// Overload 1: compare against DateTime.Now
bool IsExpired(DateTime deadline);

// Overload 2: compare against a custom reference time
bool IsExpired(DateTime deadline, DateTime reference);
```

```csharp
var sla = new DateTime(2025, 6, 15, 9, 0, 0);

bool overdueNow      = countdown.IsExpired(sla);
bool overdueYesterday = countdown.IsExpired(sla, new DateTime(2025, 6, 16));  // true
```

---

### `TimeUntil`

Returns the amount of time **remaining** until the deadline in the specified unit. Returns `0` if expired.

```csharp
decimal TimeUntil(DateTime deadline, TimeUnit unit, int? decimalPlaces = null);
```

```csharp
var deadline = DateTime.Now.AddHours(5).AddMinutes(30);

decimal hours   = countdown.TimeUntil(deadline, TimeUnit.Hours,   1); // 5.5
decimal minutes = countdown.TimeUntil(deadline, TimeUnit.Minutes, 0); // 330
decimal days    = countdown.TimeUntil(deadline, TimeUnit.Days,    2); // 0.23
```

---

### `TimeElapsed`

Returns the amount of time that has **elapsed** since the given point.

```csharp
decimal TimeElapsed(DateTime from, TimeUnit unit, int? decimalPlaces = null);
```

```csharp
var sessionStart = DateTime.Now.AddMinutes(-47);

decimal minutes = countdown.TimeElapsed(sessionStart, TimeUnit.Minutes, 1); // 47.0
decimal hours   = countdown.TimeElapsed(sessionStart, TimeUnit.Hours,   2); // 0.78
```

---

### `Progress` and `ProgressPercent`

```csharp
decimal Progress(DateTime start, DateTime end);                              // 0.0 – 1.0
decimal Progress(DateTime start, DateTime end, DateTime reference);          // custom reference
decimal ProgressPercent(DateTime start, DateTime end);                       // 0.0 – 100.0
```

```csharp
var sprintStart = new DateTime(2025, 6, 1);
var sprintEnd   = new DateTime(2025, 6, 14);

decimal ratio   = countdown.Progress(sprintStart, sprintEnd);        // e.g. 0.5714...
decimal percent = countdown.ProgressPercent(sprintStart, sprintEnd); // e.g. 57.14
```

---

### `Breakdown`

Returns the remaining time split into discrete components. All values are `0` when expired.

```csharp
Dictionary<TimeUnit, decimal> Breakdown(DateTime deadline);
```

```csharp
var deadline = DateTime.Now.AddHours(2).AddMinutes(15).AddSeconds(30);
var parts    = countdown.Breakdown(deadline);

Console.WriteLine($"{parts[TimeUnit.Hours]}h {parts[TimeUnit.Minutes]}m {parts[TimeUnit.Seconds]}s");
// 2h 15m 30s
```

---

### `Format`

Returns a human-readable countdown string. Only non-zero components are included.
Returns `"Expired"` when the deadline has passed.

```csharp
string Format(DateTime deadline, bool includeSeconds = false);
```

```csharp
var deadline = DateTime.Now.AddDays(5).AddHours(3).AddMinutes(20);

countdown.Format(deadline);               // "5d 3h 20m"
countdown.Format(deadline, true);         // "5d 3h 20m"   (seconds = 0, omitted)
countdown.Format(DateTime.Now.AddDays(-1)); // "Expired"
```

---

### `IsWithin`

Returns `true` if the deadline is **still in the future** and within the specified threshold.

```csharp
bool IsWithin(DateTime deadline, decimal amount, TimeUnit unit);
```

```csharp
var sla = DateTime.Now.AddMinutes(45);

bool nearingExpiry = countdown.IsWithin(sla, 1, TimeUnit.Hours);   // true  — within 1 hour
bool critical      = countdown.IsWithin(sla, 30, TimeUnit.Minutes); // false — more than 30 min remain
```

---

### `IsStarted`

Returns `true` if `from` is in the past or present (i.e., elapsed time > 0).

```csharp
bool IsStarted(DateTime from);
```

```csharp
var eventStart = new DateTime(2025, 6, 1, 9, 0, 0);

bool started = countdown.IsStarted(eventStart); // true if event has begun
```

---

### Practical Examples

#### Project delivery deadline

```csharp
var countdown = new ValiCountdown();
var delivery  = new DateTime(2025, 9, 30, 18, 0, 0);

if (countdown.IsExpired(delivery))
{
    Console.WriteLine("Deadline has passed.");
}
else
{
    string remaining = countdown.Format(delivery);
    decimal pct      = countdown.ProgressPercent(new DateTime(2025, 6, 1), delivery);
    Console.WriteLine($"Time remaining: {remaining}");            // "91d 6h 0m"
    Console.WriteLine($"Sprint progress: {pct:F1}%");
}
```

#### SLA monitoring with 1-hour alert

```csharp
var countdown  = new ValiCountdown();
var ticketOpen = new DateTime(2025, 6, 10, 8, 0, 0);
var slaCutoff  = ticketOpen.AddHours(8); // 8-hour SLA

bool breached = countdown.IsExpired(slaCutoff);
bool critical = countdown.IsWithin(slaCutoff, 1, TimeUnit.Hours); // alert if < 1 h left

if (breached)
    Console.WriteLine("SLA BREACHED");
else if (critical)
    Console.WriteLine($"ALERT: SLA expires in {countdown.Format(slaCutoff, true)}");
else
    Console.WriteLine($"SLA OK — {countdown.Format(slaCutoff)} remaining");
```

## Dependency Injection

```csharp
// Program.cs — ASP.NET Core
using Vali_CountDown.Extensions;

builder.Services.AddValiCountdown();
```

Inject `IValiCountdown` wherever needed:

```csharp
public class DeadlineService(IValiCountdown countdown)
{
    public string GetStatus(DateTime deadline) =>
        countdown.IsExpired(deadline) ? "Expired" : countdown.Format(deadline);
}
```

## License

Licensed under the [MIT License](../LICENSE).
Copyright © 2025 Felipe Rafael Montenegro Morriberon. All rights reserved.
