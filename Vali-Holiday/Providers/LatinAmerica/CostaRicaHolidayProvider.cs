using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.LatinAmerica;

/// <summary>
/// Holiday provider for <b>Costa Rica</b> — country code <c>CR</c>.
/// </summary>
/// <remarks>
/// <para>
/// Costa Rica observes eight fixed national holidays and two movable feasts tied to Easter.
/// The calendar reflects a blend of civic independence milestones and deep-rooted Catholic
/// religious traditions.
/// </para>
/// <para>
/// The <em>Día de Juan Santamaría</em> (11 April) honours Costa Rica's most celebrated
/// national hero. Juan Santamaría, a young drummer boy from Alajuela, sacrificed his life
/// setting fire to the enemy positions at the Battle of Rivas (11 April 1856) against the
/// filibuster army of William Walker, who sought to enslave Central America.
/// </para>
/// <para>
/// The <em>Anexión del Partido de Nicoya</em> (25 July) marks the voluntary annexation
/// of the Nicoya region (present-day Guanacaste) to Costa Rica in 1824.
/// </para>
/// <para>
/// The <em>Día de la Virgen de los Ángeles</em> (2 August) honours the Patroness of Costa Rica,
/// whose apparition in Cartago in 1635 is commemorated each year by a massive pilgrimage.
/// </para>
/// </remarks>
public sealed class CostaRicaHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "CR";

    /// <inheritdoc/>
    public override string CountryName => "Costa Rica";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "es";

    /// <inheritdoc/>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays()
    {
        const string cc = "CR";

        return new[]
        {
            // 1 January — Año Nuevo
            new HolidayInfo(
                id: "cr_new_year",
                month: 1, day: 1,
                countryCode: cc,
                name: "Año Nuevo",
                names: Names(
                    es: "Año Nuevo",
                    en: "New Year's Day",
                    pt: "Ano Novo",
                    fr: "Jour de l'An",
                    de: "Neujahrstag"),
                type: HolidayType.National,
                description: "Celebrates the beginning of the new calendar year."),

            // 11 April — Día de Juan Santamaría
            new HolidayInfo(
                id: "cr_juan_santamaria",
                month: 4, day: 11,
                countryCode: cc,
                name: "Día de Juan Santamaría",
                names: Names(
                    es: "Día de Juan Santamaría",
                    en: "Juan Santamaría Day",
                    pt: "Dia de Juan Santamaría",
                    fr: "Journée de Juan Santamaría",
                    de: "Juan-Santamaría-Tag"),
                type: HolidayType.Civic,
                description: "Honours Juan Santamaría, national hero who died on 11 April 1856 setting fire to enemy positions at the Battle of Rivas against William Walker's filibuster forces threatening Central American sovereignty."),

            // 1 May — Día del Trabajo
            new HolidayInfo(
                id: "cr_labour_day",
                month: 5, day: 1,
                countryCode: cc,
                name: "Día del Trabajo",
                names: Names(
                    es: "Día del Trabajo",
                    en: "Labour Day",
                    pt: "Dia do Trabalho",
                    fr: "Fête du Travail",
                    de: "Tag der Arbeit"),
                type: HolidayType.National,
                description: "International Workers' Day."),

            // 25 July — Anexión del Partido de Nicoya
            new HolidayInfo(
                id: "cr_nicoya_annexation",
                month: 7, day: 25,
                countryCode: cc,
                name: "Anexión del Partido de Nicoya",
                names: Names(
                    es: "Anexión del Partido de Nicoya",
                    en: "Annexation of the Nicoya District",
                    pt: "Anexação do Partido de Nicoya",
                    fr: "Annexion du Partido de Nicoya",
                    de: "Annexion des Partido de Nicoya"),
                type: HolidayType.Civic,
                description: "Commemorates the voluntary annexation of the Partido de Nicoya (present-day Guanacaste province) to Costa Rica on 25 July 1824, following a popular vote in the region."),

            // 2 August — Día de la Virgen de los Ángeles
            new HolidayInfo(
                id: "cr_virgen_angeles",
                month: 8, day: 2,
                countryCode: cc,
                name: "Día de la Virgen de los Ángeles",
                names: Names(
                    es: "Día de la Virgen de los Ángeles",
                    en: "Day of the Virgin of the Angels",
                    pt: "Dia da Virgem dos Anjos",
                    fr: "Journée de la Vierge des Anges",
                    de: "Tag der Jungfrau der Engel"),
                type: HolidayType.Religious,
                description: "Feast of Our Lady of the Angels (La Negrita), Patroness of Costa Rica, whose image appeared in Cartago in 1635. Each year hundreds of thousands of pilgrims walk to the Basilica of Our Lady of the Angels."),

            // 15 August — Día de la Madre / Asunción de la Virgen
            new HolidayInfo(
                id: "cr_mothers_day",
                month: 8, day: 15,
                countryCode: cc,
                name: "Día de la Madre",
                names: Names(
                    es: "Día de la Madre / Asunción de la Virgen",
                    en: "Mother's Day / Assumption of Mary",
                    pt: "Dia das Mães / Assunção de Maria",
                    fr: "Fête des Mères / Assomption de Marie",
                    de: "Muttertag / Mariä Himmelfahrt"),
                type: HolidayType.Religious,
                description: "Costa Rica celebrates Mother's Day on 15 August, coinciding with the Catholic feast of the Assumption of the Virgin Mary."),

            // 15 September — Día de la Independencia
            new HolidayInfo(
                id: "cr_independence",
                month: 9, day: 15,
                countryCode: cc,
                name: "Día de la Independencia",
                names: Names(
                    es: "Día de la Independencia",
                    en: "Independence Day",
                    pt: "Dia da Independência",
                    fr: "Jour de l'Indépendance",
                    de: "Unabhängigkeitstag"),
                type: HolidayType.Civic,
                description: "Commemorates Central America's independence from Spain on 15 September 1821, celebrated with school parades and the symbolic torch relay (Antorcha de la Libertad) from Guatemala to Costa Rica."),

            // 12 October — Día de las Culturas
            new HolidayInfo(
                id: "cr_cultures_day",
                month: 10, day: 12,
                countryCode: cc,
                name: "Día de las Culturas",
                names: Names(
                    es: "Día de las Culturas",
                    en: "Day of Cultures",
                    pt: "Dia das Culturas",
                    fr: "Journée des Cultures",
                    de: "Tag der Kulturen"),
                type: HolidayType.Civic,
                description: "Renamed from 'Día de la Raza' to 'Día de las Culturas' to celebrate Costa Rica's multicultural and multiethnic heritage."),

            // 25 December — Navidad
            new HolidayInfo(
                id: "cr_christmas",
                month: 12, day: 25,
                countryCode: cc,
                name: "Navidad",
                names: Names(
                    es: "Navidad",
                    en: "Christmas Day",
                    pt: "Natal",
                    fr: "Noël",
                    de: "Weihnachten"),
                type: HolidayType.Religious,
                description: "Celebrates the birth of Jesus Christ."),
        };
    }

    /// <inheritdoc/>
    /// <returns>
    /// Two movable holidays: Holy Thursday and Good Friday.
    /// </returns>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        const string cc = "CR";

        return new[]
        {
            // Holy Thursday — Easter - 3 days
            new HolidayInfo(
                id: "cr_holy_thursday",
                month: EasterCalculator.HolyThursday(year).Month,
                day: EasterCalculator.HolyThursday(year).Day,
                countryCode: cc,
                name: "Jueves Santo",
                names: Names(
                    es: "Jueves Santo",
                    en: "Holy Thursday",
                    pt: "Quinta-Feira Santa",
                    fr: "Jeudi Saint",
                    de: "Gründonnerstag"),
                type: HolidayType.Religious,
                isMovable: true,
                description: "Maundy Thursday — commemorates the Last Supper of Jesus Christ, 3 days before Easter Sunday."),

            // Good Friday — Easter - 2 days
            new HolidayInfo(
                id: "cr_good_friday",
                month: EasterCalculator.GoodFriday(year).Month,
                day: EasterCalculator.GoodFriday(year).Day,
                countryCode: cc,
                name: "Viernes Santo",
                names: Names(
                    es: "Viernes Santo",
                    en: "Good Friday",
                    pt: "Sexta-Feira Santa",
                    fr: "Vendredi Saint",
                    de: "Karfreitag"),
                type: HolidayType.Religious,
                isMovable: true,
                description: "Commemorates the crucifixion of Jesus Christ, 2 days before Easter Sunday."),
        };
    }
}
