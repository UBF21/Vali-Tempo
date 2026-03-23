using Vali_Holiday.Core;
using Vali_Holiday.Models;

namespace Vali_Holiday.Providers.NorthAmerica;

/// <summary>
/// Holiday provider for the United States of America, covering the 11 federal public holidays
/// established by 5 U.S.C. § 6103, plus 4 widely observed national observances.
/// Movable holidays that fall on a floating weekday (e.g., "third Monday of January")
/// are stored with an approximate representative date and <see cref="HolidayInfo.IsMovable"/>
/// set to <see langword="true"/>; the description field documents the exact calculation rule.
/// </summary>
public class UsaHolidayProvider : BaseHolidayProvider
{
    /// <inheritdoc/>
    public override string CountryCode => "US";

    /// <inheritdoc/>
    public override string CountryName => "United States of America";

    /// <inheritdoc/>
    public override string PrimaryLanguage => "en";

    /// <inheritdoc/>
    /// <remarks>
    /// Returns all fixed-date federal holidays and observances.
    /// Floating weekday federal holidays are returned by <see cref="GetMovableHolidays"/>.
    /// </remarks>
    protected override IEnumerable<HolidayInfo> GetFixedHolidays() => new[]
    {
        // ── Federal fixed holidays ────────────────────────────────────────────────

        new HolidayInfo(
            "us_new_year", 1, 1, "US",
            "New Year's Day",
            Names("Año Nuevo", "New Year's Day", "Ano Novo", "Jour de l'An", "Neujahrstag"),
            HolidayType.National,
            description: "Celebration of the start of the calendar year."),

        new HolidayInfo(
            "us_juneteenth", 6, 19, "US",
            "Juneteenth National Independence Day",
            Names("Día de la Independencia de Juneteenth", "Juneteenth National Independence Day",
                  "Dia Nacional de Independência Juneteenth", "Journée nationale de l'indépendance Juneteenth",
                  "Juneteenth Nationaler Unabhängigkeitstag"),
            HolidayType.Civic,
            description: "Commemorates June 19, 1865, when Union soldiers arrived in Galveston, Texas, and announced the end of slavery — more than two years after the Emancipation Proclamation. Established as a federal holiday in 2021."),

        new HolidayInfo(
            "us_independence_day", 7, 4, "US",
            "Independence Day",
            Names("Día de la Independencia", "Independence Day",
                  "Dia da Independência", "Fête de l'Indépendance", "Unabhängigkeitstag"),
            HolidayType.Civic,
            description: "Commemorates the adoption of the Declaration of Independence on July 4, 1776, marking the separation of the thirteen colonies from Great Britain."),

        new HolidayInfo(
            "us_veterans_day", 11, 11, "US",
            "Veterans Day",
            Names("Día de los Veteranos", "Veterans Day",
                  "Dia dos Veteranos", "Jour des Anciens Combattants", "Veteranentag"),
            HolidayType.Civic,
            description: "Honors all military veterans of the United States Armed Forces. Coincides with Armistice Day (November 11, 1918), marking the end of World War I."),

        new HolidayInfo(
            "us_christmas", 12, 25, "US",
            "Christmas Day",
            Names("Navidad", "Christmas Day", "Natal", "Noël", "Weihnachten"),
            HolidayType.National,
            description: "Federal holiday celebrating Christmas."),

        // ── Observances ───────────────────────────────────────────────────────────

        new HolidayInfo(
            "us_valentines_day", 2, 14, "US",
            "Valentine's Day",
            Names("Día de San Valentín", "Valentine's Day",
                  "Dia dos Namorados", "Saint-Valentin", "Valentinstag"),
            HolidayType.Observance,
            description: "Cultural observance celebrating romantic love and affection. Not a federal holiday; businesses and schools remain open."),

        new HolidayInfo(
            "us_halloween", 10, 31, "US",
            "Halloween",
            Names("Halloween / Víspera de Todos los Santos", "Halloween",
                  "Halloween / Véspera de Todos os Santos", "Halloween / Veille de la Toussaint", "Halloween / Allerheiligen-Vorabend"),
            HolidayType.Observance,
            description: "Cultural observance rooted in the Celtic festival of Samhain. Known for costumes, trick-or-treating, and jack-o'-lanterns. Not a federal holiday."),

        new HolidayInfo(
            "us_new_years_eve", 12, 31, "US",
            "New Year's Eve",
            Names("Nochevieja", "New Year's Eve",
                  "Véspera de Ano Novo", "Réveillon du Nouvel An", "Silvester"),
            HolidayType.Observance,
            description: "Cultural observance on the last day of the calendar year. Not a federal holiday."),
    };

    /// <inheritdoc/>
    /// <remarks>
    /// Returns the six floating federal holidays whose dates follow a "nth weekday of month"
    /// rule. Representative dates shown correspond to typical mid-year occurrences;
    /// the description field documents the exact floating rule for each.
    /// </remarks>
    protected override IEnumerable<HolidayInfo> GetMovableHolidays(int year)
    {
        // Third Monday of January
        var mlkDay = NthWeekday(year, 1, DayOfWeek.Monday, 3);
        // Third Monday of February
        var presidentsDay = NthWeekday(year, 2, DayOfWeek.Monday, 3);
        // Last Monday of May
        var memorialDay = LastWeekday(year, 5, DayOfWeek.Monday);
        // First Monday of September
        var laborDay = NthWeekday(year, 9, DayOfWeek.Monday, 1);
        // Second Monday of October
        var columbusDay = NthWeekday(year, 10, DayOfWeek.Monday, 2);
        // Fourth Thursday of November
        var thanksgiving = NthWeekday(year, 11, DayOfWeek.Thursday, 4);
        DateTime blackFriday = thanksgiving.AddDays(1);

        return new[]
        {
            new HolidayInfo(
                "us_mlk_day", mlkDay.Month, mlkDay.Day, "US",
                "Martin Luther King Jr. Day",
                Names("Día de Martin Luther King Jr.", "Martin Luther King Jr. Day",
                      "Dia de Martin Luther King Jr.", "Journée Martin Luther King Jr.", "Martin-Luther-King-Jr.-Tag"),
                HolidayType.National, isMovable: true,
                description: "Honors the civil rights leader Dr. Martin Luther King Jr., born January 15, 1929. Observed on the third Monday of January each year (Uniform Monday Holiday Act, 1983)."),

            new HolidayInfo(
                "us_presidents_day", presidentsDay.Month, presidentsDay.Day, "US",
                "Presidents' Day",
                Names("Día de los Presidentes", "Presidents' Day / Washington's Birthday",
                      "Dia dos Presidentes", "Jour des Présidents", "Präsidententag"),
                HolidayType.National, isMovable: true,
                description: "Officially 'Washington's Birthday', honoring George Washington (born February 22, 1732). Observed on the third Monday of February. Colloquially known as Presidents' Day and often used to honor all U.S. presidents."),

            new HolidayInfo(
                "us_memorial_day", memorialDay.Month, memorialDay.Day, "US",
                "Memorial Day",
                Names("Día de los Caídos", "Memorial Day",
                      "Dia dos Caídos", "Jour du Souvenir", "Gedenktag"),
                HolidayType.National, isMovable: true,
                description: "Honors U.S. military personnel who have died in service. Observed on the last Monday of May. Informally marks the start of the summer season."),

            new HolidayInfo(
                "us_labor_day", laborDay.Month, laborDay.Day, "US",
                "Labor Day",
                Names("Día del Trabajo", "Labor Day",
                      "Dia do Trabalho", "Fête du Travail", "Tag der Arbeit"),
                HolidayType.National, isMovable: true,
                description: "Celebrates the labor movement and the contributions of workers. Observed on the first Monday of September. Informally marks the end of the summer season."),

            new HolidayInfo(
                "us_columbus_day", columbusDay.Month, columbusDay.Day, "US",
                "Columbus Day",
                Names("Día de Colón / Día de los Pueblos Indígenas", "Columbus Day / Indigenous Peoples' Day",
                      "Dia de Colombo / Dia dos Povos Indígenas", "Jour de Christophe Colomb", "Kolumbustag"),
                HolidayType.National, isMovable: true,
                description: "Commemorates Christopher Columbus's arrival in the Americas on October 12, 1492. Observed on the second Monday of October. Many states and cities celebrate it as Indigenous Peoples' Day."),

            new HolidayInfo(
                "us_thanksgiving", thanksgiving.Month, thanksgiving.Day, "US",
                "Thanksgiving Day",
                Names("Día de Acción de Gracias", "Thanksgiving Day",
                      "Dia de Ação de Graças", "Jour de l'Action de Grâce", "Erntedankfest"),
                HolidayType.National, isMovable: true,
                description: "National holiday celebrating the harvest and giving thanks. Observed on the fourth Thursday of November each year, a tradition dating to 1863 when President Lincoln proclaimed it a national holiday."),

            new HolidayInfo(
                "us_black_friday", blackFriday.Month, blackFriday.Day, "US",
                "Black Friday",
                Names("Viernes Negro", "Black Friday",
                      "Black Friday", "Black Friday", "Black Friday"),
                HolidayType.Observance, isMovable: true,
                description: "The day after Thanksgiving, traditionally the start of the Christmas shopping season. Not a federal holiday; falls one day after the fourth Thursday of November each year."),
        };
    }

    /// <summary>
    /// Returns the <paramref name="n"/>th occurrence of <paramref name="weekday"/> in the
    /// given <paramref name="month"/> of <paramref name="year"/>.
    /// </summary>
    /// <param name="year">Four-digit calendar year.</param>
    /// <param name="month">Month number (1–12).</param>
    /// <param name="weekday">Target day of the week.</param>
    /// <param name="n">Occurrence index (1 = first, 2 = second, …).</param>
    /// <returns>A <see cref="DateTime"/> representing the computed date.</returns>
    private static DateTime NthWeekday(int year, int month, DayOfWeek weekday, int n)
    {
        var first = new DateTime(year, month, 1);
        int daysUntil = ((int)weekday - (int)first.DayOfWeek + 7) % 7;
        return first.AddDays(daysUntil + (n - 1) * 7);
    }

    /// <summary>
    /// Returns the last occurrence of <paramref name="weekday"/> in the given
    /// <paramref name="month"/> of <paramref name="year"/>.
    /// </summary>
    /// <param name="year">Four-digit calendar year.</param>
    /// <param name="month">Month number (1–12).</param>
    /// <param name="weekday">Target day of the week.</param>
    /// <returns>A <see cref="DateTime"/> representing the computed date.</returns>
    private static DateTime LastWeekday(int year, int month, DayOfWeek weekday)
    {
        var last = new DateTime(year, month, DateTime.DaysInMonth(year, month));
        int daysBack = ((int)last.DayOfWeek - (int)weekday + 7) % 7;
        return last.AddDays(-daysBack);
    }
}
