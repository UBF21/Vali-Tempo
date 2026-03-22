# Vali-Holiday
> Official public holiday data for 35 countries across Latin America, Europe, and beyond — with multilingual names, Easter calculations, and an extensible provider model.

## Features

- **35 countries** built-in across Latin America, Europe, Canada, and Australia
- **HolidayProviderFactory** — create pre-configured instances with `CreateAll()`, `CreateLatinAmerica()`, `CreateEurope()`, or `CreateOther()`
- **IsHoliday** — check whether a date is a public holiday in any supported country
- **GetHolidays** — all holidays for a year in one or multiple countries
- **GetNextHoliday / GetPreviousHoliday** — navigate holidays forward and backward from any date
- **IsLongWeekend** — detect holidays on Monday or Friday that extend the weekend
- **HolidaysThisMonth** — filter holidays by month
- **Multilingual names** — `GetName(holiday, "en")` supports `"es"`, `"en"`, `"pt"`, `"fr"`, `"de"`
- **Movable holidays** — Easter-based dates (Good Friday, Corpus Christi, etc.) computed per year
- **IHolidayProvider** — implement your own provider for unsupported countries or regional calendars
- Registered as a **singleton** via `AddValiHoliday()`
- Targets **.NET 8** and **.NET 9**

## Installation

```bash
dotnet add package Vali-Holiday
```

## Quick Start

```csharp
using Vali_Holiday.Core;

// Load all built-in providers (35 countries)
var holidays = HolidayProviderFactory.CreateAll();

// Is today a public holiday in Peru?
bool isHoliday = holidays.IsHoliday(DateTime.Today, "PE");

// What is the next holiday in Chile?
var next = holidays.GetNextHoliday(DateTime.Today, "CL");
Console.WriteLine($"{next?.Name} — {next?.Month:D2}/{next?.Day:D2}");
```

## Supported Countries

### Latin America (15 countries)

| Country | Code | Country | Code |
|---|---|---|---|
| Peru | `PE` | Uruguay | `UY` |
| Chile | `CL` | Paraguay | `PY` |
| Argentina | `AR` | Venezuela | `VE` |
| Colombia | `CO` | Panama | `PA` |
| Mexico | `MX` | Costa Rica | `CR` |
| Brazil | `BR` | Dominican Republic | `DO` |
| Ecuador | `EC` | Cuba | `CU` |
| Bolivia | `BO` | | |

### Europe (17 countries)

| Country | Code | Country | Code |
|---|---|---|---|
| Spain | `ES` | Switzerland | `CH` |
| United States | `US` | Austria | `AT` |
| United Kingdom | `GB` | Poland | `PL` |
| Germany | `DE` | Sweden | `SE` |
| France | `FR` | Norway | `NO` |
| Italy | `IT` | Denmark | `DK` |
| Portugal | `PT` | Finland | `FI` |
| Netherlands | `NL` | Ireland | `IE` |
| Belgium | `BE` | | |

### Other (3 countries)

| Country | Code |
|---|---|
| Canada | `CA` |
| Australia | `AU` |

## API Reference

### `HolidayProviderFactory`

```csharp
// All 35 countries
ValiHoliday all    = HolidayProviderFactory.CreateAll();

// Only Latin American countries (15)
ValiHoliday latam  = HolidayProviderFactory.CreateLatinAmerica();

// Only European + US countries (17)
ValiHoliday europe = HolidayProviderFactory.CreateEurope();

// Canada and Australia
ValiHoliday other  = HolidayProviderFactory.CreateOther();
```

---

### `IsHoliday`

```csharp
bool IsHoliday(DateTime date, string countryCode);
```

```csharp
var holidays = HolidayProviderFactory.CreateAll();

bool peruIndependence = holidays.IsHoliday(new DateTime(2025, 7, 28), "PE"); // true
bool randomDay        = holidays.IsHoliday(new DateTime(2025, 3, 5),  "CL"); // false
```

---

### `GetHolidays`

```csharp
// All holidays in a country for a year
IEnumerable<HolidayInfo> GetHolidays(int year, string countryCode);

// Holidays across multiple countries
IEnumerable<HolidayInfo> GetHolidays(int year, params string[] countryCodes);
```

```csharp
var peruHolidays2025 = holidays.GetHolidays(2025, "PE");
foreach (var h in peruHolidays2025)
    Console.WriteLine($"{h.Month:D2}/{h.Day:D2} — {h.Name}");

// Multi-country query
var andean = holidays.GetHolidays(2025, "PE", "CL", "BO");
```

---

### `GetNextHoliday` / `GetPreviousHoliday`

Searches up to one year ahead or back. Returns `null` if no holiday is found in that window.

```csharp
HolidayInfo? GetNextHoliday(DateTime date, string countryCode);
HolidayInfo? GetPreviousHoliday(DateTime date, string countryCode);
```

```csharp
var next = holidays.GetNextHoliday(DateTime.Today, "AR");
if (next is not null)
    Console.WriteLine($"Next holiday: {next.Name} on {next.Month:D2}/{next.Day:D2}");

var prev = holidays.GetPreviousHoliday(DateTime.Today, "MX");
```

---

### `IsLongWeekend`

Returns `true` when the date is a public holiday that falls on a **Monday** or **Friday**, creating a 3-day weekend.

```csharp
bool IsLongWeekend(DateTime date, string countryCode);
```

```csharp
// Check every day in 2025 for long weekends in Chile
for (var d = new DateTime(2025, 1, 1); d.Year == 2025; d = d.AddDays(1))
{
    if (holidays.IsLongWeekend(d, "CL"))
        Console.WriteLine($"Long weekend: {d:yyyy-MM-dd} ({d.DayOfWeek})");
}
```

---

### `HolidaysThisMonth`

```csharp
IEnumerable<HolidayInfo> HolidaysThisMonth(int year, int month, string countryCode);
```

```csharp
var julyHolidays = holidays.HolidaysThisMonth(2025, 7, "PE");
foreach (var h in julyHolidays)
    Console.WriteLine($"{h.Day:D2} — {h.Name}"); // 28 — Fiestas Patrias, 29 — ...
```

---

### Multilingual Names

`HolidayInfo` carries a `Names` dictionary keyed by BCP 47 language tags. Use `IHolidayProvider.GetName()` to get the name in a specific language with automatic fallback.

```csharp
var provider = holidays.For("PE");
var holiyday = holidays.GetNextHoliday(DateTime.Today, "PE")!;

string spanish  = provider.GetName(holiyday, "es"); // "Año Nuevo"
string english  = provider.GetName(holiyday, "en"); // "New Year's Day"
string french   = provider.GetName(holiyday, "fr"); // "Jour de l'An"
string german   = provider.GetName(holiyday, "de"); // "Neujahr"
```

---

### `HolidayInfo` Model

| Property | Type | Description |
|---|---|---|
| `Id` | `string` | Unique identifier (e.g., `"pe_independence"`) |
| `Month` | `int` | Month (1–12) |
| `Day` | `int` | Day of month (1–31) |
| `CountryCode` | `string` | ISO 3166-1 alpha-2 |
| `Name` | `string` | Name in the country's primary language |
| `Names` | `IReadOnlyDictionary<string, string>` | Multilingual names (`"es"`, `"en"`, `"pt"`, `"fr"`, `"de"`) |
| `Type` | `HolidayType` | National, Religious, Civic, etc. |
| `IsMovable` | `bool` | `true` for Easter-based or floating holidays |
| `RegionCode` | `string?` | ISO 3166-2 region code, or `null` for national holidays |
| `Description` | `string?` | Optional cultural or historical context |

```csharp
// Convert to DateTime for a specific year
DateTime date = holiday.ToDateTime(2025);

// Default string representation
Console.WriteLine(holiday); // "[PE] 07/28 - Fiestas Patrias"
```

---

### Custom `IHolidayProvider`

Implement `IHolidayProvider` to add support for a country or custom calendar not included in the built-in set.

```csharp
using Vali_Holiday.Core;
using Vali_Holiday.Models;

public class JapanHolidayProvider : IHolidayProvider
{
    public string CountryCode    => "JP";
    public string CountryName    => "Japan";
    public string PrimaryLanguage => "ja";

    public IEnumerable<HolidayInfo> GetHolidays(int year)
    {
        var names = new Dictionary<string, string>
        {
            ["ja"] = "元日",
            ["en"] = "New Year's Day"
        };

        yield return new HolidayInfo(
            id:          "jp_new_year",
            month:       1,
            day:         1,
            countryCode: "JP",
            name:        "元日",
            names:       names.AsReadOnly(),
            type:        HolidayType.National);
        // ... additional holidays
    }

    public bool IsHoliday(DateTime date) =>
        GetHolidays(date.Year).Any(h => h.Month == date.Month && h.Day == date.Day);

    public bool IsHoliday(DateOnly date) =>
        IsHoliday(date.ToDateTime(TimeOnly.MinValue));

    public HolidayInfo? GetHoliday(DateTime date) =>
        GetHolidays(date.Year).FirstOrDefault(h => h.Month == date.Month && h.Day == date.Day);

    public IEnumerable<HolidayInfo> GetHolidaysInRange(DateTime from, DateTime to)
    {
        for (int y = from.Year; y <= to.Year; y++)
            foreach (var h in GetHolidays(y))
            {
                var d = new DateTime(y, h.Month, h.Day);
                if (d >= from.Date && d <= to.Date) yield return h;
            }
    }

    public string GetName(HolidayInfo holiday, string languageCode) =>
        holiday.Names.TryGetValue(languageCode, out var name) ? name : holiday.Name;
}

// Register the custom provider
var holidays = HolidayProviderFactory.CreateAll();
holidays.Register(new JapanHolidayProvider());

bool isNewYear = holidays.IsHoliday(new DateTime(2025, 1, 1), "JP"); // true
```

---

### Practical Examples

#### Calculate business days in Peru excluding public holidays

```csharp
var holidays = HolidayProviderFactory.CreateLatinAmerica();

var start = new DateTime(2025, 7, 1);
var end   = new DateTime(2025, 7, 31);

int businessDays = 0;
for (var d = start; d <= end; d = d.AddDays(1))
{
    if (d.DayOfWeek == DayOfWeek.Saturday) continue;
    if (d.DayOfWeek == DayOfWeek.Sunday)   continue;
    if (holidays.IsHoliday(d, "PE"))        continue;
    businessDays++;
}

Console.WriteLine($"Business days in July 2025 (PE): {businessDays}");
// July has Fiestas Patrias on 28 & 29, so result will be less than 23
```

#### Detect long weekends in Chile for 2025

```csharp
var holidays = HolidayProviderFactory.CreateLatinAmerica();

Console.WriteLine("Long weekends in Chile 2025:");
for (var d = new DateTime(2025, 1, 1); d.Year == 2025; d = d.AddDays(1))
{
    if (holidays.IsLongWeekend(d, "CL"))
    {
        var info = holidays.GetNextHoliday(d, "CL");
        Console.WriteLine($"  {d:yyyy-MM-dd} ({d.DayOfWeek}) — {info?.Name}");
    }
}
```

## Dependency Injection

```csharp
// Program.cs — ASP.NET Core
using Vali_Holiday.Extensions;

// Registers ValiHoliday as a singleton pre-loaded with ALL built-in country providers
builder.Services.AddValiHoliday();
```

Inject `ValiHoliday` directly:

```csharp
public class PayrollService(ValiHoliday holidays)
{
    public bool IsWorkday(DateTime date, string countryCode)
    {
        if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) return false;
        return !holidays.IsHoliday(date, countryCode);
    }
}
```

## License

Licensed under the [Apache-2.0 License](https://www.apache.org/licenses/LICENSE-2.0).
Copyright © 2025 Felipe Rafael Montenegro Morriberon. All rights reserved.
