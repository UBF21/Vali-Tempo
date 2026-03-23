using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.Europe;

/// <summary>
/// Holiday provider for Finland (Suomi / Finland), covering the 14 national public holidays
/// (pyhäpäivät / helgdagar) as defined by the Finnish Act on Holidays (Laki yleisistä vapaapäivistä,
/// 9.12.1994/1083). Itsenäisyyspäivä (6 December) commemorates Finland's declaration of
/// independence from Russia in 1917. Vappu (1 May) is both International Workers' Day and
/// the traditional spring celebration. Juhannuspäivä (Midsommar) falls on the Saturday
/// between 20 and 26 June. Pyhäinpäivä (All Saints) falls on the Saturday between 31
/// October and 6 November; listed here as 1 November as a representative date.
/// Finland has two official languages: Finnish and Swedish.
/// </summary>
public class FinlandHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "FI";

    /// <inheritdoc/>
    public override string CountryName => "Finland";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "fi";

    /// <summary>
    /// Returns the fixed national and optional holidays of Finland.
    /// Easter-based movable holidays are returned by <see cref="GetMovableHolidays"/>.
    /// Note: Juhannuspäivä (listed as 21 June) falls on the Saturday between 20–26 June,
    /// and Pyhäinpäivä (listed as 1 November) falls on the Saturday between 31 Oct–6 Nov.
    /// </summary>
    /// <returns>A sequence of fixed <see cref="HolidayInfo"/> entries for Finland.</returns>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays() => new[]
    {
        // ── National fixed holidays ───────────────────────────────────────────────

        new HolidayInfo(
            "fi_new_year", 1, 1, "FI",
            "Uudenvuodenpäivä",
            NamesFi("Año Nuevo", "New Year's Day", "Ano Novo", "Jour de l'An", "Neujahrstag", "Uudenvuodenpäivä"),
            HolidayType.National,
            description: "Vuoden ensimmäinen päivä / Nyårsdag / First day of the new calendar year."),

        new HolidayInfo(
            "fi_epiphany", 1, 6, "FI",
            "Loppiainen",
            NamesFi("Epifanía / Reyes Magos", "Epiphany / Three Kings' Day", "Epifania / Dia de Reis", "Épiphanie / Jour des Rois", "Heilige Drei Könige", "Loppiainen"),
            HolidayType.Religious,
            description: "Loppiainen / Trettondag / Epiphany. Kolmen viisaan miehen vierailu Jeesus-lapselle. Juhlapäivä Suomessa."),

        new HolidayInfo(
            "fi_vappu", 5, 1, "FI",
            "Vappu",
            NamesFi("Día del Trabajo / Primero de Mayo", "May Day / Labour Day", "Dia do Trabalho / Primeiro de Maio", "Fête du Travail / Premier Mai", "Tag der Arbeit / Erster Mai", "Vappu"),
            HolidayType.National,
            description: "Vappu / Valborg / May Day. Sekä kansainvälinen työväenpäivä että perinteinen kevätjuhla Suomessa. Vietettiin alun perin Valpuriinöytenä (30. huhtikuuta) ja Vapuna (1. toukokuuta)."),

        new HolidayInfo(
            "fi_independence_day", 12, 6, "FI",
            "Itsenäisyyspäivä",
            NamesFi("Día de la Independencia de Finlandia", "Finnish Independence Day", "Dia da Independência da Finlândia", "Fête de l'Indépendance finlandaise", "Finnischer Unabhängigkeitstag", "Itsenäisyyspäivä"),
            HolidayType.Civic,
            description: "Suomen itsenäistyminen Venäjästä 6. joulukuuta 1917. Eduskunta hyväksyi itsenäisyysjulistuksen ja Venäjä tunnusti Suomen itsenäisyyden 4. tammikuuta 1918. Vietettään sinivalkoisten kynttilöiden sekä presidentin linnan juhlavastaanoton merkeissä."),

        new HolidayInfo(
            "fi_christmas_eve", 12, 24, "FI",
            "Jouluaatto",
            NamesFi("Nochebuena", "Christmas Eve", "Véspera de Natal", "Réveillon de Noël", "Heiliger Abend", "Jouluaatto"),
            HolidayType.Optional,
            description: "Jouluaatto / Julafton / Christmas Eve. Suomessa jouluaatto on perinteisin joulun viettopäivä. Puoliksi vapaapäivä; käytännössä vapaa monilla."),

        new HolidayInfo(
            "fi_christmas_day", 12, 25, "FI",
            "Joulupäivä",
            NamesFi("Navidad (primer día)", "Christmas Day", "Natal (primeiro dia)", "Noël (premier jour)", "Erster Weihnachtstag", "Joulupäivä"),
            HolidayType.National,
            description: "Joulupäivä / Juldagen / Christmas Day. Jeesuksen syntymän juhla."),

        new HolidayInfo(
            "fi_boxing_day", 12, 26, "FI",
            "Tapaninpäivä",
            NamesFi("Navidad (segundo día)", "Boxing Day / St. Stephen's Day", "Natal (segundo dia)", "Saint-Étienne / Lendemain de Noël", "Zweiter Weihnachtstag / Stephanustag", "Tapaninpäivä"),
            HolidayType.National,
            description: "Tapaninpäivä / Annandag jul / St. Stephen's Day. Joulun toinen päivä; pyhitetty ensimmäiselle marttyyrikristitylle, Tapani Pyhälle."),
    };

    /// <summary>
    /// Returns the 5 Easter-based movable national holidays of Finland for the given <paramref name="year"/>:
    /// Pitkäperjantai (Good Friday), Pääsiäispäivä (Easter Sunday),
    /// Toinen pääsiäispäivä (Easter Monday), Helatorstai (Ascension),
    /// and Helluntaipäivä (Pentecost Sunday).
    /// </summary>
    /// <param name="year">The four-digit calendar year.</param>
    /// <returns>A sequence of movable <see cref="HolidayInfo"/> entries for Finland.</returns>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        var goodFriday   = EasterCalculator.GoodFriday(year);
        var easter       = EasterCalculator.Easter(year);
        var easterMonday = easter.AddDays(1);
        var ascension    = EasterCalculator.Ascension(year);
        var pentecost    = easter.AddDays(49);

        // Juhannuspäivä: Saturday between June 20 and 26
        DateTime june20 = new DateTime(year, 6, 20);
        int daysToSat = ((int)DayOfWeek.Saturday - (int)june20.DayOfWeek + 7) % 7;
        DateTime juhannuspaiva = june20.AddDays(daysToSat); // Juhannuspäivä (Saturday)
        DateTime juhannusaatto = juhannuspaiva.AddDays(-1); // Juhannusaatto (Friday)

        // Pyhäinpäivä: Saturday between Oct 31 and Nov 6
        DateTime oct31 = new DateTime(year, 10, 31);
        int daysToSat2 = ((int)DayOfWeek.Saturday - (int)oct31.DayOfWeek + 7) % 7;
        DateTime pyhaInpaiva = oct31.AddDays(daysToSat2);

        return new[]
        {
            new HolidayInfo(
                "fi_good_friday", goodFriday.Month, goodFriday.Day, "FI",
                "Pitkäperjantai",
                NamesFi("Viernes Santo", "Good Friday", "Sexta-Feira Santa", "Vendredi Saint", "Karfreitag", "Pitkäperjantai"),
                HolidayType.National, isMovable: true,
                description: "Pitkäperjantai / Långfredag / Good Friday. Kaksi päivää ennen pääsiäispäivää. Jeesuksen Kristuksen ristiinnaulitsemisen muistopäivä."),

            new HolidayInfo(
                "fi_easter_sunday", easter.Month, easter.Day, "FI",
                "Pääsiäispäivä",
                NamesFi("Pascua (Domingo)", "Easter Sunday", "Páscoa (Domingo)", "Pâques (Dimanche)", "Ostersonntag", "Ensimmäinen pääsiäispäivä"),
                HolidayType.National, isMovable: true,
                description: "Ensimmäinen pääsiäispäivä / Påskdagen / Easter Sunday. Jeesuksen Kristuksen ylösnousemuksen juhla."),

            new HolidayInfo(
                "fi_easter_monday", easterMonday.Month, easterMonday.Day, "FI",
                "Toinen pääsiäispäivä",
                NamesFi("Lunes de Pascua", "Easter Monday", "Segunda-Feira de Páscoa", "Lundi de Pâques", "Ostermontag", "Toinen pääsiäispäivä"),
                HolidayType.National, isMovable: true,
                description: "Toinen pääsiäispäivä / Annandag påsk / Easter Monday. Pääsiäispäivää seuraava päivä."),

            new HolidayInfo(
                "fi_ascension", ascension.Month, ascension.Day, "FI",
                "Helatorstai",
                NamesFi("Ascensión del Señor", "Ascension Day", "Ascensão do Senhor", "Ascension du Christ", "Christi Himmelfahrt", "Helatorstai"),
                HolidayType.National, isMovable: true,
                description: "Helatorstai / Kristi himmelsfärdsdag / Ascension Day. 39 päivää pääsiäispäivän jälkeen."),

            new HolidayInfo(
                "fi_pentecost", pentecost.Month, pentecost.Day, "FI",
                "Helluntaipäivä",
                NamesFi("Pentecostés", "Pentecost Sunday / Whit Sunday", "Pentecostes", "Pentecôte", "Pfingstsonntag", "Helluntaipäivä"),
                HolidayType.National, isMovable: true,
                description: "Helluntaipäivä / Pingstdagen / Pentecost Sunday. 49 päivää pääsiäispäivän jälkeen. Pyhän Hengen tulemisen muistopäivä."),

            new HolidayInfo(
                "fi_midsummer_eve", juhannusaatto.Month, juhannusaatto.Day, "FI",
                "Juhannusaatto",
                NamesFi("Víspera de San Juan / Midsommar", "Midsummer Eve", "Véspera de São João", "Veille de la Saint-Jean", "Mittsommerabend", "Juhannusaatto"),
                HolidayType.Optional, isMovable: true,
                description: "Juhannusaatto / Midsommarafton. Juhannuksen aatto, juhannuspäivää edeltävä perjantai."),

            new HolidayInfo(
                "fi_midsummer", juhannuspaiva.Month, juhannuspaiva.Day, "FI",
                "Juhannuspäivä",
                NamesFi("Midsommar / Noche de San Juan", "Midsummer Day", "Dia de São João / Midsommar", "Fête de la Saint-Jean / Solstice d'été", "Mittsommertag", "Juhannuspäivä"),
                HolidayType.National, isMovable: true,
                description: "Juhannuspäivä / Midsommardagen. Juhannus vietetään lauantaina 20.–26. kesäkuuta. Kesäpäivänseisauksen juhla; Suomessa perinteisesti vietetään mökillä, nuotion äärellä."),

            new HolidayInfo(
                "fi_all_saints", pyhaInpaiva.Month, pyhaInpaiva.Day, "FI",
                "Pyhäinpäivä",
                NamesFi("Día de Todos los Santos", "All Saints' Day", "Dia de Todos os Santos", "Toussaint", "Allerheiligen", "Pyhäinpäivä"),
                HolidayType.National, isMovable: true,
                description: "Pyhäinpäivä / Alla helgons dag. Vietetään lauantaina 31. lokakuuta – 6. marraskuuta. Päivä kuolleiden muistamiselle."),
        };
    }

    /// <summary>
    /// Builds a multilingual name dictionary that includes a Finnish (<c>fi</c>) entry
    /// in addition to the standard five languages.
    /// </summary>
    /// <param name="es">Spanish name.</param>
    /// <param name="en">English name.</param>
    /// <param name="pt">Portuguese name.</param>
    /// <param name="fr">French name.</param>
    /// <param name="de">German name.</param>
    /// <param name="fi">Finnish name.</param>
    /// <returns>A read-only dictionary keyed by BCP 47 language tag.</returns>
    private static IReadOnlyDictionary<string, string> NamesFi(
        string es, string en, string pt, string fr, string de, string fi)
        => new Dictionary<string, string>
        {
            ["es"] = es,
            ["en"] = en,
            ["pt"] = pt,
            ["fr"] = fr,
            ["de"] = de,
            ["fi"] = fi,
        };
}
