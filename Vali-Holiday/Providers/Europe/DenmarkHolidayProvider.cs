using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.Europe;

/// <summary>
/// Holiday provider for Denmark (Danmark), covering the 5 fixed public holidays and the
/// 8 Easter-based movable holidays as established by Danish law. Grundlovsdag (5 June)
/// commemorates the signing of Denmark's first constitution (Danmarks Riges Grundlov) in
/// 1849 and the revised constitution in 1953. Store Bededag (Great Prayer Day) was a
/// unique Danish holiday observed on the fourth Friday after Easter, but was abolished
/// effective 1 January 2024 (last observed in 2023) to fund increased defence spending;
/// it is included here as an <see cref="HolidayType.Observance"/> for historical completeness.
/// </summary>
public class DenmarkHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "DK";

    /// <inheritdoc/>
    public override string CountryName => "Denmark";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "da";

    /// <summary>
    /// Returns the 5 fixed public holidays of Denmark (including one optional day).
    /// Easter-based movable holidays are returned by <see cref="GetMovableHolidays"/>.
    /// </summary>
    /// <returns>A sequence of fixed <see cref="HolidayInfo"/> entries for Denmark.</returns>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays() => new[]
    {
        // ── National fixed holidays ───────────────────────────────────────────────

        new HolidayInfo(
            "dk_new_year", 1, 1, "DK",
            "Nytårsdag",
            NamesDa("Año Nuevo", "New Year's Day", "Ano Novo", "Jour de l'An", "Neujahrstag", "Nytårsdag"),
            HolidayType.National,
            description: "Årets første dag / First day of the new calendar year."),

        new HolidayInfo(
            "dk_constitution_day", 6, 5, "DK",
            "Grundlovsdag",
            NamesDa("Día de la Constitución de Dinamarca", "Danish Constitution Day", "Dia da Constituição da Dinamarca", "Jour de la Constitution danoise", "Dänischer Verfassungstag", "Grundlovsdag"),
            HolidayType.Civic,
            description: "Grundlovsdag fejrer underskrivelsen af Danmarks første grundlov den 5. juni 1849 og den reviderede grundlov i 1953. Halv fridag i mange virksomheder; officiel fridag i det offentlige."),

        new HolidayInfo(
            "dk_christmas_eve", 12, 24, "DK",
            "Juleaftensdag",
            NamesDa("Nochebuena", "Christmas Eve", "Véspera de Natal", "Réveillon de Noël", "Heiliger Abend", "Juleaften"),
            HolidayType.Optional,
            description: "Juleaften / Christmas Eve. I Danmark fejres julen traditionelt på juleaftensdag med gaver og middag. Halvdag; i praksis fri dag for de fleste."),

        new HolidayInfo(
            "dk_christmas_day", 12, 25, "DK",
            "Juledag",
            NamesDa("Navidad (primer día)", "Christmas Day", "Natal (primeiro dia)", "Noël (premier jour)", "Erster Weihnachtstag", "Juledag"),
            HolidayType.National,
            description: "Første juledag / Christmas Day. Fejring af Jesu Kristi fødsel."),

        new HolidayInfo(
            "dk_boxing_day", 12, 26, "DK",
            "Anden juledag",
            NamesDa("Navidad (segundo día)", "Boxing Day / Second Christmas Day", "Natal (segundo dia)", "Lendemain de Noël", "Zweiter Weihnachtstag", "Anden juledag"),
            HolidayType.National,
            description: "Anden juledag / Second Christmas Day. Dagen efter første juledag."),
    };

    /// <summary>
    /// Returns the 8 Easter-based movable holidays of Denmark for the given <paramref name="year"/>:
    /// Skærtorsdag (Holy Thursday), Langfredag (Good Friday), Påskedag (Easter Sunday),
    /// Anden påskedag (Easter Monday), Store bededag (Great Prayer Day — abolished 2023,
    /// included as Observance), Kristi Himmelfartsdag (Ascension), Pinsedag (Pentecost),
    /// and Anden pinsedag (Whit Monday).
    /// </summary>
    /// <param name="year">The four-digit calendar year.</param>
    /// <returns>A sequence of movable <see cref="HolidayInfo"/> entries for Denmark.</returns>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        var holyThursday     = EasterCalculator.HolyThursday(year);
        var goodFriday       = EasterCalculator.GoodFriday(year);
        var easter           = EasterCalculator.Easter(year);
        var easterMonday     = easter.AddDays(1);
        var greatPrayerDay   = easter.AddDays(26);   // Store Bededag: 4th Friday after Easter
        var ascension        = EasterCalculator.Ascension(year);
        var pentecost        = easter.AddDays(49);
        var whitMonday       = easter.AddDays(50);

        return new[]
        {
            new HolidayInfo(
                "dk_holy_thursday", holyThursday.Month, holyThursday.Day, "DK",
                "Skærtorsdag",
                NamesDa("Jueves Santo", "Holy Thursday / Maundy Thursday", "Quinta-Feira Santa", "Jeudi Saint", "Gründonnerstag", "Skærtorsdag"),
                HolidayType.National, isMovable: true,
                description: "Skærtorsdag / Maundy Thursday. Tre dage før påskedag. Mindes Jesu sidste nadver."),

            new HolidayInfo(
                "dk_good_friday", goodFriday.Month, goodFriday.Day, "DK",
                "Langfredag",
                NamesDa("Viernes Santo", "Good Friday", "Sexta-Feira Santa", "Vendredi Saint", "Karfreitag", "Langfredag"),
                HolidayType.National, isMovable: true,
                description: "Langfredag / Good Friday. To dage før påskedag. Mindes Jesu Kristi korsfæstelse."),

            new HolidayInfo(
                "dk_easter_sunday", easter.Month, easter.Day, "DK",
                "Påskedag",
                NamesDa("Pascua (Domingo)", "Easter Sunday", "Páscoa (Domingo)", "Pâques (Dimanche)", "Ostersonntag", "Første påskedag"),
                HolidayType.National, isMovable: true,
                description: "Første påskedag / Easter Sunday. Fejring af Jesu Kristi opstandelse fra de døde."),

            new HolidayInfo(
                "dk_easter_monday", easterMonday.Month, easterMonday.Day, "DK",
                "Anden påskedag",
                NamesDa("Lunes de Pascua", "Easter Monday", "Segunda-Feira de Páscoa", "Lundi de Pâques", "Ostermontag", "Anden påskedag"),
                HolidayType.National, isMovable: true,
                description: "Anden påskedag / Easter Monday. Dagen efter påskedag."),

            new HolidayInfo(
                "dk_great_prayer_day", greatPrayerDay.Month, greatPrayerDay.Day, "DK",
                "Store Bededag",
                NamesDa("Gran Día de Oración (abolido 2023)", "Great Prayer Day (abolished 2023)", "Grande Dia de Oração (abolido 2023)", "Grand Jour de Prière (aboli 2023)", "Großer Bußtag (abgeschafft 2023)", "Store Bededag (afskaffet 2023)"),
                HolidayType.Observance, isMovable: true,
                description: "Store Bededag / Great Prayer Day. Unik dansk helligdag, 26 dage efter påskedag (4. fredag efter påske). Oprettet i 1686 ved sammenlægning af flere omvendelsesdage. Afskaffet med virkning fra 1. januar 2024 for at finansiere forsvarsudgifter. Medtaget som historisk observans."),

            new HolidayInfo(
                "dk_ascension", ascension.Month, ascension.Day, "DK",
                "Kristi Himmelfartsdag",
                NamesDa("Ascensión del Señor", "Ascension Day", "Ascensão do Senhor", "Ascension du Christ", "Christi Himmelfahrt", "Kristi Himmelfartsdag"),
                HolidayType.National, isMovable: true,
                description: "Kristi Himmelfartsdag / Ascension Day. 39 dage efter påskedag."),

            new HolidayInfo(
                "dk_pentecost", pentecost.Month, pentecost.Day, "DK",
                "Pinsedag",
                NamesDa("Pentecostés", "Pentecost Sunday / Whit Sunday", "Pentecostes", "Pentecôte", "Pfingstsonntag", "Første pinsedag"),
                HolidayType.National, isMovable: true,
                description: "Første pinsedag / Pentecost Sunday. 49 dage efter påskedag."),

            new HolidayInfo(
                "dk_whit_monday", whitMonday.Month, whitMonday.Day, "DK",
                "Anden pinsedag",
                NamesDa("Lunes de Pentecostés", "Whit Monday / Pentecost Monday", "Segunda-Feira de Pentecostes", "Lundi de Pentecôte", "Pfingstmontag", "Anden pinsedag"),
                HolidayType.National, isMovable: true,
                description: "Anden pinsedag / Whit Monday. 50 dage efter påskedag."),
        };
    }

    /// <summary>
    /// Builds a multilingual name dictionary that includes a Danish (<c>da</c>) entry
    /// in addition to the standard five languages.
    /// </summary>
    /// <param name="es">Spanish name.</param>
    /// <param name="en">English name.</param>
    /// <param name="pt">Portuguese name.</param>
    /// <param name="fr">French name.</param>
    /// <param name="de">German name.</param>
    /// <param name="da">Danish name.</param>
    /// <returns>A read-only dictionary keyed by BCP 47 language tag.</returns>
    private static IReadOnlyDictionary<string, string> NamesDa(
        string es, string en, string pt, string fr, string de, string da)
        => new Dictionary<string, string>
        {
            ["es"] = es,
            ["en"] = en,
            ["pt"] = pt,
            ["fr"] = fr,
            ["de"] = de,
            ["da"] = da,
        };
}
