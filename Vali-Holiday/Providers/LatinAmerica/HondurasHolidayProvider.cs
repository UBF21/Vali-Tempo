using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.LatinAmerica;

/// <summary>
/// Holiday provider for Honduras, based on the official calendar of non-working public
/// holidays established by Honduran legislation.
/// Covers both fixed-date national holidays and Easter-based movable observances (Semana Santa).
/// </summary>
public class HondurasHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "HN";

    /// <inheritdoc/>
    public override string CountryName => "Honduras";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "es";

    /// <inheritdoc/>
    /// <remarks>
    /// Returns all fixed Honduran national holidays. Dates are established by law and do not
    /// vary by year, with the exception of the Natalicio de Francisco Morazán, which is observed
    /// on the third Monday of October; it is listed here on its fixed calendar date (October 3)
    /// as a reference anchor — actual observance may shift to the nearest Monday.
    /// </remarks>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays() => new[]
    {
        new HolidayInfo(
            "hn_new_year", 1, 1, "HN",
            "Año Nuevo",
            Names("Año Nuevo", "New Year's Day", "Ano Novo", "Jour de l'An", "Neujahr"),
            HolidayType.National,
            description: "Celebración del inicio del año calendario."),

        new HolidayInfo(
            "hn_americas_day", 4, 14, "HN",
            "Día de las Américas",
            Names("Día de las Américas", "Day of the Americas", "Dia das Américas", "Jour des Amériques", "Tag der Amerikas"),
            HolidayType.Civic,
            description: "Conmemoración de la creación de la Unión Panamericana el 14 de abril de 1890, hoy conocida como la Organización de los Estados Americanos (OEA)."),

        new HolidayInfo(
            "hn_labor_day", 5, 1, "HN",
            "Día del Trabajo",
            Names("Día del Trabajo", "Labour Day", "Dia do Trabalho", "Fête du Travail", "Tag der Arbeit"),
            HolidayType.National,
            description: "Conmemoración internacional de los derechos de los trabajadores."),

        new HolidayInfo(
            "hn_independence", 9, 15, "HN",
            "Día de la Independencia",
            Names("Día de la Independencia", "Independence Day", "Dia da Independência", "Fête de l'Indépendance", "Unabhängigkeitstag"),
            HolidayType.Civic,
            description: "Honduras declaró su independencia de España el 15 de septiembre de 1821, junto con las demás naciones de Centroamérica."),

        new HolidayInfo(
            "hn_morazan_birthday", 10, 3, "HN",
            "Natalicio de Francisco Morazán",
            Names("Natalicio de Francisco Morazán", "Francisco Morazán's Birthday", "Aniversário de Francisco Morazán", "Anniversaire de Francisco Morazán", "Geburtstag von Francisco Morazán"),
            HolidayType.Civic,
            description: "Conmemoración del nacimiento del prócer centroamericano Francisco Morazán, el 3 de octubre de 1792. Se observa el tercer lunes de octubre; la fecha del 3 de octubre corresponde a su natalicio histórico."),

        new HolidayInfo(
            "hn_columbus_day", 10, 12, "HN",
            "Día del Descubrimiento de América",
            Names("Día del Descubrimiento de América", "Columbus Day", "Dia do Descobrimento da América", "Jour de la Découverte de l'Amérique", "Kolumbustag"),
            HolidayType.Civic,
            description: "Conmemoración del arribo de Cristóbal Colón a América el 12 de octubre de 1492, también conocido como Día de la Raza."),

        new HolidayInfo(
            "hn_armed_forces_day", 10, 21, "HN",
            "Día de las Fuerzas Armadas",
            Names("Día de las Fuerzas Armadas", "Armed Forces Day", "Dia das Forças Armadas", "Jour des Forces Armées", "Tag der Streitkräfte"),
            HolidayType.Civic,
            description: "Celebración institucional en honor a las Fuerzas Armadas de Honduras."),

        new HolidayInfo(
            "hn_christmas", 12, 25, "HN",
            "Navidad",
            Names("Navidad", "Christmas Day", "Natal", "Noël", "Weihnachten"),
            HolidayType.Religious,
            description: "Celebración del nacimiento de Jesucristo.")
    };

    /// <inheritdoc/>
    /// <remarks>
    /// Returns the three Easter-based movable holidays of Semana Santa observed in Honduras:
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
                "hn_holy_thursday", holyThursday.Month, holyThursday.Day, "HN",
                "Jueves Santo",
                Names("Jueves Santo", "Holy Thursday", "Quinta-Feira Santa", "Jeudi Saint", "Gründonnerstag"),
                HolidayType.Religious, isMovable: true,
                description: "Conmemoración de la Última Cena de Jesucristo; inicio de los días más solemnes de la Semana Santa."),

            new HolidayInfo(
                "hn_good_friday", goodFriday.Month, goodFriday.Day, "HN",
                "Viernes Santo",
                Names("Viernes Santo", "Good Friday", "Sexta-Feira Santa", "Vendredi Saint", "Karfreitag"),
                HolidayType.Religious, isMovable: true,
                description: "Conmemoración de la crucifixión y muerte de Jesucristo."),

            new HolidayInfo(
                "hn_holy_saturday", holySaturday.Month, holySaturday.Day, "HN",
                "Sábado de Gloria",
                Names("Sábado de Gloria", "Holy Saturday", "Sábado de Aleluia", "Samedi Saint", "Karsamstag"),
                HolidayType.Religious, isMovable: true,
                description: "Día de vigilia entre la muerte y la resurrección de Jesucristo; último día de duelo de la Semana Santa.")
        };
    }
}
