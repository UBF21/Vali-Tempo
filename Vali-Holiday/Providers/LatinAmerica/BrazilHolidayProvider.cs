using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.LatinAmerica;

/// <summary>
/// Holiday provider for <b>Brazil</b> (Brasil) — country code <c>BR</c>.
/// </summary>
/// <remarks>
/// <para>
/// Brazil observes nine fixed national holidays established by federal law, plus five
/// movable feasts tied to the date of Easter Sunday calculated using the Gaussian algorithm
/// for the Gregorian calendar.
/// </para>
/// <para>
/// Notable fixed holidays include <em>Tiradentes</em> (21 April), which commemorates
/// Joaquim José da Silva Xavier, martyr of the 1792 independence movement; the
/// <em>Independência do Brasil</em> (7 September 1822, Dom Pedro I); and the
/// <em>Dia da Consciência Negra</em> (20 November), instituted by Law 12.519/2011 in
/// honour of Zumbi dos Palmares, leader of the quilombo resistance.
/// </para>
/// <para>
/// Movable holidays are Carnival Monday and Tuesday (48 and 47 days before Easter),
/// Good Friday (<em>Sexta-Feira Santa</em>), Corpus Christi (60 days after Easter),
/// and Easter Sunday itself.
/// </para>
/// </remarks>
public sealed class BrazilHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "BR";

    /// <inheritdoc/>
    public override string CountryName => "Brazil";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "pt";

    /// <inheritdoc/>
    /// <returns>
    /// Nine fixed national holidays recognised across all Brazilian states and the Federal District.
    /// </returns>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays()
    {
        const string cc = "BR";

        return new[]
        {
            // 1 January — Confraternização Universal / New Year's Day
            new HolidayInfo(
                id: "br_new_year",
                month: 1, day: 1,
                countryCode: cc,
                name: "Confraternização Universal",
                names: Names(
                    es: "Confraternización Universal",
                    en: "New Year's Day",
                    pt: "Confraternização Universal",
                    fr: "Jour de l'An",
                    de: "Neujahrstag"),
                type: HolidayType.National,
                description: "Celebrates the beginning of the new year and universal brotherhood."),

            // 21 April — Tiradentes
            new HolidayInfo(
                id: "br_tiradentes",
                month: 4, day: 21,
                countryCode: cc,
                name: "Tiradentes",
                names: Names(
                    es: "Tiradentes",
                    en: "Tiradentes Day",
                    pt: "Tiradentes",
                    fr: "Jour de Tiradentes",
                    de: "Tiradentes-Tag"),
                type: HolidayType.Civic,
                description: "Commemorates Joaquim José da Silva Xavier, 'Tiradentes', martyr of the 1792 Inconfidência Mineira independence movement, executed on this date."),

            // 1 May — Dia do Trabalhador / Labour Day
            new HolidayInfo(
                id: "br_labour_day",
                month: 5, day: 1,
                countryCode: cc,
                name: "Dia do Trabalhador",
                names: Names(
                    es: "Día del Trabajo",
                    en: "Labour Day",
                    pt: "Dia do Trabalhador",
                    fr: "Fête du Travail",
                    de: "Tag der Arbeit"),
                type: HolidayType.National,
                description: "International Workers' Day, honouring the labour movement worldwide."),

            // 7 September — Independência do Brasil
            new HolidayInfo(
                id: "br_independence",
                month: 9, day: 7,
                countryCode: cc,
                name: "Independência do Brasil",
                names: Names(
                    es: "Independencia de Brasil",
                    en: "Brazil Independence Day",
                    pt: "Independência do Brasil",
                    fr: "Indépendance du Brésil",
                    de: "Unabhängigkeitstag Brasiliens"),
                type: HolidayType.Civic,
                description: "Marks the declaration of Brazilian independence from Portugal by Dom Pedro I on 7 September 1822."),

            // 12 October — Nossa Senhora Aparecida
            new HolidayInfo(
                id: "br_nossa_senhora_aparecida",
                month: 10, day: 12,
                countryCode: cc,
                name: "Nossa Senhora Aparecida",
                names: Names(
                    es: "Nuestra Señora Aparecida",
                    en: "Our Lady of Aparecida",
                    pt: "Nossa Senhora Aparecida",
                    fr: "Notre-Dame Aparecida",
                    de: "Unsere Liebe Frau von Aparecida"),
                type: HolidayType.Religious,
                description: "Feast day of Our Lady of Aparecida, Patroness of Brazil, whose image was found in the Paraíba River in 1717."),

            // 2 November — Finados
            new HolidayInfo(
                id: "br_finados",
                month: 11, day: 2,
                countryCode: cc,
                name: "Finados",
                names: Names(
                    es: "Día de los Difuntos",
                    en: "All Souls' Day",
                    pt: "Finados",
                    fr: "Jour des Morts",
                    de: "Allerseelen"),
                type: HolidayType.Religious,
                description: "All Souls' Day, dedicated to prayer and remembrance of the deceased."),

            // 15 November — Proclamação da República
            new HolidayInfo(
                id: "br_republic_day",
                month: 11, day: 15,
                countryCode: cc,
                name: "Proclamação da República",
                names: Names(
                    es: "Proclamación de la República",
                    en: "Republic Day",
                    pt: "Proclamação da República",
                    fr: "Proclamation de la République",
                    de: "Tag der Republik"),
                type: HolidayType.Civic,
                description: "Commemorates the proclamation of the Brazilian Republic on 15 November 1889, replacing the Empire."),

            // 20 November — Dia da Consciência Negra
            new HolidayInfo(
                id: "br_black_consciousness",
                month: 11, day: 20,
                countryCode: cc,
                name: "Dia da Consciência Negra",
                names: Names(
                    es: "Día de la Conciencia Negra",
                    en: "Black Consciousness Day",
                    pt: "Dia da Consciência Negra",
                    fr: "Journée de la Conscience Noire",
                    de: "Tag des Schwarzen Bewusstseins"),
                type: HolidayType.Civic,
                description: "Instituted by Law 12.519/2011, this date honours Zumbi dos Palmares (died 20 November 1695), symbol of resistance against slavery in Brazil."),

            // 25 December — Natal / Christmas
            new HolidayInfo(
                id: "br_christmas",
                month: 12, day: 25,
                countryCode: cc,
                name: "Natal",
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
    /// Five movable holidays: Carnival Monday, Carnival Tuesday, Good Friday,
    /// Corpus Christi, and Easter Sunday.
    /// </returns>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        const string cc = "BR";
        var easter          = EasterCalculator.Easter(year);
        var carnavalMonday  = EasterCalculator.CarnavalMonday(year);
        var carnavalTuesday = EasterCalculator.CarnavalTuesday(year);
        var goodFriday      = EasterCalculator.GoodFriday(year);
        var corpusChristi   = EasterCalculator.CorpusChristi(year);

        return new[]
        {
            // Carnival Monday — Easter - 48 days
            new HolidayInfo(
                id: "br_carnival_monday",
                month: carnavalMonday.Month,
                day: carnavalMonday.Day,
                countryCode: cc,
                name: "Segunda-feira de Carnaval",
                names: Names(
                    es: "Lunes de Carnaval",
                    en: "Carnival Monday",
                    pt: "Segunda-feira de Carnaval",
                    fr: "Lundi de Carnaval",
                    de: "Karnevalsmontag"),
                type: HolidayType.National,
                isMovable: true,
                description: "First day of Carnival, 48 days before Easter Sunday."),

            // Carnival Tuesday — Easter - 47 days
            new HolidayInfo(
                id: "br_carnival_tuesday",
                month: carnavalTuesday.Month,
                day: carnavalTuesday.Day,
                countryCode: cc,
                name: "Terça-feira de Carnaval",
                names: Names(
                    es: "Martes de Carnaval",
                    en: "Carnival Tuesday",
                    pt: "Terça-feira de Carnaval",
                    fr: "Mardi de Carnaval",
                    de: "Karnevalsdienstag"),
                type: HolidayType.National,
                isMovable: true,
                description: "Second day of Carnival, 47 days before Easter Sunday. Known as Mardi Gras."),

            // Good Friday — Easter - 2 days
            new HolidayInfo(
                id: "br_good_friday",
                month: goodFriday.Month,
                day: goodFriday.Day,
                countryCode: cc,
                name: "Sexta-Feira Santa",
                names: Names(
                    es: "Viernes Santo",
                    en: "Good Friday",
                    pt: "Sexta-Feira Santa",
                    fr: "Vendredi Saint",
                    de: "Karfreitag"),
                type: HolidayType.Religious,
                isMovable: true,
                description: "Commemorates the crucifixion of Jesus Christ, observed 2 days before Easter Sunday."),

            // Corpus Christi — Easter + 60 days
            new HolidayInfo(
                id: "br_corpus_christi",
                month: corpusChristi.Month,
                day: corpusChristi.Day,
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
                description: "Solemnity celebrating the Real Presence of Jesus Christ in the Eucharist, 60 days after Easter."),

            // Easter Sunday
            new HolidayInfo(
                id: "br_easter",
                month: easter.Month,
                day: easter.Day,
                countryCode: cc,
                name: "Páscoa",
                names: Names(
                    es: "Pascua",
                    en: "Easter Sunday",
                    pt: "Páscoa",
                    fr: "Pâques",
                    de: "Ostersonntag"),
                type: HolidayType.Religious,
                isMovable: true,
                description: "Celebrates the resurrection of Jesus Christ. Date computed using the Gaussian algorithm for the Gregorian calendar."),
        };
    }
}
