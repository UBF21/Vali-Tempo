using Vali_Age.Internal;
using Vali_Time.Enums;

namespace Vali_Age.Extensions;

/// <summary>
/// Stateless <see cref="DateTime"/> extension for one-line relative time humanization.
/// </summary>
public static class DateTimeHumanizeExtensions
{
    /// <summary>
    /// Returns a human-readable, culturally-localized relative time description of this
    /// <see cref="DateTime"/> compared to <paramref name="relativeTo"/> (or <see cref="DateTime.Now"/>
    /// when omitted). Stateless convenience wrapper — does not require DI. Delegates to the same
    /// internal engine used by <c>ValiAge.Humanize</c>, so results are identical.
    /// </summary>
    /// <param name="date">The date/time to describe.</param>
    /// <param name="relativeTo">The reference point in time. Defaults to <see cref="DateTime.Now"/> when <c>null</c>.</param>
    /// <param name="culture">The output language. Defaults to <see cref="HumanizeCulture.En"/>.</param>
    /// <param name="maxPrecision">
    /// The finest <see cref="TimeUnit"/> allowed in the output. See
    /// <c>ValiAge.Humanize(DateTime, DateTime?, HumanizeCulture, TimeUnit?)</c> for the full semantics.
    /// </param>
    /// <returns>A localized, human-readable relative time string.</returns>
    /// <exception cref="NotSupportedException">
    /// Thrown when <paramref name="culture"/> is not a recognised <see cref="HumanizeCulture"/>
    /// value, or <paramref name="maxPrecision"/> is <see cref="TimeUnit.Milliseconds"/> or
    /// <see cref="TimeUnit.Seconds"/> (too fine to act as a floor).
    /// </exception>
    public static string Humanize(this DateTime date, DateTime? relativeTo = null, HumanizeCulture culture = HumanizeCulture.En, TimeUnit? maxPrecision = null)
    {
        DateTime reference = relativeTo ?? DateTime.Now;
        return HumanizeEngine.Humanize(date, reference, culture, maxPrecision);
    }
}
