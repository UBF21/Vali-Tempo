# Vali-Range

> Immutable date ranges with creation helpers, set-theory operations, and calendar-aware iteration for .NET.

## Features

- `DateRange` readonly struct — inclusive start/end, `Duration(TimeUnit)`, `Contains(DateTime)`, `ToString()`
- Factory methods: `Create`, `LastUnits`, `NextUnits`, `ThisWeek`, `ThisMonth`, `ThisQuarter`, `ThisYear`
- Set-theory queries: `Contains`, `Overlaps`, `IsContainedBy`, `Intersection`, `Union`
- Transformations: `Expand`, `Shrink`, `Shift`
- Adjacency and merging: `IsAdjacent`, `Merge`, `Gaps`
- Iterators: `EachDay`, `EachWeek`, `EachMonth`, `EachWorkday`
- Splitters: `SplitByDay`, `SplitByWeek`, `SplitByMonth`, `SplitByQuarter`
- Calendar-accurate arithmetic for Months and Years
- Configurable week start (Monday ISO 8601 or Sunday)

## Installation

```bash
dotnet add package Vali-Range
```

Compiled targets: `net8.0` · `net9.0`. Compatible with **.NET 6 and .NET 7** via NuGet backward compatibility — no separate build needed.

## Quick Start

```csharp
using Vali_Range.Core;
using Vali_Range.Models;
using Vali_Time.Enums;

var valiRange = new ValiRange();

// Last 30 days up to now
DateRange last30 = valiRange.LastUnits(30, TimeUnit.Days);
Console.WriteLine(last30); // "2025-02-19 → 2025-03-21"

// Current quarter
DateRange q = valiRange.ThisQuarter();
Console.WriteLine(q.Duration(TimeUnit.Days)); // e.g. 90
```

## API Reference

### DateRange Struct

`DateRange` is an immutable value type.

```csharp
var range = new DateRange(new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));

// Properties
DateTime start = range.Start;  // 2025-01-01
DateTime end   = range.End;    // 2025-03-31
bool valid     = range.IsValid; // true (Start <= End)

// Duration in any TimeUnit
decimal days  = range.Duration(TimeUnit.Days);   // 89
decimal weeks = range.Duration(TimeUnit.Weeks);  // ~12.71
decimal hours = range.Duration(TimeUnit.Hours);  // 2136

// Containment
bool inside = range.Contains(new DateTime(2025, 2, 15)); // true
bool outside = range.Contains(new DateTime(2025, 6, 1)); // false

// Human-readable string
Console.WriteLine(range.ToString()); // "2025-01-01 → 2025-03-31"
```

---

### ValiRange

#### Create

```csharp
var valiRange = new ValiRange();

DateRange range = valiRange.Create(
    new DateTime(2025, 1, 1),
    new DateTime(2025, 12, 31));

Console.WriteLine(range); // "2025-01-01 → 2025-12-31"
```

#### LastUnits / NextUnits

```csharp
// Last 7 days (end = now)
DateRange lastWeek = valiRange.LastUnits(7, TimeUnit.Days);

// Next 3 months (start = now)
DateRange next3Months = valiRange.NextUnits(3, TimeUnit.Months);

// Last 6 hours
DateRange last6h = valiRange.LastUnits(6, TimeUnit.Hours);
```

#### Calendar Presets

```csharp
DateRange week    = valiRange.ThisWeek();                    // Mon–Sun (ISO)
DateRange weekSun = valiRange.ThisWeek(WeekStart.Sunday);   // Sun–Sat
DateRange month   = valiRange.ThisMonth();
DateRange quarter = valiRange.ThisQuarter();
DateRange year    = valiRange.ThisYear();
```

#### Contains / Overlaps / IsContainedBy

```csharp
var rangeA = valiRange.Create(new DateTime(2025, 1, 1), new DateTime(2025, 6, 30));
var rangeB = valiRange.Create(new DateTime(2025, 4, 1), new DateTime(2025, 9, 30));
var rangeC = valiRange.Create(new DateTime(2025, 2, 1), new DateTime(2025, 4, 30));

bool containsDate = valiRange.Contains(rangeA, new DateTime(2025, 3, 15)); // true
bool overlaps     = valiRange.Overlaps(rangeA, rangeB);     // true
bool isContained  = valiRange.IsContainedBy(rangeC, rangeA); // true
```

#### Intersection / Union

```csharp
var a = valiRange.Create(new DateTime(2025, 1, 1), new DateTime(2025, 6, 30));
var b = valiRange.Create(new DateTime(2025, 4, 1), new DateTime(2025, 9, 30));

DateRange? overlap = valiRange.Intersection(a, b);
// overlap: "2025-04-01 → 2025-06-30"

DateRange full = valiRange.Union(a, b);
// full: "2025-01-01 → 2025-09-30"

// Non-overlapping ranges return null
var c = valiRange.Create(new DateTime(2025, 10, 1), new DateTime(2025, 12, 31));
DateRange? noOverlap = valiRange.Intersection(a, c); // null
```

#### Expand / Shrink / Shift

```csharp
var range = valiRange.Create(new DateTime(2025, 3, 10), new DateTime(2025, 3, 20));

// Grow by 5 days on each end
DateRange wider = valiRange.Expand(range, 5, TimeUnit.Days);
Console.WriteLine(wider); // "2025-03-05 → 2025-03-25"

// Shrink by 2 days on each end
DateRange narrower = valiRange.Shrink(range, 2, TimeUnit.Days);
Console.WriteLine(narrower); // "2025-03-12 → 2025-03-18"

// Shift forward 1 week
DateRange shifted = valiRange.Shift(range, 1, TimeUnit.Weeks);
Console.WriteLine(shifted); // "2025-03-17 → 2025-03-27"

// Shift backward 1 month
DateRange shiftedBack = valiRange.Shift(range, -1, TimeUnit.Months);
Console.WriteLine(shiftedBack); // "2025-02-10 → 2025-02-20"
```

#### IsAdjacent / Merge / Gaps

```csharp
var jan = valiRange.Create(new DateTime(2025, 1, 1),  new DateTime(2025, 1, 31));
var feb = valiRange.Create(new DateTime(2025, 1, 31), new DateTime(2025, 2, 28));
var apr = valiRange.Create(new DateTime(2025, 4, 1),  new DateTime(2025, 4, 30));

bool adjacent = valiRange.IsAdjacent(jan, feb); // true

// Merge overlapping/adjacent ranges
var merged = valiRange.Merge(new[] { jan, feb, apr }).ToList();
// merged[0]: "2025-01-01 → 2025-02-28"
// merged[1]: "2025-04-01 → 2025-04-30"

// Find gaps within a container range
var container = valiRange.Create(new DateTime(2025, 1, 1), new DateTime(2025, 4, 30));
var gaps = valiRange.Gaps(new[] { jan, feb, apr }, container).ToList();
// gaps[0]: "2025-03-01 → 2025-03-31"
```

#### EachDay / EachWeek / EachMonth / EachWorkday

```csharp
var range = valiRange.Create(new DateTime(2025, 3, 1), new DateTime(2025, 3, 7));

// One DateTime per calendar day
foreach (DateTime day in valiRange.EachDay(range))
    Console.WriteLine(day.ToString("yyyy-MM-dd"));
// 2025-03-01, 2025-03-02, ..., 2025-03-07

// One DateTime per week start within the range
foreach (DateTime weekStart in valiRange.EachWeek(range))
    Console.WriteLine(weekStart.ToString("yyyy-MM-dd"));

// One DateTime per month start (1st of each month)
var q1 = valiRange.Create(new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
foreach (DateTime monthStart in valiRange.EachMonth(q1))
    Console.WriteLine(monthStart.ToString("yyyy-MM-dd"));
// 2025-01-01, 2025-02-01, 2025-03-01

// Workdays only (Mon–Fri)
foreach (DateTime workday in valiRange.EachWorkday(range))
    Console.WriteLine(workday.ToString("yyyy-MM-dd ddd"));
```

#### SplitByMonth / SplitByQuarter

```csharp
var yearRange = valiRange.ThisYear();

// One DateRange per calendar month
foreach (DateRange month in valiRange.SplitByMonth(yearRange))
    Console.WriteLine($"{month} — {month.Duration(TimeUnit.Days):F0} days");

// One DateRange per calendar quarter
foreach (DateRange q in valiRange.SplitByQuarter(yearRange))
    Console.WriteLine(q);
// "2025-01-01 → 2025-03-31"
// "2025-04-01 → 2025-06-30"
// "2025-07-01 → 2025-09-30"
// "2025-10-01 → 2025-12-31"
```

---

### Practical Example: Filter Orders Within a Range

```csharp
using Vali_Range.Core;
using Vali_Range.Models;
using Vali_Time.Enums;

record Order(int Id, DateTime PlacedAt, decimal Amount);

var valiRange = new ValiRange();
DateRange last90Days = valiRange.LastUnits(90, TimeUnit.Days);

var orders = new List<Order>
{
    new(1, DateTime.Today.AddDays(-100), 250m),
    new(2, DateTime.Today.AddDays(-60),  480m),
    new(3, DateTime.Today.AddDays(-30),  120m),
    new(4, DateTime.Today,               310m),
};

var recentOrders = orders
    .Where(o => valiRange.Contains(last90Days, o.PlacedAt))
    .ToList();

Console.WriteLine($"Orders in last 90 days: {recentOrders.Count}"); // 3
Console.WriteLine($"Total: {recentOrders.Sum(o => o.Amount):C}");
```

### Practical Example: Detect Shift Overlap

```csharp
using Vali_Range.Core;
using Vali_Range.Models;

record Shift(string Employee, DateRange Period);

var valiRange = new ValiRange();

var shifts = new List<Shift>
{
    new("Alice", valiRange.Create(new DateTime(2025, 3, 10, 8, 0, 0),  new DateTime(2025, 3, 10, 16, 0, 0))),
    new("Bob",   valiRange.Create(new DateTime(2025, 3, 10, 14, 0, 0), new DateTime(2025, 3, 10, 22, 0, 0))),
    new("Carol", valiRange.Create(new DateTime(2025, 3, 10, 18, 0, 0), new DateTime(2025, 3, 11, 2,  0, 0))),
};

// Find all pairs that overlap
for (int i = 0; i < shifts.Count; i++)
for (int j = i + 1; j < shifts.Count; j++)
{
    if (valiRange.Overlaps(shifts[i].Period, shifts[j].Period))
    {
        DateRange? shared = valiRange.Intersection(shifts[i].Period, shifts[j].Period);
        Console.WriteLine(
            $"{shifts[i].Employee} and {shifts[j].Employee} overlap: {shared}");
    }
}
// "Alice and Bob overlap: 2025-03-10 → 2025-03-10"
// "Bob and Carol overlap: 2025-03-10 → 2025-03-10"
```

## Dependency Injection

```csharp
using Vali_Range.Core;

services.AddSingleton<IValiRange, ValiRange>();
```

## License

MIT
