using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.Europe;

/// <summary>
/// Holiday provider for France, covering the 11 national public holidays
/// (jours fériés légaux) established by the Code du Travail (Article L3133-1),
/// plus the 2 additional regional holidays specific to Alsace-Moselle
/// (départements du Bas-Rhin FR-67, de la Moselle FR-57, and du Haut-Rhin FR-68),
/// which retain the Concordat of 1801 under local law (droit local alsacien-mosellan).
/// </summary>
public class FranceHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "FR";

    /// <inheritdoc/>
    public override string CountryName => "France";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "fr";

    /// <inheritdoc/>
    /// <remarks>
    /// Returns the 8 fixed national holidays plus the fixed regional holiday
    /// (Saint-Étienne) for Alsace-Moselle. Easter-based movable holidays are
    /// returned by <see cref="GetMovableHolidays"/>.
    /// </remarks>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays() => new[]
    {
        // ── National fixed holidays ───────────────────────────────────────────────

        new HolidayInfo(
            "fr_new_year", 1, 1, "FR",
            "Jour de l'An",
            Names("Año Nuevo", "New Year's Day", "Ano Novo", "Jour de l'An", "Neujahrstag"),
            HolidayType.National,
            description: "Célébration du premier jour de l'année civile / Celebration of the first day of the calendar year."),

        new HolidayInfo(
            "fr_labor_day", 5, 1, "FR",
            "Fête du Travail",
            Names("Día del Trabajo", "Labour Day", "Dia do Trabalho", "Fête du Travail", "Tag der Arbeit"),
            HolidayType.National,
            description: "Journée internationale des travailleurs. Seul jour férié légalement chômé et payé de droit en France, quelle que soit la convention collective."),

        new HolidayInfo(
            "fr_victory_1945", 5, 8, "FR",
            "Victoire 1945",
            Names("Victoria 1945 / Fin de la WWII en Europa", "Victory in Europe Day",
                  "Vitória 1945 / Fim da WWII na Europa", "Victoire 1945", "Tag des Sieges 1945"),
            HolidayType.Civic,
            description: "Commémoration de la capitulation de l'Allemagne nazie signée à Reims le 7 mai 1945 et ratifiée à Berlin le 8 mai 1945, marquant la fin de la Seconde Guerre mondiale en Europe."),

        new HolidayInfo(
            "fr_bastille_day", 7, 14, "FR",
            "Fête Nationale",
            Names("Día de la Bastilla / Fiesta Nacional de Francia", "Bastille Day / French National Day",
                  "Dia da Bastilha / Festa Nacional da França", "Fête Nationale / 14 juillet", "Nationalfeiertag Frankreichs"),
            HolidayType.Civic,
            description: "Commémoration de la prise de la Bastille le 14 juillet 1789, symbole de la Révolution française et de la chute de l'absolutisme monarchique."),

        new HolidayInfo(
            "fr_assumption", 8, 15, "FR",
            "Assomption de Marie",
            Names("Asunción de la Virgen", "Assumption of Mary",
                  "Assunção de Nossa Senhora", "Assomption de Marie", "Mariä Himmelfahrt"),
            HolidayType.Religious,
            description: "Fête de l'Assomption de la Vierge Marie / Feast of the Assumption of Mary."),

        new HolidayInfo(
            "fr_all_saints", 11, 1, "FR",
            "Toussaint",
            Names("Todos los Santos", "All Saints' Day",
                  "Dia de Todos os Santos", "Toussaint", "Allerheiligen"),
            HolidayType.Religious,
            description: "Fête de tous les saints de l'Église catholique / Feast of all the saints of the Catholic Church."),

        new HolidayInfo(
            "fr_armistice_1918", 11, 11, "FR",
            "Armistice 1918",
            Names("Armisticio 1918 / Fin de la Primera Guerra Mundial", "Armistice Day 1918",
                  "Armistício 1918 / Fim da Primeira Guerra Mundial", "Armistice 1918", "Waffenstillstand 1918"),
            HolidayType.Civic,
            description: "Commémoration de l'armistice signé le 11 novembre 1918 à 11h dans la forêt de Compiègne, mettant fin aux combats de la Première Guerre mondiale."),

        new HolidayInfo(
            "fr_christmas", 12, 25, "FR",
            "Noël",
            Names("Navidad", "Christmas Day", "Natal", "Noël", "Weihnachten"),
            HolidayType.National,
            description: "Célébration de la Nativité de Jésus-Christ / Celebration of the birth of Jesus Christ."),

        // ── Alsace-Moselle regional fixed holidays (FR-67, FR-57, FR-68) ──────────

        new HolidayInfo(
            "fr_alsace_saint_etienne", 12, 26, "FR",
            "Saint-Étienne",
            Names("San Esteban / Boxing Day (Alsacia-Mosela)", "St Stephen's Day / Boxing Day (Alsace-Moselle)",
                  "Santo Estêvão / Boxing Day (Alsácia-Mosela)", "Saint-Étienne (Alsace-Moselle)", "Zweiter Weihnachtstag (Alsace-Moselle)"),
            HolidayType.Regional, regionCode: "FR-67,FR-57,FR-68",
            description: "Jour férié supplémentaire en Alsace-Moselle (Bas-Rhin, Moselle, Haut-Rhin) au titre du droit local. Correspond au lendemain de Noël, célébration de Saint Étienne, premier martyr chrétien."),
    };

    /// <inheritdoc/>
    /// <remarks>
    /// Returns the 3 Easter-based movable national holidays (Lundi de Pâques,
    /// Ascension, Lundi de Pentecôte) and the movable regional holiday for
    /// Alsace-Moselle (Vendredi Saint / Good Friday).
    /// </remarks>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        var goodFriday   = EasterCalculator.GoodFriday(year);
        var easterMonday = EasterCalculator.Easter(year).AddDays(1);
        var ascension    = EasterCalculator.Ascension(year);
        var whitMonday   = EasterCalculator.Easter(year).AddDays(50);

        return new[]
        {
            new HolidayInfo(
                "fr_easter_monday", easterMonday.Month, easterMonday.Day, "FR",
                "Lundi de Pâques",
                Names("Lunes de Pascua", "Easter Monday",
                      "Segunda-Feira de Páscoa", "Lundi de Pâques", "Ostermontag"),
                HolidayType.National, isMovable: true,
                description: "Jour férié national, lendemain du dimanche de Pâques / National holiday on the day after Easter Sunday."),

            new HolidayInfo(
                "fr_ascension", ascension.Month, ascension.Day, "FR",
                "Ascension",
                Names("Ascensión del Señor", "Ascension of Christ",
                      "Ascensão de Cristo", "Ascension", "Christi Himmelfahrt"),
                HolidayType.National, isMovable: true,
                description: "Fête de l'Ascension du Christ, 39 jours après Pâques / Feast of the Ascension of Christ, 39 days after Easter Sunday."),

            new HolidayInfo(
                "fr_whit_monday", whitMonday.Month, whitMonday.Day, "FR",
                "Lundi de Pentecôte",
                Names("Lunes de Pentecostés", "Whit Monday / Pentecost Monday",
                      "Segunda-Feira de Pentecostes", "Lundi de Pentecôte", "Pfingstmontag"),
                HolidayType.National, isMovable: true,
                description: "Jour férié national, lendemain du dimanche de Pentecôte, 50 jours après Pâques. Depuis 2004 désigné 'Journée de solidarité' envers les personnes âgées et handicapées."),

            // ── Alsace-Moselle movable regional holiday ───────────────────────────

            new HolidayInfo(
                "fr_alsace_good_friday", goodFriday.Month, goodFriday.Day, "FR",
                "Vendredi Saint",
                Names("Viernes Santo (Alsacia-Mosela)", "Good Friday (Alsace-Moselle)",
                      "Sexta-Feira Santa (Alsácia-Mosela)", "Vendredi Saint (Alsace-Moselle)", "Karfreitag (Alsace-Moselle)"),
                HolidayType.Regional, isMovable: true, regionCode: "FR-67,FR-57,FR-68",
                description: "Jour férié supplémentaire en Alsace-Moselle au titre du droit local concordataire. Vendredi Saint, deux jours avant Pâques."),
        };
    }
}
