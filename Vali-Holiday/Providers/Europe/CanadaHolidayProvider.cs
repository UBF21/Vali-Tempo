using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.Europe;

/// <summary>
/// Holiday provider for Canada, covering the 12 federal statutory holidays as defined by
/// the Canada Labour Code (R.S.C., 1985, c. L-2) applicable to federally regulated employees.
/// Individual provinces and territories may observe additional or different holidays. Canada Day
/// (1 July) commemorates Confederation in 1867 when the British North America Act united the
/// provinces of Canada, Nova Scotia, and New Brunswick. National Day for Truth and Reconciliation
/// (30 September) was established in 2021 to honour the children who died at residential schools
/// and survivors, as well as their families and communities. Victoria Day (the Monday before
/// 25 May) celebrates Queen Victoria's birthday and the reigning sovereign. Family Day
/// (third Monday of February) and Thanksgiving (second Monday of October) are also observed.
/// </summary>
public class CanadaHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "CA";

    /// <inheritdoc/>
    public override string CountryName => "Canada";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "en";

    /// <summary>
    /// Returns the 8 fixed federal statutory holidays of Canada. Floating-Monday holidays
    /// are listed with representative dates; actual dates follow the rule in the description.
    /// Easter-based movable holidays are returned by <see cref="GetMovableHolidays"/>.
    /// </summary>
    /// <returns>A sequence of fixed <see cref="HolidayInfo"/> entries for Canada.</returns>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays() => new[]
    {
        // ── Federal statutory holidays ────────────────────────────────────────────

        new HolidayInfo(
            "ca_new_year", 1, 1, "CA",
            "New Year's Day",
            Names("Año Nuevo", "New Year's Day", "Ano Novo", "Jour de l'An", "Neujahrstag"),
            HolidayType.National,
            description: "Jour de l'An / New Year's Day. First day of the new calendar year."),

        new HolidayInfo(
            "ca_family_day", 2, 17, "CA",
            "Family Day",
            Names("Día de la Familia", "Family Day", "Dia da Família", "Jour de la famille", "Familientag"),
            HolidayType.National,
            description: "Third Monday in February. Representative date: 17 February. Observed in most Canadian provinces (not all). Alberta was the first to introduce it in 1990. Celebrates the importance of family and gives workers a day off in the gap between New Year's Day and Easter."),

        new HolidayInfo(
            "ca_victoria_day", 5, 19, "CA",
            "Victoria Day",
            Names("Día de la Reina Victoria", "Victoria Day", "Dia da Rainha Vitória", "Fête de la Reine Victoria", "Victoriatag"),
            HolidayType.Civic,
            description: "Monday before 25 May. Representative date: 19 May. Celebrates Queen Victoria's birthday (24 May 1819) and the birthday of the current sovereign. Known as la Fête de Dollard in Quebec."),

        new HolidayInfo(
            "ca_canada_day", 7, 1, "CA",
            "Canada Day",
            Names("Día de Canadá", "Canada Day", "Dia do Canadá", "Fête du Canada", "Kanada-Tag"),
            HolidayType.Civic,
            description: "Fête du Canada. Celebrates Confederation on 1 July 1867 when the British North America Act (now the Constitution Act, 1867) united the Province of Canada, Nova Scotia, and New Brunswick into a single Dominion of Canada."),

        new HolidayInfo(
            "ca_labour_day", 9, 1, "CA",
            "Labour Day",
            Names("Día del Trabajo", "Labour Day", "Dia do Trabalho", "Fête du Travail", "Tag der Arbeit"),
            HolidayType.National,
            description: "Fête du Travail. First Monday in September. Representative date: 1 September. Celebrates the achievements of the labour movement in Canada. In contrast to most of the world, Canada and the United States observe Labour Day in September rather than May."),

        new HolidayInfo(
            "ca_truth_reconciliation", 9, 30, "CA",
            "National Day for Truth and Reconciliation",
            Names("Día Nacional por la Verdad y la Reconciliación", "National Day for Truth and Reconciliation", "Dia Nacional pela Verdade e Reconciliação", "Journée nationale de la vérité et de la réconciliation", "Nationaler Tag für Wahrheit und Versöhnung"),
            HolidayType.Civic,
            description: "Orange Shirt Day. Federal statutory holiday since 2021 (Bill C-5). Honours the children who died in the residential school system, the survivors, and their families and communities. Part of the process of reconciliation between Canadians and Indigenous peoples as called for by the Truth and Reconciliation Commission."),

        new HolidayInfo(
            "ca_thanksgiving", 10, 13, "CA",
            "Thanksgiving",
            Names("Acción de Gracias", "Thanksgiving Day", "Dia de Ação de Graças", "Action de grâce", "Erntedankfest"),
            HolidayType.National,
            description: "Action de grâce. Second Monday in October. Representative date: 13 October. Canadian Thanksgiving has roots in the European harvest festival traditions and was made an annual holiday by the Canadian Parliament in 1879. Unlike the American holiday, it is not connected to the Pilgrim story."),

        new HolidayInfo(
            "ca_remembrance_day", 11, 11, "CA",
            "Remembrance Day",
            Names("Día del Armisticio / Día del Recuerdo", "Remembrance Day", "Dia do Armistício / Dia da Memória", "Jour du Souvenir", "Remembrance Day / Volkstrauertag"),
            HolidayType.Civic,
            description: "Jour du Souvenir. Commemorates members of the armed forces who died in the line of duty since World War I, whose Armistice was signed at the 11th hour of the 11th day of the 11th month, 1918."),

        new HolidayInfo(
            "ca_christmas", 12, 25, "CA",
            "Christmas Day",
            Names("Navidad", "Christmas Day", "Natal", "Noël", "Weihnachten"),
            HolidayType.National,
            description: "Noël / Christmas Day. Celebration of the birth of Jesus Christ."),

        new HolidayInfo(
            "ca_boxing_day", 12, 26, "CA",
            "Boxing Day",
            Names("Día de las Cajas / San Esteban", "Boxing Day", "Dia das Caixas / Santo Estêvão", "Lendemain de Noël", "Boxing Day"),
            HolidayType.National,
            description: "Lendemain de Noël / Boxing Day. The day after Christmas. Traditionally the day when employers gave gifts or 'boxes' to service workers. A federal holiday in Canada."),
    };

    /// <summary>
    /// Returns the 2 Easter-based movable federal holidays of Canada for the given <paramref name="year"/>:
    /// Good Friday (Easter-2) and Easter Monday (Easter+1).
    /// </summary>
    /// <param name="year">The four-digit calendar year.</param>
    /// <returns>A sequence of movable <see cref="HolidayInfo"/> entries for Canada.</returns>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        var goodFriday   = EasterCalculator.GoodFriday(year);
        var easterMonday = EasterCalculator.Easter(year).AddDays(1);

        return new[]
        {
            new HolidayInfo(
                "ca_good_friday", goodFriday.Month, goodFriday.Day, "CA",
                "Good Friday",
                Names("Viernes Santo", "Good Friday", "Sexta-Feira Santa", "Vendredi Saint", "Karfreitag"),
                HolidayType.National, isMovable: true,
                description: "Vendredi Saint / Good Friday. Commemorates the crucifixion of Jesus Christ, two days before Easter Sunday. Federal statutory holiday in Canada."),

            new HolidayInfo(
                "ca_easter_monday", easterMonday.Month, easterMonday.Day, "CA",
                "Easter Monday",
                Names("Lunes de Pascua", "Easter Monday", "Segunda-Feira de Páscoa", "Lundi de Pâques", "Ostermontag"),
                HolidayType.National, isMovable: true,
                description: "Lundi de Pâques / Easter Monday. The day after Easter Sunday. Federal statutory holiday in Canada."),
        };
    }
}
