# Changelog

[English](CHANGELOG.md) | **Español**

Todos los cambios relevantes del ecosistema Vali-Tempo se documentan en este archivo.

El formato está basado en [Keep a Changelog](https://keepachangelog.com/es-ES/1.0.0/),
y este proyecto sigue [Versionado Semántico](https://semver.org/lang/es/spec/v2.0.0.html).

## [No publicado]

---

## Vali-Tempo — 1.1.1

### Cambiado
- Se reemplazó el `ProjectReference` temporal hacia `Vali-BusinessHours`/`Vali-Bucket` por un `PackageReference` fijado, ahora que ambos están publicados. Se actualizó la dependencia de cada módulo a su última versión publicada. Sin cambios de código.

---

## Vali-Time — 1.1.0

### Agregado
- `DurationStringParser` — parser público extraído de la gramática de duración-como-string de `ValiTime`, reutilizado por el `IParsable<ValiDuration>` de `Vali-Duration`
- Enum `HumanizeCulture` (`En`/`Es`), reutilizado por `Humanize()` de `Vali-Age`
- `Constants.IsClampedMonthEnd(originalDay, candidate)` — detección compartida de "fin de mes recortado", reutilizada por `ValiAge.Exact()`

### Cambiado
- `StartOfWeek`/`IsWeekend`/`IsWeekday` se consolidaron en `DateTimeExtensions` como única fuente de verdad (antes duplicados entre paquetes)

## Vali-Range — 1.1.0

### Agregado
- `IValiRange.Subtract(DateRange a, DateRange b)` — diferencia teórica de conjuntos A \ B, completando el trío Intersection/Union/Subtract

### Cambiado
- Se agrega target net10.0. Sigue fijando `Vali-Time` en 1.0.1 — no necesita ninguno de los miembros nuevos de 1.1.0

## Vali-CountDown — 1.0.1

### Cambiado
- Se agrega target net10.0. Sin cambios en la API pública

## Vali-Holiday — 1.1.0

### Agregado
- Sobrecargas multi-país `IValiHoliday.IsHoliday(DateTime, params string[])` (ANY/alguno) e `IsHolidayInAll(DateTime, params string[])` (ALL/todos)
- `HolidayInfo.TryToDateTime(int year, out DateTime result)` — conversión segura `(Mes, Día, año)` que devuelve `false` en vez de lanzar excepción para el 29 de febrero en un año no bisiesto
- Enum `ObservedRule` y `IHolidayProvider.GetObservedDate(HolidayInfo, int)` / `IValiHoliday.GetObservedHoliday(DateTime, string)` — resolución oficial de corrimiento de fin de semana (día "observado"/"sustituto"), implementada para USA (sábado→viernes anterior, domingo→lunes siguiente) y Reino Unido (fin de semana→siguiente día hábil disponible, con manejo coordinado de colisiones entre Navidad y Boxing Day). Ningún otro proveedor declara una regla en esta versión.

### Cambiado
- `IsHoliday(DateTime, params string[])` e `IsHolidayInAll(DateTime, params string[])` ahora lanzan `ArgumentNullException` en vez de `NullReferenceException` cuando se los llama con un array `null`
- `BaseHolidayProvider.GetHolidaysInRange` y `ValiHoliday.GetNextHolidayWithYear`/`GetPreviousHolidayWithYear` ahora comparten una única validación de 29-de-febrero/año-no-bisiesto (vía `HolidayInfo.TryToDateTime`) en vez de tres chequeos escritos de forma independiente
- `HolidayInfo` ahora es un `sealed record` (antes era un `sealed class`) — agrega igualdad estructural; el formato de `ToString()` no cambia. Nota: `Names` se sigue comparando por referencia, ya que los records no comparan colecciones en profundidad
- Se movieron `UsaHolidayProvider.cs`/`CanadaHolidayProvider.cs` a `Providers/NorthAmerica/` y `AustraliaHolidayProvider.cs` a `Providers/Oceania/`, alineándolos con sus namespaces ya declarados (sin cambio de namespace ni de comportamiento)
- Se agrega target net10.0. Sigue fijando `Vali-Time` en 1.0.1 — no necesita ninguno de los miembros nuevos de 1.1.0

## Vali-TimeZone — 1.1.0

### Cambiado
- `ValiZoneInfo` ahora es un `sealed record` (antes era un `sealed class`) — agrega igualdad estructural; el formato de `ToString()` no cambia
- Se agrega target net10.0. Sigue fijando `Vali-Time` en 1.0.1 — no necesita ninguno de los miembros nuevos de 1.1.0

## Vali-Age — 1.1.0

### Agregado
- Formateo de tiempo relativo `Humanize()` (es/en) vía `HumanizeEngine`, construido sobre `HumanizeCulture` de `Vali-Time`

### Cambiado
- Depende temporalmente del código fuente de `Vali-Time` vía `ProjectReference` (ponytail) en vez de `PackageReference`, porque `HumanizeCulture` todavía no existe en el paquete publicado 1.0.1 — se cambia a `PackageReference Version="1.1.0"` una vez que Vali-Time se vuelva a publicar (ver la sección de Proceso de Release en CONTRIBUTING.md)

## Vali-Duration — 1.1.0

### Agregado
- `IParsable<ValiDuration>`/`IFormattable` vía el `DurationStringParser` compartido

### Cambiado
- Depende temporalmente del código fuente de `Vali-Time` vía `ProjectReference` (ponytail) en vez de `PackageReference`, porque `DurationStringParser` todavía no existe en el paquete publicado 1.0.1 — se cambia a `PackageReference Version="1.1.0"` una vez que Vali-Time se vuelva a publicar

## Vali-Calendar — 1.0.1

### Cambiado
- Se agrega target net10.0. Sigue fijando `Vali-Time` en 1.0.1 y `Vali-Holiday` en 1.0.0 — no necesita ninguno de sus miembros nuevos (sin cambios en la API pública)

## Vali-Schedule — 1.1.0

### Agregado
- `IValiSchedule.SkipWeekends()` y `SkipHolidays(IHolidayProvider, HolidaySkipStrategy)` para excluir ocurrencias en fin de semana/feriado, resueltas vía `NextWorkday`/`PreviousWorkday`/`Skip`
- Enum `HolidaySkipStrategy`
- `IValiSchedule.OccurrencePeriods(DateTime from, DateTime to)` — empareja ocurrencias consecutivas en períodos `Vali_Range.Models.DateRange`
- `Vali_Schedule.Cron.CronSchedule` — soporte de expresiones cron de 5 campos sin dependencias (`Parse`/`TryParse`), implementando `IValiSchedule`
- `Vali_Schedule.Cron.CronFormatException` — errores de parseo atribuidos a campo/token

### Cambiado
- Nuevas dependencias entre paquetes: `Vali-Schedule` ahora referencia a `Vali-Holiday` y `Vali-Range` (`PackageReference`, mismo patrón ya usado por `Vali-Calendar`). Solo `SkipHolidays()` requiere un `IHolidayProvider` concreto; `SkipWeekends()` no tiene ese requisito.
- Se agrega target net10.0. Sigue fijando `Vali-Time`/`Vali-Holiday`/`Vali-Range` en sus versiones publicadas — no necesita ninguno de sus miembros nuevos
- Limpieza interna (sin cambio de comportamiento observable): se extrajo `OccurrenceSkipResolver` (resolución de SkipWeekends/SkipHolidays) fuera de `ValiSchedule`; se unificó la secuencia resolver→presupuesto→deduplicar→emitir compartida por `Occurrences`/`OccurrencesInRange` en un único helper `TryResolveEmission`; se unificó la validación de "Semanal sin días" entre `Build()` y las llamadas de consulta directa en `EnsureWeeklyConfigured()`; se eliminó el método muerto, `[Obsolete]` y sin uso `CountOccurrencesBetween`; se nombró el límite de escaneo de `OccurrencesInRange` como `MaxRangeScanDays`

## Vali-Bucket — 1.0.0 (paquete nuevo)

### Agregado
- `IValiBucket.Bucketize`/`Aggregate<T>`/`GetBucketFor` — agrupación en series temporales de tamaño fijo y alineadas a un ancla para reportes/análisis, reutilizando `DateRange` de `Vali-Range`

## Vali-BusinessHours — 1.0.0 (paquete nuevo)

### Agregado
- `IValiBusinessHours.Overlap`/`OverlapOnWorkday` — solapamiento de horario laboral entre zonas horarias, correcto con DST y consciente de cruces de medianoche, componiendo `Vali-Calendar` y `Vali-TimeZone` sin acoplarlos entre sí

## Vali-Tempo — 1.1.0

### Agregado
- Registra `Vali-BusinessHours` y `Vali-Bucket` en `AddValiTempo()` (temporalmente vía `ProjectReference` — ninguno de los dos estaba publicado todavía)

### Cambiado
- El resto de los pins `PackageReference` de los módulos se mantienen en sus versiones publicadas actuales; `AddValiTempo()` solo delega en el `AddValiX()` propio de cada módulo, sin verse afectado por los miembros nuevos de esta versión. Los pins se actualizarán en versiones posteriores a medida que cada módulo se vuelva a publicar.

---

## [1.0.0] - 2025-07-01

### Agregado — Vali-Time
- `ValiTime`: conversión de tiempo entre 8 unidades (Milisegundos, Segundos, Minutos, Horas, Días, Semanas, Meses, Años)
- `ValiTime.Convert`, `SumTimes`, `SubtractTimes` (con soporte de `allowNegative`), `FormatTime`, `GetBestUnit`, `ParseTime`, `TryConvert`, `TryParseTime`, `Clamp`, `Compare`, `MultiConvert`, `Breakdown`, `FromTimeSpan`
- `ValiDate`: aritmética de fechas, límites de período (StartOf/EndOf), validaciones, API completa de Trimestres (15+ métodos)
- Interfaces `IValiTime` / `IValiDate` para DI
- Extensión de DI `AddValiTime()`

### Agregado — Vali-Range
- Struct de solo lectura `DateRange` con `Duration(TimeUnit)`, `Contains`, `IsValid`
- `ValiRange`: Create, LastUnits, NextUnits, ThisMonth/Week/Quarter/Year, Contains, Overlaps, IsContainedBy, Intersection, Union, Expand, Shrink, Shift, IsAdjacent, Merge, Gaps, EachDay, EachWeek, EachMonth, EachWorkday, SplitByMonth, SplitByQuarter
- Extensión de DI `AddValiRange()`

### Agregado — Vali-Calendar
- Struct de solo lectura `CalendarWeek` (WeekNumber, Year, Start, End)
- `ValiCalendar`: IsWorkday, WorkdaysBetween, AddWorkdays, WeekOf, WeeksInMonth, WorkdaysInMonth, WorkdaysInYear, NextWorkday, PreviousWorkday, HolidaysInMonth, HolidaysInYear
- Integración opcional con `IHolidayProvider` para cálculos de días hábiles conscientes de feriados
- Extensión de DI `AddValiCalendar()`

### Agregado — Vali-Duration
- Struct de solo lectura `ValiDuration` con precisión `decimal` (sin deriva de punto flotante)
- Métodos de fábrica: FromMilliseconds, FromSeconds, FromMinutes, FromHours, FromDays, FromWeeks
- Operadores aritméticos (+, -, *, /), operadores de comparación, IEquatable, IComparable
- Conversión implícita hacia/desde `TimeSpan`
- Conversión `Format()` y `As(TimeUnit)`

### Agregado — Vali-CountDown
- `ValiCountdown`: IsExpired, TimeUntil, TimeElapsed, Progress, ProgressPercent, Breakdown, Format, IsWithin
- Todos los métodos tienen sobrecargas con `DateTime` y con `reference`
- Extensión de DI `AddValiCountdown()`

### Agregado — Vali-Age
- Struct de solo lectura `AgeResult` (Years, Months, Days, TotalDays)
- `ValiAge`: Years, Exact, Format, Relative, IsAtLeast, IsBirthday, NextBirthday, DaysUntilBirthday
- Manejo de cumpleaños 29 de febrero: la convención usa el 28 de febrero en años no bisiestos
- Extensión de DI `AddValiAge()`

### Agregado — Vali-Schedule
- Enum `RecurrenceType`: Daily, Weekly, Monthly, Yearly, Custom
- Builder fluido `ValiSchedule`: Every, On, At, StartingFrom, EndsAfter, EndsOn, WithCustomPredicate
- NextOccurrence, PreviousOccurrence, OccursOn, Occurrences, OccurrencesInRange
- Extensión de DI `AddValiSchedule()`

### Agregado — Vali-Holiday
- Modelo `HolidayInfo` con nombres multilingües (es, en, pt, fr, de)
- `IHolidayProvider` / `BaseHolidayProvider` (patrón Template Method)
- `EasterCalculator`: algoritmo Gaussiano que cubre Pascua, Viernes Santo, Jueves Santo, Sábado Santo, Miércoles de Ceniza, Carnaval, Ascensión, Pentecostés, Corpus Christi
- Proveedores de feriados para 35+ países de LATAM y Europa
- `HolidayProviderFactory`: CreateAll(), CreateLatinAmerica(), CreateEurope()
- `ValiHoliday`: Register, For, Supports, IsHoliday, GetHolidays, GetNextHolidayWithYear, GetPreviousHolidayWithYear, IsLongWeekend, HolidaysThisMonth
- Búsqueda de feriados O(1) vía caché HashSet por año
- Extensión de DI `AddValiHoliday()`

### Agregado — Vali-TimeZone
- Modelo `ValiZoneInfo` (Id, DisplayName, StandardName, BaseOffset, SupportsDst, CountryCode, CountryName)
- Dataset curado de 45+ entradas de zonas horarias compatibles con IANA
- `ValiTimeZone`: Convert, ConvertOffset, ToUtc, FromUtc, ToDateTimeOffset, GetOffset, GetBaseOffset, IsDst, OffsetDiff, FindZone, AllZones, ZonesForCountry, IsValidZone, Now, Today, IsSameInstant, FormatWithZone
- Fallback de IANA → StandardName de Windows para compatibilidad multiplataforma
- Extensión de DI `AddValiTimeZone()`

### Agregado — Vali-Tempo
- Meta-paquete que referencia todos los paquetes del ecosistema Vali-Tempo
- `AddValiTempo()` registra todos los servicios en una sola llamada

### Corregido
- `SubtractTimes` con `allowNegative: true` ahora devuelve correctamente valores negativos (antes lanzaba `ArgumentException`)
- Se corrige el crash de `NextBirthday` / `DaysUntilBirthday` para cumpleaños 29 de febrero en años no bisiestos
- `EndOf` ahora usa `TimeSpan.FromTicks(TimeSpan.TicksPerDay - 1)` para precisión completa de ticks
- El `catch` en `TryConvert`/`TryParseTime` se acotó a tipos de excepción específicos
- `GetNextHoliday`/`GetPreviousHoliday` se reemplazaron por variantes conscientes del año

### Rendimiento
- `BaseHolidayProvider.IsHoliday` ahora usa una caché `HashSet` por año (búsqueda O(1) en vez de escaneo LINQ O(n))
- `ValiCalendar.WeeksInMonth` usa una instancia `static readonly GregorianCalendar` (elimina hasta 31 allocations por llamada)
- `ValiSchedule.Occurrences`/`OccurrencesInRange` usan un contador incremental en vez de `CountOccurrencesBetween` O(n²)
- `ValiRange.Merge` usa `Count == 0` en vez de `Any()` sobre una lista materializada

[No publicado]: https://github.com/UBF21/Vali-Tempo/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/UBF21/Vali-Tempo/releases/tag/v1.0.0
