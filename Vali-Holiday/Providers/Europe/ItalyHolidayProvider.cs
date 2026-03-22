using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Providers.Europe;

/// <summary>
/// Holiday provider for Italy, covering the 12 national public holidays (festività
/// nazionali) established by the Legge 27 maggio 1949, n. 260, as subsequently
/// amended, plus the Easter Monday movable holiday (Pasquetta) observed nationwide.
/// </summary>
public class ItalyHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "IT";

    /// <inheritdoc/>
    public override string CountryName => "Italy";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "it";

    /// <summary>
    /// Builds a multilingual name dictionary that includes Italian (<c>it</c>) as an
    /// additional language key alongside the standard five languages.
    /// </summary>
    /// <param name="it">Italian name.</param>
    /// <param name="es">Spanish name.</param>
    /// <param name="en">English name.</param>
    /// <param name="pt">Portuguese name (falls back to <paramref name="es"/> if empty).</param>
    /// <param name="fr">French name (falls back to <paramref name="en"/> if empty).</param>
    /// <param name="de">German name (falls back to <paramref name="en"/> if empty).</param>
    /// <returns>A read-only dictionary keyed by BCP 47 language tag.</returns>
    private static IReadOnlyDictionary<string, string> NamesIt(
        string it, string es, string en, string pt = "", string fr = "", string de = "")
    {
        var dict = new Dictionary<string, string>(Names(es, en, pt, fr, de))
        {
            ["it"] = it
        };
        return dict;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Returns all 10 fixed Italian national holidays. The single movable holiday
    /// (Pasquetta / Easter Monday) is returned by <see cref="GetMovableHolidays"/>.
    /// </remarks>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays() => new[]
    {
        new HolidayInfo(
            "it_new_year", 1, 1, "IT",
            "Capodanno",
            NamesIt("Capodanno", "Año Nuevo", "New Year's Day",
                    "Ano Novo", "Jour de l'An", "Neujahrstag"),
            HolidayType.National,
            description: "Primo giorno dell'anno civile / Celebration of the start of the calendar year."),

        new HolidayInfo(
            "it_epiphany", 1, 6, "IT",
            "Epifania",
            NamesIt("Epifania / La Befana", "Epifanía del Señor / Día de Reyes", "Epiphany / Three Kings' Day",
                    "Epifania / Dia de Reis", "Épiphanie / Jour des Rois", "Heilige Drei Könige"),
            HolidayType.Religious,
            description: "Festività dell'Epifania del Signore. In Italia tradizionalmente associata alla figura della Befana, che porta doni ai bambini nella notte tra il 5 e il 6 gennaio."),

        new HolidayInfo(
            "it_liberation_day", 4, 25, "IT",
            "Festa della Liberazione",
            NamesIt("Festa della Liberazione", "Día de la Liberación de Italia",
                    "Liberation Day", "Dia da Libertação", "Jour de la Libération", "Tag der Befreiung"),
            HolidayType.Civic,
            description: "Anniversario della liberazione dell'Italia dall'occupazione nazifascista il 25 aprile 1945, con la conclusione della Resistenza italiana e la resa delle forze dell'Asse nel nord Italia."),

        new HolidayInfo(
            "it_labor_day", 5, 1, "IT",
            "Festa dei Lavoratori",
            NamesIt("Festa dei Lavoratori", "Día del Trabajo", "Labour Day",
                    "Dia do Trabalho", "Fête du Travail", "Tag der Arbeit"),
            HolidayType.National,
            description: "Festa internazionale dei lavoratori / International Workers' Day."),

        new HolidayInfo(
            "it_republic_day", 6, 2, "IT",
            "Festa della Repubblica",
            NamesIt("Festa della Repubblica", "Día de la República Italiana",
                    "Republic Day", "Dia da República", "Fête de la République", "Tag der Republik"),
            HolidayType.Civic,
            description: "Anniversario del referendum istituzionale del 2 giugno 1946, in cui i cittadini italiani scelsero la forma repubblicana di governo, abolendo la monarchia sabauda. La Repubblica Italiana fu proclamata ufficialmente il 18 giugno 1946."),

        new HolidayInfo(
            "it_ferragosto", 8, 15, "IT",
            "Ferragosto",
            NamesIt("Ferragosto / Assunzione di Maria", "Ferragosto / Asunción de la Virgen",
                    "Ferragosto / Assumption of Mary",
                    "Ferragosto / Assunção de Maria", "Ferragosto / Assomption de Marie", "Ferragosto / Mariä Himmelfahrt"),
            HolidayType.Religious,
            description: "Festa dell'Assunzione di Maria Vergine. Ferragosto coincide con le principali ferie estive italiane; la tradizione pagana risale ai Feriae Augusti istituiti dall'imperatore Augusto nel 18 a.C."),

        new HolidayInfo(
            "it_all_saints", 11, 1, "IT",
            "Ognissanti",
            NamesIt("Ognissanti", "Todos los Santos", "All Saints' Day",
                    "Dia de Todos os Santos", "Toussaint", "Allerheiligen"),
            HolidayType.Religious,
            description: "Festività cattolica in onore di tutti i santi. In Italia tradizionalmente associata alla commemorazione dei defunti del 2 novembre."),

        new HolidayInfo(
            "it_immaculate", 12, 8, "IT",
            "Immacolata Concezione",
            NamesIt("Immacolata Concezione", "Inmaculada Concepción", "Immaculate Conception",
                    "Imaculada Conceição", "Immaculée Conception", "Mariä Empfängnis"),
            HolidayType.Religious,
            description: "Dogma cattolico proclamato da papa Pio IX nel 1854, che afferma che la Vergine Maria fu concepita senza peccato originale."),

        new HolidayInfo(
            "it_christmas", 12, 25, "IT",
            "Natale",
            NamesIt("Natale", "Navidad", "Christmas Day", "Natal", "Noël", "Weihnachten"),
            HolidayType.National,
            description: "Celebrazione della nascita di Gesù Cristo / Celebration of the birth of Jesus Christ."),

        new HolidayInfo(
            "it_santo_stefano", 12, 26, "IT",
            "Santo Stefano",
            NamesIt("Santo Stefano", "San Esteban / Boxing Day", "St. Stephen's Day / Boxing Day",
                    "Santo Estêvão / Boxing Day", "Saint-Étienne / Lendemain de Noël", "Zweiter Weihnachtstag / Stephanstag"),
            HolidayType.National,
            description: "Festività di Santo Stefano, primo martire cristiano. Corrisponde al Boxing Day nei paesi anglosassoni."),
    };

    /// <inheritdoc/>
    /// <remarks>
    /// Returns the single Easter-based movable holiday: Pasquetta (Easter Monday).
    /// Date is computed via <see cref="EasterCalculator"/>.
    /// </remarks>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        var easterMonday = EasterCalculator.Easter(year).AddDays(1);

        return new[]
        {
            new HolidayInfo(
                "it_easter_monday", easterMonday.Month, easterMonday.Day, "IT",
                "Pasquetta",
                NamesIt("Pasquetta / Lunedì dell'Angelo", "Lunes de Pascua", "Easter Monday",
                        "Segunda-Feira de Páscoa", "Lundi de Pâques", "Ostermontag"),
                HolidayType.National, isMovable: true,
                description: "Lunedì dopo Pasqua, noto come Pasquetta o Lunedì dell'Angelo. Festività nazionale in Italia, tradizionalmente celebrata con gite fuori porta.")
        };
    }
}
