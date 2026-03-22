using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.LatinAmerica;

/// <summary>
/// Holiday provider for Colombia, based on Ley 51 de 1983 (known as the "Ley Emiliani")
/// and its subsequent amendments. Under this law, most religious and civic holidays
/// that do not fall on a Monday are transferred to the following Monday.
/// </summary>
/// <remarks>
/// Holidays marked with "traslado lunes" in their description are subject to the
/// Monday transfer rule established by the Ley Emiliani. This library stores the
/// canonical calendar dates; callers are responsible for applying the transfer rule
/// for a specific year when exact observance dates are required.
/// Easter-based movable holidays are computed via <see cref="EasterCalculator"/>.
/// </remarks>
public class ColombiaHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "CO";

    /// <inheritdoc/>
    public override string CountryName => "Colombia";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "es";

    /// <inheritdoc/>
    /// <remarks>
    /// Returns all fixed Colombian national holidays. Entries that are subject to
    /// Monday transfer under the Ley Emiliani are noted in the <c>Description</c> field.
    /// </remarks>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays() => new[]
    {
        new HolidayInfo(
            "co_new_year", 1, 1, "CO",
            "Año Nuevo",
            Names("Año Nuevo", "New Year's Day", "Ano Novo", "Jour de l'An", "Neujahr"),
            HolidayType.National,
            description: "Celebración del inicio del año calendario. Fecha fija, sin traslado."),

        new HolidayInfo(
            "co_epiphany", 1, 6, "CO",
            "Día de Reyes",
            Names("Día de Reyes", "Epiphany", "Dia de Reis", "Épiphanie", "Heilige Drei Könige"),
            HolidayType.Religious,
            description: "Conmemoración de la adoración de los Reyes Magos al niño Jesús. Sujeto a traslado al lunes siguiente según la Ley Emiliani."),

        new HolidayInfo(
            "co_san_jose", 3, 19, "CO",
            "San José",
            Names("San José", "Saint Joseph's Day", "São José", "Saint Joseph", "Heiliger Josef"),
            HolidayType.Religious,
            description: "Festividad en honor a San José, esposo de la Virgen María y padre adoptivo de Jesucristo. Sujeto a traslado al lunes siguiente según la Ley Emiliani."),

        new HolidayInfo(
            "co_labor_day", 5, 1, "CO",
            "Día del Trabajo",
            Names("Día del Trabajo", "Labour Day", "Dia do Trabalho", "Fête du Travail", "Tag der Arbeit"),
            HolidayType.National,
            description: "Conmemoración internacional de los derechos de los trabajadores. Fecha fija, sin traslado."),

        new HolidayInfo(
            "co_san_pedro_pablo", 6, 29, "CO",
            "San Pedro y San Pablo",
            Names("San Pedro y San Pablo", "Saints Peter and Paul", "São Pedro e São Paulo", "Saint Pierre et Paul", "Peter und Paul"),
            HolidayType.Religious,
            description: "Festividad católica en honor a los apóstoles Pedro y Pablo. Sujeto a traslado al lunes siguiente según la Ley Emiliani."),

        new HolidayInfo(
            "co_independence", 7, 20, "CO",
            "Día de la Independencia",
            Names("Día de la Independencia", "Independence Day", "Dia da Independência", "Fête de l'Indépendance", "Unabhängigkeitstag"),
            HolidayType.Civic,
            description: "Conmemoración de la Declaración de Independencia de Colombia del 20 de julio de 1810 (Grito de Independencia). Fecha fija, sin traslado."),

        new HolidayInfo(
            "co_battle_boyaca", 8, 7, "CO",
            "Batalla de Boyacá",
            Names("Batalla de Boyacá", "Battle of Boyacá", "Batalha de Boyacá", "Bataille de Boyacá", "Schlacht von Boyacá"),
            HolidayType.Civic,
            description: "Conmemoración de la victoria patriota del 7 de agosto de 1819 que selló la independencia de la Nueva Granada (actual Colombia). Fecha fija, sin traslado."),

        new HolidayInfo(
            "co_assumption", 8, 15, "CO",
            "Asunción de la Virgen",
            Names("Asunción de la Virgen", "Assumption of Mary", "Assunção de Nossa Senhora", "Assomption de la Vierge", "Mariä Himmelfahrt"),
            HolidayType.Religious,
            description: "Dogma católico que celebra la elevación de la Virgen María al cielo. Sujeto a traslado al lunes siguiente según la Ley Emiliani."),

        new HolidayInfo(
            "co_columbus_day", 10, 12, "CO",
            "Día de la Raza",
            Names("Día de la Raza", "Columbus Day", "Dia da Raça", "Jour de Christophe Colomb", "Kolumbustag"),
            HolidayType.Civic,
            description: "Conmemoración del 12 de octubre de 1492, fecha del primer avistamiento de tierra americana por Cristóbal Colón. Sujeto a traslado al lunes siguiente según la Ley Emiliani."),

        new HolidayInfo(
            "co_all_saints", 11, 1, "CO",
            "Día de Todos los Santos",
            Names("Día de Todos los Santos", "All Saints' Day", "Dia de Todos os Santos", "Toussaint", "Allerheiligen"),
            HolidayType.Religious,
            description: "Festividad católica en honor a todos los santos. Sujeto a traslado al lunes siguiente según la Ley Emiliani."),

        new HolidayInfo(
            "co_cartagena", 11, 11, "CO",
            "Independencia de Cartagena",
            Names("Independencia de Cartagena", "Cartagena Independence Day", "Independência de Cartagena", "Indépendance de Carthagène", "Unabhängigkeit von Cartagena"),
            HolidayType.Civic,
            description: "Conmemoración de la declaración de independencia de la Provincia de Cartagena de Indias el 11 de noviembre de 1811. Sujeto a traslado al lunes siguiente según la Ley Emiliani."),

        new HolidayInfo(
            "co_immaculate", 12, 8, "CO",
            "Inmaculada Concepción",
            Names("Inmaculada Concepción", "Immaculate Conception", "Imaculada Conceição", "Immaculée Conception", "Mariä Empfängnis"),
            HolidayType.Religious,
            description: "Dogma católico que proclama que la Virgen María fue concebida sin pecado original. Fecha fija, sin traslado."),

        new HolidayInfo(
            "co_christmas", 12, 25, "CO",
            "Navidad",
            Names("Navidad", "Christmas Day", "Natal", "Noël", "Weihnachten"),
            HolidayType.Religious,
            description: "Celebración del nacimiento de Jesucristo. Fecha fija, sin traslado.")
    };

    /// <inheritdoc/>
    /// <remarks>
    /// Returns Easter-based movable holidays for Colombia: Holy Thursday, Good Friday,
    /// Ascension of the Lord, Corpus Christi, and the Sacred Heart of Jesus.
    /// All are computed via <see cref="EasterCalculator"/>. Ascension, Corpus Christi,
    /// and Sacred Heart are subject to Monday transfer under the Ley Emiliani.
    /// </remarks>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        var holyThursday = EasterCalculator.HolyThursday(year);
        var goodFriday = EasterCalculator.GoodFriday(year);
        var ascension = EasterCalculator.Ascension(year);
        var corpusChristi = EasterCalculator.CorpusChristi(year);
        var sacredHeart = EasterCalculator.SacredHeart(year);

        return new[]
        {
            new HolidayInfo(
                "co_holy_thursday", holyThursday.Month, holyThursday.Day, "CO",
                "Jueves Santo",
                Names("Jueves Santo", "Holy Thursday", "Quinta-Feira Santa", "Jeudi Saint", "Gründonnerstag"),
                HolidayType.Religious, isMovable: true,
                description: "Conmemoración de la Última Cena de Jesucristo. Fecha fija relativa a Pascua, sin traslado."),

            new HolidayInfo(
                "co_good_friday", goodFriday.Month, goodFriday.Day, "CO",
                "Viernes Santo",
                Names("Viernes Santo", "Good Friday", "Sexta-Feira Santa", "Vendredi Saint", "Karfreitag"),
                HolidayType.Religious, isMovable: true,
                description: "Conmemoración de la crucifixión y muerte de Jesucristo. Fecha fija relativa a Pascua, sin traslado."),

            new HolidayInfo(
                "co_ascension", ascension.Month, ascension.Day, "CO",
                "Ascensión del Señor",
                Names("Ascensión del Señor", "Ascension of the Lord", "Ascensão do Senhor", "Ascension du Seigneur", "Christi Himmelfahrt"),
                HolidayType.Religious, isMovable: true,
                description: "Celebración de la ascensión de Jesucristo al cielo, 39 días después de Pascua. Sujeto a traslado al lunes siguiente según la Ley Emiliani."),

            new HolidayInfo(
                "co_corpus_christi", corpusChristi.Month, corpusChristi.Day, "CO",
                "Corpus Christi",
                Names("Corpus Christi", "Corpus Christi", "Corpus Christi", "Fête-Dieu", "Fronleichnam"),
                HolidayType.Religious, isMovable: true,
                description: "Festividad católica en honor al Cuerpo y Sangre de Jesucristo en la Eucaristía, 60 días después de Pascua. Sujeto a traslado al lunes siguiente según la Ley Emiliani."),

            new HolidayInfo(
                "co_sacred_heart", sacredHeart.Month, sacredHeart.Day, "CO",
                "Sagrado Corazón de Jesús",
                Names("Sagrado Corazón de Jesús", "Sacred Heart of Jesus", "Sagrado Coração de Jesus", "Sacré-Cœur de Jésus", "Heiligstes Herz Jesu"),
                HolidayType.Religious, isMovable: true,
                description: "Festividad en honor al Sagrado Corazón de Jesús, 68 días después de Pascua. Sujeto a traslado al lunes siguiente según la Ley Emiliani.")
        };
    }
}
