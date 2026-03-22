using Vali_Holiday.Core;
using Vali_Holiday.Models;

namespace Vali_Holiday.Providers.LatinAmerica;

/// <summary>
/// Holiday provider for <b>Cuba</b> — country code <c>CU</c>.
/// </summary>
/// <remarks>
/// <para>
/// Cuba is a constitutionally secular socialist state, and its public holiday calendar
/// is predominantly civic, commemorating key events of the Cuban Revolution and the
/// independence wars of the 19th century. There are no Easter-based movable holidays
/// in the official Cuban calendar.
/// </para>
/// <para>
/// The <em>Día del Triunfo de la Revolución</em> (1 January) marks the entry of Fidel Castro's
/// rebel forces into Havana on 1 January 1959, ending the Batista dictatorship. It coincides
/// with the traditional New Year celebration.
/// </para>
/// <para>
/// The <em>Día de la Rebeldía Nacional</em> spans three consecutive days (25–27 July),
/// commemorating the failed but symbolically foundational assault on the Moncada Barracks
/// in Santiago de Cuba on 26 July 1953, led by Fidel Castro — the event that launched
/// the Cuban revolutionary movement.
/// </para>
/// <para>
/// The <em>Inicio de las Guerras de Independencia</em> (10 October) marks the
/// <em>Grito de Yara</em> (Cry of Yara) in 1868, when Carlos Manuel de Céspedes freed
/// his slaves and launched the Ten Years' War against Spanish colonial rule.
/// </para>
/// <para>
/// <em>Navidad</em> (25 December) was restored as an official holiday in 1998 following
/// Pope John Paul II's historic visit to Cuba, after having been suppressed since 1969.
/// </para>
/// </remarks>
public sealed class CubaHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "CU";

    /// <inheritdoc/>
    public override string CountryName => "Cuba";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "es";

    /// <inheritdoc/>
    /// <returns>
    /// Ten fixed public holidays. Cuba has no official movable (Easter-based) holidays.
    /// </returns>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays()
    {
        const string cc = "CU";

        return new[]
        {
            // 1 January — Día del Triunfo de la Revolución y Día de la Liberación Nacional
            new HolidayInfo(
                id: "cu_revolution_triumph",
                month: 1, day: 1,
                countryCode: cc,
                name: "Día del Triunfo de la Revolución",
                names: Names(
                    es: "Día del Triunfo de la Revolución y Día de la Liberación Nacional",
                    en: "Day of the Revolution's Triumph and National Liberation Day",
                    pt: "Dia do Triunfo da Revolução e Dia da Libertação Nacional",
                    fr: "Jour du Triomphe de la Révolution et Journée de la Libération Nationale",
                    de: "Tag des Triumphes der Revolution und nationaler Befreiungstag"),
                type: HolidayType.Civic,
                description: "Commemorates the triumph of the Cuban Revolution on 1 January 1959, when Fidel Castro's rebel forces entered Havana as President Batista fled. Also celebrated as New Year's Day."),

            // 2 January — Día de la Victoria de las Fuerzas Armadas Revolucionarias
            new HolidayInfo(
                id: "cu_far_victory",
                month: 1, day: 2,
                countryCode: cc,
                name: "Día de la Victoria de las Fuerzas Armadas Revolucionarias",
                names: Names(
                    es: "Día de la Victoria de las Fuerzas Armadas Revolucionarias",
                    en: "Victory Day of the Revolutionary Armed Forces",
                    pt: "Dia da Vitória das Forças Armadas Revolucionárias",
                    fr: "Journée de la Victoire des Forces Armées Révolutionnaires",
                    de: "Tag des Sieges der Revolutionären Streitkräfte"),
                type: HolidayType.Civic,
                description: "Celebrates the victory of the Revolutionary Armed Forces (FAR) and the consolidation of revolutionary power on 2 January 1959."),

            // 19 April — Día de la Victoria (Playa Girón)
            new HolidayInfo(
                id: "cu_playa_giron",
                month: 4, day: 19,
                countryCode: cc,
                name: "Día de la Victoria",
                names: Names(
                    es: "Día de la Victoria (Playa Girón)",
                    en: "Victory Day (Bay of Pigs)",
                    pt: "Dia da Vitória (Baía dos Porcos)",
                    fr: "Jour de la Victoire (Baie des Cochons)",
                    de: "Siegestag (Schweinebucht)"),
                type: HolidayType.Civic,
                description: "Commemorates Cuba's victory at the Bay of Pigs (Playa Girón) on 19 April 1961, when Cuban forces repelled the CIA-backed invasion of Cuban exiles within 72 hours, the first defeat of US imperialism in Latin America according to Cuban official history."),

            // 1 May — Día Internacional de los Trabajadores
            new HolidayInfo(
                id: "cu_labour_day",
                month: 5, day: 1,
                countryCode: cc,
                name: "Día Internacional de los Trabajadores",
                names: Names(
                    es: "Día Internacional de los Trabajadores",
                    en: "International Workers' Day",
                    pt: "Dia Internacional dos Trabalhadores",
                    fr: "Journée Internationale des Travailleurs",
                    de: "Internationaler Tag der Arbeit"),
                type: HolidayType.National,
                description: "International Workers' Day, celebrated with the massive May Day parade in Havana's Revolution Square."),

            // 25 July — Día de la Rebeldía Nacional (víspera del Moncada)
            new HolidayInfo(
                id: "cu_rebeldía_eve",
                month: 7, day: 25,
                countryCode: cc,
                name: "Día de la Rebeldía Nacional",
                names: Names(
                    es: "Día de la Rebeldía Nacional (víspera del Moncada)",
                    en: "National Rebellion Day (Moncada Eve)",
                    pt: "Dia da Rebeldia Nacional (véspera do Moncada)",
                    fr: "Journée de la Rébellion Nationale (veille du Moncada)",
                    de: "Nationaler Rebellionstag (Vorabend des Moncada)"),
                type: HolidayType.Civic,
                description: "First of three consecutive days commemorating the Moncada assault — the eve of the attack on the Moncada Barracks (26 July 1953)."),

            // 26 July — Día de la Rebeldía Nacional (asalto al Cuartel Moncada)
            new HolidayInfo(
                id: "cu_rebeldía_moncada",
                month: 7, day: 26,
                countryCode: cc,
                name: "Día de la Rebeldía Nacional",
                names: Names(
                    es: "Día de la Rebeldía Nacional (asalto al Cuartel Moncada)",
                    en: "National Rebellion Day (Moncada Barracks Assault)",
                    pt: "Dia da Rebeldia Nacional (assalto ao Quartel Moncada)",
                    fr: "Journée de la Rébellion Nationale (assaut de la Caserne Moncada)",
                    de: "Nationaler Rebellionstag (Sturm auf die Moncada-Kaserne)"),
                type: HolidayType.Civic,
                description: "Commemorates the assault on the Moncada Barracks in Santiago de Cuba on 26 July 1953, led by Fidel Castro. Although militarily defeated, this event is considered the founding act of the Cuban Revolution."),

            // 27 July — Día de la Rebeldía Nacional (continuación)
            new HolidayInfo(
                id: "cu_rebeldía_continuation",
                month: 7, day: 27,
                countryCode: cc,
                name: "Día de la Rebeldía Nacional",
                names: Names(
                    es: "Día de la Rebeldía Nacional (continuación)",
                    en: "National Rebellion Day (continuation)",
                    pt: "Dia da Rebeldia Nacional (continuação)",
                    fr: "Journée de la Rébellion Nationale (suite)",
                    de: "Nationaler Rebellionstag (Fortsetzung)"),
                type: HolidayType.Civic,
                description: "Third day of the National Rebellion commemorations (25–27 July), marking the continued struggle of the revolutionary movement following the Moncada assault."),

            // 10 October — Inicio de las Guerras de Independencia (Grito de Yara)
            new HolidayInfo(
                id: "cu_independence_wars",
                month: 10, day: 10,
                countryCode: cc,
                name: "Inicio de las Guerras de Independencia",
                names: Names(
                    es: "Inicio de las Guerras de Independencia",
                    en: "Beginning of the Independence Wars",
                    pt: "Início das Guerras de Independência",
                    fr: "Début des Guerres d'Indépendance",
                    de: "Beginn der Unabhängigkeitskriege"),
                type: HolidayType.Civic,
                description: "Marks the Grito de Yara on 10 October 1868: Carlos Manuel de Céspedes freed his slaves and launched the Ten Years' War (1868–1878), the first of Cuba's independence wars against Spanish colonial rule."),

            // 25 December — Día de Navidad
            new HolidayInfo(
                id: "cu_christmas",
                month: 12, day: 25,
                countryCode: cc,
                name: "Día de Navidad",
                names: Names(
                    es: "Día de Navidad",
                    en: "Christmas Day",
                    pt: "Dia de Natal",
                    fr: "Jour de Noël",
                    de: "Weihnachten"),
                type: HolidayType.Religious,
                description: "Christmas Day — restored as an official Cuban holiday in December 1997 (effective 1998) by Fidel Castro ahead of Pope John Paul II's historic visit to Cuba in January 1998, after having been suppressed since 1969."),

            // 31 December — Fin de Año
            new HolidayInfo(
                id: "cu_new_year_eve",
                month: 12, day: 31,
                countryCode: cc,
                name: "Fin de Año",
                names: Names(
                    es: "Fin de Año",
                    en: "New Year's Eve",
                    pt: "Réveillon",
                    fr: "Saint-Sylvestre",
                    de: "Silvester"),
                type: HolidayType.National,
                description: "New Year's Eve — celebrated nationwide as the eve of both the new year and the anniversary of the Revolution's triumph."),
        };
    }
}
