using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.Europe;

/// <summary>
/// Holiday provider for Portugal, covering the 13 national public holidays (feriados
/// nacionais obrigatórios) established by the Lei n.º 7/2009 (Código do Trabalho)
/// and subsequent amendments, including the holidays restored in 2016 (Lei n.º 8/2016)
/// that had been suspended between 2013 and 2015.
/// </summary>
public class PortugalHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "PT";

    /// <inheritdoc/>
    public override string CountryName => "Portugal";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "pt";

    /// <inheritdoc/>
    /// <remarks>
    /// Returns all 10 fixed national public holidays. The 3 movable Easter-based holidays
    /// (Good Friday, Easter Sunday, and Corpus Christi) are returned by
    /// <see cref="GetMovableHolidays"/>.
    /// </remarks>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays() => new[]
    {
        new HolidayInfo(
            "pt_new_year", 1, 1, "PT",
            "Ano Novo",
            Names("Año Nuevo", "New Year's Day", "Ano Novo", "Jour de l'An", "Neujahrstag"),
            HolidayType.National,
            description: "Celebração do primeiro dia do ano civil / Celebration of the start of the calendar year."),

        new HolidayInfo(
            "pt_freedom_day", 4, 25, "PT",
            "Dia da Liberdade",
            Names("Día de la Libertad / Revolución de los Claveles", "Freedom Day / Carnation Revolution",
                  "Dia da Liberdade / Revolução dos Cravos", "Jour de la Liberté / Révolution des Oeillets",
                  "Tag der Freiheit / Nelkenrevolution"),
            HolidayType.Civic,
            description: "Aniversário da Revolução dos Cravos de 25 de abril de 1974, golpe de estado militar que derrubou o regime do Estado Novo (ditadura salazarista) e iniciou o processo de democratização de Portugal."),

        new HolidayInfo(
            "pt_labor_day", 5, 1, "PT",
            "Dia do Trabalhador",
            Names("Día del Trabajo", "Labour Day", "Dia do Trabalhador", "Fête du Travail", "Tag der Arbeit"),
            HolidayType.National,
            description: "Dia internacional dos trabalhadores / International Workers' Day."),

        new HolidayInfo(
            "pt_portugal_day", 6, 10, "PT",
            "Dia de Portugal, de Camões e das Comunidades Portuguesas",
            Names("Día de Portugal, de Camões y de las Comunidades Portuguesas",
                  "Portugal Day / Day of Camões and the Portuguese Communities",
                  "Dia de Portugal, de Camões e das Comunidades Portuguesas",
                  "Jour du Portugal, de Camões et des Communautés Portugaises",
                  "Tag Portugals, von Camões und der Portugiesischen Gemeinschaften"),
            HolidayType.Civic,
            description: "Aniversário da morte de Luís de Camões em 10 de junho de 1580, considerado o maior poeta da língua portuguesa e autor de 'Os Lusíadas'. Dia também de homenagem às comunidades portuguesas espalhadas pelo mundo."),

        new HolidayInfo(
            "pt_assumption", 8, 15, "PT",
            "Assunção de Nossa Senhora",
            Names("Asunción de Nuestra Señora", "Assumption of Our Lady",
                  "Assunção de Nossa Senhora", "Assomption de Notre-Dame", "Mariä Himmelfahrt"),
            HolidayType.Religious,
            description: "Solenidade católica da Assunção de Maria ao Céu / Feast of the Assumption of Mary."),

        new HolidayInfo(
            "pt_republic_day", 10, 5, "PT",
            "Implantação da República",
            Names("Implantación de la República Portuguesa", "Portuguese Republic Day",
                  "Implantação da República", "Proclamation de la République Portugaise",
                  "Tag der Republik Portugal"),
            HolidayType.Civic,
            description: "Aniversário da proclamação da República Portuguesa em 5 de outubro de 1910, que pôs fim à monarquia constitucional e deu início à Primeira República."),

        new HolidayInfo(
            "pt_all_saints", 11, 1, "PT",
            "Dia de Todos-os-Santos",
            Names("Todos los Santos", "All Saints' Day",
                  "Dia de Todos-os-Santos", "Toussaint", "Allerheiligen"),
            HolidayType.Religious,
            description: "Festividade católica em honra de todos os santos / Feast of all the saints of the Catholic Church."),

        new HolidayInfo(
            "pt_restoration", 12, 1, "PT",
            "Restauração da Independência",
            Names("Restauración de la Independencia de Portugal",
                  "Restoration of Independence",
                  "Restauração da Independência",
                  "Restauration de l'Indépendance du Portugal",
                  "Wiederherstellung der Unabhängigkeit Portugals"),
            HolidayType.Civic,
            description: "Aniversário da Restauração da Independência de Portugal em 1 de dezembro de 1640, com a aclamação do Duque de Bragança como rei D. João IV, pondo fim à União Ibérica e ao domínio espanhol iniciado em 1580."),

        new HolidayInfo(
            "pt_immaculate", 12, 8, "PT",
            "Imaculada Conceição",
            Names("Inmaculada Concepción", "Immaculate Conception",
                  "Imaculada Conceição", "Immaculée Conception", "Mariä Empfängnis"),
            HolidayType.Religious,
            description: "Dogma católico proclamado pelo Papa Pio IX em 1854, que afirma que a Virgem Maria foi concebida sem pecado original. Padroeira de Portugal."),

        new HolidayInfo(
            "pt_christmas", 12, 25, "PT",
            "Natal",
            Names("Navidad", "Christmas Day", "Natal", "Noël", "Weihnachten"),
            HolidayType.National,
            description: "Celebração do nascimento de Jesus Cristo / Celebration of the birth of Jesus Christ."),
    };

    /// <inheritdoc/>
    /// <remarks>
    /// Returns the 3 Easter-based movable national holidays: Sexta-Feira Santa (Good Friday),
    /// Páscoa (Easter Sunday), and Corpo de Deus (Corpus Christi, Easter+60).
    /// Dates are computed via <see cref="EasterCalculator"/>.
    /// </remarks>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        var goodFriday   = EasterCalculator.GoodFriday(year);
        var easter       = EasterCalculator.Easter(year);
        var corpusChristi = EasterCalculator.CorpusChristi(year);

        return new[]
        {
            new HolidayInfo(
                "pt_good_friday", goodFriday.Month, goodFriday.Day, "PT",
                "Sexta-Feira Santa",
                Names("Viernes Santo", "Good Friday",
                      "Sexta-Feira Santa", "Vendredi Saint", "Karfreitag"),
                HolidayType.Religious, isMovable: true,
                description: "Comemoração da crucificação e morte de Jesus Cristo, dois dias antes do Domingo de Páscoa."),

            new HolidayInfo(
                "pt_easter", easter.Month, easter.Day, "PT",
                "Páscoa",
                Names("Pascua", "Easter Sunday",
                      "Páscoa", "Pâques", "Ostersonntag"),
                HolidayType.Religious, isMovable: true,
                description: "Celebração da ressurreição de Jesus Cristo. Data calculada pelo algoritmo gaussiano para o calendário gregoriano."),

            new HolidayInfo(
                "pt_corpus_christi", corpusChristi.Month, corpusChristi.Day, "PT",
                "Corpo de Deus",
                Names("Corpus Christi", "Corpus Christi",
                      "Corpo de Deus", "Fête-Dieu", "Fronleichnam"),
                HolidayType.Religious, isMovable: true,
                description: "Solenidade do Santíssimo Corpo e Sangue de Cristo, celebrada 60 dias após o Domingo de Páscoa."),
        };
    }
}
