using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.Europe;

/// <summary>
/// Holiday provider for Poland (Polska), covering all 13 national public holidays
/// (święta państwowe i narodowe) as established by the Polish Act on Public Holidays
/// (Ustawa z dnia 18 stycznia 1951 r. o dniach wolnych od pracy, with amendments).
/// Notable holidays include Święto Konstytucji 3 Maja (Constitution Day, 3 May 1791) —
/// the world's second oldest national constitution — and Święto Niepodległości
/// (Independence Day, 11 November 1918) marking the end of 123 years of partitions.
/// Święto Trzech Króli (Epiphany, 6 January) was reinstated as a public holiday in 2011.
/// </summary>
public class PolandHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "PL";

    /// <inheritdoc/>
    public override string CountryName => "Poland";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "pl";

    /// <summary>
    /// Returns the 9 fixed national holidays of Poland.
    /// Easter-based movable holidays are returned by <see cref="GetMovableHolidays"/>.
    /// </summary>
    /// <returns>A sequence of fixed <see cref="HolidayInfo"/> entries for Poland.</returns>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays() => new[]
    {
        // ── National fixed holidays ───────────────────────────────────────────────

        new HolidayInfo(
            "pl_new_year", 1, 1, "PL",
            "Nowy Rok",
            Namespl("Año Nuevo", "New Year's Day", "Ano Novo", "Jour de l'An", "Neujahrstag", "Nowy Rok"),
            HolidayType.National,
            description: "Pierwszy dzień nowego roku / Celebration of the first day of the new calendar year."),

        new HolidayInfo(
            "pl_epiphany", 1, 6, "PL",
            "Święto Trzech Króli",
            Namespl("Epifanía / Reyes Magos", "Epiphany / Three Kings' Day", "Epifania / Dia de Reis", "Épiphanie / Jour des Rois", "Heilige Drei Könige", "Święto Trzech Króli"),
            HolidayType.Religious,
            description: "Uroczystość Objawienia Pańskiego / Feast of the Epiphany. Przywrócone jako dzień wolny od pracy w 2011 roku. W Polsce obchodzone uroczyście z kolorowymi procesjami Trzech Króli."),

        new HolidayInfo(
            "pl_labor_day", 5, 1, "PL",
            "Święto Pracy",
            Namespl("Día del Trabajo", "Labour Day", "Dia do Trabalho", "Fête du Travail", "Tag der Arbeit", "Święto Pracy"),
            HolidayType.National,
            description: "Międzynarodowy Dzień Pracy / International Workers' Day. Dzień upamiętniający ruch robotniczy i walkę o prawa pracownicze."),

        new HolidayInfo(
            "pl_constitution_day", 5, 3, "PL",
            "Święto Konstytucji 3 Maja",
            Namespl("Día de la Constitución del 3 de Mayo", "Constitution Day (3 May)", "Dia da Constituição de 3 de Maio", "Fête de la Constitution du 3 mai", "Tag der Verfassung vom 3. Mai", "Święto Konstytucji 3 Maja"),
            HolidayType.Civic,
            description: "Upamiętnienie uchwalenia Konstytucji 3 maja 1791 roku — drugiej najstarszej konstytucji narodowej na świecie i pierwszej w Europie. Konstytucja ta ustanowiła monarchię konstytucyjną w Rzeczypospolitej Obojga Narodów."),

        new HolidayInfo(
            "pl_assumption", 8, 15, "PL",
            "Wniebowzięcie NMP",
            Namespl("Asunción de la Virgen", "Assumption of Mary", "Assunção de Nossa Senhora", "Assomption de Marie", "Mariä Himmelfahrt", "Wniebowzięcie Najświętszej Maryi Panny"),
            HolidayType.Religious,
            description: "Uroczystość Wniebowzięcia Najświętszej Maryi Panny / Feast of the Assumption of Mary. W Polsce dzień ten zbiega się z rocznicą Bitwy Warszawskiej 1920 roku (Cudu nad Wisłą), uznawanej za jeden z przełomowych momentów w historii Europy."),

        new HolidayInfo(
            "pl_all_saints", 11, 1, "PL",
            "Wszystkich Świętych",
            Namespl("Todos los Santos", "All Saints' Day", "Dia de Todos os Santos", "Toussaint", "Allerheiligen", "Wszystkich Świętych"),
            HolidayType.Religious,
            description: "Uroczystość Wszystkich Świętych / All Saints' Day. W Polsce tradycyjnie dzień odwiedzania grobów bliskich i zapalania zniczy na cmentarzach."),

        new HolidayInfo(
            "pl_independence_day", 11, 11, "PL",
            "Święto Niepodległości",
            Namespl("Día de la Independencia de Polonia", "Polish Independence Day", "Dia da Independência da Polônia", "Jour de l'Indépendance polonaise", "Polnischer Unabhängigkeitstag", "Święto Niepodległości"),
            HolidayType.Civic,
            description: "Upamiętnienie odzyskania niepodległości przez Polskę 11 listopada 1918 roku po 123 latach rozbiorów przez Rosję, Prusy i Austrię. Data ta pokrywa się z zawieszeniem broni kończącym I wojnę światową."),

        new HolidayInfo(
            "pl_christmas_1", 12, 25, "PL",
            "Boże Narodzenie",
            Namespl("Navidad (primer día)", "Christmas Day", "Natal (primeiro dia)", "Noël (premier jour)", "Erster Weihnachtstag", "Boże Narodzenie (pierwszy dzień)"),
            HolidayType.National,
            description: "Pierwszy dzień Bożego Narodzenia / Christmas Day. Dzień upamiętniający narodziny Jezusa Chrystusa."),

        new HolidayInfo(
            "pl_christmas_2", 12, 26, "PL",
            "Drugi dzień Bożego Narodzenia",
            Namespl("Navidad (segundo día)", "Second Day of Christmas / Boxing Day", "Natal (segundo dia)", "Lendemain de Noël", "Zweiter Weihnachtstag", "Drugi dzień Bożego Narodzenia"),
            HolidayType.National,
            description: "Drugi dzień Bożego Narodzenia / Second Day of Christmas. Dzień świętego Szczepana."),
    };

    /// <summary>
    /// Returns the 4 Easter-based movable national holidays of Poland for the given <paramref name="year"/>:
    /// Wielkanoc (Easter Sunday), Poniedziałek Wielkanocny (Easter Monday),
    /// Zesłanie Ducha Świętego (Pentecost), and Boże Ciało (Corpus Christi).
    /// </summary>
    /// <param name="year">The four-digit calendar year.</param>
    /// <returns>A sequence of movable <see cref="HolidayInfo"/> entries for Poland.</returns>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        var easter        = EasterCalculator.Easter(year);
        var easterMonday  = easter.AddDays(1);
        var pentecost     = easter.AddDays(49);
        var corpusChristi = EasterCalculator.CorpusChristi(year);

        return new[]
        {
            new HolidayInfo(
                "pl_easter_sunday", easter.Month, easter.Day, "PL",
                "Wielkanoc",
                Namespl("Pascua (Domingo)", "Easter Sunday", "Páscoa (Domingo)", "Pâques (Dimanche)", "Ostersonntag", "Wielkanoc"),
                HolidayType.National, isMovable: true,
                description: "Wielkanoc / Niedziela Wielkanocna. Najważniejsze święto chrześcijańskie, upamiętniające zmartwychwstanie Jezusa Chrystusa."),

            new HolidayInfo(
                "pl_easter_monday", easterMonday.Month, easterMonday.Day, "PL",
                "Poniedziałek Wielkanocny",
                Namespl("Lunes de Pascua", "Easter Monday", "Segunda-Feira de Páscoa", "Lundi de Pâques", "Ostermontag", "Poniedziałek Wielkanocny"),
                HolidayType.National, isMovable: true,
                description: "Śmigus-dyngus / Lany Poniedziałek / Easter Monday. Dzień po Wielkanocy, znany w Polsce ze zwyczaju polewania się wodą."),

            new HolidayInfo(
                "pl_pentecost", pentecost.Month, pentecost.Day, "PL",
                "Zesłanie Ducha Świętego",
                Namespl("Pentecostés", "Pentecost Sunday / Whit Sunday", "Pentecostes", "Pentecôte", "Pfingstsonntag", "Zesłanie Ducha Świętego"),
                HolidayType.National, isMovable: true,
                description: "Zesłanie Ducha Świętego / Niedziela Zielonych Świątek. 49 dni po Wielkanocy. Upamiętnienie zstąpienia Ducha Świętego na apostołów."),

            new HolidayInfo(
                "pl_corpus_christi", corpusChristi.Month, corpusChristi.Day, "PL",
                "Boże Ciało",
                Namespl("Corpus Christi", "Corpus Christi", "Corpo de Deus", "Fête-Dieu", "Fronleichnam", "Boże Ciało"),
                HolidayType.National, isMovable: true,
                description: "Uroczystość Najświętszego Ciała i Krwi Chrystusa / Corpus Christi. 60 dni po Wielkanocy. W Polsce obchodzone uroczyście z procesjami przez ulice miast i wsi."),
        };
    }

    /// <summary>
    /// Builds a multilingual name dictionary that includes a Polish (<c>pl</c>) entry
    /// in addition to the standard five languages.
    /// </summary>
    /// <param name="es">Spanish name.</param>
    /// <param name="en">English name.</param>
    /// <param name="pt">Portuguese name.</param>
    /// <param name="fr">French name.</param>
    /// <param name="de">German name.</param>
    /// <param name="pl">Polish name.</param>
    /// <returns>A read-only dictionary keyed by BCP 47 language tag.</returns>
    private static IReadOnlyDictionary<string, string> Namespl(
        string es, string en, string pt, string fr, string de, string pl)
        => new Dictionary<string, string>
        {
            ["es"] = es,
            ["en"] = en,
            ["pt"] = pt,
            ["fr"] = fr,
            ["de"] = de,
            ["pl"] = pl,
        };
}
