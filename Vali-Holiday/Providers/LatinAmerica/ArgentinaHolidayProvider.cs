using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.LatinAmerica;

/// <summary>
/// Holiday provider for Argentina, based on Ley 27.399 (Feriados Nacionales) and
/// supplementary decrees. Covers both mandatory fixed holidays and Easter-based
/// movable observances including Carnival.
/// </summary>
/// <remarks>
/// Some Argentine holidays are subject to "traslado": if the canonical date falls on
/// a Tuesday or Wednesday the holiday moves to the previous Monday; if it falls on
/// a Thursday or Friday it moves to the following Monday. This library stores the
/// canonical dates. Callers should apply the traslado rule per the annual decree
/// where exact observance dates are needed.
/// Note: the holiday for General San Martín (August 17) is observed on the third
/// Monday of August under the traslado rule, but is stored here as August 17 (the
/// historical date of his death in 1850).
/// </remarks>
public class ArgentinaHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "AR";

    /// <inheritdoc/>
    public override string CountryName => "Argentina";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "es";

    /// <inheritdoc/>
    /// <remarks>
    /// Returns all fixed Argentine national holidays as established by Ley 27.399
    /// and complementary decrees. Dates subject to Monday transfer are noted in
    /// each entry's <c>Description</c>.
    /// </remarks>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays() => new[]
    {
        new HolidayInfo(
            "ar_new_year", 1, 1, "AR",
            "Año Nuevo",
            Names("Año Nuevo", "New Year's Day", "Ano Novo", "Jour de l'An", "Neujahr"),
            HolidayType.National,
            description: "Celebración del inicio del año calendario."),

        new HolidayInfo(
            "ar_memory_day", 3, 24, "AR",
            "Día Nacional de la Memoria por la Verdad y la Justicia",
            Names("Día Nacional de la Memoria por la Verdad y la Justicia", "National Day of Remembrance for Truth and Justice", "Dia Nacional da Memória pela Verdade e Justiça", "Journée Nationale de la Mémoire pour la Vérité et la Justice", "Nationaler Tag des Gedenkens für Wahrheit und Gerechtigkeit"),
            HolidayType.Civic,
            description: "Conmemoración del golpe de Estado del 24 de marzo de 1976 que instauró la última dictadura cívico-militar en Argentina. Instituido por Ley 25.633."),

        new HolidayInfo(
            "ar_malvinas", 4, 2, "AR",
            "Día del Veterano y de los Caídos en la Guerra de Malvinas",
            Names("Día del Veterano y de los Caídos en la Guerra de Malvinas", "Falklands War Veterans and Fallen Day", "Dia do Veterano e dos Caídos na Guerra das Malvinas", "Jour des Vétérans et des Tombés à la Guerre des Malouines", "Tag der Veteranen und Gefallenen des Falklandkrieges"),
            HolidayType.Civic,
            description: "Homenaje a los veteranos y caídos argentinos en la Guerra de Malvinas de 1982. El 2 de abril de 1982 se produjo el desembarco argentino en las islas."),

        new HolidayInfo(
            "ar_labor_day", 5, 1, "AR",
            "Día del Trabajo",
            Names("Día del Trabajo", "Labour Day", "Dia do Trabalho", "Fête du Travail", "Tag der Arbeit"),
            HolidayType.National,
            description: "Conmemoración internacional de los derechos de los trabajadores."),

        new HolidayInfo(
            "ar_may_revolution", 5, 25, "AR",
            "Día de la Revolución de Mayo",
            Names("Día de la Revolución de Mayo", "May Revolution Day", "Dia da Revolução de Maio", "Jour de la Révolution de Mai", "Tag der Mairevolution"),
            HolidayType.Civic,
            description: "Conmemoración de la Revolución de Mayo del 25 de mayo de 1810, que constituyó el primer gobierno patrio argentino (Primera Junta) y el inicio del camino hacia la independencia."),

        new HolidayInfo(
            "ar_belgrano", 6, 20, "AR",
            "Paso a la Inmortalidad del General Manuel Belgrano",
            Names("Paso a la Inmortalidad del General Manuel Belgrano", "General Manuel Belgrano Memorial Day", "Dia da Imortalidade do General Manuel Belgrano", "Jour du Général Manuel Belgrano", "Gedenktag für General Manuel Belgrano"),
            HolidayType.Civic,
            description: "Conmemoración del fallecimiento del General Manuel Belgrano, creador de la Bandera Nacional Argentina, ocurrido el 20 de junio de 1820."),

        new HolidayInfo(
            "ar_independence", 7, 9, "AR",
            "Día de la Independencia",
            Names("Día de la Independencia", "Independence Day", "Dia da Independência", "Fête de l'Indépendance", "Unabhängigkeitstag"),
            HolidayType.Civic,
            description: "Conmemoración de la Declaración de la Independencia de las Provincias Unidas en Sud América el 9 de julio de 1816 en el Congreso de Tucumán."),

        new HolidayInfo(
            "ar_san_martin", 8, 17, "AR",
            "Paso a la Inmortalidad del General José de San Martín",
            Names("Paso a la Inmortalidad del General José de San Martín", "General José de San Martín Memorial Day", "Dia da Imortalidade do General José de San Martín", "Jour du Général José de San Martín", "Gedenktag für General José de San Martín"),
            HolidayType.Civic,
            description: "Conmemoración del fallecimiento del Libertador José de San Martín el 17 de agosto de 1850 en Boulogne-sur-Mer, Francia. Se observa el tercer lunes de agosto (regla de traslado)."),

        new HolidayInfo(
            "ar_diversity_day", 10, 12, "AR",
            "Día del Respeto a la Diversidad Cultural",
            Names("Día del Respeto a la Diversidad Cultural", "Day of Respect for Cultural Diversity", "Dia do Respeito à Diversidade Cultural", "Jour du Respect de la Diversité Culturelle", "Tag des Respekts für kulturelle Vielfalt"),
            HolidayType.Civic,
            description: "Reconoce la pluralidad cultural de Argentina y los derechos de los pueblos originarios. Reemplazó al 'Día de la Raza' por Decreto 1584/2010. Se traslada al lunes siguiente si no cae en lunes."),

        new HolidayInfo(
            "ar_sovereignty", 11, 20, "AR",
            "Día de la Soberanía Nacional",
            Names("Día de la Soberanía Nacional", "National Sovereignty Day", "Dia da Soberania Nacional", "Jour de la Souveraineté Nationale", "Tag der Nationalen Souveränität"),
            HolidayType.Civic,
            description: "Conmemoración de la Batalla de la Vuelta de Obligado del 20 de noviembre de 1845, donde fuerzas argentinas resistieron la intervención anglofrancesa. Se traslada al cuarto lunes de noviembre."),

        new HolidayInfo(
            "ar_immaculate", 12, 8, "AR",
            "Inmaculada Concepción de María",
            Names("Inmaculada Concepción de María", "Immaculate Conception", "Imaculada Conceição de Maria", "Immaculée Conception de Marie", "Mariä Empfängnis"),
            HolidayType.Religious,
            description: "Dogma católico que proclama que la Virgen María fue concebida sin pecado original."),

        new HolidayInfo(
            "ar_christmas", 12, 25, "AR",
            "Navidad",
            Names("Navidad", "Christmas Day", "Natal", "Noël", "Weihnachten"),
            HolidayType.Religious,
            description: "Celebración del nacimiento de Jesucristo.")
    };

    /// <inheritdoc/>
    /// <remarks>
    /// Returns Easter-based movable holidays for Argentina: Carnival Monday,
    /// Carnival Tuesday, Holy Thursday, Good Friday, and Easter Sunday itself.
    /// Dates are computed via <see cref="EasterCalculator"/>.
    /// </remarks>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        var carnavalMonday = EasterCalculator.CarnavalMonday(year);
        var carnavalTuesday = EasterCalculator.CarnavalTuesday(year);
        var holyThursday = EasterCalculator.HolyThursday(year);
        var goodFriday = EasterCalculator.GoodFriday(year);
        var easter = EasterCalculator.Easter(year);

        return new[]
        {
            new HolidayInfo(
                "ar_carnaval_monday", carnavalMonday.Month, carnavalMonday.Day, "AR",
                "Lunes de Carnaval",
                Names("Lunes de Carnaval", "Carnival Monday", "Segunda-Feira de Carnaval", "Lundi de Carnaval", "Rosenmontag"),
                HolidayType.National, isMovable: true,
                description: "Primer día de los festejos de Carnaval. Feriado nacional en Argentina, 48 días antes de Pascua."),

            new HolidayInfo(
                "ar_carnaval_tuesday", carnavalTuesday.Month, carnavalTuesday.Day, "AR",
                "Martes de Carnaval",
                Names("Martes de Carnaval", "Carnival Tuesday", "Terça-Feira de Carnaval", "Mardi Gras", "Faschingsdienstag"),
                HolidayType.National, isMovable: true,
                description: "Segundo día de los festejos de Carnaval. Feriado nacional en Argentina, 47 días antes de Pascua."),

            new HolidayInfo(
                "ar_holy_thursday", holyThursday.Month, holyThursday.Day, "AR",
                "Jueves Santo",
                Names("Jueves Santo", "Holy Thursday", "Quinta-Feira Santa", "Jeudi Saint", "Gründonnerstag"),
                HolidayType.Religious, isMovable: true,
                description: "Conmemoración de la Última Cena de Jesucristo."),

            new HolidayInfo(
                "ar_good_friday", goodFriday.Month, goodFriday.Day, "AR",
                "Viernes Santo",
                Names("Viernes Santo", "Good Friday", "Sexta-Feira Santa", "Vendredi Saint", "Karfreitag"),
                HolidayType.Religious, isMovable: true,
                description: "Conmemoración de la crucifixión y muerte de Jesucristo."),

            new HolidayInfo(
                "ar_easter", easter.Month, easter.Day, "AR",
                "Pascua",
                Names("Pascua", "Easter Sunday", "Páscoa", "Pâques", "Ostersonntag"),
                HolidayType.Religious, isMovable: true,
                description: "Celebración de la Resurrección de Jesucristo. Día de mayor importancia en el calendario litúrgico cristiano.")
        };
    }
}
