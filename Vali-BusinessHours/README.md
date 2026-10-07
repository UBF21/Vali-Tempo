# Vali-BusinessHours

[![NuGet](https://img.shields.io/nuget/v/Vali-BusinessHours.svg)](https://www.nuget.org/packages/Vali-BusinessHours)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](../LICENSE)

> Resolves the simultaneous business-hours overlap window between two IANA timezones —
> the classic "what time works for both of us" cross-timezone scheduling problem.

## Features

- **Pairwise timezone overlap** — given a daily business-hours block (`TimeOnly` start/end)
  per zone, resolves the UTC window where both are open at once.
- **DST-correct** — the offset of each zone is resolved at the specific instant evaluated
  (via `Vali-TimeZone`), never a cached base offset.
- **Midnight-crossing aware** — a zone's business hours can map to the calendar day before
  or after the reference date once projected to the other zone; the overlap search checks
  all three candidate days.
- **Workday-aware variant** — `OverlapOnWorkday` short-circuits to "no overlap" when the
  reference date isn't a workday in either zone's own `Vali-Calendar`.
- Composes `Vali-Calendar` and `Vali-TimeZone` without coupling either package to the other
  (see the ADR in the repo for why this is a separate package).

## Installation

```bash
dotnet add package Vali-BusinessHours
```

## Quick Start

```csharp
using Vali_BusinessHours.Core;
using Vali_BusinessHours.Models;
using Vali_TimeZone.Core;

IValiTimeZone timeZone = new ValiTimeZone();
IValiBusinessHours businessHours = new ValiBusinessHours(timeZone);

var lima = new BusinessHoursWindow("America/Lima", new TimeOnly(9, 0), new TimeOnly(18, 0));
var madrid = new BusinessHoursWindow("Europe/Madrid", new TimeOnly(9, 0), new TimeOnly(18, 0));

BusinessHoursOverlapResult result = businessHours.Overlap(lima, madrid, new DateTime(2025, 6, 2));

if (result.HasOverlap)
{
    Console.WriteLine($"Overlap (Lima time): {result.StartInZoneA} → {result.EndInZoneA}");
    Console.WriteLine($"Overlap (Madrid time): {result.StartInZoneB} → {result.EndInZoneB}");
}
```

## API Reference

### `Overlap(BusinessHoursWindow a, BusinessHoursWindow b, DateTime referenceDate)`

Resolves the overlap for a calendar date, interpreted in `a`'s timezone.

```csharp
BusinessHoursOverlapResult Overlap(BusinessHoursWindow a, BusinessHoursWindow b, DateTime referenceDate);
```

### `OverlapOnWorkday((BusinessHoursWindow, IValiCalendar) a, (BusinessHoursWindow, IValiCalendar) b, DateTime referenceDate)`

Same as `Overlap`, but requires `referenceDate` to be a workday in both calendars first —
useful when each side has its own holiday calendar.

```csharp
using Vali_Calendar.Core;

IValiCalendar peruCalendar = new ValiCalendar(holidayProvider: HolidayProviderFactory.Create("PE"));
IValiCalendar spainCalendar = new ValiCalendar(holidayProvider: HolidayProviderFactory.Create("ES"));

var result = businessHours.OverlapOnWorkday(
    (lima, peruCalendar),
    (madrid, spainCalendar),
    new DateTime(2025, 6, 2));
```

### `BusinessHoursOverlapResult`

| Member | Meaning |
|---|---|
| `HasOverlap` | `true` if the two windows share any instant. |
| `StartUtc` / `EndUtc` | The overlap window as absolute UTC instants. |
| `StartInZoneA` / `EndInZoneA` | The overlap window projected to the first window's zone. |
| `StartInZoneB` / `EndInZoneB` | The overlap window projected to the second window's zone. |

All members are `null` when `HasOverlap` is `false`.

## v1 Scope

- Exactly **two** timezones per call. Three or more zones at once is out of scope for v1.
- A single daily business-hours block per zone — no recurrence, no per-day exceptions.
- No automatic holiday filtering inside `Overlap` — combine with your own `IValiCalendar` +
  `IHolidayProvider` (see `OverlapOnWorkday`), or filter the reference date yourself.
- No meeting-scheduling features (no calendars, invitees, or slot negotiation) — this is a
  calculation primitive, not a scheduling product.

See `.claude/memory/plans/business-hours-overlap/` in the repo for the full ADR behind these
decisions.

## Dependency Injection

```csharp
// Program.cs
builder.Services.AddValiTimeZone();      // IValiBusinessHours resolves IValiTimeZone from the container
builder.Services.AddValiBusinessHours(); // registers IValiBusinessHours as a singleton
```

## License

MIT © 2025 Felipe Rafael Montenegro Morriberon
