using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.LatinAmerica;

/// <summary>
/// Holiday provider for <b>Bolivia</b> (Estado Plurinacional de Bolivia) — country code <c>BO</c>.
/// </summary>
/// <remarks>
/// <para>
/// Bolivia observes nine fixed national holidays plus three movable feasts tied to Easter
/// and one regional Carnival observance.
/// </para>
/// <para>
/// Key fixed dates include the <em>Día del Estado Plurinacional</em> (22 January), established
/// in 2010 following the adoption of the new Political Constitution that recognised Bolivia as a
/// plurinational state. The <em>Año Nuevo Andino Amazónico</em> (21 June, <em>Willkakuti</em>)
/// marks the Andean and Amazonian New Year at the winter solstice, rooted in indigenous
/// Aymara and Quechua cosmovision.
/// </para>
/// <para>
/// The <em>Día de la Revolución Agraria</em> (2 August) commemorates the 1953 Agrarian Reform
/// decreed by President Víctor Paz Estenssoro, which redistributed land to indigenous communities.
/// Independence Day (6 August 1825) celebrates the formal declaration of independence named after
/// Simón Bolívar, who gave the country its name.
/// </para>
/// <para>
/// Carnival varies by region and is included as an <see cref="HolidayType.Observance"/> on
/// the Tuesday before Ash Wednesday (Easter − 47 days).
/// </para>
/// </remarks>
public sealed class BoliviaHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "BO";

    /// <inheritdoc/>
    public override string CountryName => "Bolivia";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "es";

    /// <inheritdoc/>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays()
    {
        const string cc = "BO";

        return new[]
        {
            // 1 January — Año Nuevo
            new HolidayInfo(
                id: "bo_new_year",
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

            // 22 January — Día del Estado Plurinacional
            new HolidayInfo(
                id: "bo_plurinational_state",
                month: 1, day: 22,
                countryCode: cc,
                name: "Día del Estado Plurinacional",
                names: Names(
                    es: "Día del Estado Plurinacional",
                    en: "Plurinational State Day",
                    pt: "Dia do Estado Plurinacional",
                    fr: "Journée de l'État Plurinational",
                    de: "Tag des Plurinationalen Staates"),
                type: HolidayType.Civic,
                description: "Established in 2010, this date marks the inauguration of President Evo Morales and the transformation of Bolivia into a plurinational state recognising indigenous peoples' rights."),

            // 1 May — Día del Trabajo
            new HolidayInfo(
                id: "bo_labour_day",
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

            // 21 June — Año Nuevo Andino Amazónico (Willkakuti)
            new HolidayInfo(
                id: "bo_andean_new_year",
                month: 6, day: 21,
                countryCode: cc,
                name: "Año Nuevo Andino Amazónico",
                names: Names(
                    es: "Año Nuevo Andino Amazónico",
                    en: "Andean Amazonian New Year",
                    pt: "Ano Novo Andino Amazônico",
                    fr: "Nouvel An Andin Amazonien",
                    de: "Andines Amazonisches Neujahr"),
                type: HolidayType.National,
                description: "Willkakuti ('Return of the Sun' in Aymara) — Andean and Amazonian New Year celebrated at the winter solstice, rooted in indigenous cosmovision of the Aymara and Quechua peoples."),

            // 2 August — Día de la Revolución Agraria
            new HolidayInfo(
                id: "bo_agrarian_revolution",
                month: 8, day: 2,
                countryCode: cc,
                name: "Día de la Revolución Agraria",
                names: Names(
                    es: "Día de la Revolución Agraria",
                    en: "Agrarian Revolution Day",
                    pt: "Dia da Revolução Agrária",
                    fr: "Journée de la Révolution Agraire",
                    de: "Tag der Agrarrevolution"),
                type: HolidayType.Civic,
                description: "Commemorates the Agrarian Reform Decree of 2 August 1953 signed by President Víctor Paz Estenssoro, which redistributed land to indigenous Bolivian communities."),

            // 6 August — Día de la Independencia
            new HolidayInfo(
                id: "bo_independence",
                month: 8, day: 6,
                countryCode: cc,
                name: "Día de la Independencia",
                names: Names(
                    es: "Día de la Independencia",
                    en: "Independence Day",
                    pt: "Dia da Independência",
                    fr: "Jour de l'Indépendance",
                    de: "Unabhängigkeitstag"),
                type: HolidayType.Civic,
                description: "Celebrates Bolivia's formal declaration of independence on 6 August 1825. The country was named after Simón Bolívar, who championed its liberation."),

            // 12 October — Día de la Descolonización
            new HolidayInfo(
                id: "bo_decolonization_day",
                month: 10, day: 12,
                countryCode: cc,
                name: "Día de la Descolonización",
                names: Names(
                    es: "Día de la Descolonización",
                    en: "Decolonization Day",
                    pt: "Dia da Descolonização",
                    fr: "Journée de la Décolonisation",
                    de: "Tag der Dekolonisierung"),
                type: HolidayType.Civic,
                description: "Bolivia renamed 12 October as 'Día de la Descolonización' to highlight indigenous resistance against European colonisation, replacing the former 'Día de la Raza'."),

            // 2 November — Día de los Difuntos
            new HolidayInfo(
                id: "bo_all_souls",
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
                description: "All Souls' Day — day of prayer and remembrance for the deceased."),

            // 25 December — Navidad
            new HolidayInfo(
                id: "bo_christmas",
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
    /// Three movable holidays (Holy Thursday, Good Friday, Corpus Christi) plus Carnival as an observance.
    /// </returns>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        const string cc = "BO";

        return new[]
        {
            // Carnival — Easter - 47 days (regional observance, varies by region)
            new HolidayInfo(
                id: "bo_carnival",
                month: EasterCalculator.CarnavalTuesday(year).Month,
                day: EasterCalculator.CarnavalTuesday(year).Day,
                countryCode: cc,
                name: "Carnaval",
                names: Names(
                    es: "Carnaval",
                    en: "Carnival",
                    pt: "Carnaval",
                    fr: "Carnaval",
                    de: "Karneval"),
                type: HolidayType.Observance,
                isMovable: true,
                description: "Carnival observed on the Tuesday before Ash Wednesday (Easter − 47 days). Dates and duration vary significantly by region; Oruro hosts one of the most celebrated Carnivals in the world (UNESCO Intangible Heritage)."),

            // Holy Thursday — Easter - 3 days
            new HolidayInfo(
                id: "bo_holy_thursday",
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
                description: "Maundy Thursday — commemorates the Last Supper of Jesus Christ, 3 days before Easter."),

            // Good Friday — Easter - 2 days
            new HolidayInfo(
                id: "bo_good_friday",
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
                id: "bo_corpus_christi",
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
                description: "Solemnity celebrating the Real Presence of Jesus Christ in the Eucharist, 60 days after Easter Sunday."),
        };
    }
}
