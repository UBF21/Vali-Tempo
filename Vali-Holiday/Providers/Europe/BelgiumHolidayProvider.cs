using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.Europe;

/// <summary>
/// Holiday provider for Belgium (Belgique / België / Belgien), covering the 10 official
/// public holidays established by Belgian federal law (loi du 4 janvier 1974 relative aux
/// jours fériés). Belgium has three official languages (French, Dutch, German) and three
/// federal Regions (Brussels, Wallonia, Flanders), each with its own community holiday.
/// This provider covers the 10 national holidays. The Fête Nationale (21 juillet) marks the
/// date in 1831 when Léopold I took the oath as the first King of the Belgians.
/// </summary>
public class BelgiumHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "BE";

    /// <inheritdoc/>
    public override string CountryName => "Belgium";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "fr";

    /// <summary>
    /// Returns the 7 fixed national holidays of Belgium.
    /// Easter-based movable holidays are returned by <see cref="GetMovableHolidays"/>.
    /// </summary>
    /// <returns>A sequence of fixed <see cref="HolidayInfo"/> entries for Belgium.</returns>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays() => new[]
    {
        // ── National fixed holidays ───────────────────────────────────────────────

        new HolidayInfo(
            "be_new_year", 1, 1, "BE",
            "Nouvel An",
            Names("Año Nuevo", "New Year's Day", "Ano Novo", "Nouvel An", "Neujahrstag"),
            HolidayType.National,
            description: "Premier jour de l'année civile / Eerste dag van het nieuwe jaar / Celebration of the start of the new calendar year."),

        new HolidayInfo(
            "be_labor_day", 5, 1, "BE",
            "Fête du Travail",
            Names("Día del Trabajo", "Labour Day", "Dia do Trabalho", "Fête du Travail", "Tag der Arbeit"),
            HolidayType.National,
            description: "Journée internationale des travailleurs / Internationale Dag van de Arbeid. Célébration du mouvement ouvrier et des droits des travailleurs."),

        new HolidayInfo(
            "be_national_day", 7, 21, "BE",
            "Fête Nationale",
            Names("Fiesta Nacional de Bélgica", "Belgian National Day", "Dia Nacional da Bélgica", "Fête Nationale belge", "Belgischer Nationalfeiertag"),
            HolidayType.Civic,
            description: "Commémoration du 21 juillet 1831, date à laquelle Léopold I prêta serment comme premier roi des Belges après l'indépendance vis-à-vis des Pays-Bas. Nationale feestdag van België / Nationaler Feiertag Belgiens."),

        new HolidayInfo(
            "be_assumption", 8, 15, "BE",
            "Assomption",
            Names("Asunción de la Virgen", "Assumption of Mary", "Assunção de Nossa Senhora", "Assomption de Marie", "Mariä Himmelfahrt"),
            HolidayType.Religious,
            description: "Fête de l'Assomption de la Vierge Marie / Feast of the Assumption of Mary. Jour férié légal en Belgique."),

        new HolidayInfo(
            "be_all_saints", 11, 1, "BE",
            "Toussaint",
            Names("Todos los Santos", "All Saints' Day", "Dia de Todos os Santos", "Toussaint", "Allerheiligen"),
            HolidayType.Religious,
            description: "Fête catholique en l'honneur de tous les saints / Catholic feast honouring all saints. Jour de recueillement et de visite aux cimetières en Belgique."),

        new HolidayInfo(
            "be_armistice", 11, 11, "BE",
            "Armistice",
            Names("Armisticio 1918", "Armistice Day", "Armistício 1918", "Armistice 1918", "Waffenstillstand 1918"),
            HolidayType.Civic,
            description: "Commémoration de l'armistice du 11 novembre 1918 mettant fin à la Première Guerre mondiale. La Belgique fut profondément touchée par les deux guerres mondiales, notamment les batailles de l'Yser et de Passchendaele sur son territoire."),

        new HolidayInfo(
            "be_christmas", 12, 25, "BE",
            "Noël",
            Names("Navidad", "Christmas Day", "Natal", "Noël", "Weihnachten"),
            HolidayType.National,
            description: "Célébration de la Nativité de Jésus-Christ / Celebration of the birth of Jesus Christ."),
    };

    /// <summary>
    /// Returns the 3 Easter-based movable national holidays of Belgium for the given <paramref name="year"/>:
    /// Lundi de Pâques (Easter Monday), Ascension, and Lundi de Pentecôte (Whit Monday).
    /// </summary>
    /// <param name="year">The four-digit calendar year.</param>
    /// <returns>A sequence of movable <see cref="HolidayInfo"/> entries for Belgium.</returns>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        var easterMonday = EasterCalculator.Easter(year).AddDays(1);
        var ascension    = EasterCalculator.Ascension(year);
        var whitMonday   = EasterCalculator.Easter(year).AddDays(50);

        return new[]
        {
            new HolidayInfo(
                "be_easter_monday", easterMonday.Month, easterMonday.Day, "BE",
                "Lundi de Pâques",
                Names("Lunes de Pascua", "Easter Monday", "Segunda-Feira de Páscoa", "Lundi de Pâques", "Ostermontag"),
                HolidayType.National, isMovable: true,
                description: "Jour férié national, lendemain du dimanche de Pâques. Eerste paasdag plus één dag / Ostermontagsfeier."),

            new HolidayInfo(
                "be_ascension", ascension.Month, ascension.Day, "BE",
                "Ascension",
                Names("Ascensión del Señor", "Ascension of Christ", "Ascensão de Cristo", "Ascension", "Christi Himmelfahrt"),
                HolidayType.National, isMovable: true,
                description: "Fête de l'Ascension du Christ, 39 jours après Pâques. Jour férié légal en Belgique."),

            new HolidayInfo(
                "be_whit_monday", whitMonday.Month, whitMonday.Day, "BE",
                "Lundi de Pentecôte",
                Names("Lunes de Pentecostés", "Whit Monday / Pentecost Monday", "Segunda-Feira de Pentecostes", "Lundi de Pentecôte", "Pfingstmontag"),
                HolidayType.National, isMovable: true,
                description: "Jour férié national, lendemain du dimanche de Pentecôte, 50 jours après Pâques."),
        };
    }
}
