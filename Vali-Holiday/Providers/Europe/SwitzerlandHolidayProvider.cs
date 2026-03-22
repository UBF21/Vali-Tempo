using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.Europe;

/// <summary>
/// Holiday provider for Switzerland (Schweiz / Suisse / Svizzera), covering the 3 national
/// public holidays recognised by federal law (Bundesgesetz über die Arbeit) plus the 6
/// Easter-based movable national holidays. Switzerland is a federal state of 26 cantons,
/// each sovereign in determining its own public holidays beyond the federal minimum.
/// The Bundesfeiertag (1 August) commemorates the Federal Charter of 1291 (Bundesbrief)
/// signed by the forest cantons of Uri, Schwyz and Unterwalden — considered the founding
/// act of the Swiss Confederation. Regional entries include Stephanstag (26 December) in
/// the canton of Bern (CH-BE) and Berchtoldstag (2 January) observed in several cantons.
/// </summary>
public class SwitzerlandHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "CH";

    /// <inheritdoc/>
    public override string CountryName => "Switzerland";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "de";

    /// <summary>
    /// Returns the fixed national holidays of Switzerland plus selected cantonal holidays.
    /// Easter-based movable holidays are returned by <see cref="GetMovableHolidays"/>.
    /// </summary>
    /// <returns>A sequence of fixed <see cref="HolidayInfo"/> entries for Switzerland.</returns>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays() => new[]
    {
        // ── National fixed holidays ───────────────────────────────────────────────

        new HolidayInfo(
            "ch_new_year", 1, 1, "CH",
            "Neujahr",
            Names("Año Nuevo", "New Year's Day", "Ano Novo", "Nouvel An", "Neujahr"),
            HolidayType.National,
            description: "Neujahrstag / Nouvel An / Capodanno. Erster Tag des neuen Jahres, bundesweiter gesetzlicher Feiertag."),

        new HolidayInfo(
            "ch_national_day", 8, 1, "CH",
            "Bundesfeiertag",
            Names("Día Nacional de Suiza", "Swiss National Day", "Dia Nacional da Suíça", "Fête nationale suisse", "Bundesfeiertag"),
            HolidayType.Civic,
            description: "Gedenktag der Unterzeichnung des Bundesbriefs am 1. August 1291 durch die Urkantone Uri, Schwyz und Unterwalden. Seit 1994 offizieller Bundesfeiertag (früher nur inoffiziell gefeiert). Nationaler Feiertag der Schweizerischen Eidgenossenschaft."),

        new HolidayInfo(
            "ch_christmas", 12, 25, "CH",
            "Weihnachten",
            Names("Navidad", "Christmas Day", "Natal", "Noël", "Weihnachten"),
            HolidayType.National,
            description: "Weihnachtstag / Noël / Natale. Feier der Geburt Jesu Christi. Bundesweiter gesetzlicher Feiertag."),

        // ── Kanton Bern (CH-BE) – regional fixed holidays ─────────────────────────

        new HolidayInfo(
            "ch_be_stephanstag", 12, 26, "CH",
            "Stephanstag",
            Names("San Esteban", "St. Stephen's Day / Boxing Day", "Santo Estêvão", "Saint-Étienne", "Stephanstag"),
            HolidayType.Regional, regionCode: "CH-BE",
            description: "Stephanstag / Saint-Étienne. Gesetzlicher Feiertag im Kanton Bern und weiteren Kantonen. Feier des heiligen Stephanus, des ersten christlichen Märtyrers."),

        // ── Mehrere Kantone – Berchtoldstag ───────────────────────────────────────

        new HolidayInfo(
            "ch_berchtoldstag", 1, 2, "CH",
            "Berchtoldstag",
            Names("Día de Berchtold", "Berchtold's Day", "Dia de Berchtold", "Jour de Berchtold", "Berchtoldstag"),
            HolidayType.Regional, regionCode: "CH-BE",
            description: "Berchtoldstag, 2. Januar. Kantonaler Feiertag in Bern, Aargau, Thurgau, Schaffhausen und weiteren Kantonen. Traditionell der Tag nach Neujahr, geht auf den Herzog Berchtold V. von Zähringen zurück."),
    };

    /// <summary>
    /// Returns the Easter-based movable national holidays of Switzerland for the given <paramref name="year"/>:
    /// Karfreitag (Good Friday), Ostersonntag, Ostermontag, Auffahrt (Ascension),
    /// Pfingstsonntag, and Pfingstmontag.
    /// </summary>
    /// <param name="year">The four-digit calendar year.</param>
    /// <returns>A sequence of movable <see cref="HolidayInfo"/> entries for Switzerland.</returns>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        var goodFriday   = EasterCalculator.GoodFriday(year);
        var easter       = EasterCalculator.Easter(year);
        var easterMonday = easter.AddDays(1);
        var ascension    = EasterCalculator.Ascension(year);
        var pentecost    = easter.AddDays(49);
        var whitMonday   = easter.AddDays(50);

        return new[]
        {
            new HolidayInfo(
                "ch_good_friday", goodFriday.Month, goodFriday.Day, "CH",
                "Karfreitag",
                Names("Viernes Santo", "Good Friday", "Sexta-Feira Santa", "Vendredi Saint", "Karfreitag"),
                HolidayType.National, isMovable: true,
                description: "Karfreitag / Vendredi Saint / Venerdì Santo. Gedenktag der Kreuzigung Jesu Christi, zwei Tage vor Ostersonntag. Bundesweiter gesetzlicher Feiertag in der Schweiz."),

            new HolidayInfo(
                "ch_easter_sunday", easter.Month, easter.Day, "CH",
                "Ostersonntag",
                Names("Pascua (Domingo)", "Easter Sunday", "Páscoa (Domingo)", "Pâques (Dimanche)", "Ostersonntag"),
                HolidayType.National, isMovable: true,
                description: "Ostersonntag / Pâques / Pasqua. Feier der Auferstehung Jesu Christi."),

            new HolidayInfo(
                "ch_easter_monday", easterMonday.Month, easterMonday.Day, "CH",
                "Ostermontag",
                Names("Lunes de Pascua", "Easter Monday", "Segunda-Feira de Páscoa", "Lundi de Pâques", "Ostermontag"),
                HolidayType.National, isMovable: true,
                description: "Ostermontag / Lundi de Pâques / Lunedì dell'Angelo. Einen Tag nach Ostersonntag. Gesetzlicher Feiertag in der Schweiz."),

            new HolidayInfo(
                "ch_ascension", ascension.Month, ascension.Day, "CH",
                "Auffahrt",
                Names("Ascensión del Señor", "Ascension Day", "Ascensão do Senhor", "Ascension", "Christi Himmelfahrt"),
                HolidayType.National, isMovable: true,
                description: "Auffahrt / Christi Himmelfahrt / Ascension. 39 Tage nach Ostersonntag. Bundesweiter gesetzlicher Feiertag."),

            new HolidayInfo(
                "ch_pentecost_sunday", pentecost.Month, pentecost.Day, "CH",
                "Pfingstsonntag",
                Names("Pentecostés (Domingo)", "Pentecost Sunday / Whit Sunday", "Pentecostes (Domingo)", "Pentecôte (Dimanche)", "Pfingstsonntag"),
                HolidayType.National, isMovable: true,
                description: "Pfingstsonntag / Dimanche de Pentecôte. 49 Tage nach Ostersonntag. Feier der Herabkunft des Heiligen Geistes."),

            new HolidayInfo(
                "ch_whit_monday", whitMonday.Month, whitMonday.Day, "CH",
                "Pfingstmontag",
                Names("Lunes de Pentecostés", "Whit Monday / Pentecost Monday", "Segunda-Feira de Pentecostes", "Lundi de Pentecôte", "Pfingstmontag"),
                HolidayType.National, isMovable: true,
                description: "Pfingstmontag / Lundi de Pentecôte. 50 Tage nach Ostersonntag. Gesetzlicher Feiertag in der Schweiz."),
        };
    }
}
