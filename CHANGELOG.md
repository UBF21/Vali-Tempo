# Changelog

All notable changes to the Vali-Tempo ecosystem will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

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
