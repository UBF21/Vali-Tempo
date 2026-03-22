using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.Europe;

/// <summary>
/// Holiday provider for Australia, covering the 8 national public holidays observed
/// across all states and territories, plus selected regional holidays. Australia has no
/// single national public holiday law; the federal government sets holidays for
/// Commonwealth public service employees while each state/territory determines its own.
/// The listed national holidays are those universally observed. Australia Day (26 January)
/// commemorates the arrival of the First Fleet at Port Jackson in 1788; when 26 January
/// falls on a Saturday or Sunday the holiday is moved to the following Monday. ANZAC Day
/// (25 April) honours Australian and New Zealand Army Corps soldiers who served in World War I,
/// particularly the Gallipoli Campaign of 1915. Regional entries include Canberra Day
/// (ACT) and Melbourne Cup Day (Victoria).
/// </summary>
public class AustraliaHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "AU";

    /// <inheritdoc/>
    public override string CountryName => "Australia";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "en";

    /// <summary>
    /// Returns the fixed national and regional holidays of Australia.
    /// Easter-based movable holidays are returned by <see cref="GetMovableHolidays"/>.
    /// </summary>
    /// <returns>A sequence of fixed <see cref="HolidayInfo"/> entries for Australia.</returns>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays() => new[]
    {
        // ── National fixed holidays ───────────────────────────────────────────────

        new HolidayInfo(
            "au_new_year", 1, 1, "AU",
            "New Year's Day",
            Names("Año Nuevo", "New Year's Day", "Ano Novo", "Jour de l'An", "Neujahrstag"),
            HolidayType.National,
            description: "First day of the new calendar year. If 1 January falls on a weekend, the holiday is typically observed on the following Monday."),

        new HolidayInfo(
            "au_australia_day", 1, 27, "AU",
            "Australia Day",
            Names("Día de Australia", "Australia Day", "Dia da Austrália", "Jour de l'Australie", "Australien-Tag"),
            HolidayType.Civic,
            description: "Commemorates the arrival of the First Fleet at Port Jackson, New South Wales, on 26 January 1788, when Captain Arthur Phillip raised the British flag. Observed on 26 January; when 26 January falls on a Saturday or Sunday, the public holiday is the following Monday. Representative date: 27 January."),

        new HolidayInfo(
            "au_anzac_day", 4, 25, "AU",
            "ANZAC Day",
            Names("Día del ANZAC", "ANZAC Day", "Dia do ANZAC", "Jour de l'ANZAC", "ANZAC-Tag"),
            HolidayType.Civic,
            description: "Australia and New Zealand Army Corps Day. Commemorates the first major military action fought by Australian and New Zealand forces during World War I at Gallipoli on 25 April 1915. Dawn services are held across Australia."),

        new HolidayInfo(
            "au_christmas", 12, 25, "AU",
            "Christmas Day",
            Names("Navidad", "Christmas Day", "Natal", "Noël", "Weihnachten"),
            HolidayType.National,
            description: "Celebration of the birth of Jesus Christ. If 25 December falls on a Saturday or Sunday, additional substitute public holidays may be proclaimed."),

        new HolidayInfo(
            "au_boxing_day", 12, 26, "AU",
            "Boxing Day",
            Names("Día de las Cajas / San Esteban", "Boxing Day", "Dia das Caixas / Santo Estêvão", "Lendemain de Noël", "Boxing Day"),
            HolidayType.National,
            description: "Boxing Day / St. Stephen's Day. The day after Christmas. Traditionally the day when employers gave gifts to service workers. Observed as a public holiday across all Australian states and territories."),

        // ── Australian Capital Territory (AU-ACT) – regional ─────────────────────

        new HolidayInfo(
            "au_act_canberra_day", 3, 10, "AU",
            "Canberra Day",
            Names("Día de Canberra", "Canberra Day", "Dia de Canberra", "Jour de Canberra", "Canberra-Tag"),
            HolidayType.Regional, regionCode: "AU-ACT",
            description: "Second Monday in March in the Australian Capital Territory. Representative date: 10 March. Celebrates the founding of Canberra, the national capital, which was named on 12 March 1913 by the Governor-General's wife, Lady Denman."),

        // ── Victoria (AU-VIC) – regional ──────────────────────────────────────────

        new HolidayInfo(
            "au_vic_melbourne_cup", 11, 4, "AU",
            "Melbourne Cup Day",
            Names("Día de la Copa Melbourne", "Melbourne Cup Day", "Dia da Copa de Melbourne", "Jour de la Melbourne Cup", "Melbourne-Cup-Tag"),
            HolidayType.Regional, regionCode: "AU-VIC",
            description: "First Tuesday in November in the Melbourne metropolitan area (and some regional areas of Victoria). Representative date: 4 November. Celebrates the Melbourne Cup horse race, known as 'the race that stops a nation', held at Flemington Racecourse since 1861."),
    };

    /// <summary>
    /// Returns the 4 Easter-based movable national holidays of Australia for the given <paramref name="year"/>:
    /// Good Friday (Easter-2), Holy Saturday (Easter-1), Easter Sunday, and Easter Monday (Easter+1).
    /// </summary>
    /// <param name="year">The four-digit calendar year.</param>
    /// <returns>A sequence of movable <see cref="HolidayInfo"/> entries for Australia.</returns>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        var goodFriday   = EasterCalculator.GoodFriday(year);
        var holySaturday = EasterCalculator.HolySaturday(year);
        var easter       = EasterCalculator.Easter(year);
        var easterMonday = easter.AddDays(1);

        return new[]
        {
            new HolidayInfo(
                "au_good_friday", goodFriday.Month, goodFriday.Day, "AU",
                "Good Friday",
                Names("Viernes Santo", "Good Friday", "Sexta-Feira Santa", "Vendredi Saint", "Karfreitag"),
                HolidayType.National, isMovable: true,
                description: "Commemorates the crucifixion of Jesus Christ, two days before Easter Sunday. National public holiday across all Australian states and territories."),

            new HolidayInfo(
                "au_holy_saturday", holySaturday.Month, holySaturday.Day, "AU",
                "Holy Saturday",
                Names("Sábado Santo", "Holy Saturday", "Sábado de Aleluia", "Samedi Saint", "Karsamstag"),
                HolidayType.National, isMovable: true,
                description: "Holy Saturday / Easter Eve. The day between Good Friday and Easter Sunday. A public holiday in Australia (varies slightly by state/territory)."),

            new HolidayInfo(
                "au_easter_sunday", easter.Month, easter.Day, "AU",
                "Easter Sunday",
                Names("Domingo de Pascua", "Easter Sunday", "Domingo de Páscoa", "Dimanche de Pâques", "Ostersonntag"),
                HolidayType.National, isMovable: true,
                description: "Celebrates the resurrection of Jesus Christ. Observed as a public holiday in Australia."),

            new HolidayInfo(
                "au_easter_monday", easterMonday.Month, easterMonday.Day, "AU",
                "Easter Monday",
                Names("Lunes de Pascua", "Easter Monday", "Segunda-Feira de Páscoa", "Lundi de Pâques", "Ostermontag"),
                HolidayType.National, isMovable: true,
                description: "The day after Easter Sunday. National public holiday across all Australian states and territories."),
        };
    }
}
