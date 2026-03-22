using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.LatinAmerica;

/// <summary>
/// Holiday provider for <b>Uruguay</b> — country code <c>UY</c>.
/// </summary>
/// <remarks>
/// <para>
/// Uruguay is a constitutionally secular state, and its official holiday nomenclature
/// deliberately avoids religious references. Accordingly, many observances carry civic names:
/// <em>Día de la Familia</em> instead of Christmas, <em>Semana de Turismo</em> instead of
/// Holy Week, and <em>Día de los Difuntos</em> instead of All Souls' Day.
/// </para>
/// <para>
/// The fixed calendar covers key milestones of Uruguayan independence: the
/// <em>Desembarco de los 33 Orientales</em> (19 April 1825), the
/// <em>Batalla de Las Piedras</em> (18 May 1811), the <em>Natalicio de Artigas</em>
/// (19 June — also <em>Día del Nunca Más</em>), the <em>Jura de la Constitución</em>
/// (18 July 1830), and the <em>Día de la Independencia</em> (25 August 1825).
/// </para>
/// <para>
/// Movable holidays include Carnival Monday and Tuesday plus the three days of
/// <em>Semana de Turismo</em> (Holy Thursday, Good Friday, and Holy Saturday),
/// which are all labelled with their secular Uruguayan names.
/// </para>
/// </remarks>
public sealed class UruguayHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "UY";

    /// <inheritdoc/>
    public override string CountryName => "Uruguay";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "es";

    /// <inheritdoc/>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays()
    {
        const string cc = "UY";

        return new[]
        {
            // 1 January — Año Nuevo
            new HolidayInfo(
                id: "uy_new_year",
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

            // 6 January — Día de Reyes / Día de los Niños
            new HolidayInfo(
                id: "uy_epiphany",
                month: 1, day: 6,
                countryCode: cc,
                name: "Día de Reyes",
                names: Names(
                    es: "Día de Reyes / Día de los Niños",
                    en: "Epiphany / Children's Day",
                    pt: "Dia de Reis / Dia das Crianças",
                    fr: "Épiphanie / Jour des Enfants",
                    de: "Heilige Drei Könige / Kindertag"),
                type: HolidayType.National,
                description: "Epiphany — in Uruguay also celebrated as Children's Day, when children traditionally receive gifts."),

            // 19 April — Desembarco de los 33 Orientales
            new HolidayInfo(
                id: "uy_33_orientales",
                month: 4, day: 19,
                countryCode: cc,
                name: "Desembarco de los 33 Orientales",
                names: Names(
                    es: "Desembarco de los 33 Orientales",
                    en: "Landing of the 33 Orientales",
                    pt: "Desembarque dos 33 Orientais",
                    fr: "Débarquement des 33 Orientaux",
                    de: "Landung der 33 Orientalen"),
                type: HolidayType.Civic,
                description: "Commemorates the landing on 19 April 1825 of the 33 Orientales led by Juan Antonio Lavalleja, which launched the campaign for Uruguayan independence from the Empire of Brazil."),

            // 1 May — Día de los Trabajadores
            new HolidayInfo(
                id: "uy_labour_day",
                month: 5, day: 1,
                countryCode: cc,
                name: "Día de los Trabajadores",
                names: Names(
                    es: "Día de los Trabajadores",
                    en: "Workers' Day",
                    pt: "Dia dos Trabalhadores",
                    fr: "Fête du Travail",
                    de: "Tag der Arbeit"),
                type: HolidayType.National,
                description: "International Workers' Day."),

            // 18 May — Batalla de Las Piedras
            new HolidayInfo(
                id: "uy_battle_las_piedras",
                month: 5, day: 18,
                countryCode: cc,
                name: "Batalla de Las Piedras",
                names: Names(
                    es: "Batalla de Las Piedras",
                    en: "Battle of Las Piedras",
                    pt: "Batalha de Las Piedras",
                    fr: "Bataille de Las Piedras",
                    de: "Schlacht von Las Piedras"),
                type: HolidayType.Civic,
                description: "Commemorates the Battle of Las Piedras on 18 May 1811, a decisive Uruguayan victory led by José Gervasio Artigas against Spanish royalist forces."),

            // 19 June — Natalicio de Artigas y Día del Nunca Más
            new HolidayInfo(
                id: "uy_artigas_day",
                month: 6, day: 19,
                countryCode: cc,
                name: "Natalicio de Artigas y Día del Nunca Más",
                names: Names(
                    es: "Natalicio de Artigas y Día del Nunca Más",
                    en: "Artigas' Birthday and Day of Never Again",
                    pt: "Natalício de Artigas e Dia do Nunca Mais",
                    fr: "Anniversaire d'Artigas et Journée du Jamais Plus",
                    de: "Geburtstag Artigas' und Tag des Nie Wieder"),
                type: HolidayType.Civic,
                description: "Celebrates the birth of José Gervasio Artigas (19 June 1764), founding father and national hero of Uruguay. Also observed as 'Día del Nunca Más', commemorating victims of the 1973–1985 military dictatorship."),

            // 18 July — Jura de la Constitución
            new HolidayInfo(
                id: "uy_constitution_day",
                month: 7, day: 18,
                countryCode: cc,
                name: "Jura de la Constitución",
                names: Names(
                    es: "Jura de la Constitución",
                    en: "Constitution Day",
                    pt: "Juramento da Constituição",
                    fr: "Serment de la Constitution",
                    de: "Verfassungstag"),
                type: HolidayType.Civic,
                description: "Commemorates the swearing-in of Uruguay's first Constitution on 18 July 1830, establishing the republic."),

            // 25 August — Día de la Independencia
            new HolidayInfo(
                id: "uy_independence",
                month: 8, day: 25,
                countryCode: cc,
                name: "Día de la Independencia",
                names: Names(
                    es: "Día de la Independencia",
                    en: "Independence Day",
                    pt: "Dia da Independência",
                    fr: "Jour de l'Indépendance",
                    de: "Unabhängigkeitstag"),
                type: HolidayType.Civic,
                description: "Marks the declaration of Uruguayan independence on 25 August 1825, formally separating from the Empire of Brazil and initiating the process that led to the 1828 treaty."),

            // 12 October — Día de la Raza / Día de la Diversidad Cultural
            new HolidayInfo(
                id: "uy_cultural_diversity",
                month: 10, day: 12,
                countryCode: cc,
                name: "Día de la Diversidad Cultural",
                names: Names(
                    es: "Día de la Diversidad Cultural",
                    en: "Day of Cultural Diversity",
                    pt: "Dia da Diversidade Cultural",
                    fr: "Journée de la Diversité Culturelle",
                    de: "Tag der kulturellen Vielfalt"),
                type: HolidayType.Civic,
                description: "Formerly 'Día de la Raza'; renamed by Law 18.589 (2009) to 'Día de la Diversidad Cultural', celebrating Uruguay's multicultural heritage."),

            // 2 November — Día de los Difuntos
            new HolidayInfo(
                id: "uy_all_souls",
                month: 11, day: 2,
                countryCode: cc,
                name: "Día de los Difuntos",
                names: Names(
                    es: "Día de los Difuntos",
                    en: "Day of the Dead",
                    pt: "Dia dos Finados",
                    fr: "Jour des Défunts",
                    de: "Allerseelen"),
                type: HolidayType.National,
                description: "Day of remembrance for the deceased. Uruguay uses the secular name 'Día de los Difuntos' rather than the Catholic 'Día de Todos los Santos'."),

            // 25 December — Día de la Familia
            new HolidayInfo(
                id: "uy_family_day",
                month: 12, day: 25,
                countryCode: cc,
                name: "Día de la Familia",
                names: Names(
                    es: "Día de la Familia",
                    en: "Family Day",
                    pt: "Dia da Família",
                    fr: "Journée de la Famille",
                    de: "Tag der Familie"),
                type: HolidayType.National,
                description: "Uruguay uses the secular name 'Día de la Familia' for 25 December, reflecting the country's separation of church and state."),
        };
    }

    /// <inheritdoc/>
    /// <returns>
    /// Five movable holidays: Carnival Monday, Carnival Tuesday, and the three days of
    /// Semana de Turismo (Holy Thursday, Good Friday, Holy Saturday).
    /// </returns>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        const string cc = "UY";

        return new[]
        {
            // Carnival Monday — Easter - 48 days
            new HolidayInfo(
                id: "uy_carnival_monday",
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
                description: "Carnival Monday, 48 days before Easter. Uruguay's Carnival is the longest in the world, lasting about 40 days."),

            // Carnival Tuesday — Easter - 47 days
            new HolidayInfo(
                id: "uy_carnival_tuesday",
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
                description: "Carnival Tuesday (Mardi Gras), 47 days before Easter."),

            // Semana de Turismo — Jueves (Holy Thursday — Easter - 3 days)
            new HolidayInfo(
                id: "uy_turismo_thursday",
                month: EasterCalculator.HolyThursday(year).Month,
                day: EasterCalculator.HolyThursday(year).Day,
                countryCode: cc,
                name: "Semana de Turismo (Jueves)",
                names: Names(
                    es: "Semana de Turismo — Jueves",
                    en: "Tourism Week — Thursday",
                    pt: "Semana de Turismo — Quinta-Feira",
                    fr: "Semaine du Tourisme — Jeudi",
                    de: "Tourismuswoche — Donnerstag"),
                type: HolidayType.National,
                isMovable: true,
                description: "Uruguay's secular name for Holy Thursday. 'Semana de Turismo' (Tourism Week) replaces 'Semana Santa' reflecting the country's secular constitution. Falls 3 days before Easter."),

            // Semana de Turismo — Viernes (Good Friday — Easter - 2 days)
            new HolidayInfo(
                id: "uy_turismo_friday",
                month: EasterCalculator.GoodFriday(year).Month,
                day: EasterCalculator.GoodFriday(year).Day,
                countryCode: cc,
                name: "Semana de Turismo (Viernes)",
                names: Names(
                    es: "Semana de Turismo — Viernes",
                    en: "Tourism Week — Friday",
                    pt: "Semana de Turismo — Sexta-Feira",
                    fr: "Semaine du Tourisme — Vendredi",
                    de: "Tourismuswoche — Freitag"),
                type: HolidayType.National,
                isMovable: true,
                description: "Uruguay's secular name for Good Friday. Falls 2 days before Easter Sunday."),

            // Semana de Turismo — Sábado (Holy Saturday — Easter - 1 day)
            new HolidayInfo(
                id: "uy_turismo_saturday",
                month: EasterCalculator.HolySaturday(year).Month,
                day: EasterCalculator.HolySaturday(year).Day,
                countryCode: cc,
                name: "Semana de Turismo (Sábado)",
                names: Names(
                    es: "Semana de Turismo — Sábado",
                    en: "Tourism Week — Saturday",
                    pt: "Semana de Turismo — Sábado",
                    fr: "Semaine du Tourisme — Samedi",
                    de: "Tourismuswoche — Samstag"),
                type: HolidayType.National,
                isMovable: true,
                description: "Uruguay's secular name for Holy Saturday. Falls 1 day before Easter Sunday."),
        };
    }
}
