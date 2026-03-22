using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.Europe;

/// <summary>
/// Holiday provider for the Netherlands (Nederland), covering the 7 fixed national holidays
/// and the 7 Easter-based movable public holidays as recognised under Dutch law.
/// Koningsdag (King's Day, 27 April) celebrates the birthday of King Willem-Alexander;
/// Bevrijdingsdag (Liberation Day, 5 May) commemorates the end of German occupation in 1945
/// and is culturally observed every year, though it is an officially mandated public holiday
/// only every five years in accordance with the 1990 governmental agreement.
/// </summary>
public class NetherlandsHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "NL";

    /// <inheritdoc/>
    public override string CountryName => "Netherlands";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "nl";

    /// <summary>
    /// Returns the fixed national holidays of the Netherlands.
    /// Easter-based movable holidays are returned by <see cref="GetMovableHolidays"/>.
    /// </summary>
    /// <returns>A sequence of fixed <see cref="HolidayInfo"/> entries for the Netherlands.</returns>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays() => new[]
    {
        // ── National fixed holidays ───────────────────────────────────────────────

        new HolidayInfo(
            "nl_new_year", 1, 1, "NL",
            "Nieuwjaarsdag",
            NamesNl("Año Nuevo", "New Year's Day", "Ano Novo", "Jour de l'An", "Neujahrstag", "Nieuwjaarsdag"),
            HolidayType.National,
            description: "Eerste dag van het nieuwe jaar / Celebration of the first day of the new calendar year."),

        new HolidayInfo(
            "nl_kings_day", 4, 27, "NL",
            "Koningsdag",
            NamesNl("Día del Rey", "King's Day", "Dia do Rei", "Jour du Roi", "Königstag", "Koningsdag"),
            HolidayType.National,
            description: "Verjaardag van Koning Willem-Alexander (27 april 1967). Depuis 2014 remplace la Koninginnedag (30 april), qui célébrait l'anniversaire de la reine Beatrix. Journée nationale marquée par des marchés aux puces orange et des festivités dans tout le pays."),

        new HolidayInfo(
            "nl_liberation_day", 5, 5, "NL",
            "Bevrijdingsdag",
            NamesNl("Día de la Liberación", "Liberation Day", "Dia da Libertação", "Jour de la Libération", "Befreiungstag", "Bevrijdingsdag"),
            HolidayType.Civic,
            description: "Herdenking van de bevrijding van Nederland van de Duitse bezetting op 5 mei 1945. Cultureel jaarlijks gevierd; officieel verplichte vrije dag elke vijf jaar (lustrum) volgens het akkoord van 1990. Nationale feestdag met festivals, concerten en herdenkingen."),

        new HolidayInfo(
            "nl_christmas_1", 12, 25, "NL",
            "Eerste Kerstdag",
            NamesNl("Navidad (primer día)", "Christmas Day", "Natal (primeiro dia)", "Noël (premier jour)", "Erster Weihnachtstag", "Eerste Kerstdag"),
            HolidayType.National,
            description: "Eerste kerstdag / Christmas Day. Viering van de geboorte van Jezus Christus."),

        new HolidayInfo(
            "nl_christmas_2", 12, 26, "NL",
            "Tweede Kerstdag",
            NamesNl("Navidad (segundo día)", "Boxing Day / Second Christmas Day", "Natal (segundo dia)", "Lendemain de Noël", "Zweiter Weihnachtstag", "Tweede Kerstdag"),
            HolidayType.National,
            description: "Tweede kerstdag / Second Christmas Day. Traditioneel een dag voor familie en rust."),
    };

    /// <summary>
    /// Returns the Easter-based movable holidays for the Netherlands for the given <paramref name="year"/>.
    /// Includes Goede Vrijdag (Good Friday), both Easter days, Hemelvaartsdag (Ascension),
    /// and both Pentecost days.
    /// </summary>
    /// <param name="year">The four-digit calendar year.</param>
    /// <returns>A sequence of movable <see cref="HolidayInfo"/> entries for the Netherlands.</returns>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        var goodFriday      = EasterCalculator.GoodFriday(year);
        var easterSunday    = EasterCalculator.Easter(year);
        var easterMonday    = easterSunday.AddDays(1);
        var ascension       = EasterCalculator.Ascension(year);
        var pentecost       = easterSunday.AddDays(49);
        var whitMonday      = easterSunday.AddDays(50);

        return new[]
        {
            new HolidayInfo(
                "nl_good_friday", goodFriday.Month, goodFriday.Day, "NL",
                "Goede Vrijdag",
                NamesNl("Viernes Santo", "Good Friday", "Sexta-Feira Santa", "Vendredi Saint", "Karfreitag", "Goede Vrijdag"),
                HolidayType.National, isMovable: true,
                description: "Herdenking van de kruisiging van Jezus Christus, twee dagen vóór Pasen. Officieel nationale feestdag in Nederland."),

            new HolidayInfo(
                "nl_easter_sunday", easterSunday.Month, easterSunday.Day, "NL",
                "Eerste Paasdag",
                NamesNl("Pascua (primer día)", "Easter Sunday", "Páscoa (primeiro dia)", "Pâques (premier jour)", "Ostersonntag", "Eerste Paasdag"),
                HolidayType.National, isMovable: true,
                description: "Eerste paasdag. Viering van de opstanding van Jezus Christus uit de dood."),

            new HolidayInfo(
                "nl_easter_monday", easterMonday.Month, easterMonday.Day, "NL",
                "Tweede Paasdag",
                NamesNl("Pascua (segundo día)", "Easter Monday", "Páscoa (segundo dia)", "Lundi de Pâques", "Ostermontag", "Tweede Paasdag"),
                HolidayType.National, isMovable: true,
                description: "Tweede paasdag, een dag na Eerste Paasdag. Nationale feestdag."),

            new HolidayInfo(
                "nl_ascension", ascension.Month, ascension.Day, "NL",
                "Hemelvaartsdag",
                NamesNl("Ascensión del Señor", "Ascension Day", "Ascensão do Senhor", "Ascension", "Christi Himmelfahrt", "Hemelvaartsdag"),
                HolidayType.National, isMovable: true,
                description: "Hemelvaartsdag, 39 dagen na Pasen. Viering van de hemelvaart van Jezus Christus. Nationale feestdag."),

            new HolidayInfo(
                "nl_pentecost_sunday", pentecost.Month, pentecost.Day, "NL",
                "Eerste Pinksterdag",
                NamesNl("Pentecostés (primer día)", "Pentecost Sunday / Whit Sunday", "Pentecostes (primeiro dia)", "Pentecôte (premier jour)", "Pfingstsonntag", "Eerste Pinksterdag"),
                HolidayType.National, isMovable: true,
                description: "Eerste Pinksterdag, 49 dagen na Pasen. Viering van de nederdaling van de Heilige Geest."),

            new HolidayInfo(
                "nl_pentecost_monday", whitMonday.Month, whitMonday.Day, "NL",
                "Tweede Pinksterdag",
                NamesNl("Pentecostés (segundo día)", "Whit Monday / Pentecost Monday", "Pentecostes (segundo dia)", "Lundi de Pentecôte", "Pfingstmontag", "Tweede Pinksterdag"),
                HolidayType.National, isMovable: true,
                description: "Tweede Pinksterdag, 50 dagen na Pasen. Nationale feestdag."),
        };
    }

    /// <summary>
    /// Builds a multilingual name dictionary that includes a Dutch (<c>nl</c>) entry
    /// in addition to the standard five languages.
    /// </summary>
    /// <param name="es">Spanish name.</param>
    /// <param name="en">English name.</param>
    /// <param name="pt">Portuguese name.</param>
    /// <param name="fr">French name.</param>
    /// <param name="de">German name.</param>
    /// <param name="nl">Dutch name.</param>
    /// <returns>A read-only dictionary keyed by BCP 47 language tag.</returns>
    private static IReadOnlyDictionary<string, string> NamesNl(
        string es, string en, string pt, string fr, string de, string nl)
        => new Dictionary<string, string>
        {
            ["es"] = es,
            ["en"] = en,
            ["pt"] = pt,
            ["fr"] = fr,
            ["de"] = de,
            ["nl"] = nl,
        };
}
