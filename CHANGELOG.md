# Changelog

**English** | [Español](CHANGELOG.es.md)

All notable changes to the Vali-Tempo ecosystem will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

---

## Vali-Tempo — 1.1.1

### Changed
- Replaced the temporary `Vali-BusinessHours`/`Vali-Bucket` `ProjectReference` with a pinned `PackageReference` now that both are published. Bumped every other module dependency to its latest published version. No code change.

---

## Vali-Time — 1.1.0

### Added
- `DurationStringParser` — public parser extracted from `ValiTime`'s duration-string grammar, reused by `Vali-Duration`'s `IParsable<ValiDuration>`
- `HumanizeCulture` enum (`En`/`Es`), reused by `Vali-Age`'s `Humanize()`
- `Constants.IsClampedMonthEnd(originalDay, candidate)` — shared month-end-clamp detection, reused by `ValiAge.Exact()`

### Changed
- `StartOfWeek`/`IsWeekend`/`IsWeekday` consolidated into `DateTimeExtensions` as the single source of truth (previously duplicated across packages)

## Vali-Range — 1.1.0

### Added
- `IValiRange.Subtract(DateRange a, DateRange b)` — set-theoretic difference A \ B, completing the Intersection/Union/Subtract trio

### Changed
- Add net10.0 target. Still pins `Vali-Time` at 1.0.1 — doesn't need any of the new 1.1.0 members

## Vali-CountDown — 1.0.1

### Changed
- Add net10.0 target. No public API change

## Vali-Holiday — 1.1.0

### Added
- `IValiHoliday.IsHoliday(DateTime, params string[])` (ANY) and `IsHolidayInAll(DateTime, params string[])` (ALL) multi-country overloads
- `HolidayInfo.TryToDateTime(int year, out DateTime result)` — safe `(Month, Day, year)` conversion that returns `false` instead of throwing for Feb 29 in a non-leap year
- `ObservedRule` enum and `IHolidayProvider.GetObservedDate(HolidayInfo, int)` / `IValiHoliday.GetObservedHoliday(DateTime, string)` — official weekend-shift ("observed"/"substitute" day) resolution, implemented for USA (Saturday→previous Friday, Sunday→following Monday) and United Kingdom (weekend→next available weekday, with coordinated Christmas Day/Boxing Day collision handling). No other provider declares a rule in this release.

### Changed
- `IsHoliday(DateTime, params string[])` and `IsHolidayInAll(DateTime, params string[])` now throw `ArgumentNullException` instead of `NullReferenceException` when called with a `null` array
- `BaseHolidayProvider.GetHolidaysInRange` and `ValiHoliday.GetNextHolidayWithYear`/`GetPreviousHolidayWithYear` now share a single Feb-29/non-leap-year guard (via `HolidayInfo.TryToDateTime`) instead of three independently-written checks
- `HolidayInfo` is now a `sealed record` (previously a `sealed class`) — adds structural equality; `ToString()` format is unchanged. Note: `Names` is still compared by reference, since records do not deep-compare collections
- Moved `UsaHolidayProvider.cs`/`CanadaHolidayProvider.cs` to `Providers/NorthAmerica/` and `AustraliaHolidayProvider.cs` to `Providers/Oceania/`, matching their already-declared namespaces (no namespace or behavior change)
- Add net10.0 target. Still pins `Vali-Time` at 1.0.1 — doesn't need any of the new 1.1.0 members

## Vali-TimeZone — 1.1.0

### Changed
- `ValiZoneInfo` is now a `sealed record` (previously a `sealed class`) — adds structural equality; `ToString()` format is unchanged
- Add net10.0 target. Still pins `Vali-Time` at 1.0.1 — doesn't need any of the new 1.1.0 members

## Vali-Age — 1.1.0

### Added
- `Humanize()` relative-time formatting (es/en) via `HumanizeEngine`, built on `Vali-Time`'s `HumanizeCulture`

### Changed
- Temporarily depends on `Vali-Time`'s source via `ProjectReference` (ponytail) instead of a `PackageReference`, because `HumanizeCulture` doesn't exist in the published 1.0.1 package yet — switch to `PackageReference Version="1.1.0"` once Vali-Time republishes (see CONTRIBUTING.md's Release Process)

## Vali-Duration — 1.1.0

### Added
- `IParsable<ValiDuration>`/`IFormattable` via the shared `DurationStringParser`

### Changed
- Temporarily depends on `Vali-Time`'s source via `ProjectReference` (ponytail) instead of a `PackageReference`, because `DurationStringParser` doesn't exist in the published 1.0.1 package yet — switch to `PackageReference Version="1.1.0"` once Vali-Time republishes

## Vali-Calendar — 1.0.1

### Changed
- Add net10.0 target. Still pins `Vali-Time` at 1.0.1 and `Vali-Holiday` at 1.0.0 — doesn't need any of their new members (no public API change)

## Vali-Schedule — 1.1.0

### Added
- `IValiSchedule.SkipWeekends()` and `SkipHolidays(IHolidayProvider, HolidaySkipStrategy)` to exclude weekend/holiday occurrences, resolved via `NextWorkday`/`PreviousWorkday`/`Skip`
- `HolidaySkipStrategy` enum
- `IValiSchedule.OccurrencePeriods(DateTime from, DateTime to)` — pairs consecutive occurrences into `Vali_Range.Models.DateRange` periods
- `Vali_Schedule.Cron.CronSchedule` — zero-dependency 5-field cron expression support (`Parse`/`TryParse`), implementing `IValiSchedule`
- `Vali_Schedule.Cron.CronFormatException` — field/token-attributed parse errors

### Changed
- New cross-package dependencies: `Vali-Schedule` now references `Vali-Holiday` and `Vali-Range` (`PackageReference`, same pattern already used by `Vali-Calendar`). Only `SkipHolidays()` requires a concrete `IHolidayProvider`; `SkipWeekends()` has no such requirement.
- Add net10.0 target. Still pins `Vali-Time`/`Vali-Holiday`/`Vali-Range` at their published versions — doesn't need any of their new members
- Internal cleanup (no observable behavior change): extracted `OccurrenceSkipResolver` (SkipWeekends/SkipHolidays resolution) out of `ValiSchedule`; deduplicated the resolve→budget→dedup→emit sequence shared by `Occurrences`/`OccurrencesInRange` into a single `TryResolveEmission` helper; unified the "Weekly without days" validation between `Build()` and direct query calls into `EnsureWeeklyConfigured()`; removed the dead, `[Obsolete]`, unused `CountOccurrencesBetween`; named the `OccurrencesInRange` scan ceiling as `MaxRangeScanDays`

## Vali-Bucket — 1.0.0 (new package)

### Added
- `IValiBucket.Bucketize`/`Aggregate<T>`/`GetBucketFor` — fixed-size, anchor-aligned time-series bucketing for reporting/analytics, reusing `Vali-Range`'s `DateRange`

## Vali-BusinessHours — 1.0.0 (new package)

### Added
- `IValiBusinessHours.Overlap`/`OverlapOnWorkday` — cross-timezone business-hours overlap, DST-correct and midnight-crossing aware, composing `Vali-Calendar` and `Vali-TimeZone` without coupling them to each other

## Vali-Tempo — 1.1.0

### Added
- Registers `Vali-BusinessHours` and `Vali-Bucket` in `AddValiTempo()` (temporarily via `ProjectReference` — neither is published yet)

### Changed
- All other module `PackageReference` pins stay at their currently-published versions; `AddValiTempo()` only forwards to each module's own `AddValiX()`, unaffected by this release's new members. Pins will bump in follow-up releases as each module republishes.

---

## [1.0.0] - 2025-07-01

### Added — Vali-Time
- `ValiTime`: time conversion across 8 units (Milliseconds, Seconds, Minutes, Hours, Days, Weeks, Months, Years)
- `ValiTime.Convert`, `SumTimes`, `SubtractTimes` (with `allowNegative` support), `FormatTime`, `GetBestUnit`, `ParseTime`, `TryConvert`, `TryParseTime`, `Clamp`, `Compare`, `MultiConvert`, `Breakdown`, `FromTimeSpan`
- `ValiDate`: date arithmetic, period boundaries (StartOf/EndOf), validations, full Quarter API (15+ methods)
- `IValiTime` / `IValiDate` interfaces for DI
- `AddValiTime()` DI extension

### Added — Vali-Range
- `DateRange` readonly struct with `Duration(TimeUnit)`, `Contains`, `IsValid`
- `ValiRange`: Create, LastUnits, NextUnits, ThisMonth/Week/Quarter/Year, Contains, Overlaps, IsContainedBy, Intersection, Union, Expand, Shrink, Shift, IsAdjacent, Merge, Gaps, EachDay, EachWeek, EachMonth, EachWorkday, SplitByMonth, SplitByQuarter
- `AddValiRange()` DI extension

### Added — Vali-Calendar
- `CalendarWeek` readonly struct (WeekNumber, Year, Start, End)
- `ValiCalendar`: IsWorkday, WorkdaysBetween, AddWorkdays, WeekOf, WeeksInMonth, WorkdaysInMonth, WorkdaysInYear, NextWorkday, PreviousWorkday, HolidaysInMonth, HolidaysInYear
- Optional `IHolidayProvider` integration for holiday-aware workday calculations
- `AddValiCalendar()` DI extension

### Added — Vali-Duration
- `ValiDuration` readonly struct with `decimal` precision (no floating-point drift)
- Factory methods: FromMilliseconds, FromSeconds, FromMinutes, FromHours, FromDays, FromWeeks
- Arithmetic operators (+, -, *, /), comparison operators, IEquatable, IComparable
- Implicit conversion to/from `TimeSpan`
- `Format()` and `As(TimeUnit)` conversion

### Added — Vali-CountDown
- `ValiCountdown`: IsExpired, TimeUntil, TimeElapsed, Progress, ProgressPercent, Breakdown, Format, IsWithin
- All methods have both `DateTime` and `reference` overloads
- `AddValiCountdown()` DI extension

### Added — Vali-Age
- `AgeResult` readonly struct (Years, Months, Days, TotalDays)
- `ValiAge`: Years, Exact, Format, Relative, IsAtLeast, IsBirthday, NextBirthday, DaysUntilBirthday
- Feb 29 birthday handling: convention uses Feb 28 in non-leap years
- `AddValiAge()` DI extension

### Added — Vali-Schedule
- `RecurrenceType` enum: Daily, Weekly, Monthly, Yearly, Custom
- `ValiSchedule` fluent builder: Every, On, At, StartingFrom, EndsAfter, EndsOn, WithCustomPredicate
- NextOccurrence, PreviousOccurrence, OccursOn, Occurrences, OccurrencesInRange
- `AddValiSchedule()` DI extension

### Added — Vali-Holiday
- `HolidayInfo` model with multilingual names (es, en, pt, fr, de)
- `IHolidayProvider` / `BaseHolidayProvider` (Template Method pattern)
- `EasterCalculator`: Gaussian algorithm covering Easter, GoodFriday, HolyThursday, HolySaturday, AshWednesday, Carnival, Ascension, Pentecost, CorpusChristi
- Holiday providers for 35+ countries across LATAM and Europe
- `HolidayProviderFactory`: CreateAll(), CreateLatinAmerica(), CreateEurope()
- `ValiHoliday`: Register, For, Supports, IsHoliday, GetHolidays, GetNextHolidayWithYear, GetPreviousHolidayWithYear, IsLongWeekend, HolidaysThisMonth
- O(1) holiday lookup via per-year HashSet cache
- `AddValiHoliday()` DI extension

### Added — Vali-TimeZone
- `ValiZoneInfo` model (Id, DisplayName, StandardName, BaseOffset, SupportsDst, CountryCode, CountryName)
- Curated dataset of 45+ IANA-compatible timezone entries
- `ValiTimeZone`: Convert, ConvertOffset, ToUtc, FromUtc, ToDateTimeOffset, GetOffset, GetBaseOffset, IsDst, OffsetDiff, FindZone, AllZones, ZonesForCountry, IsValidZone, Now, Today, IsSameInstant, FormatWithZone
- IANA → Windows StandardName fallback for cross-platform compatibility
- `AddValiTimeZone()` DI extension

### Added — Vali-Tempo
- Meta-package referencing all Vali-Tempo ecosystem packages
- `AddValiTempo()` registers all services in one call

### Fixed
- `SubtractTimes` with `allowNegative: true` now correctly returns negative values (was throwing `ArgumentException`)
- `NextBirthday` / `DaysUntilBirthday` crash for Feb 29 birthdays in non-leap years
- `EndOf` now uses `TimeSpan.FromTicks(TimeSpan.TicksPerDay - 1)` for full tick precision
- `catch` in `TryConvert`/`TryParseTime` narrowed to specific exception types
- `GetNextHoliday`/`GetPreviousHoliday` replaced with year-aware variants

### Performance
- `BaseHolidayProvider.IsHoliday` now uses a per-year `HashSet` cache (O(1) lookup vs O(n) LINQ scan)
- `ValiCalendar.WeeksInMonth` uses a `static readonly GregorianCalendar` instance (eliminates up to 31 allocations per call)
- `ValiSchedule.Occurrences`/`OccurrencesInRange` use an incremental counter instead of O(n²) `CountOccurrencesBetween`
- `ValiRange.Merge` uses `Count == 0` instead of `Any()` on a materialized list

[Unreleased]: https://github.com/UBF21/Vali-Tempo/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/UBF21/Vali-Tempo/releases/tag/v1.0.0
