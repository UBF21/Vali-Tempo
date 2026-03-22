using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.Europe;

/// <summary>
/// Holiday provider for Norway (Norge), covering all 12 national public holidays
/// (helligdager og offentlige høytidsdager) as defined by the Norwegian Public Holidays Act
/// (Lov om helligdager og helligdagsfred av 24. februar 1995). Norway observes both civic
/// and Christian holidays. Grunnlovsdag (17 May) celebrates Constitution Day — the date in
/// 1814 when Norway's Constitution was signed at Eidsvoll, making it one of the oldest
/// constitutions in the world. The day is celebrated with children's parades
/// (barnetoget) and national dress (bunad) throughout the country.
/// </summary>
public class NorwayHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "NO";

    /// <inheritdoc/>
    public override string CountryName => "Norway";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "no";

    /// <summary>
    /// Returns the 5 fixed national holidays of Norway.
    /// Easter-based movable holidays are returned by <see cref="GetMovableHolidays"/>.
    /// </summary>
    /// <returns>A sequence of fixed <see cref="HolidayInfo"/> entries for Norway.</returns>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays() => new[]
    {
        // ── National fixed holidays ───────────────────────────────────────────────

        new HolidayInfo(
            "no_new_year", 1, 1, "NO",
            "Nyttårsdag",
            NamesNo("Año Nuevo", "New Year's Day", "Ano Novo", "Jour de l'An", "Neujahrstag", "Nyttårsdag"),
            HolidayType.National,
            description: "Første dag i det nye året / First day of the new calendar year."),

        new HolidayInfo(
            "no_labor_day", 5, 1, "NO",
            "Arbeidernes dag",
            NamesNo("Día del Trabajo", "International Workers' Day", "Dia do Trabalho", "Fête du Travail", "Tag der Arbeit", "Arbeidernes dag"),
            HolidayType.National,
            description: "Internasjonale arbeiderdag / International Workers' Day. Markerer arbeiderbevegelsens kamp for rettigheter."),

        new HolidayInfo(
            "no_constitution_day", 5, 17, "NO",
            "Grunnlovsdag",
            NamesNo("Día de la Constitución de Noruega", "Norwegian Constitution Day", "Dia da Constituição da Noruega", "Fête de la Constitution norvégienne", "Norwegischer Verfassungstag", "Grunnlovsdag"),
            HolidayType.Civic,
            description: "Nasjonaldagen / Syttende mai. Feiring av Grunnlovens undertegnelse på Eidsvoll 17. mai 1814 — en av de eldste grunnlovene i verden. Feiret med barnetog, bunad og 17. mai-taler i hele landet."),

        new HolidayInfo(
            "no_christmas_1", 12, 25, "NO",
            "Første juledag",
            NamesNo("Navidad (primer día)", "Christmas Day", "Natal (primeiro dia)", "Noël (premier jour)", "Erster Weihnachtstag", "Første juledag"),
            HolidayType.National,
            description: "Første juledag / Christmas Day. Feiring av Jesu Kristi fødsel."),

        new HolidayInfo(
            "no_christmas_2", 12, 26, "NO",
            "Andre juledag",
            NamesNo("Navidad (segundo día)", "Boxing Day / Second Christmas Day", "Natal (segundo dia)", "Lendemain de Noël", "Zweiter Weihnachtstag", "Andre juledag"),
            HolidayType.National,
            description: "Andre juledag / Second Christmas Day / St. Stephen's Day. Dagen etter første juledag."),
    };

    /// <summary>
    /// Returns the 7 Easter-based movable national holidays of Norway for the given <paramref name="year"/>:
    /// Skjærtorsdag (Holy Thursday), Langfredag (Good Friday), Påskedag (Easter Sunday),
    /// Andre påskedag (Easter Monday), Kristi Himmelfartsdag (Ascension),
    /// Pinsedag (Pentecost Sunday), and Andre pinsedag (Whit Monday).
    /// </summary>
    /// <param name="year">The four-digit calendar year.</param>
    /// <returns>A sequence of movable <see cref="HolidayInfo"/> entries for Norway.</returns>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        var holyThursday = EasterCalculator.HolyThursday(year);
        var goodFriday   = EasterCalculator.GoodFriday(year);
        var easter       = EasterCalculator.Easter(year);
        var easterMonday = easter.AddDays(1);
        var ascension    = EasterCalculator.Ascension(year);
        var pentecost    = easter.AddDays(49);
        var whitMonday   = easter.AddDays(50);

        return new[]
        {
            new HolidayInfo(
                "no_holy_thursday", holyThursday.Month, holyThursday.Day, "NO",
                "Skjærtorsdag",
                NamesNo("Jueves Santo", "Holy Thursday / Maundy Thursday", "Quinta-Feira Santa", "Jeudi Saint", "Gründonnerstag", "Skjærtorsdag"),
                HolidayType.National, isMovable: true,
                description: "Skjærtorsdag / Maundy Thursday. Tre dager før påskedag. Minnet om Jesu siste måltid (nattverden)."),

            new HolidayInfo(
                "no_good_friday", goodFriday.Month, goodFriday.Day, "NO",
                "Langfredag",
                NamesNo("Viernes Santo", "Good Friday", "Sexta-Feira Santa", "Vendredi Saint", "Karfreitag", "Langfredag"),
                HolidayType.National, isMovable: true,
                description: "Langfredag / Good Friday. To dager før påskedag. Minnet om Jesu Kristi korsfestelse."),

            new HolidayInfo(
                "no_easter_sunday", easter.Month, easter.Day, "NO",
                "Påskedag",
                NamesNo("Pascua (Domingo)", "Easter Sunday", "Páscoa (Domingo)", "Pâques (Dimanche)", "Ostersonntag", "Første påskedag"),
                HolidayType.National, isMovable: true,
                description: "Første påskedag / Påskedag. Feiring av Jesu Kristi oppstandelse fra de døde."),

            new HolidayInfo(
                "no_easter_monday", easterMonday.Month, easterMonday.Day, "NO",
                "Andre påskedag",
                NamesNo("Lunes de Pascua", "Easter Monday", "Segunda-Feira de Páscoa", "Lundi de Pâques", "Ostermontag", "Andre påskedag"),
                HolidayType.National, isMovable: true,
                description: "Andre påskedag / Easter Monday. Dagen etter påskedag."),

            new HolidayInfo(
                "no_ascension", ascension.Month, ascension.Day, "NO",
                "Kristi Himmelfartsdag",
                NamesNo("Ascensión del Señor", "Ascension Day", "Ascensão do Senhor", "Ascension du Christ", "Christi Himmelfahrt", "Kristi Himmelfartsdag"),
                HolidayType.National, isMovable: true,
                description: "Kristi Himmelfartsdag / Ascension Day. 39 dager etter påskedag."),

            new HolidayInfo(
                "no_pentecost", pentecost.Month, pentecost.Day, "NO",
                "Pinsedag",
                NamesNo("Pentecostés", "Pentecost Sunday / Whit Sunday", "Pentecostes", "Pentecôte", "Pfingstsonntag", "Første pinsedag"),
                HolidayType.National, isMovable: true,
                description: "Første pinsedag / Pinsedag. 49 dager etter påskedag. Minnet om Den Hellige Ånds komme."),

            new HolidayInfo(
                "no_whit_monday", whitMonday.Month, whitMonday.Day, "NO",
                "Andre pinsedag",
                NamesNo("Lunes de Pentecostés", "Whit Monday / Pentecost Monday", "Segunda-Feira de Pentecostes", "Lundi de Pentecôte", "Pfingstmontag", "Andre pinsedag"),
                HolidayType.National, isMovable: true,
                description: "Andre pinsedag / Whit Monday. 50 dager etter påskedag."),
        };
    }

    /// <summary>
    /// Builds a multilingual name dictionary that includes a Norwegian (<c>no</c>) entry
    /// in addition to the standard five languages.
    /// </summary>
    /// <param name="es">Spanish name.</param>
    /// <param name="en">English name.</param>
    /// <param name="pt">Portuguese name.</param>
    /// <param name="fr">French name.</param>
    /// <param name="de">German name.</param>
    /// <param name="no">Norwegian name.</param>
    /// <returns>A read-only dictionary keyed by BCP 47 language tag.</returns>
    private static IReadOnlyDictionary<string, string> NamesNo(
        string es, string en, string pt, string fr, string de, string no)
        => new Dictionary<string, string>
        {
            ["es"] = es,
            ["en"] = en,
            ["pt"] = pt,
            ["fr"] = fr,
            ["de"] = de,
            ["no"] = no,
        };
}
