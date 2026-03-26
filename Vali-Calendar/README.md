# Vali-Calendar

> Calendar-aware workday calculations, week metadata, and optional public holiday integration for .NET.

## Features

- `WeekOf(date)` returns a `CalendarWeek` with week number, ISO year, start and end dates
- `WeeksInMonth` and `WeekCountInMonth` for calendar grid generation
- `IsWorkday` — excludes weekends and, when a holiday provider is configured, also public holidays
- `WorkdaysBetween`, `WorkdaysInMonth`, `WorkdaysInYear` for business-day counting
- `AddWorkdays` to advance a date by N business days, skipping weekends and holidays
- `NextWorkday` / `PreviousWorkday` to find the nearest valid business day
- `HolidaysInMonth` / `HolidaysInYear` powered by a pluggable `IHolidayProvider`
- `IsFirstDayOfMonth`, `IsLastDayOfMonth`, `DaysInMonth`, `DaysOfMonth`, `IsLeapYear`, `DaysInYear`
- Configurable week start: Monday (ISO 8601) or Sunday
- Zero dependencies beyond Vali-Time and Vali-Holiday (both optional at runtime)

## Installation

```bash
dotnet add package Vali-Calendar
```

Compiled targets: `net8.0` · `net9.0`. Compatible with **.NET 6 and .NET 7** via NuGet backward compatibility — no separate build needed.

## Quick Start

```csharp
using Vali_Calendar.Core;

// Without holiday provider — weekends only
var calendar = new ValiCalendar();

DateTime orderDate    = new DateTime(2025, 3, 20); // Thursday
DateTime deliveryDate = calendar.AddWorkdays(orderDate, 5);
Console.WriteLine(deliveryDate.ToString("yyyy-MM-dd")); // "2025-03-27"

int workdays = calendar.WorkdaysBetween(
    new DateTime(2025, 3, 1),
    new DateTime(2025, 3, 31));
Console.WriteLine($"Workdays in March 2025: {workdays}"); // 21
```

## API Reference

### Constructor

```csharp
// Weekends only (no holiday awareness)
var calendar = new ValiCalendar();
var calendarSun = new ValiCalendar(WeekStart.Sunday);

// With a holiday provider (also excludes public holidays from workday logic)
// IHolidayProvider is implemented in Vali-Holiday or by your own class
var calendarWithHolidays = new ValiCalendar(WeekStart.Monday, myHolidayProvider);
```

---

### Week Methods

#### WeekOf

Returns a `CalendarWeek` for any date.

```csharp
var calendar = new ValiCalendar();
var date = new DateTime(2025, 3, 21); // Friday

CalendarWeek week = calendar.WeekOf(date);
Console.WriteLine(week.WeekNumber);             // 12
Console.WriteLine(week.Year);                   // 2025
Console.WriteLine(week.Start.ToString("yyyy-MM-dd")); // "2025-03-17"
Console.WriteLine(week.End.ToString("yyyy-MM-dd"));   // "2025-03-23"
```

#### CurrentWeek

```csharp
CalendarWeek thisWeek = calendar.CurrentWeek();
```

#### WeeksInMonth / WeekCountInMonth

Useful for building calendar grid views.

```csharp
// All calendar weeks that overlap with March 2025
IEnumerable<CalendarWeek> weeks = calendar.WeeksInMonth(2025, 3);
foreach (var w in weeks)
    Console.WriteLine($"Week {w.WeekNumber}: {w.Start:yyyy-MM-dd} → {w.End:yyyy-MM-dd}");

int count = calendar.WeekCountInMonth(2025, 3); // 6
```

---

### Month Methods

```csharp
var date = new DateTime(2025, 3, 1);
calendar.IsFirstDayOfMonth(date); // true
calendar.IsLastDayOfMonth(date);  // false

var lastDay = new DateTime(2025, 3, 31);
calendar.IsLastDayOfMonth(lastDay); // true

int days = calendar.DaysInMonth(new DateTime(2025, 2, 10)); // 28

// Enumerate every day in February 2025
foreach (DateTime d in calendar.DaysOfMonth(2025, 2))
    Console.WriteLine(d.ToString("yyyy-MM-dd"));
```

---

### Workday Methods

#### IsWorkday

```csharp
var calendar = new ValiCalendar();

calendar.IsWorkday(new DateTime(2025, 3, 21)); // true  (Friday)
calendar.IsWorkday(new DateTime(2025, 3, 22)); // false (Saturday)
calendar.IsWorkday(new DateTime(2025, 3, 23)); // false (Sunday)
```

With a holiday provider, a public holiday on a weekday also returns `false`.

#### WorkdaysBetween

Inclusive count of workdays from `from` to `to`.

```csharp
int count = calendar.WorkdaysBetween(
    new DateTime(2025, 3, 1),
    new DateTime(2025, 3, 31));
Console.WriteLine(count); // 21

int countYear = calendar.WorkdaysInYear(2025);
Console.WriteLine(countYear); // 261
```

#### AddWorkdays

Advance a date by exactly N business days, skipping weekends (and holidays when configured).

```csharp
// Start on a Friday
DateTime start  = new DateTime(2025, 3, 21);
DateTime result = calendar.AddWorkdays(start, 3);
Console.WriteLine(result.ToString("yyyy-MM-dd")); // "2025-03-26" (Wed, skips Sat+Sun)

// Negative: go backward
DateTime back = calendar.AddWorkdays(start, -3);
Console.WriteLine(back.ToString("yyyy-MM-dd")); // "2025-03-18" (Tue)
```

#### NextWorkday / PreviousWorkday

If the date is already a workday it is returned unchanged.

```csharp
// Saturday → next workday is Monday
DateTime sat    = new DateTime(2025, 3, 22);
DateTime next   = calendar.NextWorkday(sat);
Console.WriteLine(next.ToString("yyyy-MM-dd")); // "2025-03-24"

DateTime friday = new DateTime(2025, 3, 21);
DateTime prev   = calendar.PreviousWorkday(friday);
Console.WriteLine(prev.ToString("yyyy-MM-dd")); // "2025-03-21" (already a workday)
```

---

### Holiday Integration

Holiday methods require an `IHolidayProvider` to be passed to the constructor. Without it, calling `HolidaysInMonth` or `HolidaysInYear` throws `InvalidOperationException`.

```csharp
// Assuming MyHolidayProvider implements IHolidayProvider
var provider = new MyHolidayProvider();
var calendarH = new ValiCalendar(WeekStart.Monday, provider);

// Check if a holiday provider is set
Console.WriteLine(calendarH.HasHolidayProvider); // true

// List holidays in April 2025
foreach (HolidayInfo h in calendarH.HolidaysInMonth(2025, 4))
    Console.WriteLine($"{h.Date:yyyy-MM-dd} — {h.Name}");

// List all holidays in 2025
foreach (HolidayInfo h in calendarH.HolidaysInYear(2025))
    Console.WriteLine($"{h.Date:yyyy-MM-dd} — {h.Name}");
```

---

### Year Methods

```csharp
calendar.IsLeapYear(2024); // true
calendar.IsLeapYear(2025); // false
calendar.DaysInYear(2024); // 366
calendar.DaysInYear(2025); // 365
```

---

### Practical Example: Delivery Date Without vs With Holidays

```csharp
using Vali_Calendar.Core;

// --- Without holiday provider ---
var calendar = new ValiCalendar();

DateTime orderFriday  = new DateTime(2025, 4, 17); // Thursday before Easter
DateTime deliveryBase = calendar.AddWorkdays(orderFriday, 5);
Console.WriteLine(deliveryBase.ToString("yyyy-MM-dd")); // "2025-04-24"

// --- With holiday provider (Easter Friday = 2025-04-18 is a holiday) ---
// var provider = new MyHolidayProvider(); // returns 2025-04-18 as a holiday
// var calendarH = new ValiCalendar(WeekStart.Monday, provider);
// DateTime deliveryWithHoliday = calendarH.AddWorkdays(orderFriday, 5);
// Console.WriteLine(deliveryWithHoliday.ToString("yyyy-MM-dd")); // "2025-04-25" (one extra day)

// --- Count workdays in the current month ---
int workdaysThisMonth = calendar.WorkdaysInMonth(2025, 3);
Console.WriteLine($"March 2025 has {workdaysThisMonth} workdays."); // 21

// --- Calendar week view for a month ---
foreach (CalendarWeek week in calendar.WeeksInMonth(2025, 3))
{
    Console.WriteLine(
        $"W{week.WeekNumber:D2} ({week.Year}): " +
        $"{week.Start:MMM dd} – {week.End:MMM dd}");
}
```

## Dependency Injection

```csharp
using Vali_Calendar.Core;

// Without holidays
services.AddSingleton<IValiCalendar, ValiCalendar>();

// With a holiday provider registered separately
services.AddSingleton<IHolidayProvider, MyHolidayProvider>();
services.AddSingleton<IValiCalendar>(sp =>
    new ValiCalendar(
        WeekStart.Monday,
        sp.GetRequiredService<IHolidayProvider>()));
```

## License

MIT

## Donations

If this package is useful to you, consider supporting its development:

- **Latin America** — [MercadoPago](https://link.mercadopago.com.pe/felipermm)
- **International** — [PayPal](https://paypal.me/felipeRMM?country.x=PE&locale.x=es_XC)

---
