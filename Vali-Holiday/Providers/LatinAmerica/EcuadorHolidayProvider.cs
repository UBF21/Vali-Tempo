using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.LatinAmerica;

/// <summary>
/// Holiday provider for <b>Ecuador</b> — country code <c>EC</c>.
/// </summary>
/// <remarks>
/// <para>
/// Ecuador observes eleven fixed national holidays and four movable feasts tied to Easter.
/// The fixed calendar commemorates key events of the independence process, including the
/// <em>Primer Grito de la Independencia</em> (10 August 1809), the
/// <em>Batalla de Pichincha</em> (24 May 1822) that sealed independence from Spain, and
/// the independences of Guayaquil (9 October 1820) and Cuenca (3 November 1820).
/// </para>
/// <para>
/// The <em>Natalicio de Simón Bolívar</em> (24 July) honours the Liberator who led
/// the unification of Gran Colombia. The <em>Día de la Interculturalidad</em> (12 October)
/// replaced the former "Día de la Raza" to celebrate Ecuador's multicultural heritage.
/// </para>
/// <para>
/// Movable holidays follow the Catholic liturgical calendar: Holy Thursday, Good Friday,
/// and the two days of Carnival (Monday and Tuesday before Ash Wednesday).
/// </para>
/// </remarks>
public sealed class EcuadorHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "EC";

    /// <inheritdoc/>
    public override string CountryName => "Ecuador";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "es";

    /// <inheritdoc/>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays()
    {
        const string cc = "EC";

        return new[]
        {
            // 1 January — Año Nuevo
            new HolidayInfo(
                id: "ec_new_year",
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

            // 1 May — Día del Trabajo
            new HolidayInfo(
                id: "ec_labour_day",
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

            // 24 May — Batalla de Pichincha
            new HolidayInfo(
                id: "ec_battle_pichincha",
                month: 5, day: 24,
                countryCode: cc,
                name: "Batalla de Pichincha",
                names: Names(
                    es: "Batalla de Pichincha",
                    en: "Battle of Pichincha",
                    pt: "Batalha de Pichincha",
                    fr: "Bataille de Pichincha",
                    de: "Schlacht von Pichincha"),
                type: HolidayType.Civic,
                description: "Commemorates the decisive Battle of Pichincha on 24 May 1822, which secured Ecuador's independence from Spain under Field Marshal Antonio José de Sucre."),

            // 24 July — Natalicio de Simón Bolívar
            new HolidayInfo(
                id: "ec_bolivar_birthday",
                month: 7, day: 24,
                countryCode: cc,
                name: "Natalicio de Simón Bolívar",
                names: Names(
                    es: "Natalicio de Simón Bolívar",
                    en: "Simón Bolívar's Birthday",
                    pt: "Natalício de Simón Bolívar",
                    fr: "Naissance de Simón Bolívar",
                    de: "Geburtstag Simón Bolívars"),
                type: HolidayType.Civic,
                description: "Celebrates the birth of Simón Bolívar (24 July 1783), Libertador of South America and founding father of Gran Colombia."),

            // 10 August — Primer Grito de la Independencia
            new HolidayInfo(
                id: "ec_independence_cry",
                month: 8, day: 10,
                countryCode: cc,
                name: "Primer Grito de la Independencia",
                names: Names(
                    es: "Primer Grito de la Independencia",
                    en: "First Cry of Independence",
                    pt: "Primeiro Grito da Independência",
                    fr: "Premier Cri de l'Indépendance",
                    de: "Erster Unabhängigkeitsruf"),
                type: HolidayType.Civic,
                description: "Marks the first declaration of independence from Spain on 10 August 1809 in Quito, the earliest in Hispanic America."),

            // 9 October — Independencia de Guayaquil
            new HolidayInfo(
                id: "ec_guayaquil_independence",
                month: 10, day: 9,
                countryCode: cc,
                name: "Independencia de Guayaquil",
                names: Names(
                    es: "Independencia de Guayaquil",
                    en: "Independence of Guayaquil",
                    pt: "Independência de Guayaquil",
                    fr: "Indépendance de Guayaquil",
                    de: "Unabhängigkeit Guayaquils"),
                type: HolidayType.Civic,
                description: "Celebrates the independence of Guayaquil from Spain on 9 October 1820."),

            // 12 October — Día de la Interculturalidad
            new HolidayInfo(
                id: "ec_interculturality_day",
                month: 10, day: 12,
                countryCode: cc,
                name: "Día de la Interculturalidad",
                names: Names(
                    es: "Día de la Interculturalidad",
                    en: "Day of Interculturality",
                    pt: "Dia da Interculturalidade",
                    fr: "Journée de l'Interculturalité",
                    de: "Tag der Interkulturalität"),
                type: HolidayType.Civic,
                description: "Formerly 'Día de la Raza'; renamed to celebrate Ecuador's multicultural and multiethnic heritage."),

            // 2 November — Día de los Difuntos
            new HolidayInfo(
                id: "ec_all_souls",
                month: 11, day: 2,
                countryCode: cc,
                name: "Día de los Difuntos",
                names: Names(
                    es: "Día de los Difuntos",
                    en: "All Souls' Day",
                    pt: "Dia dos Finados",
                    fr: "Jour des Morts",
                    de: "Allerseelen"),
                type: HolidayType.Religious,
                description: "All Souls' Day — day of remembrance and prayer for the departed."),

            // 3 November — Independencia de Cuenca
            new HolidayInfo(
                id: "ec_cuenca_independence",
                month: 11, day: 3,
                countryCode: cc,
                name: "Independencia de Cuenca",
                names: Names(
                    es: "Independencia de Cuenca",
                    en: "Independence of Cuenca",
                    pt: "Independência de Cuenca",
                    fr: "Indépendance de Cuenca",
                    de: "Unabhängigkeit von Cuenca"),
                type: HolidayType.Civic,
                description: "Commemorates the independence of the city of Cuenca from Spain on 3 November 1820."),

            // 6 December — Fundación de Quito
            new HolidayInfo(
                id: "ec_quito_foundation",
                month: 12, day: 6,
                countryCode: cc,
                name: "Fundación de Quito",
                names: Names(
                    es: "Fundación de Quito",
                    en: "Foundation of Quito",
                    pt: "Fundação de Quito",
                    fr: "Fondation de Quito",
                    de: "Gründung von Quito"),
                type: HolidayType.Civic,
                description: "Celebrates the Spanish foundation of Quito on 6 December 1534 by Sebastián de Benalcázar."),

            // 25 December — Navidad
            new HolidayInfo(
                id: "ec_christmas",
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
    /// Four movable holidays: Carnival Monday, Carnival Tuesday, Holy Thursday, and Good Friday.
    /// </returns>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        const string cc = "EC";

        return new[]
        {
            // Carnival Monday — Easter - 48 days
            new HolidayInfo(
                id: "ec_carnival_monday",
                month: EasterCalculator.CarnavalMonday(year).Month,
                day: EasterCalculator.CarnavalMonday(year).Day,
                countryCode: cc,
                name: "Lunes de Carnaval",
                names: Names(
                    es: "Lunes de Carnaval",
                    en: "Carnival Monday",
                    pt: "Segunda-feira de Carnaval",
                    fr: "Lundi de Carnaval",
                    de: "Karnevalsmontag"),
                type: HolidayType.National,
                isMovable: true,
                description: "Carnival Monday, 48 days before Easter Sunday."),

            // Carnival Tuesday — Easter - 47 days
            new HolidayInfo(
                id: "ec_carnival_tuesday",
                month: EasterCalculator.CarnavalTuesday(year).Month,
                day: EasterCalculator.CarnavalTuesday(year).Day,
                countryCode: cc,
                name: "Martes de Carnaval",
                names: Names(
                    es: "Martes de Carnaval",
                    en: "Carnival Tuesday",
                    pt: "Terça-feira de Carnaval",
                    fr: "Mardi de Carnaval",
                    de: "Karnevalsdienstag"),
                type: HolidayType.National,
                isMovable: true,
                description: "Carnival Tuesday (Mardi Gras), 47 days before Easter Sunday."),

            // Holy Thursday — Easter - 3 days
            new HolidayInfo(
                id: "ec_holy_thursday",
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
                description: "Maundy Thursday, commemorating the Last Supper of Jesus Christ, 3 days before Easter."),

            // Good Friday — Easter - 2 days
            new HolidayInfo(
                id: "ec_good_friday",
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
