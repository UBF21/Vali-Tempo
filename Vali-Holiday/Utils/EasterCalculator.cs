namespace Vali_Holiday.Utils;

/// <summary>
/// Provides static methods for computing Easter Sunday and related movable feast dates
/// using the Gaussian algorithm for the Gregorian calendar.
/// </summary>
public static class EasterCalculator
{
    /// <summary>
    /// Computes Easter Sunday for the given <paramref name="year"/> using the Gaussian algorithm
    /// applied to the Gregorian calendar.
    /// </summary>
    /// <param name="year">The four-digit calendar year (e.g., 2025).</param>
    /// <returns>A <see cref="DateTime"/> representing Easter Sunday at midnight.</returns>
    public static DateTime Easter(int year)
    {
        if (year < 1583)
            throw new ArgumentOutOfRangeException(nameof(year), year,
                "The Gaussian Easter algorithm is only valid for Gregorian calendar years (1583 and later).");
        int a = year % 19;
        int b = year / 100;
        int c = year % 100;
        int d = b / 4;
        int e = b % 4;
        int f = (b + 8) / 25;
        int g = (b - f + 1) / 3;
        int h = (19 * a + b - d - g + 15) % 30;
        int i = c / 4;
        int k = c % 4;
        int l = (32 + 2 * e + 2 * i - h - k) % 7;
        int m = (a + 11 * h + 22 * l) / 451;
        int month = (h + l - 7 * m + 114) / 31;
        int day = ((h + l - 7 * m + 114) % 31) + 1;
        return new DateTime(year, month, day);
    }

    /// <summary>
    /// Returns Good Friday (Viernes Santo) — 2 days before Easter Sunday.
    /// </summary>
    /// <param name="year">The four-digit calendar year.</param>
    /// <returns>A <see cref="DateTime"/> representing Good Friday.</returns>
    public static DateTime GoodFriday(int year) => Easter(year).AddDays(-2);

    /// <summary>
    /// Returns Holy Thursday / Maundy Thursday (Jueves Santo) — 3 days before Easter Sunday.
    /// </summary>
    /// <param name="year">The four-digit calendar year.</param>
    /// <returns>A <see cref="DateTime"/> representing Holy Thursday.</returns>
    public static DateTime HolyThursday(int year) => Easter(year).AddDays(-3);

    /// <summary>
    /// Returns Holy Saturday (Sábado Santo) — 1 day before Easter Sunday.
    /// </summary>
    /// <param name="year">The four-digit calendar year.</param>
    /// <returns>A <see cref="DateTime"/> representing Holy Saturday.</returns>
    public static DateTime HolySaturday(int year) => Easter(year).AddDays(-1);

    /// <summary>
    /// Returns Ash Wednesday (Miércoles de Ceniza) — 46 days before Easter Sunday.
    /// </summary>
    /// <param name="year">The four-digit calendar year.</param>
    /// <returns>A <see cref="DateTime"/> representing Ash Wednesday.</returns>
    public static DateTime AshWednesday(int year) => Easter(year).AddDays(-46);

    /// <summary>
    /// Returns Carnival Monday (Lunes de Carnaval) — 48 days before Easter Sunday.
    /// </summary>
    /// <param name="year">The four-digit calendar year.</param>
    /// <returns>A <see cref="DateTime"/> representing Carnival Monday.</returns>
    public static DateTime CarnavalMonday(int year) => Easter(year).AddDays(-48);

    /// <summary>
    /// Returns Carnival Tuesday (Martes de Carnaval) — 47 days before Easter Sunday.
    /// </summary>
    /// <param name="year">The four-digit calendar year.</param>
    /// <returns>A <see cref="DateTime"/> representing Carnival Tuesday.</returns>
    public static DateTime CarnavalTuesday(int year) => Easter(year).AddDays(-47);

    /// <summary>
    /// Returns the Ascension of Christ (Ascensión del Señor) — 39 days after Easter Sunday.
    /// </summary>
    /// <param name="year">The four-digit calendar year.</param>
    /// <returns>A <see cref="DateTime"/> representing Ascension Thursday.</returns>
    public static DateTime Ascension(int year) => Easter(year).AddDays(39);

    /// <summary>
    /// Returns Pentecost Sunday (Pentecostés) — 49 days after Easter Sunday.
    /// </summary>
    /// <param name="year">The four-digit calendar year.</param>
    /// <returns>A <see cref="DateTime"/> representing Pentecost Sunday.</returns>
    public static DateTime Pentecost(int year) => Easter(year).AddDays(49);

    /// <summary>
    /// Returns Corpus Christi — 60 days after Easter Sunday.
    /// </summary>
    /// <param name="year">The four-digit calendar year.</param>
    /// <returns>A <see cref="DateTime"/> representing Corpus Christi.</returns>
    public static DateTime CorpusChristi(int year) => Easter(year).AddDays(60);

    /// <summary>
    /// Returns the Feast of the Sacred Heart of Jesus (Sagrado Corazón de Jesús) —
    /// 68 days after Easter Sunday.
    /// </summary>
    /// <param name="year">The four-digit calendar year.</param>
    /// <returns>A <see cref="DateTime"/> representing the Sacred Heart feast.</returns>
    public static DateTime SacredHeart(int year) => Easter(year).AddDays(68);
}
