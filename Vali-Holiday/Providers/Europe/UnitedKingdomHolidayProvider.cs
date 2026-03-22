using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.Europe;

/// <summary>
/// Holiday provider for the United Kingdom, covering the Bank Holidays for England and
/// Wales as defined under the Banking and Financial Dealings Act 1971, plus regional
/// holidays specific to Scotland (GB-SCT) and Northern Ireland (GB-NIR).
/// </summary>
public class UnitedKingdomHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "GB";

    /// <inheritdoc/>
    public override string CountryName => "United Kingdom";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "en";

    /// <inheritdoc/>
    /// <remarks>
    /// Returns all fixed Bank Holidays for England and Wales, plus fixed regional
    /// bank holidays for Scotland and Northern Ireland. Easter-based movable holidays
    /// are returned by <see cref="GetMovableHolidays"/>.
    /// </remarks>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays() => new[]
    {
        // ── England & Wales – fixed bank holidays ─────────────────────────────────

        new HolidayInfo(
            "gb_new_year", 1, 1, "GB",
            "New Year's Day",
            Names("Año Nuevo", "New Year's Day", "Ano Novo", "Jour de l'An", "Neujahrstag"),
            HolidayType.National,
            description: "Bank holiday marking the first day of the calendar year."),

        new HolidayInfo(
            "gb_christmas", 12, 25, "GB",
            "Christmas Day",
            Names("Navidad", "Christmas Day", "Natal", "Noël", "Weihnachten"),
            HolidayType.National,
            description: "Bank holiday celebrating Christmas."),

        new HolidayInfo(
            "gb_boxing_day", 12, 26, "GB",
            "Boxing Day",
            Names("Día de San Esteban / Boxing Day", "Boxing Day",
                  "Dia de Santo Estêvão / Boxing Day", "Lendemain de Noël", "Zweiter Weihnachtstag"),
            HolidayType.National,
            description: "Bank holiday on the day after Christmas, historically the day when servants and tradespeople received gifts ('boxes') from their employers."),

        // ── Scotland (GB-SCT) – fixed regional bank holidays ──────────────────────

        new HolidayInfo(
            "gb_sct_second_january", 1, 2, "GB",
            "Second of January",
            Names("2 de enero", "Second of January",
                  "2 de janeiro", "2 janvier", "2. Januar"),
            HolidayType.Regional, regionCode: "GB-SCT",
            description: "A Scottish bank holiday traditionally allowing time for recovery after Hogmanay (New Year) celebrations."),

        new HolidayInfo(
            "gb_sct_st_andrews", 11, 30, "GB",
            "St Andrew's Day",
            Names("Día de San Andrés", "St Andrew's Day",
                  "Dia de Santo André", "Jour de Saint-André", "Andreastag"),
            HolidayType.Regional, regionCode: "GB-SCT",
            description: "Feast day of Saint Andrew, patron saint of Scotland. Designated a Scottish bank holiday by the St Andrew's Day Bank Holiday (Scotland) Act 2007."),

        // ── Northern Ireland (GB-NIR) – fixed regional bank holidays ──────────────

        new HolidayInfo(
            "gb_nir_st_patrick", 3, 17, "GB",
            "St Patrick's Day",
            Names("Día de San Patricio", "St Patrick's Day",
                  "Dia de São Patrício", "Jour de la Saint-Patrick", "Patrickstag"),
            HolidayType.Regional, regionCode: "GB-NIR",
            description: "Feast day of Saint Patrick, patron saint of Ireland. Bank holiday in Northern Ireland commemorating the arrival of Christianity in Ireland."),

        new HolidayInfo(
            "gb_nir_battle_boyne", 7, 12, "GB",
            "Battle of the Boyne",
            Names("Batalla del Boyne / Día de Orangemen", "Battle of the Boyne / Orangemen's Day",
                  "Batalha do Boyne", "Bataille de la Boyne", "Schlacht am Boyne"),
            HolidayType.Regional, regionCode: "GB-NIR",
            description: "Commemorates the Battle of the Boyne (July 1, 1690 O.S. / July 11, 1690 N.S.), in which William III of Orange defeated James II. Celebrated by the Orange Order with marches across Northern Ireland."),
    };

    /// <inheritdoc/>
    /// <remarks>
    /// Returns the Easter-based and floating weekday bank holidays: Good Friday, Easter Monday,
    /// Early May Bank Holiday (first Monday of May), Spring Bank Holiday (last Monday of May),
    /// and Summer Bank Holiday (last Monday of August).
    /// </remarks>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        var goodFriday  = EasterCalculator.GoodFriday(year);
        var easterMonday = EasterCalculator.Easter(year).AddDays(1);
        var earlyMay     = NthWeekday(year, 5, DayOfWeek.Monday, 1);
        var springBank   = LastWeekday(year, 5, DayOfWeek.Monday);
        var summerBank   = LastWeekday(year, 8, DayOfWeek.Monday);

        return new[]
        {
            new HolidayInfo(
                "gb_good_friday", goodFriday.Month, goodFriday.Day, "GB",
                "Good Friday",
                Names("Viernes Santo", "Good Friday",
                      "Sexta-Feira Santa", "Vendredi Saint", "Karfreitag"),
                HolidayType.National, isMovable: true,
                description: "Bank holiday commemorating the crucifixion of Jesus Christ, observed two days before Easter Sunday."),

            new HolidayInfo(
                "gb_easter_monday", easterMonday.Month, easterMonday.Day, "GB",
                "Easter Monday",
                Names("Lunes de Pascua", "Easter Monday",
                      "Segunda-Feira de Páscoa", "Lundi de Pâques", "Ostermontag"),
                HolidayType.National, isMovable: true,
                description: "Bank holiday on the day after Easter Sunday."),

            new HolidayInfo(
                "gb_early_may", earlyMay.Month, earlyMay.Day, "GB",
                "Early May Bank Holiday",
                Names("Festivo de Principios de Mayo", "Early May Bank Holiday",
                      "Feriado Bancário de Início de Maio", "Jour férié de début mai", "Frühjahrsfeiertag"),
                HolidayType.National, isMovable: true,
                description: "Bank holiday observed on the first Monday of May. Introduced in 1978 as the United Kingdom's equivalent to Labour Day."),

            new HolidayInfo(
                "gb_spring_bank", springBank.Month, springBank.Day, "GB",
                "Spring Bank Holiday",
                Names("Festivo de Primavera", "Spring Bank Holiday",
                      "Feriado Bancário de Primavera", "Jour férié de printemps", "Frühlingsfeiertag"),
                HolidayType.National, isMovable: true,
                description: "Bank holiday observed on the last Monday of May, replacing the former Whit Monday observance."),

            new HolidayInfo(
                "gb_summer_bank", summerBank.Month, summerBank.Day, "GB",
                "Summer Bank Holiday",
                Names("Festivo de Verano", "Summer Bank Holiday",
                      "Feriado Bancário de Verão", "Jour férié d'été", "Sommerfeiertag"),
                HolidayType.National, isMovable: true,
                description: "Bank holiday observed on the last Monday of August in England, Wales, and Northern Ireland."),
        };
    }

    /// <summary>
    /// Returns the <paramref name="n"/>th occurrence of <paramref name="weekday"/> in the
    /// given <paramref name="month"/> of <paramref name="year"/>.
    /// </summary>
    private static DateTime NthWeekday(int year, int month, DayOfWeek weekday, int n)
    {
        var first = new DateTime(year, month, 1);
        int daysUntil = ((int)weekday - (int)first.DayOfWeek + 7) % 7;
        return first.AddDays(daysUntil + (n - 1) * 7);
    }

    /// <summary>
    /// Returns the last occurrence of <paramref name="weekday"/> in the given
    /// <paramref name="month"/> of <paramref name="year"/>.
    /// </summary>
    private static DateTime LastWeekday(int year, int month, DayOfWeek weekday)
    {
        var last = new DateTime(year, month, DateTime.DaysInMonth(year, month));
        int daysBack = ((int)last.DayOfWeek - (int)weekday + 7) % 7;
        return last.AddDays(-daysBack);
    }
}
