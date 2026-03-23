using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.Europe;

/// <summary>
/// Holiday provider for Ireland (Éire), covering the 10 public holidays (laethanta saoire
/// poiblí) as established by the Organisation of Working Time Act 1997 and its amendments.
/// St. Brigid's Day (first Monday of February, or 1 February if that is a Friday) was
/// added in 2023 as Ireland's first new public holiday in decades, honouring the patron
/// saint Brigid of Kildare and coinciding with the Celtic festival of Imbolc. Floating
/// Monday holidays (May, June, August, October) are listed with approximate representative
/// dates and their floating rule is noted in the description. St. Patrick's Day (17 March)
/// is Ireland's national holiday, celebrated worldwide by the Irish diaspora.
/// </summary>
public class IrelandHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "IE";

    /// <inheritdoc/>
    public override string CountryName => "Ireland";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "en";

    /// <summary>
    /// Returns the 9 fixed public holidays of Ireland (including one movable Easter holiday
    /// handled in <see cref="GetMovableHolidays"/>). Floating Monday bank holidays are listed
    /// with representative dates; actual dates are the first/last Monday of the given month.
    /// </summary>
    /// <returns>A sequence of fixed <see cref="HolidayInfo"/> entries for Ireland.</returns>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays() => new[]
    {
        // ── National fixed holidays ───────────────────────────────────────────────

        new HolidayInfo(
            "ie_new_year", 1, 1, "IE",
            "New Year's Day",
            Names("Año Nuevo", "New Year's Day", "Ano Novo", "Jour de l'An", "Neujahrstag"),
            HolidayType.National,
            description: "First day of the new calendar year / Lá Caille."),

        new HolidayInfo(
            "ie_st_patricks_day", 3, 17, "IE",
            "St. Patrick's Day",
            Names("Día de San Patricio", "St. Patrick's Day", "Dia de São Patrício", "Jour de la Saint-Patrick", "Sankt-Patrick-Tag"),
            HolidayType.Civic,
            description: "Lá Fhéile Pádraig. National holiday of Ireland celebrating the death of Saint Patrick (c. 385–461), the foremost patron saint of Ireland, who brought Christianity to the island. Celebrated worldwide by the Irish diaspora with parades and wearing green."),

        new HolidayInfo(
            "ie_christmas_day", 12, 25, "IE",
            "Christmas Day",
            Names("Navidad", "Christmas Day", "Natal", "Noël", "Weihnachten"),
            HolidayType.National,
            description: "Lá Nollag. Celebration of the birth of Jesus Christ."),

        new HolidayInfo(
            "ie_st_stephens_day", 12, 26, "IE",
            "St. Stephen's Day",
            Names("Día de San Esteban", "St. Stephen's Day / Wren Day", "Dia de Santo Estêvão", "Jour de Saint-Étienne", "Stephanustag"),
            HolidayType.National,
            description: "Lá Fhéile Stiofáin / Lá an Dreoilín (Wren Day). Celebration of St. Stephen, the first Christian martyr. In Irish tradition known as Wren Day, when 'wren boys' go door-to-door in costumes."),
    };

    /// <summary>
    /// Returns the 1 Easter-based movable public holiday of Ireland for the given <paramref name="year"/>:
    /// Easter Monday (Monday after Easter Sunday).
    /// </summary>
    /// <param name="year">The four-digit calendar year.</param>
    /// <returns>A sequence of movable <see cref="HolidayInfo"/> entries for Ireland.</returns>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        var easterMonday = EasterCalculator.Easter(year).AddDays(1);

        // St. Brigid's Day: 1 Feb if Friday, otherwise first Monday of February
        var feb1 = new DateTime(year, 2, 1);
        var stBrigid = feb1.DayOfWeek == DayOfWeek.Friday
            ? feb1
            : feb1.AddDays(((int)DayOfWeek.Monday - (int)feb1.DayOfWeek + 7) % 7);

        // First Monday of May
        var may1 = new DateTime(year, 5, 1);
        var mayBank = may1.AddDays(((int)DayOfWeek.Monday - (int)may1.DayOfWeek + 7) % 7);

        // First Monday of June
        var jun1 = new DateTime(year, 6, 1);
        var juneBank = jun1.AddDays(((int)DayOfWeek.Monday - (int)jun1.DayOfWeek + 7) % 7);

        // First Monday of August
        var aug1 = new DateTime(year, 8, 1);
        var augBank = aug1.AddDays(((int)DayOfWeek.Monday - (int)aug1.DayOfWeek + 7) % 7);

        // Last Monday of October
        var octLast = new DateTime(year, 10, DateTime.DaysInMonth(year, 10));
        var octBank = octLast.AddDays(-(((int)octLast.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7));

        if (year >= 2023)
        {
            yield return new HolidayInfo(
                "ie_st_brigid", stBrigid.Month, stBrigid.Day, "IE",
                "St. Brigid's Day",
                Names("Día de Santa Brígida", "St. Brigid's Day", "Dia de Santa Brígida", "Jour de Sainte Brigide", "Tag der Heiligen Brigid"),
                HolidayType.National, isMovable: true,
                description: "Lá Fhéile Bríde. New public holiday since 2023 honouring St. Brigid of Kildare (c. 451–525), patron saint of Ireland, and coinciding with the Celtic festival of Imbolc (start of spring). Observed on 1 February if that day is a Friday, otherwise the first Monday of February.");
        }

        yield return new HolidayInfo(
            "ie_easter_monday", easterMonday.Month, easterMonday.Day, "IE",
            "Easter Monday",
            Names("Lunes de Pascua", "Easter Monday", "Segunda-Feira de Páscoa", "Lundi de Pâques", "Ostermontag"),
            HolidayType.National, isMovable: true,
            description: "Luan Cásca. The day after Easter Sunday. Public holiday in Ireland. Historically significant as 24 April 1916 was Easter Monday — the date of the Easter Rising that led to Irish independence.");

        yield return new HolidayInfo(
            "ie_may_bank_holiday", mayBank.Month, mayBank.Day, "IE",
            "May Bank Holiday",
            Names("Día Festivo de Mayo", "May Bank Holiday", "Feriado de Maio", "Jour férié de mai", "Mai-Feiertag"),
            HolidayType.National, isMovable: true,
            description: "First Monday in May / Luan an Bhealtaine.");

        yield return new HolidayInfo(
            "ie_june_bank_holiday", juneBank.Month, juneBank.Day, "IE",
            "June Bank Holiday",
            Names("Día Festivo de Junio", "June Bank Holiday", "Feriado de Junho", "Jour férié de juin", "Juni-Feiertag"),
            HolidayType.National, isMovable: true,
            description: "First Monday in June / Luan an Mheithimh.");

        yield return new HolidayInfo(
            "ie_august_bank_holiday", augBank.Month, augBank.Day, "IE",
            "August Bank Holiday",
            Names("Día Festivo de Agosto", "August Bank Holiday", "Feriado de Agosto", "Jour férié d'août", "August-Feiertag"),
            HolidayType.National, isMovable: true,
            description: "First Monday in August / Luan Lúnasa. Coincides with Lúnasa, the ancient Celtic harvest festival.");

        yield return new HolidayInfo(
            "ie_october_bank_holiday", octBank.Month, octBank.Day, "IE",
            "October Bank Holiday",
            Names("Día Festivo de Octubre", "October Bank Holiday", "Feriado de Outubro", "Jour férié d'octobre", "Oktober-Feiertag"),
            HolidayType.National, isMovable: true,
            description: "Last Monday in October / Luan Dheireadh Fómhair.");
    }
}
