using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.LatinAmerica;

/// <summary>
/// Holiday provider for Mexico, based on the Ley Federal del Trabajo (LFT),
/// Artículo 74, which defines the mandatory non-working days. Additionally
/// includes widely observed but non-mandatory dates classified as
/// <see cref="HolidayType.Observance"/>.
/// </summary>
/// <remarks>
/// The LFT mandates exactly seven holidays per year. Three of them use a
/// "lunes más cercano" (nearest Monday) rule:
/// <list type="bullet">
///   <item><description>February 5 — observed on the first Monday of February.</description></item>
///   <item><description>March 21 — observed on the third Monday of March.</description></item>
///   <item><description>November 20 — observed on the third Monday of November.</description></item>
/// </list>
/// This library stores the canonical historical dates (Feb 5, Mar 21, Nov 20).
/// Callers should compute the nearest Monday for each year when exact observance
/// dates are required.
/// <para>
/// Non-mandatory observances (Valentine's Day, Mother's Day, Día de Muertos) are
/// included for completeness with <see cref="HolidayType.Observance"/> so they can
/// be easily filtered out.
/// </para>
/// </remarks>
public class MexicoHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "MX";

    /// <inheritdoc/>
    public override string CountryName => "Mexico";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "es";

    /// <inheritdoc/>
    /// <remarks>
    /// Returns the seven mandatory holidays defined by Artículo 74 of the
    /// Ley Federal del Trabajo, plus culturally significant observances.
    /// Mandatory holidays are typed <see cref="HolidayType.National"/> or
    /// <see cref="HolidayType.Civic"/>; observances are typed
    /// <see cref="HolidayType.Observance"/>.
    /// </remarks>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays() => new[]
    {
        // ── Mandatory holidays (LFT Art. 74) ─────────────────────────────────

        new HolidayInfo(
            "mx_new_year", 1, 1, "MX",
            "Año Nuevo",
            Names("Año Nuevo", "New Year's Day", "Ano Novo", "Jour de l'An", "Neujahr"),
            HolidayType.National,
            description: "Celebración del inicio del año calendario. Fecha fija obligatoria por el Artículo 74 de la LFT."),

        new HolidayInfo(
            "mx_labor_day", 5, 1, "MX",
            "Día del Trabajo",
            Names("Día del Trabajo", "Labour Day", "Dia do Trabalho", "Fête du Travail", "Tag der Arbeit"),
            HolidayType.National,
            description: "Conmemoración internacional de los derechos de los trabajadores. Fecha fija obligatoria por el Artículo 74 de la LFT."),

        new HolidayInfo(
            "mx_independence_day", 9, 16, "MX",
            "Día de la Independencia",
            Names("Día de la Independencia", "Independence Day", "Dia da Independência", "Fête de l'Indépendance", "Unabhängigkeitstag"),
            HolidayType.Civic,
            description: "Conmemoración del Grito de Independencia dado por Miguel Hidalgo y Costilla la madrugada del 16 de septiembre de 1810 en Dolores, Guanajuato, que inició la Guerra de Independencia de México. Fecha fija obligatoria."),

        new HolidayInfo(
            "mx_christmas", 12, 25, "MX",
            "Navidad",
            Names("Navidad", "Christmas Day", "Natal", "Noël", "Weihnachten"),
            HolidayType.National,
            description: "Celebración del nacimiento de Jesucristo. Fecha fija obligatoria por el Artículo 74 de la LFT."),

        // ── Cultural observances (non-mandatory) ─────────────────────────────

        new HolidayInfo(
            "mx_valentines_day", 2, 14, "MX",
            "Día de San Valentín",
            Names("Día de San Valentín", "Valentine's Day", "Dia dos Namorados", "Saint-Valentin", "Valentinstag"),
            HolidayType.Observance,
            description: "Celebración del amor y la amistad. No es feriado obligatorio según la LFT, pero es ampliamente observada a nivel cultural y comercial en México."),

        new HolidayInfo(
            "mx_mothers_day", 5, 10, "MX",
            "Día de las Madres",
            Names("Día de las Madres", "Mother's Day", "Dia das Mães", "Fête des Mères", "Muttertag"),
            HolidayType.Observance,
            description: "Celebración en honor a las madres mexicanas. A diferencia de otros países, en México se observa siempre el 10 de mayo (fecha fija), no en un lunes móvil. No es feriado obligatorio según la LFT."),

        new HolidayInfo(
            "mx_dia_muertos_1", 11, 1, "MX",
            "Día de Todos los Santos / Día de Muertos (niños)",
            Names("Día de Todos los Santos / Día de Muertos (niños)", "All Saints' Day / Day of the Dead (children)", "Dia de Todos os Santos / Dia dos Mortos (crianças)", "Toussaint / Jour des Morts (enfants)", "Allerheiligen / Día de Muertos (Kinder)"),
            HolidayType.Observance,
            description: "Primera jornada de la celebración del Día de Muertos, dedicada a los niños fallecidos. Declarada Patrimonio Cultural Inmaterial de la Humanidad por la UNESCO en 2008. No es feriado obligatorio según la LFT."),

        new HolidayInfo(
            "mx_dia_muertos_2", 11, 2, "MX",
            "Día de Muertos",
            Names("Día de Muertos", "Day of the Dead", "Dia dos Mortos", "Jour des Morts", "Tag der Toten"),
            HolidayType.Observance,
            description: "Segunda jornada de la celebración del Día de Muertos, dedicada a los adultos fallecidos. Tradición precolombina fusionada con el catolicismo que honra a los difuntos con ofrendas y visitas al cementerio. No es feriado obligatorio según la LFT.")
    };

    /// <inheritdoc/>
    /// <remarks>
    /// Returns the three floating-Monday mandatory holidays under Artículo 74 of the
    /// Ley Federal del Trabajo: Constitution Day (first Monday of February), Juárez
    /// Birthday (third Monday of March), and Revolution Day (third Monday of November).
    /// </remarks>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        var constitutionDay = NthMonday(year, 2, 1);
        var juarezBirthday  = NthMonday(year, 3, 3);
        var revolutionDay   = NthMonday(year, 11, 3);

        return new[]
        {
            new HolidayInfo(
                "mx_constitution_day", constitutionDay.Month, constitutionDay.Day, "MX",
                "Día de la Constitución",
                Names("Día de la Constitución", "Constitution Day", "Dia da Constituição", "Jour de la Constitution", "Tag der Verfassung"),
                HolidayType.Civic, isMovable: true,
                description: "Conmemoración de la promulgación de la Constitución Política de los Estados Unidos Mexicanos del 5 de febrero de 1917. Se observa el primer lunes de febrero (regla de lunes más cercano, LFT Art. 74)."),

            new HolidayInfo(
                "mx_juarez_birthday", juarezBirthday.Month, juarezBirthday.Day, "MX",
                "Natalicio de Benito Juárez",
                Names("Natalicio de Benito Juárez", "Benito Juárez's Birthday", "Aniversário de Benito Juárez", "Anniversaire de Benito Juárez", "Geburtstag von Benito Juárez"),
                HolidayType.Civic, isMovable: true,
                description: "Conmemoración del natalicio del Benemérito de las Américas, Benito Pablo Juárez García, nacido el 21 de marzo de 1806 en Guelatao, Oaxaca. Se observa el tercer lunes de marzo (regla de lunes más cercano, LFT Art. 74)."),

            new HolidayInfo(
                "mx_revolution_day", revolutionDay.Month, revolutionDay.Day, "MX",
                "Día de la Revolución Mexicana",
                Names("Día de la Revolución Mexicana", "Revolution Day", "Dia da Revolução Mexicana", "Jour de la Révolution Mexicaine", "Tag der Mexikanischen Revolution"),
                HolidayType.Civic, isMovable: true,
                description: "Conmemoración del inicio de la Revolución Mexicana el 20 de noviembre de 1910, encabezada por Francisco I. Madero contra la dictadura de Porfirio Díaz. Se observa el tercer lunes de noviembre (regla de lunes más cercano, LFT Art. 74)."),
        };
    }

    private static DateTime NthMonday(int year, int month, int n)
    {
        var d = new DateTime(year, month, 1);
        int diff = ((int)DayOfWeek.Monday - (int)d.DayOfWeek + 7) % 7;
        return d.AddDays(diff + (n - 1) * 7);
    }
}
