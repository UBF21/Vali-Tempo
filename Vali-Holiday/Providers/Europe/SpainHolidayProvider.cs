using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.Europe;

/// <summary>
/// Holiday provider for Spain, covering the 9 national public holidays (festivos nacionales)
/// established by the Estatuto de los Trabajadores (Real Decreto 2001/1983 and subsequent
/// updates), plus regional holidays (festivos autonómicos) for all 17 Comunidades Autónomas.
/// </summary>
public class SpainHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "ES";

    /// <inheritdoc/>
    public override string CountryName => "Spain";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "es";

    /// <inheritdoc/>
    /// <remarks>
    /// Returns all fixed national holidays and fixed regional holidays for each
    /// Comunidad Autónoma. Movable national holidays (Good Friday) are returned by
    /// <see cref="GetMovableHolidays"/>.
    /// </remarks>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays() => new[]
    {
        // ── National fixed holidays ──────────────────────────────────────────────

        new HolidayInfo(
            "es_new_year", 1, 1, "ES",
            "Año Nuevo",
            Names("Año Nuevo", "New Year's Day", "Ano Novo", "Jour de l'An", "Neujahrstag"),
            HolidayType.National,
            description: "Celebración del inicio del año calendario."),

        new HolidayInfo(
            "es_epiphany", 1, 6, "ES",
            "Epifanía del Señor",
            Names("Epifanía del Señor / Día de Reyes", "Epiphany / Three Kings' Day",
                  "Epifania / Dia de Reis", "Épiphanie / Jour des Rois", "Heilige Drei Könige"),
            HolidayType.Religious,
            description: "Conmemoración de la visita de los Reyes Magos al niño Jesús. Día de intercambio de regalos en España."),

        new HolidayInfo(
            "es_labor_day", 5, 1, "ES",
            "Día del Trabajo",
            Names("Día del Trabajo / Fiesta del Trabajo", "Labour Day / Workers' Day",
                  "Dia do Trabalho", "Fête du Travail", "Tag der Arbeit"),
            HolidayType.National,
            description: "Conmemoración internacional de los derechos de los trabajadores."),

        new HolidayInfo(
            "es_assumption", 8, 15, "ES",
            "Asunción de la Virgen",
            Names("Asunción de la Virgen", "Assumption of Mary",
                  "Assunção de Nossa Senhora", "Assomption de Marie", "Mariä Himmelfahrt"),
            HolidayType.Religious,
            description: "Dogma católico que proclama la elevación corporal de la Virgen María a los cielos al final de su vida terrena."),

        new HolidayInfo(
            "es_hispanidad", 10, 12, "ES",
            "Fiesta Nacional de España",
            Names("Fiesta Nacional de España / Día de la Hispanidad",
                  "Spain National Day / Columbus Day",
                  "Dia da Hispanidade", "Fête Nationale d'Espagne", "Spanischer Nationalfeiertag"),
            HolidayType.Civic,
            description: "Conmemoración de la llegada de Cristóbal Colón a América el 12 de octubre de 1492. Es también el Día de la Guardia Civil y del Ejército."),

        new HolidayInfo(
            "es_all_saints", 11, 1, "ES",
            "Todos los Santos",
            Names("Todos los Santos", "All Saints' Day",
                  "Dia de Todos os Santos", "Toussaint", "Allerheiligen"),
            HolidayType.Religious,
            description: "Festividad católica en honor a todos los santos canonizados."),

        new HolidayInfo(
            "es_constitution", 12, 6, "ES",
            "Día de la Constitución Española",
            Names("Día de la Constitución Española", "Spanish Constitution Day",
                  "Dia da Constituição Espanhola", "Jour de la Constitution espagnole", "Tag der Spanischen Verfassung"),
            HolidayType.Civic,
            description: "Conmemoración de la aprobación por referéndum de la Constitución Española de 1978, que instauró la democracia parlamentaria."),

        new HolidayInfo(
            "es_immaculate", 12, 8, "ES",
            "Inmaculada Concepción",
            Names("Inmaculada Concepción", "Immaculate Conception",
                  "Imaculada Conceição", "Immaculée Conception", "Mariä Empfängnis"),
            HolidayType.Religious,
            description: "Dogma católico proclamado por el Papa Pío IX en 1854, según el cual la Virgen María fue concebida sin pecado original."),

        new HolidayInfo(
            "es_christmas", 12, 25, "ES",
            "Navidad",
            Names("Navidad", "Christmas Day", "Natal", "Noël", "Weihnachten"),
            HolidayType.Religious,
            description: "Celebración del nacimiento de Jesucristo."),

        // ── Regional: Cataluña (ES-CT) ───────────────────────────────────────────

        new HolidayInfo(
            "es_ct_diada", 9, 11, "ES",
            "Diada de Catalunya",
            Names("Diada de Catalunya", "Catalonia National Day",
                  "Dia Nacional da Catalunha", "Fête nationale de Catalogne", "Kataloniens Nationalfeiertag"),
            HolidayType.Regional, regionCode: "ES-CT",
            description: "Conmemoración de la caída de Barcelona el 11 de septiembre de 1714 durante la Guerra de Sucesión Española."),

        new HolidayInfo(
            "es_ct_sant_joan", 6, 24, "ES",
            "Sant Joan",
            Names("Sant Joan / San Juan", "Saint John's Day",
                  "São João", "Saint Jean", "Johannistag"),
            HolidayType.Regional, regionCode: "ES-CT",
            description: "Festividad de San Juan Bautista, celebrada con verbenas, fuegos artificiales y la Nit de Sant Joan."),

        // ── Regional: Madrid (ES-MD) ─────────────────────────────────────────────

        new HolidayInfo(
            "es_md_dos_mayo", 5, 2, "ES",
            "Fiesta de la Comunidad de Madrid",
            Names("Fiesta de la Comunidad de Madrid / Dos de Mayo",
                  "Madrid Community Day / Second of May",
                  "Dia da Comunidade de Madrid", "Fête de la Communauté de Madrid", "Fest der Gemeinschaft Madrid"),
            HolidayType.Regional, regionCode: "ES-MD",
            description: "Conmemoración del levantamiento popular madrileño del 2 de mayo de 1808 contra las tropas napoleónicas."),

        new HolidayInfo(
            "es_md_santiago", 7, 25, "ES",
            "Santiago Apóstol",
            Names("Santiago Apóstol", "Saint James the Apostle",
                  "Santiago Apóstolo", "Saint Jacques Apôtre", "Sankt Jakobus der Apostel"),
            HolidayType.Regional, regionCode: "ES-MD",
            description: "Festividad del patrón de España, Santiago Apóstol."),

        // ── Regional: Andalucía (ES-AN) ──────────────────────────────────────────

        new HolidayInfo(
            "es_an_andalucia", 2, 28, "ES",
            "Día de Andalucía",
            Names("Día de Andalucía", "Andalusia Day",
                  "Dia da Andaluzia", "Jour de l'Andalousie", "Tag Andalusiens"),
            HolidayType.Regional, regionCode: "ES-AN",
            description: "Conmemoración del referéndum de iniciativa autonómica del 28 de febrero de 1980, que aprobó el proceso autonómico andaluz."),

        // ── Regional: País Vasco (ES-PV) ─────────────────────────────────────────

        new HolidayInfo(
            "es_pv_euskadi", 10, 25, "ES",
            "Euskadi Eguna",
            Names("Día del País Vasco / Euskadi Eguna", "Basque Country Day",
                  "Dia do País Basco", "Jour du Pays Basque", "Tag des Baskenlandes"),
            HolidayType.Regional, regionCode: "ES-PV",
            description: "Festividad del Estatuto de Guernica, aprobado el 25 de octubre de 1979, que reconoció la autonomía del País Vasco."),

        // ── Regional: Valencia (ES-VC) ───────────────────────────────────────────

        new HolidayInfo(
            "es_vc_sant_josep", 3, 19, "ES",
            "Sant Josep",
            Names("Sant Josep / San José (La Cremà de las Fallas)", "Saint Joseph's Day / Las Fallas",
                  "São José / Fallas", "Saint Joseph / Las Fallas", "Sankt Josef / Las Fallas"),
            HolidayType.Regional, regionCode: "ES-VC",
            description: "Festividad de San José, patrón de los carpinteros, que coincide con la cremà o quema de las Fallas de Valencia, declaradas Patrimonio Inmaterial de la Humanidad."),

        // ── Regional: Galicia (ES-GA) ────────────────────────────────────────────

        new HolidayInfo(
            "es_ga_galicia", 7, 25, "ES",
            "Día de Galicia",
            Names("Día de Galicia / Santiago Apóstol", "Galicia Day / Saint James",
                  "Dia da Galiza", "Jour de la Galice", "Tag Galiciens"),
            HolidayType.Regional, regionCode: "ES-GA",
            description: "Día Nacional de Galicia, que coincide con la festividad del apóstol Santiago, patrón de España."),

        // ── Regional: Aragón (ES-AR) ─────────────────────────────────────────────

        new HolidayInfo(
            "es_ar_aragon", 4, 23, "ES",
            "Día de Aragón",
            Names("Día de Aragón / San Jorge", "Aragon Day / Saint George",
                  "Dia de Aragão", "Jour d'Aragon", "Tag Aragons"),
            HolidayType.Regional, regionCode: "ES-AR",
            description: "Día de la Comunidad de Aragón, festividad de San Jorge, patrón de Aragón, cuya onomástica se celebra el 23 de abril."),

        // ── Regional: Castilla y León (ES-CL) ────────────────────────────────────

        new HolidayInfo(
            "es_cl_castilla_leon", 4, 23, "ES",
            "Día de Castilla y León",
            Names("Día de Castilla y León", "Castile and León Day",
                  "Dia de Castela e Leão", "Jour de Castille-et-León", "Tag von Kastilien und León"),
            HolidayType.Regional, regionCode: "ES-CL",
            description: "Conmemoración de la Batalla de Villalar del 23 de abril de 1521, en la que los comuneros castellanos fueron derrotados por el ejército de Carlos I."),

        // ── Regional: Canarias (ES-CN) ────────────────────────────────────────────

        new HolidayInfo(
            "es_cn_canarias", 5, 30, "ES",
            "Día de Canarias",
            Names("Día de Canarias", "Canary Islands Day",
                  "Dia das Canárias", "Jour des Îles Canaries", "Tag der Kanarischen Inseln"),
            HolidayType.Regional, regionCode: "ES-CN",
            description: "Conmemoración de la primera sesión del Parlamento de Canarias, celebrada el 30 de mayo de 1983."),

        // ── Regional: Baleares (ES-IB) ────────────────────────────────────────────

        new HolidayInfo(
            "es_ib_baleares", 3, 1, "ES",
            "Dia de les Illes Balears",
            Names("Día de las Islas Baleares / Dia de les Illes Balears", "Balearic Islands Day",
                  "Dia das Ilhas Baleares", "Jour des Îles Baléares", "Tag der Balearischen Inseln"),
            HolidayType.Regional, regionCode: "ES-IB",
            description: "Conmemoración de la aprobación del Estatuto de Autonomía de las Islas Baleares el 1 de marzo de 1983."),

        // ── Regional: Extremadura (ES-EX) ─────────────────────────────────────────

        new HolidayInfo(
            "es_ex_extremadura", 9, 8, "ES",
            "Día de Extremadura",
            Names("Día de Extremadura / Virgen de Guadalupe", "Extremadura Day / Our Lady of Guadalupe",
                  "Dia da Estremadura", "Jour de l'Estrémadure", "Tag Extremaduras"),
            HolidayType.Regional, regionCode: "ES-EX",
            description: "Festividad de la Virgen de Guadalupe, patrona de Extremadura, declarada Reina de la Hispanidad."),

        // ── Regional: Murcia (ES-MC) ──────────────────────────────────────────────

        new HolidayInfo(
            "es_mc_murcia", 6, 9, "ES",
            "Día de la Región de Murcia",
            Names("Día de la Región de Murcia", "Region of Murcia Day",
                  "Dia da Região de Múrcia", "Jour de la Région de Murcie", "Tag der Region Murcia"),
            HolidayType.Regional, regionCode: "ES-MC",
            description: "Conmemoración del 9 de junio de 1983, fecha de la primera sesión de la Asamblea Regional de Murcia."),

        // ── Regional: La Rioja (ES-RI) ────────────────────────────────────────────

        new HolidayInfo(
            "es_ri_la_rioja", 6, 9, "ES",
            "Día de La Rioja",
            Names("Día de La Rioja", "La Rioja Day",
                  "Dia de La Rioja", "Jour de La Rioja", "Tag von La Rioja"),
            HolidayType.Regional, regionCode: "ES-RI",
            description: "Conmemoración del 9 de junio de 1982, fecha en que se aprobó el Estatuto de Autonomía de La Rioja."),

        // ── Regional: Cantabria (ES-CB) ───────────────────────────────────────────

        new HolidayInfo(
            "es_cb_cantabria", 7, 28, "ES",
            "Día de las Instituciones de Cantabria",
            Names("Día de las Instituciones de Cantabria", "Cantabria Institutions Day",
                  "Dia das Instituições da Cantábria", "Jour des Institutions de Cantabrie", "Tag der Institutionen Kantabriens"),
            HolidayType.Regional, regionCode: "ES-CB",
            description: "Conmemoración del 28 de julio de 1778, fecha de la creación de la Junta General de Cantabria."),

        // ── Regional: Asturias (ES-AS) ────────────────────────────────────────────

        new HolidayInfo(
            "es_as_asturias", 9, 8, "ES",
            "Día de Asturias",
            Names("Día de Asturias / Virgen de Covadonga", "Asturias Day / Our Lady of Covadonga",
                  "Dia das Astúrias", "Jour des Asturies", "Tag Asturiens"),
            HolidayType.Regional, regionCode: "ES-AS",
            description: "Festividad de la Virgen de Covadonga, patrona de Asturias, asociada a la batalla de Covadonga (718 d.C.) que inició la Reconquista."),

        // ── Regional: Navarra (ES-NC) ─────────────────────────────────────────────

        new HolidayInfo(
            "es_nc_navarra", 12, 3, "ES",
            "Día de Navarra",
            Names("Día de Navarra", "Navarre Day",
                  "Dia da Navarra", "Jour de Navarre", "Tag Navarras"),
            HolidayType.Regional, regionCode: "ES-NC",
            description: "Conmemoración del 3 de diciembre de 1841, fecha de la aprobación de la Ley Paccionada que reguló la incorporación de Navarra al Estado español."),
    };

    /// <inheritdoc/>
    /// <remarks>
    /// Returns the single Easter-based movable national holiday: Good Friday (Viernes Santo).
    /// Date is computed via <see cref="EasterCalculator"/>.
    /// </remarks>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        var goodFriday = EasterCalculator.GoodFriday(year);

        return new[]
        {
            new HolidayInfo(
                "es_good_friday", goodFriday.Month, goodFriday.Day, "ES",
                "Viernes Santo",
                Names("Viernes Santo", "Good Friday",
                      "Sexta-Feira Santa", "Vendredi Saint", "Karfreitag"),
                HolidayType.Religious, isMovable: true,
                description: "Conmemoración de la crucifixión y muerte de Jesucristo. Festivo nacional en España.")
        };
    }
}
