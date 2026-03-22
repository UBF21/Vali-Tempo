using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.LatinAmerica;

/// <summary>
/// Holiday provider for <b>Venezuela</b> (República Bolivariana de Venezuela) — country code <c>VE</c>.
/// </summary>
/// <remarks>
/// <para>
/// Venezuela observes fifteen fixed holidays (two of which are <see cref="HolidayType.Optional"/>)
/// and five movable feasts tied to Easter. The calendar blends Catholic religious observances
/// with civic commemorations of the independence process led by Simón Bolívar.
/// </para>
/// <para>
/// Key civic dates include: <em>19 de Abril</em> (Declaración de Independencia, 1810),
/// <em>Batalla de Carabobo</em> (24 June 1821 — the battle that sealed Venezuelan independence),
/// <em>5 de Julio</em> (formal Declaration of Independence, 1811), and
/// <em>Natalicio del Libertador</em> (24 July).
/// </para>
/// <para>
/// The <em>Muerte del Libertador</em> (17 December 1830) is unique to Venezuela among
/// Bolivarian republics, marking the death of Simón Bolívar in Santa Marta, Colombia.
/// </para>
/// <para>
/// <em>Nochebuena</em> (24 December) and <em>Fin de Año</em> (31 December) are classified as
/// <see cref="HolidayType.Optional"/> half-day holidays; in practice, many businesses
/// close at midday.
/// </para>
/// </remarks>
public sealed class VenezuelaHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "VE";

    /// <inheritdoc/>
    public override string CountryName => "Venezuela";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "es";

    /// <inheritdoc/>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays()
    {
        const string cc = "VE";

        return new[]
        {
            // 1 January — Año Nuevo
            new HolidayInfo(
                id: "ve_new_year",
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

            // 6 January — Día de Reyes
            new HolidayInfo(
                id: "ve_epiphany",
                month: 1, day: 6,
                countryCode: cc,
                name: "Día de Reyes",
                names: Names(
                    es: "Día de Reyes",
                    en: "Epiphany",
                    pt: "Dia de Reis",
                    fr: "Épiphanie",
                    de: "Heilige Drei Könige"),
                type: HolidayType.Religious,
                description: "Feast of the Epiphany, commemorating the visit of the Magi to the infant Jesus."),

            // 19 March — San José
            new HolidayInfo(
                id: "ve_san_jose",
                month: 3, day: 19,
                countryCode: cc,
                name: "San José",
                names: Names(
                    es: "San José",
                    en: "Saint Joseph's Day",
                    pt: "São José",
                    fr: "Saint-Joseph",
                    de: "Josefstag"),
                type: HolidayType.Religious,
                description: "Feast of Saint Joseph, patron of workers and of the universal Church."),

            // 19 April — Declaración de Independencia
            new HolidayInfo(
                id: "ve_independence_declaration",
                month: 4, day: 19,
                countryCode: cc,
                name: "Declaración de Independencia",
                names: Names(
                    es: "Declaración de Independencia",
                    en: "Independence Declaration Day",
                    pt: "Declaração de Independência",
                    fr: "Déclaration d'Indépendance",
                    de: "Unabhängigkeitserklärung"),
                type: HolidayType.Civic,
                description: "Marks 19 April 1810, when the Caracas Cabildo removed the Spanish Captain-General and established a governing junta, initiating Venezuela's independence movement."),

            // 1 May — Día del Trabajador
            new HolidayInfo(
                id: "ve_labour_day",
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

            // 24 June — Batalla de Carabobo y San Juan Bautista
            new HolidayInfo(
                id: "ve_carabobo",
                month: 6, day: 24,
                countryCode: cc,
                name: "Batalla de Carabobo",
                names: Names(
                    es: "Batalla de Carabobo y San Juan Bautista",
                    en: "Battle of Carabobo and Saint John the Baptist",
                    pt: "Batalha de Carabobo e São João Batista",
                    fr: "Bataille de Carabobo et Saint Jean-Baptiste",
                    de: "Schlacht von Carabobo und Johannes der Täufer"),
                type: HolidayType.Civic,
                description: "Commemorates the decisive Battle of Carabobo on 24 June 1821, won by Simón Bolívar and Antonio José de Sucre, which effectively ended Spanish rule in Venezuela. Also the feast of Saint John the Baptist."),

            // 29 June — San Pedro y San Pablo
            new HolidayInfo(
                id: "ve_ss_peter_paul",
                month: 6, day: 29,
                countryCode: cc,
                name: "San Pedro y San Pablo",
                names: Names(
                    es: "San Pedro y San Pablo",
                    en: "Saints Peter and Paul",
                    pt: "São Pedro e São Paulo",
                    fr: "Saints Pierre et Paul",
                    de: "Peter und Paul"),
                type: HolidayType.Religious,
                description: "Feast of the Apostles Saints Peter and Paul."),

            // 5 July — Día de la Independencia
            new HolidayInfo(
                id: "ve_independence",
                month: 7, day: 5,
                countryCode: cc,
                name: "Día de la Independencia",
                names: Names(
                    es: "Día de la Independencia",
                    en: "Independence Day",
                    pt: "Dia da Independência",
                    fr: "Jour de l'Indépendance",
                    de: "Unabhängigkeitstag"),
                type: HolidayType.Civic,
                description: "Celebrates the signing of Venezuela's Declaration of Independence on 5 July 1811, making it the first South American country to formally declare independence from Spain."),

            // 24 July — Natalicio del Libertador Simón Bolívar
            new HolidayInfo(
                id: "ve_bolivar_birthday",
                month: 7, day: 24,
                countryCode: cc,
                name: "Natalicio del Libertador",
                names: Names(
                    es: "Natalicio del Libertador Simón Bolívar",
                    en: "Simón Bolívar's Birthday",
                    pt: "Natalício do Libertador Simón Bolívar",
                    fr: "Naissance du Libertador Simón Bolívar",
                    de: "Geburtstag des Libertadors Simón Bolívar"),
                type: HolidayType.Civic,
                description: "Celebrates the birth of Simón Bolívar in Caracas on 24 July 1783, Libertador of Venezuela, Colombia, Ecuador, Peru, and Bolivia."),

            // 12 October — Día de la Resistencia Indígena
            new HolidayInfo(
                id: "ve_indigenous_resistance",
                month: 10, day: 12,
                countryCode: cc,
                name: "Día de la Resistencia Indígena",
                names: Names(
                    es: "Día de la Resistencia Indígena",
                    en: "Indigenous Resistance Day",
                    pt: "Dia da Resistência Indígena",
                    fr: "Journée de la Résistance Indigène",
                    de: "Tag des indigenen Widerstands"),
                type: HolidayType.Civic,
                description: "Renamed from 'Día de la Raza' by Decree 2028 (2002) to honour indigenous peoples' resistance against European colonisation beginning in 1492."),

            // 8 December — Inmaculada Concepción
            new HolidayInfo(
                id: "ve_immaculate_conception",
                month: 12, day: 8,
                countryCode: cc,
                name: "Inmaculada Concepción",
                names: Names(
                    es: "Inmaculada Concepción",
                    en: "Immaculate Conception",
                    pt: "Imaculada Conceição",
                    fr: "Immaculée Conception",
                    de: "Mariä Empfängnis"),
                type: HolidayType.Religious,
                description: "Feast of the Immaculate Conception of the Virgin Mary."),

            // 17 December — Muerte del Libertador Simón Bolívar
            new HolidayInfo(
                id: "ve_bolivar_death",
                month: 12, day: 17,
                countryCode: cc,
                name: "Muerte del Libertador",
                names: Names(
                    es: "Muerte del Libertador Simón Bolívar",
                    en: "Death of the Liberator Simón Bolívar",
                    pt: "Morte do Libertador Simón Bolívar",
                    fr: "Mort du Libertador Simón Bolívar",
                    de: "Tod des Libertadors Simón Bolívar"),
                type: HolidayType.Civic,
                description: "Commemorates the death of Simón Bolívar on 17 December 1830 in Santa Marta, Colombia, after being exiled from Gran Colombia."),

            // 24 December — Nochebuena (Optional)
            new HolidayInfo(
                id: "ve_christmas_eve",
                month: 12, day: 24,
                countryCode: cc,
                name: "Nochebuena",
                names: Names(
                    es: "Nochebuena",
                    en: "Christmas Eve",
                    pt: "Véspera de Natal",
                    fr: "Veille de Noël",
                    de: "Heiligabend"),
                type: HolidayType.Optional,
                description: "Christmas Eve — optional half-day holiday; many businesses close at midday."),

            // 25 December — Navidad
            new HolidayInfo(
                id: "ve_christmas",
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

            // 31 December — Fin de Año (Optional)
            new HolidayInfo(
                id: "ve_new_year_eve",
                month: 12, day: 31,
                countryCode: cc,
                name: "Fin de Año",
                names: Names(
                    es: "Fin de Año",
                    en: "New Year's Eve",
                    pt: "Réveillon",
                    fr: "Saint-Sylvestre",
                    de: "Silvester"),
                type: HolidayType.Optional,
                description: "New Year's Eve — optional half-day holiday; many businesses close at midday."),
        };
    }

    /// <inheritdoc/>
    /// <returns>
    /// Five movable holidays: Carnival Monday, Carnival Tuesday, Holy Thursday, Good Friday, and Corpus Christi.
    /// </returns>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        const string cc = "VE";

        return new[]
        {
            // Carnival Monday — Easter - 48 days
            new HolidayInfo(
                id: "ve_carnival_monday",
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
                id: "ve_carnival_tuesday",
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
                id: "ve_holy_thursday",
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
                id: "ve_good_friday",
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

            // Corpus Christi — Easter + 60 days
            new HolidayInfo(
                id: "ve_corpus_christi",
                month: EasterCalculator.CorpusChristi(year).Month,
                day: EasterCalculator.CorpusChristi(year).Day,
                countryCode: cc,
                name: "Corpus Christi",
                names: Names(
                    es: "Corpus Christi",
                    en: "Corpus Christi",
                    pt: "Corpus Christi",
                    fr: "Fête-Dieu",
                    de: "Fronleichnam"),
                type: HolidayType.Religious,
                isMovable: true,
                description: "Solemnity of the Body and Blood of Christ, 60 days after Easter Sunday."),
        };
    }
}
