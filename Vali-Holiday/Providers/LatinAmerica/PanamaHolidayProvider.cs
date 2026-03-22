using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.LatinAmerica;

/// <summary>
/// Holiday provider for <b>Panama</b> — country code <c>PA</c>.
/// </summary>
/// <remarks>
/// <para>
/// Panama observes ten fixed national holidays and three movable feasts tied to Easter.
/// The calendar is heavily shaped by the country's complex political history, culminating
/// in the November cluster of independence and separation anniversaries.
/// </para>
/// <para>
/// The <em>Día de los Mártires</em> (9 January) is one of the most emotionally charged
/// holidays: it commemorates the 1964 Flag Riots, when Panamanian students were killed
/// while trying to fly the Panamanian flag alongside the US flag in the former Canal Zone,
/// an event that ultimately accelerated the return of the canal to Panamanian sovereignty.
/// </para>
/// <para>
/// November is known as the "Fiestas Patrias" month with four consecutive holidays: the
/// <em>Separación de Colombia</em> (3 November 1903), <em>Día de la Bandera</em> (4 November),
/// <em>Consolidación de la Separación</em> (5 November, observed in Colón), and the
/// <em>Primer Grito de Independencia de la Villa de Los Santos</em> (10 November 1821),
/// followed by the <em>Independencia de España</em> (28 November 1821).
/// </para>
/// </remarks>
public sealed class PanamaHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "PA";

    /// <inheritdoc/>
    public override string CountryName => "Panama";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "es";

    /// <inheritdoc/>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays()
    {
        const string cc = "PA";

        return new[]
        {
            // 1 January — Año Nuevo
            new HolidayInfo(
                id: "pa_new_year",
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

            // 9 January — Día de los Mártires
            new HolidayInfo(
                id: "pa_martyrs_day",
                month: 1, day: 9,
                countryCode: cc,
                name: "Día de los Mártires",
                names: Names(
                    es: "Día de los Mártires",
                    en: "Martyrs' Day",
                    pt: "Dia dos Mártires",
                    fr: "Journée des Martyrs",
                    de: "Tag der Märtyrer"),
                type: HolidayType.Civic,
                description: "Commemorates the Flag Riots of 9 January 1964, when Panamanian students and civilians were killed while protesting US control of the Canal Zone. The events accelerated the process that led to the 1977 Torrijos-Carter Treaties and Panama's sovereignty over the canal."),

            // 1 May — Día del Trabajo
            new HolidayInfo(
                id: "pa_labour_day",
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

            // 3 November — Día de la Separación de Colombia
            new HolidayInfo(
                id: "pa_separation_colombia",
                month: 11, day: 3,
                countryCode: cc,
                name: "Separación de Colombia",
                names: Names(
                    es: "Día de la Separación de Colombia",
                    en: "Separation from Colombia Day",
                    pt: "Separação da Colômbia",
                    fr: "Séparation de la Colombie",
                    de: "Trennung von Kolumbien"),
                type: HolidayType.Civic,
                description: "Marks Panama's separation from Colombia on 3 November 1903, facilitated by the United States in exchange for rights to build the Panama Canal."),

            // 4 November — Día de la Bandera
            new HolidayInfo(
                id: "pa_flag_day",
                month: 11, day: 4,
                countryCode: cc,
                name: "Día de la Bandera",
                names: Names(
                    es: "Día de la Bandera",
                    en: "Flag Day",
                    pt: "Dia da Bandeira",
                    fr: "Journée du Drapeau",
                    de: "Flaggentag"),
                type: HolidayType.Civic,
                description: "Celebrates the Panamanian national flag, first raised on 4 November 1903 following independence."),

            // 5 November — Consolidación de la Separación (Colón)
            new HolidayInfo(
                id: "pa_consolidation_colon",
                month: 11, day: 5,
                countryCode: cc,
                name: "Consolidación de la Separación",
                names: Names(
                    es: "Consolidación de la Separación",
                    en: "Consolidation of Separation",
                    pt: "Consolidação da Separação",
                    fr: "Consolidation de la Séparation",
                    de: "Konsolidierung der Trennung"),
                type: HolidayType.Civic,
                description: "Commemorates the events of 5 November 1903 in Colón, when the city completed its adherence to the newly independent Republic of Panama."),

            // 10 November — Primer Grito de Independencia de la Villa de Los Santos
            new HolidayInfo(
                id: "pa_first_cry_independence",
                month: 11, day: 10,
                countryCode: cc,
                name: "Primer Grito de Independencia",
                names: Names(
                    es: "Primer Grito de Independencia de la Villa de Los Santos",
                    en: "First Cry of Independence of Villa de Los Santos",
                    pt: "Primeiro Grito de Independência de Villa de Los Santos",
                    fr: "Premier Cri d'Indépendance de la Villa de Los Santos",
                    de: "Erster Unabhängigkeitsruf von Villa de Los Santos"),
                type: HolidayType.Civic,
                description: "Commemorates the first declaration of independence from Spain on 10 November 1821 by the town of La Villa de Los Santos, which set in motion the independence of the entire Isthmus."),

            // 28 November — Independencia de España
            new HolidayInfo(
                id: "pa_independence_spain",
                month: 11, day: 28,
                countryCode: cc,
                name: "Independencia de España",
                names: Names(
                    es: "Independencia de España",
                    en: "Independence from Spain",
                    pt: "Independência da Espanha",
                    fr: "Indépendance de l'Espagne",
                    de: "Unabhängigkeit von Spanien"),
                type: HolidayType.Civic,
                description: "Celebrates the formal independence of Panama from Spain on 28 November 1821, when Panama voluntarily joined Gran Colombia."),

            // 8 December — Día de la Madre / Inmaculada Concepción
            new HolidayInfo(
                id: "pa_mothers_day",
                month: 12, day: 8,
                countryCode: cc,
                name: "Día de la Madre",
                names: Names(
                    es: "Día de la Madre / Inmaculada Concepción",
                    en: "Mother's Day / Immaculate Conception",
                    pt: "Dia das Mães / Imaculada Conceição",
                    fr: "Fête des Mères / Immaculée Conception",
                    de: "Muttertag / Mariä Empfängnis"),
                type: HolidayType.Religious,
                description: "In Panama, the feast of the Immaculate Conception (8 December) is also officially celebrated as Mother's Day."),

            // 25 December — Navidad
            new HolidayInfo(
                id: "pa_christmas",
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
    /// Three movable holidays: Carnival Monday, Carnival Tuesday, and Good Friday.
    /// </returns>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        const string cc = "PA";

        return new[]
        {
            // Carnival Monday — Easter - 48 days
            new HolidayInfo(
                id: "pa_carnival_monday",
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
                description: "Carnival Monday — Panama's Carnival in Las Tablas and the capital is one of the most celebrated in Latin America, 48 days before Easter."),

            // Carnival Tuesday — Easter - 47 days
            new HolidayInfo(
                id: "pa_carnival_tuesday",
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

            // Good Friday — Easter - 2 days
            new HolidayInfo(
                id: "pa_good_friday",
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
