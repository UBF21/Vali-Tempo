using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.LatinAmerica;

/// <summary>
/// Holiday provider for <b>Dominican Republic</b> (República Dominicana) — country code <c>DO</c>.
/// </summary>
/// <remarks>
/// <para>
/// The Dominican Republic observes eleven fixed national holidays and one movable feast
/// tied to Easter. The calendar is rich with commemorations of the independence struggle
/// led by Juan Pablo Duarte and the Trinitarios.
/// </para>
/// <para>
/// The <em>Día de la Altagracia</em> (21 January) honours the Virgin of Altagracia,
/// Patroness of the Dominican Republic and the oldest Marian shrine in the Americas.
/// </para>
/// <para>
/// The <em>Natalicio de Duarte</em> (26 January) and <em>Día de la Independencia Nacional</em>
/// (27 February 1844) commemorate Juan Pablo Duarte, co-founder of La Trinitaria (1838),
/// and the independence of the Dominican Republic from Haiti.
/// </para>
/// <para>
/// The <em>Día de la Restauración</em> (16 August 1863) marks the restoration of the republic
/// after the Spanish Annexation (1861–1865), one of the few successful anti-colonial revolutions
/// in the Caribbean. The <em>Día de la Constitución</em> (6 November) commemorates the first
/// Dominican constitution, adopted in 1844.
/// </para>
/// <para>
/// The <em>Día de la Virgen de las Mercedes</em> (24 September) honours the patroness of
/// the Dominican Armed Forces.
/// </para>
/// </remarks>
public sealed class DominicanRepublicHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "DO";

    /// <inheritdoc/>
    public override string CountryName => "Dominican Republic";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "es";

    /// <inheritdoc/>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays()
    {
        const string cc = "DO";

        return new[]
        {
            // 1 January — Año Nuevo
            new HolidayInfo(
                id: "do_new_year",
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
                id: "do_epiphany",
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
                description: "Feast of the Epiphany — the Three Wise Men are celebrated with gift-giving traditions for children."),

            // 21 January — Día de la Altagracia
            new HolidayInfo(
                id: "do_altagracia",
                month: 1, day: 21,
                countryCode: cc,
                name: "Día de la Altagracia",
                names: Names(
                    es: "Día de la Altagracia",
                    en: "Our Lady of Altagracia Day",
                    pt: "Dia de Nossa Senhora de Altagrácia",
                    fr: "Journée de Notre-Dame d'Altagracia",
                    de: "Tag Unserer Lieben Frau von Altagracia"),
                type: HolidayType.Religious,
                description: "Feast day of the Virgin of Altagracia, Patroness of the Dominican Republic. Her sanctuary in Higüey holds one of the oldest Marian images in the Americas, dating to the early 16th century."),

            // 26 January — Natalicio de Duarte
            new HolidayInfo(
                id: "do_duarte_birthday",
                month: 1, day: 26,
                countryCode: cc,
                name: "Natalicio de Duarte",
                names: Names(
                    es: "Natalicio de Duarte",
                    en: "Duarte's Birthday",
                    pt: "Natalício de Duarte",
                    fr: "Naissance de Duarte",
                    de: "Geburtstag Duartes"),
                type: HolidayType.Civic,
                description: "Celebrates the birth of Juan Pablo Duarte (26 January 1813), founding father of the Dominican Republic and co-founder of La Trinitaria (1838), the secret society that fought for independence from Haiti."),

            // 27 February — Día de la Independencia Nacional
            new HolidayInfo(
                id: "do_independence",
                month: 2, day: 27,
                countryCode: cc,
                name: "Día de la Independencia Nacional",
                names: Names(
                    es: "Día de la Independencia Nacional",
                    en: "National Independence Day",
                    pt: "Dia da Independência Nacional",
                    fr: "Jour de l'Indépendance Nationale",
                    de: "Nationaler Unabhängigkeitstag"),
                type: HolidayType.Civic,
                description: "Marks the Dominican Republic's declaration of independence from Haiti on 27 February 1844, led by Juan Pablo Duarte and the Trinitarios."),

            // 1 May — Día del Trabajo
            new HolidayInfo(
                id: "do_labour_day",
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

            // 16 June — Día de la Fundación de la Sociedad la Trinitaria
            new HolidayInfo(
                id: "do_trinitaria_foundation",
                month: 6, day: 16,
                countryCode: cc,
                name: "Fundación de la Sociedad la Trinitaria",
                names: Names(
                    es: "Día de la Fundación de la Sociedad la Trinitaria",
                    en: "Foundation of La Trinitaria Society",
                    pt: "Fundação da Sociedade La Trinitaria",
                    fr: "Fondation de la Société La Trinitaria",
                    de: "Gründung der Gesellschaft La Trinitaria"),
                type: HolidayType.Civic,
                description: "Commemorates the founding of La Trinitaria on 16 July 1838 by Juan Pablo Duarte — the secret patriotic society that organised the independence movement against Haitian rule."),

            // 16 August — Día de la Restauración
            new HolidayInfo(
                id: "do_restoration_day",
                month: 8, day: 16,
                countryCode: cc,
                name: "Día de la Restauración",
                names: Names(
                    es: "Día de la Restauración",
                    en: "Restoration Day",
                    pt: "Dia da Restauração",
                    fr: "Journée de la Restauration",
                    de: "Restaurationstag"),
                type: HolidayType.Civic,
                description: "Marks the proclamation of the restoration of the Dominican Republic on 16 August 1863, ending the Spanish Annexation (1861–1865) — one of the few successful anti-colonial revolutions in Caribbean history."),

            // 24 September — Día de la Virgen de las Mercedes
            new HolidayInfo(
                id: "do_virgen_mercedes",
                month: 9, day: 24,
                countryCode: cc,
                name: "Día de la Virgen de las Mercedes",
                names: Names(
                    es: "Día de la Virgen de las Mercedes",
                    en: "Our Lady of Mercy Day",
                    pt: "Dia de Nossa Senhora das Mercês",
                    fr: "Journée de Notre-Dame de la Merci",
                    de: "Tag Unserer Lieben Frau von der Barmherzigkeit"),
                type: HolidayType.Religious,
                description: "Feast of Our Lady of Mercy, official patroness of the Dominican Republic and its Armed Forces. Tradition holds that this apparition aided Christopher Columbus in battle in 1495."),

            // 6 November — Día de la Constitución
            new HolidayInfo(
                id: "do_constitution_day",
                month: 11, day: 6,
                countryCode: cc,
                name: "Día de la Constitución",
                names: Names(
                    es: "Día de la Constitución",
                    en: "Constitution Day",
                    pt: "Dia da Constituição",
                    fr: "Journée de la Constitution",
                    de: "Verfassungstag"),
                type: HolidayType.Civic,
                description: "Commemorates the adoption of the first Dominican Constitution on 6 November 1844, establishing the republic's foundational legal framework."),

            // 25 December — Navidad
            new HolidayInfo(
                id: "do_christmas",
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
    /// One movable holiday: Good Friday.
    /// </returns>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        const string cc = "DO";

        return new[]
        {
            // Good Friday — Easter - 2 days
            new HolidayInfo(
                id: "do_good_friday",
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
