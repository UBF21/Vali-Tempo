using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.Europe;

/// <summary>
/// Holiday provider for Sweden (Sverige), covering the 11 national public holidays
/// (helgdagar) plus 2 optional days as defined by the Swedish Public Holidays Act
/// (Lag (1989:253) om allmänna helgdagar). Sveriges Nationaldag (6 June) was established
/// as an official public holiday in 2005 but has been celebrated since 1893; it marks
/// the date in 1523 when Gustav Vasa was elected king, effectively establishing Swedish
/// independence from the Kalmar Union with Denmark. Midsommardagen falls on the Saturday
/// between 20 and 26 June each year, and Alla Helgons Dag falls on the Saturday between
/// 31 October and 6 November.
/// </summary>
public class SwedenHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "SE";

    /// <inheritdoc/>
    public override string CountryName => "Sweden";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "sv";

    /// <summary>
    /// Returns the fixed national and optional holidays of Sweden.
    /// Easter-based movable holidays are returned by <see cref="GetMovableHolidays"/>.
    /// Note: Midsommardagen (listed as 25 June) and Alla Helgons Dag (listed as 1 November)
    /// are representative fixed dates; the actual dates follow the Saturday rule described
    /// in the class summary.
    /// </summary>
    /// <returns>A sequence of fixed <see cref="HolidayInfo"/> entries for Sweden.</returns>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays() => new[]
    {
        // ── National fixed holidays ───────────────────────────────────────────────

        new HolidayInfo(
            "se_new_year", 1, 1, "SE",
            "Nyårsdagen",
            NamessSv("Año Nuevo", "New Year's Day", "Ano Novo", "Jour de l'An", "Neujahrstag", "Nyårsdagen"),
            HolidayType.National,
            description: "Årets första dag / First day of the new calendar year."),

        new HolidayInfo(
            "se_epiphany", 1, 6, "SE",
            "Trettondedag Jul",
            NamessSv("Epifanía / Reyes Magos", "Epiphany / Three Kings' Day", "Epifania / Dia de Reis", "Épiphanie / Jour des Rois", "Heilige Drei Könige", "Trettondedag Jul"),
            HolidayType.Religious,
            description: "Trettondedag Jul / Trettondagen. Den trettonde dagen efter juldag. Firande av de vises ankomst till Betlehem."),

        new HolidayInfo(
            "se_labor_day", 5, 1, "SE",
            "Första Maj",
            NamessSv("Día del Trabajo", "Labour Day / May Day", "Dia do Trabalho", "Fête du Travail", "Tag der Arbeit", "Första Maj"),
            HolidayType.National,
            description: "Internationell arbetardag / International Workers' Day. Sedan 1939 allmän helgdag i Sverige."),

        new HolidayInfo(
            "se_national_day", 6, 6, "SE",
            "Sveriges Nationaldag",
            NamessSv("Día Nacional de Suecia", "Sweden's National Day", "Dia Nacional da Suécia", "Fête nationale suédoise", "Schwedischer Nationalfeiertag", "Sveriges Nationaldag"),
            HolidayType.Civic,
            description: "Nationaldag sedan 2005 (tidigare kallad Svenska Flaggans Dag). Markerar dagen då Gustav Vasa valdes till kung den 6 juni 1523 och Sverige frigjorde sig från Kalmarunionen med Danmark. Firades traditionellt sedan 1893."),

        new HolidayInfo(
            "se_midsummer_eve", 6, 24, "SE",
            "Midsommarafton",
            NamessSv("Víspera de San Juan / Midsommar", "Midsummer Eve", "Véspera de São João", "Veille de la Saint-Jean", "Mittsommerabend", "Midsommarafton"),
            HolidayType.Optional,
            description: "Midsommarafton — dagen före midsommardagen. Traditionellt den viktigaste firningsdagen runt sommarsolståndet i Sverige. Halvdag i praktiken; officiellt inte en röd dag men allmänt ledig."),

        new HolidayInfo(
            "se_midsummer_day", 6, 25, "SE",
            "Midsommardagen",
            NamessSv("Midsommar / Día de San Juan", "Midsummer Day", "Dia de São João / Midsommar", "Saint-Jean / Fête du Solstice", "Mittsommertag", "Midsommardagen"),
            HolidayType.National,
            description: "Midsommardagen infaller på lördagen mellan 20 och 26 juni. Firar sommarsolståndet — en av de viktigaste högtiderna i Sverige med majstångresning, folkdräkter och traditionell mat. Listed as 25 June (representative date); actual date is the Saturday between 20–26 June."),

        new HolidayInfo(
            "se_all_saints", 11, 1, "SE",
            "Alla Helgons Dag",
            NamessSv("Día de Todos los Santos", "All Saints' Day", "Dia de Todos os Santos", "Toussaint", "Allerheiligen", "Alla Helgons Dag"),
            HolidayType.National,
            description: "Alla Helgons Dag infaller på lördagen mellan 31 oktober och 6 november. Dag för minne av de döda och besök på gravar. Listed as 1 November (representative date); actual date is the Saturday between 31 October–6 November."),

        new HolidayInfo(
            "se_christmas_eve", 12, 24, "SE",
            "Julafton",
            NamessSv("Nochebuena", "Christmas Eve", "Véspera de Natal", "Réveillon de Noël", "Heiliger Abend", "Julafton"),
            HolidayType.Optional,
            description: "Julafton — kvällen innan juldagen. I Sverige firas julen traditionellt mer på julafton än på juldagen. Officiellt halvdag; i praktiken ledig dag."),

        new HolidayInfo(
            "se_christmas_day", 12, 25, "SE",
            "Juldagen",
            NamessSv("Navidad (primer día)", "Christmas Day", "Natal (primeiro dia)", "Noël (premier jour)", "Erster Weihnachtstag", "Juldagen"),
            HolidayType.National,
            description: "Juldagen / Christmas Day. Firande av Jesu Kristi födelse."),

        new HolidayInfo(
            "se_boxing_day", 12, 26, "SE",
            "Annandag Jul",
            NamessSv("Navidad (segundo día)", "Boxing Day / Second Christmas Day", "Natal (segundo dia)", "Lendemain de Noël", "Zweiter Weihnachtstag", "Annandag Jul"),
            HolidayType.National,
            description: "Annandag Jul / Stefansdagen. Dagen efter juldagen; en av de äldsta helgdagarna i den svenska kalendern."),

        new HolidayInfo(
            "se_new_years_eve", 12, 31, "SE",
            "Nyårsafton",
            NamessSv("Nochevieja", "New Year's Eve", "Véspera de Ano Novo", "Réveillon du Nouvel An", "Silvester", "Nyårsafton"),
            HolidayType.Optional,
            description: "Nyårsafton / New Year's Eve. Sista dagen på året. Officiellt halvdag; i praktiken ledig dag i Sverige."),
    };

    /// <summary>
    /// Returns the 5 Easter-based movable national holidays of Sweden for the given <paramref name="year"/>:
    /// Långfredagen (Good Friday), Påskdagen (Easter Sunday), Annandag Påsk (Easter Monday),
    /// Kristi Himmelsfärdsdag (Ascension), and Pingstdagen (Pentecost Sunday).
    /// </summary>
    /// <param name="year">The four-digit calendar year.</param>
    /// <returns>A sequence of movable <see cref="HolidayInfo"/> entries for Sweden.</returns>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        var goodFriday   = EasterCalculator.GoodFriday(year);
        var easter       = EasterCalculator.Easter(year);
        var easterMonday = easter.AddDays(1);
        var ascension    = EasterCalculator.Ascension(year);
        var pentecost    = easter.AddDays(49);

        return new[]
        {
            new HolidayInfo(
                "se_good_friday", goodFriday.Month, goodFriday.Day, "SE",
                "Långfredagen",
                NamessSv("Viernes Santo", "Good Friday", "Sexta-Feira Santa", "Vendredi Saint", "Karfreitag", "Långfredagen"),
                HolidayType.National, isMovable: true,
                description: "Långfredagen / Good Friday. Minnet av Jesu Kristi korsfästelse, två dagar före påskdagen."),

            new HolidayInfo(
                "se_easter_sunday", easter.Month, easter.Day, "SE",
                "Påskdagen",
                NamessSv("Pascua (Domingo)", "Easter Sunday", "Páscoa (Domingo)", "Pâques (Dimanche)", "Ostersonntag", "Påskdagen"),
                HolidayType.National, isMovable: true,
                description: "Påskdagen / Easter Sunday. Firande av Jesu Kristi uppståndelse från de döda."),

            new HolidayInfo(
                "se_easter_monday", easterMonday.Month, easterMonday.Day, "SE",
                "Annandag Påsk",
                NamessSv("Lunes de Pascua", "Easter Monday", "Segunda-Feira de Páscoa", "Lundi de Pâques", "Ostermontag", "Annandag Påsk"),
                HolidayType.National, isMovable: true,
                description: "Annandag Påsk / Easter Monday. Dagen efter påskdagen."),

            new HolidayInfo(
                "se_ascension", ascension.Month, ascension.Day, "SE",
                "Kristi Himmelsfärdsdag",
                NamessSv("Ascensión del Señor", "Ascension Day", "Ascensão do Senhor", "Ascension du Christ", "Christi Himmelfahrt", "Kristi Himmelsfärdsdag"),
                HolidayType.National, isMovable: true,
                description: "Kristi Himmelsfärdsdag / Ascension Day. 39 dagar efter påskdagen."),

            new HolidayInfo(
                "se_pentecost", pentecost.Month, pentecost.Day, "SE",
                "Pingstdagen",
                NamessSv("Pentecostés", "Pentecost Sunday / Whit Sunday", "Pentecostes", "Pentecôte", "Pfingstsonntag", "Pingstdagen"),
                HolidayType.National, isMovable: true,
                description: "Pingstdagen / Pentecost Sunday. 49 dagar efter påskdagen. Minnet av den Helige Andes utgjutelse."),
        };
    }

    /// <summary>
    /// Builds a multilingual name dictionary that includes a Swedish (<c>sv</c>) entry
    /// in addition to the standard five languages.
    /// </summary>
    /// <param name="es">Spanish name.</param>
    /// <param name="en">English name.</param>
    /// <param name="pt">Portuguese name.</param>
    /// <param name="fr">French name.</param>
    /// <param name="de">German name.</param>
    /// <param name="sv">Swedish name.</param>
    /// <returns>A read-only dictionary keyed by BCP 47 language tag.</returns>
    private static IReadOnlyDictionary<string, string> NamessSv(
        string es, string en, string pt, string fr, string de, string sv)
        => new Dictionary<string, string>
        {
            ["es"] = es,
            ["en"] = en,
            ["pt"] = pt,
            ["fr"] = fr,
            ["de"] = de,
            ["sv"] = sv,
        };
}
