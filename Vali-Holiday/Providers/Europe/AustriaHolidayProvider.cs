using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.Europe;

/// <summary>
/// Holiday provider for Austria (Österreich), covering all 13 national public holidays
/// (gesetzliche Feiertage) established by the Austrian Feiertagsruhegesetz (BGBl. Nr. 153/1957
/// as amended). Austria guarantees 13 paid public holidays per year — among the highest in
/// Europe. The Nationalfeiertag (26 October) commemorates the date in 1955 when Austria
/// signed the Declaration of Neutrality, regaining full sovereignty after post-war occupation.
/// Mariä Empfängnis (8 December) is the Feast of the Immaculate Conception, reflecting
/// Austria's predominantly Catholic heritage.
/// </summary>
public class AustriaHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "AT";

    /// <inheritdoc/>
    public override string CountryName => "Austria";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "de";

    /// <summary>
    /// Returns the 9 fixed national holidays of Austria.
    /// Easter-based movable holidays are returned by <see cref="GetMovableHolidays"/>.
    /// </summary>
    /// <returns>A sequence of fixed <see cref="HolidayInfo"/> entries for Austria.</returns>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays() => new[]
    {
        // ── National fixed holidays ───────────────────────────────────────────────

        new HolidayInfo(
            "at_new_year", 1, 1, "AT",
            "Neujahr",
            Names("Año Nuevo", "New Year's Day", "Ano Novo", "Jour de l'An", "Neujahr"),
            HolidayType.National,
            description: "Neujahrstag / New Year's Day. Erster Tag des neuen Jahres, gesetzlicher Feiertag in Österreich."),

        new HolidayInfo(
            "at_epiphany", 1, 6, "AT",
            "Heilige Drei Könige",
            Names("Epifanía / Reyes Magos", "Epiphany / Three Kings' Day", "Epifania / Dia de Reis", "Épiphanie / Jour des Rois", "Heilige Drei Könige"),
            HolidayType.Religious,
            description: "Dreikönigsfest / Festa dell'Epifania. Feier der Ankunft der Heiligen Drei Könige bei dem neugeborenen Jesus. Gesetzlicher Feiertag in Österreich (und in Deutschland nur in Bayern, BW und SA)."),

        new HolidayInfo(
            "at_labor_day", 5, 1, "AT",
            "Staatsfeiertag",
            Names("Día del Trabajo / Fiesta Nacional", "National Holiday / Labour Day", "Dia do Trabalho / Festa Nacional", "Fête du Travail / Fête nationale", "Staatsfeiertag / Tag der Arbeit"),
            HolidayType.National,
            description: "Österreichischer Staatsfeiertag und internationaler Tag der Arbeit. Am 1. Mai 1890 fanden die ersten Maikundgebungen der Arbeiterbewegung in Österreich statt."),

        new HolidayInfo(
            "at_assumption", 8, 15, "AT",
            "Mariä Himmelfahrt",
            Names("Asunción de la Virgen", "Assumption of Mary", "Assunção de Nossa Senhora", "Assomption de Marie", "Mariä Himmelfahrt"),
            HolidayType.Religious,
            description: "Mariä Himmelfahrt / Assumption of Mary. Feier der leiblichen Aufnahme Mariens in den Himmel. Gesetzlicher Feiertag in Österreich."),

        new HolidayInfo(
            "at_national_day", 10, 26, "AT",
            "Nationalfeiertag",
            Names("Día Nacional de Austria", "Austrian National Day", "Dia Nacional da Áustria", "Fête nationale autrichienne", "Österreichischer Nationalfeiertag"),
            HolidayType.Civic,
            description: "Gedenktag der österreichischen Neutralitätserklärung vom 26. Oktober 1955, durch die Österreich nach zehnjähriger Besatzung durch die Alliierten seine volle Souveränität zurückerlangte. Seit 1965 gesetzlicher Feiertag."),

        new HolidayInfo(
            "at_all_saints", 11, 1, "AT",
            "Allerheiligen",
            Names("Todos los Santos", "All Saints' Day", "Dia de Todos os Santos", "Toussaint", "Allerheiligen"),
            HolidayType.Religious,
            description: "Allerheiligen / All Saints' Day. Feier aller heiliggesprochenen Christen. Gesetzlicher Feiertag in Österreich."),

        new HolidayInfo(
            "at_immaculate_conception", 12, 8, "AT",
            "Mariä Empfängnis",
            Names("Inmaculada Concepción", "Feast of the Immaculate Conception", "Imaculada Conceição", "Immaculée Conception", "Mariä Empfängnis"),
            HolidayType.Religious,
            description: "Hochfest der ohne Erbsünde empfangenen Jungfrau und Gottesmutter Maria / Feast of the Immaculate Conception of the Virgin Mary. Gesetzlicher Feiertag in Österreich."),

        new HolidayInfo(
            "at_christmas_1", 12, 25, "AT",
            "Christtag",
            Names("Navidad (primer día)", "Christmas Day", "Natal (primeiro dia)", "Noël (premier jour)", "Erster Weihnachtstag"),
            HolidayType.National,
            description: "Christtag / Erster Weihnachtstag / Christmas Day. Feier der Geburt Jesu Christi. Gesetzlicher Feiertag in Österreich."),

        new HolidayInfo(
            "at_christmas_2", 12, 26, "AT",
            "Stefanitag",
            Names("Navidad (segundo día)", "St. Stephen's Day / Second Christmas Day", "Natal (segundo dia)", "Saint-Étienne / Lendemain de Noël", "Zweiter Weihnachtstag / Stefanitag"),
            HolidayType.National,
            description: "Stefanitag / Zweiter Weihnachtstag / St. Stephen's Day. Feier des heiligen Stephanus, des ersten Märtyrers des Christentums. Gesetzlicher Feiertag in Österreich."),
    };

    /// <summary>
    /// Returns the 4 Easter-based movable national holidays of Austria for the given <paramref name="year"/>:
    /// Ostermontag, Christi Himmelfahrt, Pfingstmontag, and Fronleichnam (Corpus Christi).
    /// </summary>
    /// <param name="year">The four-digit calendar year.</param>
    /// <returns>A sequence of movable <see cref="HolidayInfo"/> entries for Austria.</returns>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        var easterMonday  = EasterCalculator.Easter(year).AddDays(1);
        var ascension     = EasterCalculator.Ascension(year);
        var whitMonday    = EasterCalculator.Easter(year).AddDays(50);
        var corpusChristi = EasterCalculator.CorpusChristi(year);

        return new[]
        {
            new HolidayInfo(
                "at_easter_monday", easterMonday.Month, easterMonday.Day, "AT",
                "Ostermontag",
                Names("Lunes de Pascua", "Easter Monday", "Segunda-Feira de Páscoa", "Lundi de Pâques", "Ostermontag"),
                HolidayType.National, isMovable: true,
                description: "Ostermontag / Easter Monday. Einen Tag nach Ostersonntag. Gesetzlicher Feiertag in Österreich."),

            new HolidayInfo(
                "at_ascension", ascension.Month, ascension.Day, "AT",
                "Christi Himmelfahrt",
                Names("Ascensión del Señor", "Ascension of Christ", "Ascensão de Cristo", "Ascension du Christ", "Christi Himmelfahrt"),
                HolidayType.National, isMovable: true,
                description: "Christi Himmelfahrt / Ascension Thursday. 39 Tage nach Ostersonntag. Gesetzlicher Feiertag in Österreich."),

            new HolidayInfo(
                "at_whit_monday", whitMonday.Month, whitMonday.Day, "AT",
                "Pfingstmontag",
                Names("Lunes de Pentecostés", "Whit Monday / Pentecost Monday", "Segunda-Feira de Pentecostes", "Lundi de Pentecôte", "Pfingstmontag"),
                HolidayType.National, isMovable: true,
                description: "Pfingstmontag / Whit Monday. 50 Tage nach Ostersonntag. Gesetzlicher Feiertag in Österreich."),

            new HolidayInfo(
                "at_corpus_christi", corpusChristi.Month, corpusChristi.Day, "AT",
                "Fronleichnam",
                Names("Corpus Christi", "Corpus Christi", "Corpo de Deus", "Fête-Dieu", "Fronleichnam"),
                HolidayType.National, isMovable: true,
                description: "Fronleichnam / Corpus Christi. 60 Tage nach Ostersonntag. Hochfest des Leibes und Blutes Christi. Gesetzlicher Feiertag in Österreich."),
        };
    }
}
