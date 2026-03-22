using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.LatinAmerica;

/// <summary>
/// Holiday provider for Peru, based on the current Decreto Supremo that
/// regulates official non-working public holidays (feriados nacionales).
/// Covers both fixed-date and Easter-based movable holidays.
/// </summary>
public class PeruHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "PE";

    /// <inheritdoc/>
    public override string CountryName => "Peru";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "es";

    /// <inheritdoc/>
    /// <remarks>
    /// Returns all fixed Peruvian national holidays. The dates are defined by
    /// Decreto Supremo and do not vary by year.
    /// </remarks>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays() => new[]
    {
        new HolidayInfo(
            "pe_new_year", 1, 1, "PE",
            "Año Nuevo",
            Names("Año Nuevo", "New Year's Day", "Ano Novo", "Jour de l'An", "Neujahr"),
            HolidayType.National,
            description: "Celebración del inicio del año calendario."),

        new HolidayInfo(
            "pe_labor_day", 5, 1, "PE",
            "Día del Trabajo",
            Names("Día del Trabajo", "Labour Day", "Dia do Trabalho", "Fête du Travail", "Tag der Arbeit"),
            HolidayType.National,
            description: "Conmemoración internacional de los derechos de los trabajadores."),

        new HolidayInfo(
            "pe_san_pedro_pablo", 6, 29, "PE",
            "San Pedro y San Pablo",
            Names("San Pedro y San Pablo", "Saints Peter and Paul", "São Pedro e São Paulo", "Saint Pierre et Paul", "Peter und Paul"),
            HolidayType.Religious,
            description: "Festividad católica en honor a los apóstoles Pedro y Pablo."),

        new HolidayInfo(
            "pe_independence_1", 7, 28, "PE",
            "Día de la Independencia Nacional",
            Names("Día de la Independencia Nacional", "Independence Day", "Dia da Independência Nacional", "Fête de l'Indépendance", "Unabhängigkeitstag"),
            HolidayType.Civic,
            description: "Proclamación de la independencia del Perú el 28 de julio de 1821 por José de San Martín."),

        new HolidayInfo(
            "pe_independence_2", 7, 29, "PE",
            "Día de la Gran Parada Militar",
            Names("Día de la Gran Parada Militar", "Day of the Armed Forces", "Dia das Forças Armadas", "Jour des Forces Armées", "Tag der Streitkräfte"),
            HolidayType.Civic,
            description: "Desfile militar en honor a las Fuerzas Armadas del Perú."),

        new HolidayInfo(
            "pe_battle_junin", 8, 6, "PE",
            "Batalla de Junín",
            Names("Batalla de Junín", "Battle of Junín", "Batalha de Junín", "Bataille de Junín", "Schlacht von Junín"),
            HolidayType.Civic,
            description: "Conmemoración de la victoria patriota del 6 de agosto de 1824, decisiva para la independencia sudamericana."),

        new HolidayInfo(
            "pe_santa_rosa", 8, 30, "PE",
            "Santa Rosa de Lima",
            Names("Santa Rosa de Lima", "Saint Rose of Lima", "Santa Rosa de Lima", "Sainte Rose de Lima", "Heilige Rosa von Lima"),
            HolidayType.Religious,
            description: "Patrona de Lima, del Perú, de las Américas y de Filipinas. Primera santa nacida en América."),

        new HolidayInfo(
            "pe_angamos", 10, 8, "PE",
            "Combate de Angamos",
            Names("Combate de Angamos", "Battle of Angamos", "Combate de Angamos", "Combat d'Angamos", "Seeschlacht von Angamos"),
            HolidayType.Civic,
            description: "Combate naval del 8 de octubre de 1879 durante la Guerra del Pacífico; el monitor Huáscar fue capturado."),

        new HolidayInfo(
            "pe_all_saints", 11, 1, "PE",
            "Día de Todos los Santos",
            Names("Día de Todos los Santos", "All Saints' Day", "Dia de Todos os Santos", "Toussaint", "Allerheiligen"),
            HolidayType.Religious,
            description: "Festividad católica en honor a todos los santos."),

        new HolidayInfo(
            "pe_immaculate", 12, 8, "PE",
            "Inmaculada Concepción",
            Names("Inmaculada Concepción", "Immaculate Conception", "Imaculada Conceição", "Immaculée Conception", "Mariä Empfängnis"),
            HolidayType.Religious,
            description: "Dogma católico que proclama que la Virgen María fue concebida sin pecado original."),

        new HolidayInfo(
            "pe_ayacucho", 12, 9, "PE",
            "Batalla de Ayacucho",
            Names("Batalla de Ayacucho", "Battle of Ayacucho", "Batalha de Ayacucho", "Bataille d'Ayacucho", "Schlacht von Ayacucho"),
            HolidayType.Civic,
            description: "Batalla del 9 de diciembre de 1824 que consolidó la independencia de América del Sur."),

        new HolidayInfo(
            "pe_christmas", 12, 25, "PE",
            "Navidad del Señor",
            Names("Navidad del Señor", "Christmas Day", "Natal do Senhor", "Noël", "Weihnachten"),
            HolidayType.Religious,
            description: "Celebración del nacimiento de Jesucristo.")
    };

    /// <inheritdoc/>
    /// <remarks>
    /// Returns the two Easter-based movable holidays observed in Peru:
    /// Holy Thursday and Good Friday. Dates are computed via <see cref="EasterCalculator"/>.
    /// </remarks>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        var holyThursday = EasterCalculator.HolyThursday(year);
        var goodFriday = EasterCalculator.GoodFriday(year);

        return new[]
        {
            new HolidayInfo(
                "pe_holy_thursday", holyThursday.Month, holyThursday.Day, "PE",
                "Jueves Santo",
                Names("Jueves Santo", "Holy Thursday", "Quinta-Feira Santa", "Jeudi Saint", "Gründonnerstag"),
                HolidayType.Religious, isMovable: true,
                description: "Conmemoración de la Última Cena de Jesucristo."),

            new HolidayInfo(
                "pe_good_friday", goodFriday.Month, goodFriday.Day, "PE",
                "Viernes Santo",
                Names("Viernes Santo", "Good Friday", "Sexta-Feira Santa", "Vendredi Saint", "Karfreitag"),
                HolidayType.Religious, isMovable: true,
                description: "Conmemoración de la crucifixión y muerte de Jesucristo.")
        };
    }
}
