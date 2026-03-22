using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.Europe;

/// <summary>
/// Holiday provider for Germany, covering the 9 national public holidays
/// (bundesweite gesetzliche Feiertage) shared by all 16 Bundesländer, plus
/// regionally observed holidays for the most populous states as defined by
/// the individual Landesgesetze (state holiday laws).
/// </summary>
public class GermanyHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "DE";

    /// <inheritdoc/>
    public override string CountryName => "Germany";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "de";

    /// <inheritdoc/>
    /// <remarks>
    /// Returns all fixed national holidays (Tag der Deutschen Einheit, Neujahrstag,
    /// Tag der Arbeit, and both Christmas days) plus fixed regional holidays for
    /// Bayern, Nordrhein-Westfalen, Baden-Württemberg, Thüringen, Sachsen, and Berlin.
    /// Easter-based movable national and regional holidays are returned by
    /// <see cref="GetMovableHolidays"/>.
    /// </remarks>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays() => new[]
    {
        // ── National fixed holidays ───────────────────────────────────────────────

        new HolidayInfo(
            "de_new_year", 1, 1, "DE",
            "Neujahrstag",
            Names("Año Nuevo", "New Year's Day", "Ano Novo", "Jour de l'An", "Neujahrstag"),
            HolidayType.National,
            description: "Beginn des Kalenderjahres / Celebration of the start of the calendar year."),

        new HolidayInfo(
            "de_labor_day", 5, 1, "DE",
            "Tag der Arbeit",
            Names("Día del Trabajo", "Labour Day", "Dia do Trabalho", "Fête du Travail", "Tag der Arbeit"),
            HolidayType.National,
            description: "Internationaler Tag der Arbeit / International Workers' Day, commemorating the labour movement."),

        new HolidayInfo(
            "de_reunification", 10, 3, "DE",
            "Tag der Deutschen Einheit",
            Names("Día de la Unidad Alemana", "German Unity Day",
                  "Dia da Unidade Alemã", "Jour de l'Unité Allemande", "Tag der Deutschen Einheit"),
            HolidayType.Civic,
            description: "Gedenktag der Wiedervereinigung Deutschlands am 3. Oktober 1990, als die DDR der Bundesrepublik Deutschland beitrat."),

        new HolidayInfo(
            "de_christmas_1", 12, 25, "DE",
            "Erster Weihnachtstag",
            Names("Navidad (primer día)", "Christmas Day", "Natal (primeiro dia)", "Noël (premier jour)", "Erster Weihnachtstag"),
            HolidayType.National,
            description: "Erster gesetzlicher Weihnachtsfeiertag / Christmas Day."),

        new HolidayInfo(
            "de_christmas_2", 12, 26, "DE",
            "Zweiter Weihnachtstag",
            Names("Navidad (segundo día)", "Boxing Day / Second Christmas Day",
                  "Natal (segundo dia)", "Lendemain de Noël", "Zweiter Weihnachtstag"),
            HolidayType.National,
            description: "Zweiter gesetzlicher Weihnachtsfeiertag / St. Stephen's Day / Boxing Day."),

        // ── Bayern (DE-BY) – fixed regional holidays ──────────────────────────────

        new HolidayInfo(
            "de_by_epiphany", 1, 6, "DE",
            "Heilige Drei Könige",
            Names("Epifanía del Señor / Reyes Magos", "Epiphany / Three Kings' Day",
                  "Epifania / Dia de Reis", "Épiphanie / Jour des Rois", "Heilige Drei Könige"),
            HolidayType.Regional, regionCode: "DE-BY",
            description: "Dreikönigsfest / Feast of Epiphany. Gesetzlicher Feiertag in Bayern, Baden-Württemberg und Sachsen-Anhalt."),

        new HolidayInfo(
            "de_by_assumption", 8, 15, "DE",
            "Mariä Himmelfahrt",
            Names("Asunción de la Virgen", "Assumption of Mary",
                  "Assunção de Nossa Senhora", "Assomption de Marie", "Mariä Himmelfahrt"),
            HolidayType.Regional, regionCode: "DE-BY",
            description: "Mariä Himmelfahrt / Assumption of Mary. Gesetzlicher Feiertag in Bayern (nur in Gemeinden mit überwiegend katholischer Bevölkerung) und im Saarland."),

        new HolidayInfo(
            "de_by_all_saints", 11, 1, "DE",
            "Allerheiligen",
            Names("Todos los Santos", "All Saints' Day",
                  "Dia de Todos os Santos", "Toussaint", "Allerheiligen"),
            HolidayType.Regional, regionCode: "DE-BY",
            description: "Allerheiligen / All Saints' Day. Gesetzlicher Feiertag in Bayern, Baden-Württemberg, Nordrhein-Westfalen, Rheinland-Pfalz und dem Saarland."),

        // ── Nordrhein-Westfalen (DE-NW) – fixed regional holidays ─────────────────

        new HolidayInfo(
            "de_nw_all_saints", 11, 1, "DE",
            "Allerheiligen",
            Names("Todos los Santos", "All Saints' Day",
                  "Dia de Todos os Santos", "Toussaint", "Allerheiligen"),
            HolidayType.Regional, regionCode: "DE-NW",
            description: "Allerheiligen / All Saints' Day. Gesetzlicher Feiertag in Nordrhein-Westfalen."),

        // ── Baden-Württemberg (DE-BW) – fixed regional holidays ───────────────────

        new HolidayInfo(
            "de_bw_epiphany", 1, 6, "DE",
            "Heilige Drei Könige",
            Names("Epifanía del Señor / Reyes Magos", "Epiphany / Three Kings' Day",
                  "Epifania / Dia de Reis", "Épiphanie / Jour des Rois", "Heilige Drei Könige"),
            HolidayType.Regional, regionCode: "DE-BW",
            description: "Dreikönigsfest / Feast of Epiphany. Gesetzlicher Feiertag in Baden-Württemberg."),

        new HolidayInfo(
            "de_bw_all_saints", 11, 1, "DE",
            "Allerheiligen",
            Names("Todos los Santos", "All Saints' Day",
                  "Dia de Todos os Santos", "Toussaint", "Allerheiligen"),
            HolidayType.Regional, regionCode: "DE-BW",
            description: "Allerheiligen / All Saints' Day. Gesetzlicher Feiertag in Baden-Württemberg."),

        // ── Thüringen (DE-TH) – fixed regional holidays ───────────────────────────

        new HolidayInfo(
            "de_th_reformation", 10, 31, "DE",
            "Reformationstag",
            Names("Día de la Reforma Protestante", "Reformation Day",
                  "Dia da Reforma Protestante", "Jour de la Réforme", "Reformationstag"),
            HolidayType.Regional, regionCode: "DE-TH",
            description: "Gedenktag des Thesenanschlags Martin Luthers am 31. Oktober 1517, der die Reformation der Kirche einleitete. Feiertag in den mehrheitlich protestantischen Bundesländern."),

        // ── Sachsen (DE-SN) – fixed regional holidays ─────────────────────────────

        new HolidayInfo(
            "de_sn_reformation", 10, 31, "DE",
            "Reformationstag",
            Names("Día de la Reforma Protestante", "Reformation Day",
                  "Dia da Reforma Protestante", "Jour de la Réforme", "Reformationstag"),
            HolidayType.Regional, regionCode: "DE-SN",
            description: "Reformationstag / Reformation Day. Gesetzlicher Feiertag in Sachsen."),

        new HolidayInfo(
            "de_sn_buss_bettag", 11, 18, "DE",
            "Buß- und Bettag",
            Names("Día de Arrepentimiento y Oración", "Repentance and Prayer Day",
                  "Dia de Arrependimento e Oração", "Jour du Repentir et de la Prière", "Buß- und Bettag"),
            HolidayType.Regional, regionCode: "DE-SN",
            description: "Buß- und Bettag (Mittwoch vor dem 23. November) / Day of Repentance and Prayer. Einziger Bundesland-Feiertag ausschließlich in Sachsen, seit 1995 erhalten, als er bundesweit abgeschafft wurde. Approximate date: third Wednesday before November 23."),

        // ── Berlin (DE-BE) – fixed regional holidays ──────────────────────────────

        new HolidayInfo(
            "de_be_frauentag", 3, 8, "DE",
            "Internationaler Frauentag",
            Names("Día Internacional de la Mujer", "International Women's Day",
                  "Dia Internacional da Mulher", "Journée internationale des femmes", "Internationaler Frauentag"),
            HolidayType.Regional, regionCode: "DE-BE",
            description: "Internationaler Frauentag. Seit 2019 gesetzlicher Feiertag in Berlin; die Stadt war das erste Bundesland, das diesen Tag als offiziellen Feiertag einführte."),
    };

    /// <inheritdoc/>
    /// <remarks>
    /// Returns all Easter-based movable holidays, both national and regional:
    /// Karfreitag (Good Friday), Ostermontag (Easter Monday), Christi Himmelfahrt (Ascension),
    /// Pfingstmontag (Whit Monday), and Fronleichnam (Corpus Christi) for Bayern,
    /// Nordrhein-Westfalen, and Baden-Württemberg.
    /// </remarks>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        var goodFriday    = EasterCalculator.GoodFriday(year);
        var easterMonday  = EasterCalculator.Easter(year).AddDays(1);
        var ascension     = EasterCalculator.Ascension(year);
        var whitMonday    = EasterCalculator.Easter(year).AddDays(50);
        var corpusChristi = EasterCalculator.CorpusChristi(year);

        return new[]
        {
            // ── National movable holidays ─────────────────────────────────────────

            new HolidayInfo(
                "de_good_friday", goodFriday.Month, goodFriday.Day, "DE",
                "Karfreitag",
                Names("Viernes Santo", "Good Friday",
                      "Sexta-Feira Santa", "Vendredi Saint", "Karfreitag"),
                HolidayType.National, isMovable: true,
                description: "Karfreitag / Good Friday. Bundesweiter gesetzlicher Feiertag, zwei Tage vor Ostersonntag."),

            new HolidayInfo(
                "de_easter_monday", easterMonday.Month, easterMonday.Day, "DE",
                "Ostermontag",
                Names("Lunes de Pascua", "Easter Monday",
                      "Segunda-Feira de Páscoa", "Lundi de Pâques", "Ostermontag"),
                HolidayType.National, isMovable: true,
                description: "Ostermontag / Easter Monday. Bundesweiter gesetzlicher Feiertag, einen Tag nach Ostersonntag."),

            new HolidayInfo(
                "de_ascension", ascension.Month, ascension.Day, "DE",
                "Christi Himmelfahrt",
                Names("Ascensión del Señor", "Ascension of Christ",
                      "Ascensão de Cristo", "Ascension du Christ", "Christi Himmelfahrt"),
                HolidayType.National, isMovable: true,
                description: "Christi Himmelfahrt / Ascension Thursday. Bundesweiter gesetzlicher Feiertag, 39 Tage nach Ostersonntag. Wird in Deutschland auch als Vatertag gefeiert."),

            new HolidayInfo(
                "de_whit_monday", whitMonday.Month, whitMonday.Day, "DE",
                "Pfingstmontag",
                Names("Lunes de Pentecostés", "Whit Monday / Pentecost Monday",
                      "Segunda-Feira de Pentecostes", "Lundi de Pentecôte", "Pfingstmontag"),
                HolidayType.National, isMovable: true,
                description: "Pfingstmontag / Whit Monday. Bundesweiter gesetzlicher Feiertag, 50 Tage nach Ostersonntag."),

            // ── Bayern (DE-BY) – movable regional holidays ────────────────────────

            new HolidayInfo(
                "de_by_corpus_christi", corpusChristi.Month, corpusChristi.Day, "DE",
                "Fronleichnam",
                Names("Corpus Christi", "Corpus Christi",
                      "Corpo de Deus", "Fête-Dieu", "Fronleichnam"),
                HolidayType.Regional, isMovable: true, regionCode: "DE-BY",
                description: "Fronleichnam / Corpus Christi. Gesetzlicher Feiertag in Bayern, Baden-Württemberg, Hessen, Nordrhein-Westfalen, Rheinland-Pfalz und dem Saarland. 60 Tage nach Ostersonntag."),

            // ── Nordrhein-Westfalen (DE-NW) – movable regional holidays ───────────

            new HolidayInfo(
                "de_nw_corpus_christi", corpusChristi.Month, corpusChristi.Day, "DE",
                "Fronleichnam",
                Names("Corpus Christi", "Corpus Christi",
                      "Corpo de Deus", "Fête-Dieu", "Fronleichnam"),
                HolidayType.Regional, isMovable: true, regionCode: "DE-NW",
                description: "Fronleichnam / Corpus Christi. Gesetzlicher Feiertag in Nordrhein-Westfalen. 60 Tage nach Ostersonntag."),

            // ── Baden-Württemberg (DE-BW) – movable regional holidays ─────────────

            new HolidayInfo(
                "de_bw_corpus_christi", corpusChristi.Month, corpusChristi.Day, "DE",
                "Fronleichnam",
                Names("Corpus Christi", "Corpus Christi",
                      "Corpo de Deus", "Fête-Dieu", "Fronleichnam"),
                HolidayType.Regional, isMovable: true, regionCode: "DE-BW",
                description: "Fronleichnam / Corpus Christi. Gesetzlicher Feiertag in Baden-Württemberg. 60 Tage nach Ostersonntag."),
        };
    }
}
