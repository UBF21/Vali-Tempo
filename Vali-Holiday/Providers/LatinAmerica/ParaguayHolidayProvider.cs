using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.LatinAmerica;

/// <summary>
/// Holiday provider for <b>Paraguay</b> — country code <c>PY</c>.
/// </summary>
/// <remarks>
/// <para>
/// Paraguay observes ten fixed national holidays and two movable feasts tied to Easter.
/// The calendar reflects the country's history, with several holidays commemorating
/// the bloody Paraguayan War (Guerra del Paraguay / Guerra de la Triple Alianza, 1864–1870)
/// and the Chaco War (1932–1935).
/// </para>
/// <para>
/// The <em>Día de los Héroes</em> (1 March) marks the Battle of Cerro Corá (1 March 1870),
/// the final battle of the War of the Triple Alliance and the date of death of President
/// Francisco Solano López, the last defender of Paraguay.
/// </para>
/// <para>
/// Independence is celebrated over two consecutive days: <em>Víspera de la Independencia</em>
/// (14 May) and <em>Día de la Independencia Nacional</em> (15 May 1811).
/// </para>
/// <para>
/// The <em>Día de la Paz del Chaco</em> (12 June) commemorates the ceasefire that ended
/// the Chaco War with Bolivia in 1935, while <em>Victoria de Boquerón</em> (29 September)
/// honours the key battle of that war in 1932. The <em>Virgen de Caacupé</em> (8 December)
/// is the feast day of the Patroness of Paraguay.
/// </para>
/// </remarks>
public sealed class ParaguayHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "PY";

    /// <inheritdoc/>
    public override string CountryName => "Paraguay";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "es";

    /// <inheritdoc/>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays()
    {
        const string cc = "PY";

        return new[]
        {
            // 1 January — Año Nuevo
            new HolidayInfo(
                id: "py_new_year",
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

            // 1 March — Día de los Héroes
            new HolidayInfo(
                id: "py_heroes_day",
                month: 3, day: 1,
                countryCode: cc,
                name: "Día de los Héroes",
                names: Names(
                    es: "Día de los Héroes",
                    en: "Heroes' Day",
                    pt: "Dia dos Heróis",
                    fr: "Journée des Héros",
                    de: "Tag der Helden"),
                type: HolidayType.Civic,
                description: "Marks the Battle of Cerro Corá on 1 March 1870, the final battle of the War of the Triple Alliance, where President Francisco Solano López died defending Paraguay against Brazil, Argentina, and Uruguay."),

            // 1 May — Día del Trabajador
            new HolidayInfo(
                id: "py_labour_day",
                month: 5, day: 1,
                countryCode: cc,
                name: "Día del Trabajador",
                names: Names(
                    es: "Día del Trabajador",
                    en: "Workers' Day",
                    pt: "Dia do Trabalhador",
                    fr: "Fête du Travail",
                    de: "Tag der Arbeit"),
                type: HolidayType.National,
                description: "International Workers' Day."),

            // 14 May — Víspera de la Independencia Nacional
            new HolidayInfo(
                id: "py_independence_eve",
                month: 5, day: 14,
                countryCode: cc,
                name: "Víspera de la Independencia Nacional",
                names: Names(
                    es: "Víspera de la Independencia Nacional",
                    en: "National Independence Eve",
                    pt: "Véspera da Independência Nacional",
                    fr: "Veille de l'Indépendance Nationale",
                    de: "Vorabend der nationalen Unabhängigkeit"),
                type: HolidayType.Civic,
                description: "Eve of Paraguay's Independence Day — commemorates the night of 14 May 1811 when the independence movement was set in motion."),

            // 15 May — Día de la Independencia Nacional
            new HolidayInfo(
                id: "py_independence",
                month: 5, day: 15,
                countryCode: cc,
                name: "Día de la Independencia Nacional",
                names: Names(
                    es: "Día de la Independencia Nacional",
                    en: "National Independence Day",
                    pt: "Dia da Independência Nacional",
                    fr: "Jour de l'Indépendance Nationale",
                    de: "Nationaler Unabhängigkeitstag"),
                type: HolidayType.Civic,
                description: "Celebrates Paraguay's declaration of independence from Spain on 15 May 1811."),

            // 12 June — Día de la Paz del Chaco
            new HolidayInfo(
                id: "py_chaco_peace",
                month: 6, day: 12,
                countryCode: cc,
                name: "Día de la Paz del Chaco",
                names: Names(
                    es: "Día de la Paz del Chaco",
                    en: "Chaco Peace Day",
                    pt: "Dia da Paz do Chaco",
                    fr: "Journée de la Paix du Chaco",
                    de: "Tag des Chacofriedens"),
                type: HolidayType.Civic,
                description: "Commemorates the ceasefire agreement of 12 June 1935 that ended the Chaco War (1932–1935) between Paraguay and Bolivia. Paraguay retained the disputed Chaco Boreal territory."),

            // 15 August — Fundación de Asunción y Virgen de la Asunción
            new HolidayInfo(
                id: "py_asuncion_foundation",
                month: 8, day: 15,
                countryCode: cc,
                name: "Fundación de Asunción",
                names: Names(
                    es: "Fundación de Asunción y Virgen de la Asunción",
                    en: "Foundation of Asunción and Assumption of Mary",
                    pt: "Fundação de Assunção e Assunção de Nossa Senhora",
                    fr: "Fondation d'Asunción et Assomption de Marie",
                    de: "Gründung von Asunción und Mariä Himmelfahrt"),
                type: HolidayType.Civic,
                description: "Celebrates the founding of Asunción on 15 August 1537 by Juan de Salazar y Espinosa, coinciding with the Catholic feast of the Assumption of the Virgin Mary."),

            // 29 September — Victoria de Boquerón
            new HolidayInfo(
                id: "py_victoria_boqueron",
                month: 9, day: 29,
                countryCode: cc,
                name: "Victoria de Boquerón",
                names: Names(
                    es: "Victoria de Boquerón",
                    en: "Victory of Boquerón",
                    pt: "Vitória de Boquerón",
                    fr: "Victoire de Boquerón",
                    de: "Sieg von Boquerón"),
                type: HolidayType.Civic,
                description: "Commemorates the Paraguayan victory at the Battle of Boquerón (29 September 1932), a key engagement during the Chaco War against Bolivia."),

            // 8 December — Virgen de Caacupé
            new HolidayInfo(
                id: "py_caacupe",
                month: 12, day: 8,
                countryCode: cc,
                name: "Virgen de Caacupé",
                names: Names(
                    es: "Virgen de Caacupé",
                    en: "Our Lady of Caacupé",
                    pt: "Nossa Senhora de Caacupé",
                    fr: "Notre-Dame de Caacupé",
                    de: "Unsere Liebe Frau von Caacupé"),
                type: HolidayType.Religious,
                description: "Feast day of the Virgin of Caacupé, Patroness of Paraguay. Hundreds of thousands of pilgrims gather in Caacupé each year on this date."),

            // 25 December — Navidad
            new HolidayInfo(
                id: "py_christmas",
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
        const string cc = "PY";

        return new[]
        {
            // Holy Thursday — Easter - 3 days
            new HolidayInfo(
                id: "py_holy_thursday",
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
                description: "Maundy Thursday — commemorates the Last Supper, 3 days before Easter Sunday."),

            // Good Friday — Easter - 2 days
            new HolidayInfo(
                id: "py_good_friday",
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
