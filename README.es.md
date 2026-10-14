# Vali-Tempo

[English](README.md) | **Español**

[![NuGet](https://img.shields.io/nuget/v/Vali-Tempo.svg)](https://www.nuget.org/packages/Vali-Tempo)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-6%20%7C%207%20%7C%208%20%7C%209%20%7C%2010-purple.svg)](https://dotnet.microsoft.com)

> Un ecosistema modular de utilidades .NET para tiempo, fechas, zonas horarias, calendarios y más — construido para precisión y composabilidad.

## Índice de paquetes

| Paquete | Descripción | Carpeta |
|---------|-------------|--------|
| [Vali-Time](./Vali-Time) | Conversión de unidades de tiempo, aritmética de fechas y utilidades de trimestre | [Vali-Time/](./Vali-Time) |
| [Vali-Range](./Vali-Range) | Creación, consulta, iteración y operaciones de conjuntos sobre rangos de fechas | [Vali-Range/](./Vali-Range) |
| [Vali-Calendar](./Vali-Calendar) | Cálculo de días hábiles, semanas de calendario e integración de feriados | [Vali-Calendar/](./Vali-Calendar) |
| [Vali-Duration](./Vali-Duration) | Value type inmutable de duración con operadores aritméticos | [Vali-Duration/](./Vali-Duration) |
| [Vali-CountDown](./Vali-CountDown) | Temporizadores de cuenta regresiva y seguimiento de plazos | [Vali-CountDown/](./Vali-CountDown) |
| [Vali-Age](./Vali-Age) | Cálculo de edad a partir de fechas de nacimiento con control de precisión | [Vali-Age/](./Vali-Age) |
| [Vali-Schedule](./Vali-Schedule) | Definición de horarios recurrentes y generación de ocurrencias | [Vali-Schedule/](./Vali-Schedule) |
| [Vali-Holiday](./Vali-Holiday) | Implementaciones de proveedores de feriados (35+ países) | [Vali-Holiday/](./Vali-Holiday) |
| [Vali-TimeZone](./Vali-TimeZone) | Conversión y descubrimiento de zonas horarias | [Vali-TimeZone/](./Vali-TimeZone) |
| [Vali-BusinessHours](./Vali-BusinessHours) | Solapamiento de horario laboral entre zonas horarias (correcto con DST, cruces de medianoche) | [Vali-BusinessHours/](./Vali-BusinessHours) |
| [Vali-Bucket](./Vali-Bucket) | Agrupación y agregación de series temporales en buckets de tamaño fijo y alineados a un ancla | [Vali-Bucket/](./Vali-Bucket) |
| **Vali-Tempo** | Meta-paquete — instala todo el ecosistema Vali-Tempo | [Vali-Tempo/](./Vali-Tempo) |

## Instalación

```bash
# Meta-paquete (incluye todo)
dotnet add package Vali-Tempo

# O instalá solo lo que necesitás:
dotnet add package Vali-Time
dotnet add package Vali-Range
dotnet add package Vali-Calendar
dotnet add package Vali-Duration
dotnet add package Vali-CountDown
dotnet add package Vali-Age
dotnet add package Vali-Schedule
dotnet add package Vali-Holiday
dotnet add package Vali-TimeZone
dotnet add package Vali-BusinessHours
dotnet add package Vali-Bucket
```

## Inyección de dependencias

Registrá todos los paquetes con una sola llamada usando el meta-paquete:

```csharp
// Program.cs
builder.Services.AddValiTempo();
```

Esto registra todos los servicios con ciclo de vida singleton:
- `IValiTime` → `ValiTime`
- `IValiDate` → `ValiDate`
- `IValiRange` → `ValiRange`
- `IValiCalendar` → `ValiCalendar`
- `IValiCountdown` → `ValiCountdown`
- `IValiAge` → `ValiAge`
- `IValiSchedule` → `ValiSchedule`
- `IValiHoliday` → `ValiHoliday`
- `IValiTimeZone` → `ValiTimeZone`
- `IValiBusinessHours` → `ValiBusinessHours`
- `IValiBucket` → `ValiBucket`

## Ejemplo rápido — usando varios módulos juntos

```csharp
using Vali_Time.Core;
using Vali_Time.Enums;
using Vali_Range.Core;
using Vali_Range.Models;
using Vali_Calendar.Core;
using Vali_Duration.Models;
using Vali_Holiday.Core;
using Vali_TimeZone.Core;

// --- Vali-Time: convertir y formatear una duración ---
var valiTime = new ValiTime();
decimal hours = valiTime.Convert(7200m, TimeUnit.Seconds, TimeUnit.Hours);
string label  = valiTime.FormatTime(hours, TimeUnit.Hours); // "2.00 h"

// --- Vali-Duration: medir tiempo transcurrido desde un Stopwatch ---
var stopwatch = System.Diagnostics.Stopwatch.StartNew();
// ... trabajo ...
stopwatch.Stop();
ValiDuration elapsed = ValiDuration.FromTimeSpan(stopwatch.Elapsed);
Console.WriteLine(elapsed.Format()); // ej. "342.00 ms"

// --- Vali-Range: definir una ventana de reporte ---
var range = new ValiRange();
DateRange thisQuarter = range.ThisQuarter();
Console.WriteLine(thisQuarter); // "2025-01-01 → 2025-03-31"

// --- Vali-Calendar: calcular la fecha de entrega en 5 días hábiles ---
var calendar = new ValiCalendar();
DateTime orderDate    = new DateTime(2025, 3, 20);
DateTime deliveryDate = calendar.AddWorkdays(orderDate, 5);
Console.WriteLine(deliveryDate.ToString("yyyy-MM-dd")); // "2025-03-27"

// --- Vali-TimeZone: obtener la hora actual en una zona horaria específica ---
var timeZone = new ValiTimeZone();
DateTime nowInLima = timeZone.Now("America/Lima");
Console.WriteLine(timeZone.FormatWithZone(nowInLima, "America/Lima"));

// --- Vali-Holiday: verificar si hoy es feriado en Perú ---
var holiday = new ValiHoliday();
bool isHoliday = holiday.IsHoliday(nowInLima, "PE");
Console.WriteLine($"Feriado en Perú: {isHoliday}");

// --- Combinado: contar días hábiles dentro del rango del trimestre ---
int workdaysInQuarter = calendar.WorkdaysBetween(thisQuarter.Start, thisQuarter.End);
Console.WriteLine($"Días hábiles este trimestre: {workdaysInQuarter}");
```

## Arquitectura del ecosistema

```
Vali-Tempo (meta-paquete)
│
├── Vali-Time        — IValiTime, IValiDate          (base)
├── Vali-Range       — IValiRange, DateRange
├── Vali-Calendar    — IValiCalendar, CalendarWeek
├── Vali-Duration    — ValiDuration (struct)
├── Vali-CountDown   — IValiCountdown
├── Vali-Age         — IValiAge, AgeResult
├── Vali-Schedule    — IValiSchedule, ScheduleConfig
├── Vali-Holiday     — IValiHoliday, IHolidayProvider (35+ países)
├── Vali-TimeZone    — IValiTimeZone, ValiZoneInfo
├── Vali-BusinessHours — IValiBusinessHours (compone Vali-Calendar + Vali-TimeZone)
└── Vali-Bucket      — IValiBucket, TimeBucket
```

- **Vali-Time** es la base: enum `TimeUnit`, `ValiTime` (conversión de unidades), `ValiDate` (aritmética de fechas).
- **Vali-Range** se apoya en `TimeUnit` para ofrecer `DateRange` y `ValiRange` con operaciones de teoría de conjuntos.
- **Vali-Calendar** usa las convenciones de `ValiDate` y acepta un `IHolidayProvider` (de **Vali-Holiday**) para que la lógica de días hábiles sea consciente del país.
- **Vali-Duration** es un value type independiente que usa `TimeUnit` para su método de conversión `As()`.
- **Vali-TimeZone** provee resolución, conversión y formateo de zonas horarias compatibles con IANA.
- **Vali-Tempo** es un meta-paquete de conveniencia que incluye todo lo anterior.

## Compatibilidad

Todos los paquetes del ecosistema Vali-Tempo se compilan contra **net8.0**, **net9.0** y **net10.0**.
Gracias a la resolución de compatibilidad hacia atrás integrada en NuGet, también son totalmente
usables desde **cualquier proyecto que apunte a .NET 6 o .NET 7** — NuGet selecciona automáticamente
el target framework disponible más cercano sin necesidad de un build separado.

| Paquete | .NET 6 | .NET 7 | .NET 8 | .NET 9 | .NET 10 |
|---------|:------:|:------:|:------:|:------:|:-------:|
| Vali-Time | ✅ | ✅ | ✅ | ✅ | ✅ |
| Vali-Range | ✅ | ✅ | ✅ | ✅ | ✅ |
| Vali-Calendar | ✅ | ✅ | ✅ | ✅ | ✅ |
| Vali-Duration | ✅ | ✅ | ✅ | ✅ | ✅ |
| Vali-CountDown | ✅ | ✅ | ✅ | ✅ | ✅ |
| Vali-Age | ✅ | ✅ | ✅ | ✅ | ✅ |
| Vali-Schedule | ✅ | ✅ | ✅ | ✅ | ✅ |
| Vali-Holiday | ✅ | ✅ | ✅ | ✅ | ✅ |
| Vali-TimeZone | ✅ | ✅ | ✅ | ✅ | ✅ |
| Vali-BusinessHours | ✅ | ✅ | ✅ | ✅ | ✅ |
| Vali-Bucket | ✅ | ✅ | ✅ | ✅ | ✅ |
| Vali-Tempo | ✅ | ✅ | ✅ | ✅ | ✅ |

> **Nota:** .NET 6 y .NET 7 llegaron a su fin de vida en noviembre de 2024 y mayo de 2024 respectivamente.
> Si bien estos runtimes ya no reciben actualizaciones de seguridad de Microsoft, los paquetes
> siguen siendo compatibles para equipos que todavía no migraron. Para proyectos nuevos, se recomienda
> fuertemente .NET 8 (LTS) o .NET 10 (LTS).

### Referencia de versiones de .NET

| Versión | Tipo | Fin de soporte |
|---------|------|-----------------|
| .NET 6 | LTS | Noviembre 2024 (EOL) |
| .NET 7 | STS | Mayo 2024 (EOL) |
| .NET 8 | LTS | Noviembre 2026 ✅ |
| .NET 9 | STS | Mayo 2026 ✅ |
| .NET 10 | LTS | Noviembre 2028 ✅ |

## READMEs de paquetes individuales

- [Vali-Time README](./Vali-Time/README.md)
- [Vali-Range README](./Vali-Range/README.md)
- [Vali-Calendar README](./Vali-Calendar/README.md)
- [Vali-Duration README](./Vali-Duration/README.md)
- [Vali-CountDown README](./Vali-CountDown/README.md)
- [Vali-Age README](./Vali-Age/README.md)
- [Vali-Schedule README](./Vali-Schedule/README.md)
- [Vali-Holiday README](./Vali-Holiday/README.md)
- [Vali-TimeZone README](./Vali-TimeZone/README.md)
- [Vali-BusinessHours README](./Vali-BusinessHours/README.md)
- [Vali-Bucket README](./Vali-Bucket/README.md)
- [Vali-Tempo (meta-paquete) README](./Vali-Tempo/README.md)

## Estándares de Desarrollo

> Generado por `/flows/onboard` — 2026-10-07. Fuente completa en [CONTRIBUTING.md](CONTRIBUTING.md); valores de tamaño/complejidad no cubiertos ahí están en `.claude/memory/project-standards.md` (local, no versionado).

- `decimal` siempre para tiempo/duración — nunca `double`/`float`
- `readonly struct` para value types (`DateRange`, `ValiDuration`, `AgeResult`, `CalendarWeek`)
- Cada servicio implementa `IValiX` + expone `AddValiX()`
- Cero dependencias externas por paquete salvo `Microsoft.Extensions.DependencyInjection.Abstractions`
- XML `<summary>` obligatorio en miembros públicos
- Tests con `DateTime` fijo (nunca `.Now`), cubriendo Feb 29/bisiestos/DST/negativos
- Commits: Conventional Commits con scope de paquete (`fix(Vali-Age): ...`)

## Licencia

MIT © 2025 Felipe Rafael Montenegro Morriberon

## Donaciones

Si Vali-Tempo te resulta útil, considerá apoyar su desarrollo:

- **Latinoamérica** — [MercadoPago](https://link.mercadopago.com.pe/felipermm)
- **Internacional** — [PayPal](https://paypal.me/felipeRMM?country.x=PE&locale.x=es_XC)

---
