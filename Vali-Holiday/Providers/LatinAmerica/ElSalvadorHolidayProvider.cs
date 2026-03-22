using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.LatinAmerica;

/// <summary>
/// Holiday provider for El Salvador, based on the official calendar of non-working
/// public holidays established by Salvadoran legislation.
/// Covers both fixed-date national holidays and Easter-based movable observances (Semana Santa).
/// </summary>
public class ElSalvadorHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "SV";

    /// <inheritdoc/>
    public override string CountryName => "El Salvador";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "es";

    /// <inheritdoc/>
    /// <remarks>
    /// Returns all fixed Salvadoran national holidays. Dates are established by law and do not
    /// vary by year. Includes the two principal days of the Fiestas Agostinas en honor al
    /// Divino Salvador del Mundo, Patrono de El Salvador (5 and 6 de agosto).
    /// </remarks>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays() => new[]
    {
        new HolidayInfo(
            "sv_new_year", 1, 1, "SV",
            "Año Nuevo",
            Names("Año Nuevo", "New Year's Day", "Ano Novo", "Jour de l'An", "Neujahr"),
            HolidayType.National,
            description: "Celebración del inicio del año calendario."),

        new HolidayInfo(
            "sv_labor_day", 5, 1, "SV",
            "Día del Trabajo",
            Names("Día del Trabajo", "Labour Day", "Dia do Trabalho", "Fête du Travail", "Tag der Arbeit"),
            HolidayType.National,
            description: "Conmemoración internacional de los derechos de los trabajadores."),

        new HolidayInfo(
            "sv_divine_savior_eve", 8, 5, "SV",
            "Festividades en honor al Divino Salvador del Mundo",
            Names(
                "Festividades en honor al Divino Salvador del Mundo",
                "Festivities in Honor of the Divine Savior of the World",
                "Festividades em honra ao Divino Salvador do Mundo",
                "Fêtes en l'honneur du Divin Sauveur du Monde",
                "Festlichkeiten zu Ehren des Göttlichen Retters der Welt"),
            HolidayType.Religious,
            description: "Primer día de las Fiestas Agostinas en honor al Divino Salvador del Mundo, Patrono de El Salvador. Las celebraciones abarcan del 1 al 6 de agosto con actividades culturales y religiosas."),

        new HolidayInfo(
            "sv_divine_savior", 8, 6, "SV",
            "Fiesta del Divino Salvador del Mundo",
            Names(
                "Fiesta del Divino Salvador del Mundo",
                "Feast of the Divine Savior of the World",
                "Festa do Divino Salvador do Mundo",
                "Fête du Divin Sauveur du Monde",
                "Fest des Göttlichen Retters der Welt"),
            HolidayType.Religious,
            description: "Día principal de las Fiestas Agostinas: celebración del Patrono de El Salvador, el Divino Salvador del Mundo. Coincide con la Transfiguración del Señor."),

        new HolidayInfo(
            "sv_independence", 9, 15, "SV",
            "Día de la Independencia",
            Names("Día de la Independencia", "Independence Day", "Dia da Independência", "Fête de l'Indépendance", "Unabhängigkeitstag"),
            HolidayType.Civic,
            description: "El Salvador declaró su independencia de España el 15 de septiembre de 1821, junto con las demás naciones de Centroamérica."),

        new HolidayInfo(
            "sv_all_souls", 11, 2, "SV",
            "Día de los Difuntos",
            Names("Día de los Difuntos", "All Souls' Day", "Dia dos Finados", "Jour des Défunts", "Allerseelen"),
            HolidayType.Religious,
            description: "Conmemoración católica de los fieles difuntos; los salvadoreños visitan los cementerios para recordar a sus seres queridos."),

        new HolidayInfo(
            "sv_christmas", 12, 25, "SV",
            "Navidad",
            Names("Navidad", "Christmas Day", "Natal", "Noël", "Weihnachten"),
            HolidayType.Religious,
            description: "Celebración del nacimiento de Jesucristo.")
    };

    /// <inheritdoc/>
    /// <remarks>
    /// Returns the three Easter-based movable holidays of Semana Santa observed in El Salvador:
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
                "sv_holy_thursday", holyThursday.Month, holyThursday.Day, "SV",
                "Jueves Santo",
                Names("Jueves Santo", "Holy Thursday", "Quinta-Feira Santa", "Jeudi Saint", "Gründonnerstag"),
                HolidayType.Religious, isMovable: true,
                description: "Conmemoración de la Última Cena de Jesucristo; inicio de los días más solemnes de Semana Santa."),

            new HolidayInfo(
                "sv_good_friday", goodFriday.Month, goodFriday.Day, "SV",
                "Viernes Santo",
                Names("Viernes Santo", "Good Friday", "Sexta-Feira Santa", "Vendredi Saint", "Karfreitag"),
                HolidayType.Religious, isMovable: true,
                description: "Conmemoración de la crucifixión y muerte de Jesucristo."),

            new HolidayInfo(
                "sv_holy_saturday", holySaturday.Month, holySaturday.Day, "SV",
                "Sábado de Gloria",
                Names("Sábado de Gloria", "Holy Saturday", "Sábado de Aleluia", "Samedi Saint", "Karsamstag"),
                HolidayType.Religious, isMovable: true,
                description: "Día de recogimiento entre la muerte y la resurrección de Jesucristo; último día de los ritos de Semana Santa.")
        };
    }
}
