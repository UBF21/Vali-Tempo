# Vali-Age
> Age calculations, birthday utilities, and relative time formatting for .NET.

## Features

- **Years** — whole years of age relative to today or a custom reference date (2 overloads)
- **Exact** — complete breakdown into years, months, days, and total days via `AgeResult`
- **Format** — human-readable age string (`"32 years, 4 months, 12 days"`)
- **Relative** — relative time description: `"3 days ago"`, `"in 2 weeks"`, `"just now"`
- **IsAtLeast** — validate minimum age in years, months, or days (form validation, access control)
- **IsBirthday** — check whether today (or any date) is someone's birthday
- **NextBirthday** — returns the `DateTime` of the next upcoming birthday
- **DaysUntilBirthday** — integer count of days until the next birthday
- Compiled targets: `net8.0` · `net9.0` — compatible with **.NET 6+** via NuGet backward compatibility
- Registered as a **singleton** via `AddValiAge()`

## Installation

```bash
dotnet add package Vali-Age
```

> Compatible with .NET 6, 7, 8, and 9. For new projects, .NET 8 (LTS) or .NET 9 is recommended.

## Quick Start

```csharp
using Vali_Age.Core;
using Vali_Time.Enums;

var age = new ValiAge();

var dob = new DateTime(1992, 7, 15);

int  years      = age.Years(dob);          // 32
bool isToday    = age.IsBirthday(dob);     // true on July 15
int  daysLeft   = age.DaysUntilBirthday(dob);
string relative = age.Relative(dob);       // "32 years ago"
```

## API Reference

### `Years`

Returns the number of **complete years** elapsed from `birthDate`.

```csharp
int Years(DateTime birthDate);                          // vs DateTime.Today
int Years(DateTime birthDate, DateTime reference);      // vs custom date
```

```csharp
var dob = new DateTime(1990, 3, 21);

int age        = age.Years(dob);                                      // 35
int ageInFuture = age.Years(dob, new DateTime(2030, 1, 1));           // 39
```

---

### `Exact`

Returns an `AgeResult` struct with `Years`, `Months`, `Days`, and `TotalDays`.

```csharp
AgeResult Exact(DateTime birthDate);
AgeResult Exact(DateTime birthDate, DateTime reference);
```

```csharp
var dob    = new DateTime(1992, 11, 5);
var result = age.Exact(dob);

Console.WriteLine(result.Years);     // 32
Console.WriteLine(result.Months);    // 4
Console.WriteLine(result.Days);      // 16
Console.WriteLine(result.TotalDays); // 11828
Console.WriteLine(result);           // "32 years, 4 months, 16 days"
```

---

### `Format`

Returns the same string as `AgeResult.ToString()`.

```csharp
string Format(DateTime birthDate);
string Format(DateTime birthDate, DateTime reference);
```

```csharp
string label = age.Format(new DateTime(1992, 11, 5)); // "32 years, 4 months, 16 days"
```

---

### `Relative`

Returns a human-readable description of the time difference relative to now (or a custom reference).

| Absolute difference | Past example | Future example |
|---|---|---|
| < 60 seconds | `"just now"` | `"just now"` |
| < 60 minutes | `"5 minutes ago"` | `"in 5 minutes"` |
| < 24 hours | `"3 hours ago"` | `"in 3 hours"` |
| < 7 days | `"2 days ago"` | `"in 2 days"` |
| < 30 days | `"1 week ago"` | `"in 2 weeks"` |
| < 12 months | `"4 months ago"` | `"in 4 months"` |
| >= 12 months | `"2 years ago"` | `"in 2 years"` |

```csharp
string Relative(DateTime date);
string Relative(DateTime date, DateTime reference);
```

```csharp
var postDate = DateTime.Now.AddDays(-3);
Console.WriteLine(age.Relative(postDate));               // "3 days ago"

var futureEvent = DateTime.Now.AddHours(2);
Console.WriteLine(age.Relative(futureEvent));            // "in 2 hours"

Console.WriteLine(age.Relative(DateTime.Now.AddSeconds(-10))); // "just now"
```

---

### `IsAtLeast`

Validates whether a person has reached a minimum age threshold.

```csharp
bool IsAtLeast(DateTime birthDate, int amount, DatePart part);
bool IsAtLeast(DateTime birthDate, int amount, DatePart part, DateTime reference);
```

Supported `DatePart` values: `Year`, `Month`, `Day`.

```csharp
var dob = new DateTime(2007, 5, 20);

bool isAdult       = age.IsAtLeast(dob, 18, DatePart.Year);  // true/false depending on today
bool atLeast6Months = age.IsAtLeast(dob, 6, DatePart.Month);
bool atLeast1000Days = age.IsAtLeast(dob, 1000, DatePart.Day);
```

---

### `IsBirthday`

```csharp
bool IsBirthday(DateTime birthDate);
bool IsBirthday(DateTime birthDate, DateTime reference);
```

```csharp
var dob = new DateTime(1990, 6, 15);

bool isToday = age.IsBirthday(dob);                                           // true on June 15
bool wasYest  = age.IsBirthday(dob, new DateTime(DateTime.Today.Year, 6, 14)); // false
```

---

### `NextBirthday`

Returns the `DateTime` of the next occurrence of the birthday. If today is the birthday, returns next year's date.

```csharp
DateTime NextBirthday(DateTime birthDate);
DateTime NextBirthday(DateTime birthDate, DateTime reference);
```

```csharp
var dob  = new DateTime(1990, 12, 25);
var next = age.NextBirthday(dob); // DateTime(currentYear or +1, 12, 25)

Console.WriteLine(next.ToString("yyyy-MM-dd")); // e.g. "2025-12-25"
```

---

### `DaysUntilBirthday`

```csharp
int DaysUntilBirthday(DateTime birthDate);
int DaysUntilBirthday(DateTime birthDate, DateTime reference);
```

```csharp
var dob  = new DateTime(1990, 12, 25);
int days = age.DaysUntilBirthday(dob); // e.g. 188
```

---

### Practical Examples

#### User profile: age and next birthday

```csharp
var valiAge = new ValiAge();
var dob     = new DateTime(1990, 8, 22);

AgeResult exact    = valiAge.Exact(dob);
DateTime  nextBday = valiAge.NextBirthday(dob);
int       daysLeft = valiAge.DaysUntilBirthday(dob);
bool      isToday  = valiAge.IsBirthday(dob);

Console.WriteLine($"Age: {exact}");                          // "34 years, 6 months, 27 days"
Console.WriteLine($"Next birthday: {nextBday:MMMM dd, yyyy}"); // "August 22, 2025"
Console.WriteLine($"Days until birthday: {daysLeft}");
if (isToday) Console.WriteLine("Happy birthday!");
```

#### Registration form: legal age validation

```csharp
var valiAge = new ValiAge();
var dob     = new DateTime(2010, 3, 15); // user-submitted date

if (!valiAge.IsAtLeast(dob, 18, DatePart.Year))
{
    return ValidationResult.Fail("You must be at least 18 years old to register.");
}

// Show the user their exact age on the confirmation screen
string ageLabel = valiAge.Format(dob); // "15 years, 0 months, 6 days"
```

## Dependency Injection

```csharp
// Program.cs — ASP.NET Core
using Vali_Age.Extensions;

builder.Services.AddValiAge();
```

Inject `IValiAge` wherever needed:

```csharp
public class UserProfileService(IValiAge valiAge)
{
    public string GetAgeLabel(DateTime dob) => valiAge.Format(dob);

    public bool CanVote(DateTime dob) => valiAge.IsAtLeast(dob, 18, DatePart.Year);
}
```

## License

Licensed under the [Apache-2.0 License](https://www.apache.org/licenses/LICENSE-2.0).
Copyright © 2025 Felipe Rafael Montenegro Morriberon. All rights reserved.
