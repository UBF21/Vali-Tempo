using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.LatinAmerica;

/// <summary>
/// Holiday provider for Chile, based on Ley 19.668 (Ley de Feriados) and its
/// subsequent modifications. Covers fixed national holidays and Easter-based
/// movable observances.
/// </summary>
/// <remarks>
/// Notable rule: several holidays in Chile are transferred to the nearest Monday
/// when they fall on a weekend. This library stores the canonical calendar date;
/// callers should apply transfer rules according to each year's official decree
/// if exact observance dates are required.
/// </remarks>
public class ChileHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "CL";

    /// <inheritdoc/>
    public override string CountryName => "Chile";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "es";

    /// <inheritdoc/>
    /// <remarks>
    /// Returns all fixed Chilean national holidays as defined by Ley 19.668 and
    /// subsequent amendments. Dates that are subject to Monday transfer rules are
    /// noted in the <c>Description</c> field of each entry.
    /// </remarks>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays() => new[]
    {
        new HolidayInfo(
            "cl_new_year", 1, 1, "CL",
            "Año Nuevo",
            Names("Año Nuevo", "New Year's Day", "Ano Novo", "Jour de l'An", "Neujahr"),
            HolidayType.National,
            description: "Celebración del inicio del año calendario."),

        new HolidayInfo(
            "cl_labor_day", 5, 1, "CL",
            "Día Nacional del Trabajo",
            Names("Día Nacional del Trabajo", "National Labour Day", "Dia Nacional do Trabalho", "Fête Nationale du Travail", "Nationaler Tag der Arbeit"),
            HolidayType.National,
            description: "Conmemoración internacional de los derechos de los trabajadores."),

        new HolidayInfo(
            "cl_naval_glories", 5, 21, "CL",
            "Día de las Glorias Navales",
            Names("Día de las Glorias Navales", "Navy Day", "Dia das Glórias Navais", "Jour des Gloires Navales", "Tag der Marineglorie"),
            HolidayType.Civic,
            description: "Conmemoración del Combate Naval de Iquique del 21 de mayo de 1879, durante la Guerra del Pacífico. El capitán Arturo Prat murió heroicamente a bordo de la corbeta Esmeralda."),

        new HolidayInfo(
            "cl_indigenous_peoples", 6, 20, "CL",
            "Día Nacional de los Pueblos Indígenas",
            Names("Día Nacional de los Pueblos Indígenas", "National Day of Indigenous Peoples", "Dia Nacional dos Povos Indígenas", "Journée Nationale des Peuples Autochtones", "Nationaler Tag der Indigenen Völker"),
            HolidayType.National,
            description: "Celebra el solsticio de invierno del hemisferio sur (Inti Raymi y We Tripantu). La ley fija la fecha al solsticio astronómico, que generalmente cae el 20 o 21 de junio. Aquí se registra el 20 de junio como fecha referencial."),

        new HolidayInfo(
            "cl_san_pedro_pablo", 6, 29, "CL",
            "San Pedro y San Pablo",
            Names("San Pedro y San Pablo", "Saints Peter and Paul", "São Pedro e São Paulo", "Saint Pierre et Paul", "Peter und Paul"),
            HolidayType.Religious,
            description: "Festividad católica en honor a los apóstoles Pedro y Pablo. Se traslada al lunes siguiente si cae en martes a jueves."),

        new HolidayInfo(
            "cl_virgen_carmen", 7, 16, "CL",
            "Virgen del Carmen",
            Names("Virgen del Carmen", "Our Lady of Mount Carmel", "Nossa Senhora do Carmo", "Notre-Dame du Mont-Carmel", "Unsere Liebe Frau vom Berge Karmel"),
            HolidayType.Religious,
            description: "Festividad de la Patrona de Chile y de las Fuerzas Armadas. Instituida feriado nacional en honor a la victoria del Ejército Libertador en 1817."),

        new HolidayInfo(
            "cl_assumption", 8, 15, "CL",
            "Asunción de la Virgen",
            Names("Asunción de la Virgen", "Assumption of Mary", "Assunção de Nossa Senhora", "Assomption de la Vierge", "Mariä Himmelfahrt"),
            HolidayType.Religious,
            description: "Dogma católico que celebra la elevación de la Virgen María al cielo en cuerpo y alma al término de su vida terrenal."),

        new HolidayInfo(
            "cl_independence", 9, 18, "CL",
            "Día de la Independencia Nacional",
            Names("Día de la Independencia Nacional", "Independence Day", "Dia da Independência Nacional", "Fête de l'Indépendance", "Unabhängigkeitstag"),
            HolidayType.Civic,
            description: "Conmemoración de la Primera Junta Nacional de Gobierno del 18 de septiembre de 1810, inicio del proceso de independencia chileno. Celebradas como Fiestas Patrias."),

        new HolidayInfo(
            "cl_army_day", 9, 19, "CL",
            "Día de las Glorias del Ejército",
            Names("Día de las Glorias del Ejército", "Army Day", "Dia das Glórias do Exército", "Jour des Gloires de l'Armée", "Tag des Heeres"),
            HolidayType.Civic,
            description: "Conmemoración de la Batalla de Chacabuco (1817) y homenaje a las Fuerzas Armadas de Chile. Forma parte de las Fiestas Patrias junto al 18 de septiembre."),

        new HolidayInfo(
            "cl_columbus_day", 10, 12, "CL",
            "Día del Encuentro de Dos Mundos",
            Names("Día del Encuentro de Dos Mundos", "Day of the Encounter of Two Worlds", "Dia do Encontro de Dois Mundos", "Jour de la Rencontre de Deux Mondes", "Tag der Begegnung zweier Welten"),
            HolidayType.Civic,
            description: "Conmemoración del 12 de octubre de 1492, fecha del primer avistamiento de tierra americana por Cristóbal Colón. Chile adoptó el nombre 'Encuentro de Dos Mundos' para resaltar el diálogo intercultural. Se traslada al lunes siguiente si no cae en lunes."),

        new HolidayInfo(
            "cl_evangelical_day", 10, 31, "CL",
            "Día de las Iglesias Evangélicas y Protestantes",
            Names("Día de las Iglesias Evangélicas y Protestantes", "Reformation Day", "Dia das Igrejas Evangélicas e Protestantes", "Jour des Églises Évangéliques et Protestantes", "Tag der Evangelischen und Protestantischen Kirchen"),
            HolidayType.Religious,
            description: "Conmemoración del inicio de la Reforma Protestante (1517) y reconocimiento de las iglesias evangélicas y protestantes en Chile. Establecido por Ley 20.299."),

        new HolidayInfo(
            "cl_all_saints", 11, 1, "CL",
            "Día de Todos los Santos",
            Names("Día de Todos los Santos", "All Saints' Day", "Dia de Todos os Santos", "Toussaint", "Allerheiligen"),
            HolidayType.Religious,
            description: "Festividad católica en honor a todos los santos canonizados y no canonizados."),

        new HolidayInfo(
            "cl_immaculate", 12, 8, "CL",
            "Inmaculada Concepción",
            Names("Inmaculada Concepción", "Immaculate Conception", "Imaculada Conceição", "Immaculée Conception", "Mariä Empfängnis"),
            HolidayType.Religious,
            description: "Dogma católico que proclama que la Virgen María fue concebida sin pecado original."),

        new HolidayInfo(
            "cl_christmas", 12, 25, "CL",
            "Navidad",
            Names("Navidad", "Christmas Day", "Natal", "Noël", "Weihnachten"),
            HolidayType.Religious,
            description: "Celebración del nacimiento de Jesucristo."),

        new HolidayInfo(
            "cl_new_year_eve", 12, 31, "CL",
            "Último Día del Año",
            Names("Último Día del Año", "New Year's Eve", "Último Dia do Ano", "Réveillon du Nouvel An", "Silvester"),
            HolidayType.Optional,
            description: "Feriado bancario cuando cae en día hábil (lunes a viernes). No es feriado obligatorio para todos los sectores.")
    };

    /// <inheritdoc/>
    /// <remarks>
    /// Returns Easter-based movable holidays for Chile: Good Friday and Holy Saturday.
    /// Dates are computed via <see cref="EasterCalculator"/>.
    /// </remarks>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        var goodFriday = EasterCalculator.GoodFriday(year);
        var holySaturday = EasterCalculator.HolySaturday(year);

        return new[]
        {
            new HolidayInfo(
                "cl_good_friday", goodFriday.Month, goodFriday.Day, "CL",
                "Viernes Santo",
                Names("Viernes Santo", "Good Friday", "Sexta-Feira Santa", "Vendredi Saint", "Karfreitag"),
                HolidayType.Religious, isMovable: true,
                description: "Conmemoración de la crucifixión y muerte de Jesucristo."),

            new HolidayInfo(
                "cl_holy_saturday", holySaturday.Month, holySaturday.Day, "CL",
                "Sábado Santo",
                Names("Sábado Santo", "Holy Saturday", "Sábado Santo", "Samedi Saint", "Karsamstag"),
                HolidayType.Religious, isMovable: true,
                description: "Vigilia pascual entre la muerte y resurrección de Jesucristo.")
        };
    }
}
