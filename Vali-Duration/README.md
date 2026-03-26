# Vali-Duration

[![NuGet](https://img.shields.io/nuget/v/Vali-Duration.svg)](https://www.nuget.org/packages/Vali-Duration)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-6%20%7C%207%20%7C%208%20%7C%209-purple.svg)](https://dotnet.microsoft.com)

**Vali-Duration** provides a high-precision, immutable duration value type (`ValiDuration`) that uses `decimal` arithmetic internally. It avoids the floating-point rounding errors common with `double`-based `TimeSpan` calculations and supports fluent conversions, formatting, and arithmetic operators.

## Features

- `decimal`-precision internal representation — no floating-point drift
- Factory methods: `FromMilliseconds`, `FromSeconds`, `FromMinutes`, `FromHours`, `FromDays`, `FromWeeks`
- Convert to any `TimeUnit` via `As(TimeUnit)`
- Named accessors: `TotalMilliseconds`, `TotalSeconds`, `TotalMinutes`, `TotalHours`, `TotalDays`, `TotalWeeks`
- Arithmetic operators: `+`, `-`, `*`, `/`
- Comparison operators: `<`, `>`, `<=`, `>=`, `==`, `!=`
- Implicit conversion to/from `TimeSpan`
- Human-readable `Format()` output
- Implements `IEquatable<ValiDuration>` and `IComparable<ValiDuration>`

## Installation

```bash
dotnet add package Vali-Duration
```

> Compiled targets: `net8.0` · `net9.0`. Compatible with **.NET 6 and .NET 7** via NuGet backward compatibility — no separate build needed.

## Quick Start

```csharp
using Vali_Duration.Models;
using Vali_Time.Enums;

// Create a duration
var duration = ValiDuration.FromHours(1.5m);

// Access in different units
Console.WriteLine(duration.TotalMinutes);      // 90
Console.WriteLine(duration.As(TimeUnit.Seconds)); // 5400

// Arithmetic
var d1 = ValiDuration.FromMinutes(30);
var d2 = ValiDuration.FromMinutes(45);
var total = d1 + d2; // 75 minutes

var doubled = d1 * 2m; // 60 minutes

// Comparison
bool isLonger = d2 > d1; // true

// Implicit TimeSpan conversion
TimeSpan ts = ValiDuration.FromHours(2);
ValiDuration fromTs = TimeSpan.FromMinutes(90);

// Formatting
Console.WriteLine(ValiDuration.FromHours(1.5m).Format());
// "1h 30m 0s"
```

## Factory Methods

```csharp
ValiDuration.FromMilliseconds(500m);
ValiDuration.FromSeconds(90m);
ValiDuration.FromMinutes(45m);
ValiDuration.FromHours(2.5m);
ValiDuration.FromDays(1.5m);
ValiDuration.FromWeeks(2m);
```

## Conversion

```csharp
var d = ValiDuration.FromHours(2);

d.TotalMilliseconds  // 7_200_000
d.TotalSeconds       // 7_200
d.TotalMinutes       // 120
d.TotalHours         // 2
d.TotalDays          // 0.0833...
d.TotalWeeks         // 0.0119...

// Generic conversion
d.As(TimeUnit.Minutes) // 120
d.As(TimeUnit.Days)    // 0.0833...
```

## Operators

```csharp
var a = ValiDuration.FromMinutes(60);
var b = ValiDuration.FromMinutes(30);

var sum  = a + b;    // 90 min
var diff = a - b;    // 30 min
var mult = a * 2m;   // 120 min
var div  = a / 2m;   // 30 min

bool eq  = a == b;   // false
bool gt  = a > b;    // true
bool lte = b <= a;   // true
```

## TimeSpan Interoperability

```csharp
// Implicit from TimeSpan
ValiDuration d = TimeSpan.FromHours(1);

// Implicit to TimeSpan
TimeSpan ts = ValiDuration.FromMinutes(90);

// Use anywhere TimeSpan is accepted
Task.Delay((TimeSpan)ValiDuration.FromSeconds(5));
```

## Why Not TimeSpan?

`TimeSpan` uses `double` internally, which causes rounding errors in financial and billing calculations:

```csharp
// TimeSpan — floating-point drift
var ts = TimeSpan.FromHours(1.1) + TimeSpan.FromHours(1.2);
Console.WriteLine(ts.TotalHours); // 2.2999999999999998 ← drift

// ValiDuration — decimal precision
var vd = ValiDuration.FromHours(1.1m) + ValiDuration.FromHours(1.2m);
Console.WriteLine(vd.TotalHours);  // 2.3 ← exact
```

## Part of the Vali-Tempo Ecosystem

| Package | Description |
|---------|-------------|
| [Vali-Time](../Vali-Time/README.md) | Core time conversion and formatting |
| [Vali-Range](../Vali-Range/README.md) | Date range operations |
| [Vali-Calendar](../Vali-Calendar/README.md) | Workday and week calculations |
| **Vali-Duration** | High-precision duration struct |
| [Vali-CountDown](../Vali-CountDown/README.md) | Countdown and deadline tracking |
| [Vali-Age](../Vali-Age/README.md) | Age calculation utilities |
| [Vali-Schedule](../Vali-Schedule/README.md) | Recurring event scheduling |
| [Vali-Holiday](../Vali-Holiday/README.md) | Holiday providers for 35+ countries |
| [Vali-TimeZone](../Vali-TimeZone/README.md) | Timezone conversion and discovery |
| [Vali-Tempo](../Vali-Tempo/README.md) | Meta-package: all of the above |

## License

MIT © 2025 Felipe Rafael Montenegro Morriberon

## Donations

If this package is useful to you, consider supporting its development:

- **Latin America** — [MercadoPago](https://link.mercadopago.com.pe/felipermm)
- **International** — [PayPal](https://paypal.me/felipeRMM?country.x=PE&locale.x=es_XC)

---
