using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.LatinAmerica;

/// <summary>
/// Holiday provider for Guatemala, based on the official calendar of non-working
/// public holidays established by Guatemalan legislation.
/// Covers both fixed-date national holidays and Easter-based movable observances (Semana Santa).
/// </summary>
public class GuatemalaHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "GT";

    /// <inheritdoc/>
    public override string CountryName => "Guatemala";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "es";

    /// <inheritdoc/>
    /// <remarks>
    /// Returns all fixed Guatemalan national holidays. Dates are defined by law and do not
    /// vary by year. Optional holidays (Nochebuena and Fin de Año) are included with
    /// <see cref="HolidayType.Optional"/>.
    /// </remarks>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays() => new[]
    {
        new HolidayInfo(
            "gt_new_year", 1, 1, "GT",
            "Año Nuevo",
            Names("Año Nuevo", "New Year's Day", "Ano Novo", "Jour de l'An", "Neujahr"),
            HolidayType.National,
            description: "Celebración del inicio del año calendario."),

        new HolidayInfo(
            "gt_labor_day", 5, 1, "GT",
            "Día del Trabajo",
            Names("Día del Trabajo", "Labour Day", "Dia do Trabalho", "Fête du Travail", "Tag der Arbeit"),
            HolidayType.National,
            description: "Conmemoración internacional de los derechos de los trabajadores."),

        new HolidayInfo(
            "gt_army_day", 6, 30, "GT",
            "Día del Ejército",
            Names("Día del Ejército", "Army Day", "Dia do Exército", "Jour de l'Armée", "Tag der Armee"),
            HolidayType.Civic,
            description: "Conmemoración de la Revolución Liberal de 1871, cuando el ejército liberal tomó el poder. Actualmente celebrado como Día del Ejército de Guatemala."),

        new HolidayInfo(
            "gt_assumption", 8, 15, "GT",
            "Día de la Asunción",
            Names("Día de la Asunción", "Assumption Day", "Dia da Assunção", "Assomption", "Mariä Himmelfahrt"),
            HolidayType.Religious,
            description: "Celebración en honor a la Virgen de la Asunción, Patrona de la Ciudad de Guatemala."),

        new HolidayInfo(
            "gt_independence", 9, 15, "GT",
            "Día de la Independencia",
            Names("Día de la Independencia", "Independence Day", "Dia da Independência", "Fête de l'Indépendance", "Unabhängigkeitstag"),
            HolidayType.Civic,
            description: "Guatemala declaró su independencia de España el 15 de septiembre de 1821, junto con otras naciones centroamericanas."),

        new HolidayInfo(
            "gt_revolution_day", 10, 20, "GT",
            "Día de la Revolución",
            Names("Día de la Revolución", "Revolution Day", "Dia da Revolução", "Jour de la Révolution", "Tag der Revolution"),
            HolidayType.Civic,
            description: "Conmemoración de la Revolución de Octubre de 1944, movimiento democratizador que derrocó al general Ubico e impulsó la primavera democrática guatemalteca."),

        new HolidayInfo(
            "gt_all_saints", 11, 1, "GT",
            "Día de Todos los Santos",
            Names("Día de Todos los Santos", "All Saints' Day", "Dia de Todos os Santos", "Toussaint", "Allerheiligen"),
            HolidayType.Religious,
            description: "Festividad católica en honor a todos los santos."),

        new HolidayInfo(
            "gt_christmas_eve", 12, 24, "GT",
            "Nochebuena",
            Names("Nochebuena", "Christmas Eve", "Véspera de Natal", "Réveillon de Noël", "Heiligabend"),
            HolidayType.Optional,
            description: "Víspera de la Navidad, celebrada en familia la noche del 24 de diciembre."),

        new HolidayInfo(
            "gt_christmas", 12, 25, "GT",
            "Navidad",
            Names("Navidad", "Christmas Day", "Natal", "Noël", "Weihnachten"),
            HolidayType.Religious,
            description: "Celebración del nacimiento de Jesucristo."),

        new HolidayInfo(
            "gt_new_years_eve", 12, 31, "GT",
            "Fin de Año",
            Names("Fin de Año", "New Year's Eve", "Réveillon de Ano Novo", "Saint-Sylvestre", "Silvester"),
            HolidayType.Optional,
            description: "Víspera del Año Nuevo, celebración del último día del año calendario.")
    };

    /// <inheritdoc/>
    /// <remarks>
    /// Returns the three Easter-based movable holidays of Semana Santa observed in Guatemala:
    /// Jueves Santo (Holy Thursday), Viernes Santo (Good Friday) and Sábado de Gloria (Holy Saturday).
    /// Dates are computed via <see cref="EasterCalculator"/>.
    /// </remarks>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        var holyThursday = EasterCalculator.HolyThursday(year);
        var goodFriday   = EasterCalculator.GoodFriday(year);
        var holySaturday = EasterCalculator.HolySaturday(year);

        return new[]
        {
            new HolidayInfo(
                "gt_holy_thursday", holyThursday.Month, holyThursday.Day, "GT",
                "Jueves Santo",
                Names("Jueves Santo", "Holy Thursday", "Quinta-Feira Santa", "Jeudi Saint", "Gründonnerstag"),
                HolidayType.Religious, isMovable: true,
                description: "Conmemoración de la Última Cena de Jesucristo; inicio oficial de la Semana Santa en Guatemala."),

            new HolidayInfo(
                "gt_good_friday", goodFriday.Month, goodFriday.Day, "GT",
                "Viernes Santo",
                Names("Viernes Santo", "Good Friday", "Sexta-Feira Santa", "Vendredi Saint", "Karfreitag"),
                HolidayType.Religious, isMovable: true,
                description: "Conmemoración de la crucifixión y muerte de Jesucristo. Las procesiones de Semana Santa en Antigua Guatemala son Patrimonio Cultural Intangible."),

            new HolidayInfo(
                "gt_holy_saturday", holySaturday.Month, holySaturday.Day, "GT",
                "Sábado de Gloria",
                Names("Sábado de Gloria", "Holy Saturday", "Sábado de Aleluia", "Samedi Saint", "Karsamstag"),
                HolidayType.Religious, isMovable: true,
                description: "Día de recogimiento entre la muerte y la resurrección de Jesucristo; último día de la Semana Santa en Guatemala.")
        };
    }
}
