using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.LatinAmerica;

/// <summary>
/// Holiday provider for Nicaragua, based on the official calendar of non-working public
/// holidays established by Nicaraguan legislation.
/// Covers both fixed-date national holidays and Easter-based movable observances (Semana Santa).
/// </summary>
public class NicaraguaHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "NI";

    /// <inheritdoc/>
    public override string CountryName => "Nicaragua";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "es";

    /// <inheritdoc/>
    /// <remarks>
    /// Returns all fixed Nicaraguan national holidays. Dates are established by law and do not
    /// vary by year. Includes civic, religious and patriotic observances unique to Nicaragua,
    /// such as the Día de la Revolución Sandinista and La Purísima.
    /// </remarks>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays() => new[]
    {
        new HolidayInfo(
            "ni_new_year", 1, 1, "NI",
            "Año Nuevo",
            Names("Año Nuevo", "New Year's Day", "Ano Novo", "Jour de l'An", "Neujahr"),
            HolidayType.National,
            description: "Celebración del inicio del año calendario."),

        new HolidayInfo(
            "ni_labor_day", 5, 1, "NI",
            "Día del Trabajo",
            Names("Día del Trabajo", "Labour Day", "Dia do Trabalho", "Fête du Travail", "Tag der Arbeit"),
            HolidayType.National,
            description: "Conmemoración internacional de los derechos de los trabajadores."),

        new HolidayInfo(
            "ni_mothers_day", 5, 30, "NI",
            "Día de las Madres",
            Names("Día de las Madres", "Mother's Day", "Dia das Mães", "Fête des Mères", "Muttertag"),
            HolidayType.National,
            description: "Celebración en honor a las madres nicaragüenses. Nicaragua conmemora esta festividad el 30 de mayo."),

        new HolidayInfo(
            "ni_sandinista_revolution", 7, 19, "NI",
            "Día de la Revolución Popular Sandinista",
            Names(
                "Día de la Revolución Popular Sandinista",
                "Day of the Sandinista Revolution",
                "Dia da Revolução Popular Sandinista",
                "Jour de la Révolution Sandiniste",
                "Tag der Sandinistischen Revolution"),
            HolidayType.Civic,
            description: "Conmemoración del triunfo de la Revolución Sandinista el 19 de julio de 1979, cuando el Frente Sandinista de Liberación Nacional (FSLN) derrocó la dictadura de Anastasio Somoza Debayle."),

        new HolidayInfo(
            "ni_san_jacinto", 9, 14, "NI",
            "Batalla de San Jacinto",
            Names("Batalla de San Jacinto", "Battle of San Jacinto", "Batalha de San Jacinto", "Bataille de San Jacinto", "Schlacht von San Jacinto"),
            HolidayType.Civic,
            description: "Conmemoración de la victoria nicaragüense en la Batalla de San Jacinto el 14 de septiembre de 1856, donde las fuerzas nacionales derrotaron a los filibusteros del mercenario estadounidense William Walker."),

        new HolidayInfo(
            "ni_independence", 9, 15, "NI",
            "Día de la Independencia",
            Names("Día de la Independencia", "Independence Day", "Dia da Independência", "Fête de l'Indépendance", "Unabhängigkeitstag"),
            HolidayType.Civic,
            description: "Nicaragua declaró su independencia de España el 15 de septiembre de 1821, junto con las demás naciones de Centroamérica."),

        new HolidayInfo(
            "ni_all_souls", 11, 2, "NI",
            "Día de los Difuntos",
            Names("Día de los Difuntos", "All Souls' Day", "Dia dos Finados", "Jour des Défunts", "Allerseelen"),
            HolidayType.Religious,
            description: "Conmemoración católica de los fieles difuntos; los nicaragüenses visitan los cementerios para honrar la memoria de sus seres queridos."),

        new HolidayInfo(
            "ni_immaculate_conception", 12, 8, "NI",
            "Inmaculada Concepción de María",
            Names(
                "Inmaculada Concepción de María",
                "Immaculate Conception",
                "Imaculada Conceição de Maria",
                "Immaculée Conception de Marie",
                "Mariä Empfängnis"),
            HolidayType.Religious,
            description: "La Purísima es la festividad más importante de Nicaragua. El 7 de diciembre se celebra 'La Gritería', donde los nicaragüenses gritan '¿Quién causa tanta alegría? ¡La Concepción de María!' al frente de altares dedicados a la Virgen María."),

        new HolidayInfo(
            "ni_christmas", 12, 25, "NI",
            "Navidad",
            Names("Navidad", "Christmas Day", "Natal", "Noël", "Weihnachten"),
            HolidayType.Religious,
            description: "Celebración del nacimiento de Jesucristo.")
    };

    /// <inheritdoc/>
    /// <remarks>
    /// Returns the two Easter-based movable holidays of Semana Santa observed in Nicaragua:
    /// Jueves Santo (Holy Thursday) and Viernes Santo (Good Friday).
    /// Dates are computed via <see cref="EasterCalculator"/>.
    /// </remarks>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        var holyThursday = EasterCalculator.HolyThursday(year);
        var goodFriday   = EasterCalculator.GoodFriday(year);

        return new[]
        {
            new HolidayInfo(
                "ni_holy_thursday", holyThursday.Month, holyThursday.Day, "NI",
                "Jueves Santo",
                Names("Jueves Santo", "Holy Thursday", "Quinta-Feira Santa", "Jeudi Saint", "Gründonnerstag"),
                HolidayType.Religious, isMovable: true,
                description: "Conmemoración de la Última Cena de Jesucristo; inicio de los días más solemnes de la Semana Santa."),

            new HolidayInfo(
                "ni_good_friday", goodFriday.Month, goodFriday.Day, "NI",
                "Viernes Santo",
                Names("Viernes Santo", "Good Friday", "Sexta-Feira Santa", "Vendredi Saint", "Karfreitag"),
                HolidayType.Religious, isMovable: true,
                description: "Conmemoración de la crucifixión y muerte de Jesucristo.")
        };
    }
}
