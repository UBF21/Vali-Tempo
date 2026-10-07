using Vali_Time.Enums;
using Vali_Time.Utils;

namespace Vali_Age.Internal;

/// <summary>
/// Granularity bucket resolved from an absolute number of seconds between two points in time.
/// </summary>
internal enum HumanizeBucket
{
    Moment,
    Minute,
    Hour,
    Day,
    Week,
    Month,
    Year
}

/// <summary>
/// Result of resolving a time difference into a granularity bucket.
/// </summary>
internal readonly record struct BucketResult(HumanizeBucket Bucket, long Value, bool IsPast);

/// <summary>
/// Pure-data singular/plural/past/future text for a countable bucket (Minute/Hour/Day/Week/Month/Year).
/// <c>PastOne</c>/<c>FutureOne</c> are used for <c>Value == 1</c> (e.g. "yesterday"); <c>PastMany</c>/
/// <c>FutureMany</c> carry a <c>"{0}"</c> placeholder for <c>Value &gt;= 2</c>.
/// </summary>
internal sealed record BucketTexts(string PastOne, string PastMany, string FutureOne, string FutureMany);

/// <summary>
/// Complete set of literal strings needed to render <see cref="HumanizeEngine"/> output for one
/// <see cref="HumanizeCulture"/>. Adding a new culture is adding one new instance of this record —
/// no logic changes. <paramref name="Buckets"/> holds one <see cref="BucketTexts"/> per countable
/// bucket; <paramref name="LessThan"/> holds the "less than one &lt;unit&gt;" (past, future) pair
/// used when <c>maxPrecision</c> collapses a finer bucket.
/// </summary>
internal sealed record CultureTemplates(
    string MomentPast,
    string MomentFuture,
    Dictionary<HumanizeBucket, BucketTexts> Buckets,
    Dictionary<HumanizeBucket, (string Past, string Future)> LessThan);

/// <summary>
/// Internal engine shared by <c>ValiAge.Humanize</c> and the <c>DateTime.Humanize</c> extension.
/// Resolves a time difference into a granularity bucket and renders it as a localized string.
/// </summary>
internal static class HumanizeEngine
{
    internal static readonly Dictionary<HumanizeCulture, CultureTemplates> Templates = new()
    {
        [HumanizeCulture.En] = new CultureTemplates(
            MomentPast: "a moment ago",
            MomentFuture: "in a moment",
            Buckets: new Dictionary<HumanizeBucket, BucketTexts>
            {
                [HumanizeBucket.Minute] = new BucketTexts("a minute ago", "{0} minutes ago", "in a minute", "in {0} minutes"),
                [HumanizeBucket.Hour] = new BucketTexts("an hour ago", "{0} hours ago", "in an hour", "in {0} hours"),
                [HumanizeBucket.Day] = new BucketTexts("yesterday", "{0} days ago", "tomorrow", "in {0} days"),
                [HumanizeBucket.Week] = new BucketTexts("last week", "{0} weeks ago", "next week", "in {0} weeks"),
                [HumanizeBucket.Month] = new BucketTexts("last month", "{0} months ago", "next month", "in {0} months"),
                [HumanizeBucket.Year] = new BucketTexts("last year", "{0} years ago", "next year", "in {0} years")
            },
            LessThan: new Dictionary<HumanizeBucket, (string Past, string Future)>
            {
                [HumanizeBucket.Minute] = ("less than a minute ago", "in less than a minute"),
                [HumanizeBucket.Hour] = ("less than an hour ago", "in less than an hour"),
                [HumanizeBucket.Day] = ("less than a day ago", "in less than a day"),
                [HumanizeBucket.Week] = ("less than a week ago", "in less than a week"),
                [HumanizeBucket.Month] = ("less than a month ago", "in less than a month"),
                [HumanizeBucket.Year] = ("less than a year ago", "in less than a year")
            }),

        [HumanizeCulture.Es] = new CultureTemplates(
            MomentPast: "hace un momento",
            MomentFuture: "en un momento",
            Buckets: new Dictionary<HumanizeBucket, BucketTexts>
            {
                [HumanizeBucket.Minute] = new BucketTexts("hace un minuto", "hace {0} minutos", "en un minuto", "en {0} minutos"),
                [HumanizeBucket.Hour] = new BucketTexts("hace una hora", "hace {0} horas", "en una hora", "en {0} horas"),
                [HumanizeBucket.Day] = new BucketTexts("ayer", "hace {0} días", "mañana", "en {0} días"),
                [HumanizeBucket.Week] = new BucketTexts("la semana pasada", "hace {0} semanas", "la próxima semana", "en {0} semanas"),
                [HumanizeBucket.Month] = new BucketTexts("el mes pasado", "hace {0} meses", "el próximo mes", "en {0} meses"),
                [HumanizeBucket.Year] = new BucketTexts("el año pasado", "hace {0} años", "el próximo año", "en {0} años")
            },
            LessThan: new Dictionary<HumanizeBucket, (string Past, string Future)>
            {
                [HumanizeBucket.Minute] = ("hace menos de un minuto", "en menos de un minuto"),
                [HumanizeBucket.Hour] = ("hace menos de una hora", "en menos de una hora"),
                [HumanizeBucket.Day] = ("hace menos de un día", "en menos de un día"),
                [HumanizeBucket.Week] = ("hace menos de una semana", "en menos de una semana"),
                [HumanizeBucket.Month] = ("hace menos de un mes", "en menos de un mes"),
                [HumanizeBucket.Year] = ("hace menos de un año", "en menos de un año")
            })
    };

    /// <summary>
    /// Computes the absolute difference in seconds between <paramref name="date"/> and
    /// <paramref name="reference"/> using <c>decimal</c> ticks arithmetic (never <c>double</c>),
    /// so <see cref="DateTime.MinValue"/>/<see cref="DateTime.MaxValue"/> never overflow.
    /// </summary>
    internal static decimal AbsSeconds(DateTime date, DateTime reference)
    {
        decimal diffTicks = (decimal)reference.Ticks - (decimal)date.Ticks;
        return Math.Abs(diffTicks) / TimeSpan.TicksPerSecond;
    }

    /// <summary>
    /// A tie (<c>Δs == 0</c>) is treated as past.
    /// </summary>
    internal static bool IsPast(DateTime date, DateTime reference) => reference >= date;

    /// <summary>
    /// Resolves an absolute second difference into its natural granularity bucket.
    /// </summary>
    internal static BucketResult Resolve(decimal absSeconds, bool isPast)
    {
        if (absSeconds < Constants.SecondsInMinute)
            return new BucketResult(HumanizeBucket.Moment, 0, isPast);

        if (absSeconds < Constants.SecondsInHour)
            return new BucketResult(HumanizeBucket.Minute, (long)(absSeconds / Constants.SecondsInMinute), isPast);

        if (absSeconds < Constants.SecondsInDay)
            return new BucketResult(HumanizeBucket.Hour, (long)(absSeconds / Constants.SecondsInHour), isPast);

        if (absSeconds < Constants.SecondsInWeek)
            return new BucketResult(HumanizeBucket.Day, (long)(absSeconds / Constants.SecondsInDay), isPast);

        if (absSeconds < Constants.SecondsInMonth)
            return new BucketResult(HumanizeBucket.Week, (long)(absSeconds / Constants.SecondsInWeek), isPast);

        if (absSeconds < Constants.SecondsInYear)
            return new BucketResult(HumanizeBucket.Month, (long)(absSeconds / Constants.SecondsInMonth), isPast);

        return new BucketResult(HumanizeBucket.Year, (long)(absSeconds / Constants.SecondsInYear), isPast);
    }

    /// <summary>
    /// Maps a <see cref="TimeUnit"/> precision floor to its bucket and seconds-per-unit, used to
    /// recompute <c>N</c> when the natural bucket is finer than the requested <paramref name="maxPrecision"/>.
    /// </summary>
    /// <exception cref="NotSupportedException">
    /// Thrown when <paramref name="maxPrecision"/> is <see cref="TimeUnit.Milliseconds"/> or
    /// <see cref="TimeUnit.Seconds"/> — too fine to act as a floor.
    /// </exception>
    private static (HumanizeBucket Bucket, decimal SecondsPerUnit) MapPrecision(TimeUnit maxPrecision)
    {
        return maxPrecision switch
        {
            TimeUnit.Minutes => (HumanizeBucket.Minute, Constants.SecondsInMinute),
            TimeUnit.Hours => (HumanizeBucket.Hour, Constants.SecondsInHour),
            TimeUnit.Days => (HumanizeBucket.Day, Constants.SecondsInDay),
            TimeUnit.Weeks => (HumanizeBucket.Week, Constants.SecondsInWeek),
            TimeUnit.Months => (HumanizeBucket.Month, Constants.SecondsInMonth),
            TimeUnit.Years => (HumanizeBucket.Year, Constants.SecondsInYear),
            _ => throw new NotSupportedException(
                $"TimeUnit '{maxPrecision}' is too fine to use as a Humanize maxPrecision floor. " +
                "Supported values: Minutes, Hours, Days, Weeks, Months, Years.")
        };
    }

    /// <summary>
    /// Renders <paramref name="date"/> vs. <paramref name="reference"/> as a localized relative
    /// time string, applying the optional <paramref name="maxPrecision"/> floor.
    /// </summary>
    /// <exception cref="NotSupportedException">
    /// Thrown when <paramref name="culture"/> is not a recognised <see cref="HumanizeCulture"/>,
    /// or <paramref name="maxPrecision"/> is too fine (see <see cref="MapPrecision"/>).
    /// </exception>
    internal static string Humanize(DateTime date, DateTime reference, HumanizeCulture culture, TimeUnit? maxPrecision)
    {
        if (!Templates.TryGetValue(culture, out CultureTemplates? templates))
            throw new NotSupportedException(
                $"HumanizeCulture '{culture}' is not supported. Supported cultures: {string.Join(", ", Templates.Keys)}.");

        bool isPast = IsPast(date, reference);
        decimal absSeconds = AbsSeconds(date, reference);
        BucketResult natural = Resolve(absSeconds, isPast);

        if (maxPrecision is null)
            return Format(natural, templates);

        (HumanizeBucket capBucket, decimal secondsPerUnit) = MapPrecision(maxPrecision.Value);

        // The natural bucket is already as coarse as, or coarser than, the requested floor.
        if (natural.Bucket >= capBucket)
            return Format(natural, templates);

        long cappedValue = (long)(absSeconds / secondsPerUnit);

        if (cappedValue == 0)
        {
            (string past, string future) = templates.LessThan[capBucket];
            return isPast ? past : future;
        }

        return Format(new BucketResult(capBucket, cappedValue, isPast), templates);
    }

    private static string Format(BucketResult result, CultureTemplates t)
    {
        if (result.Bucket == HumanizeBucket.Moment)
            return result.IsPast ? t.MomentPast : t.MomentFuture;

        BucketTexts bucketTexts = t.Buckets[result.Bucket];

        if (result.Value == 1)
            return result.IsPast ? bucketTexts.PastOne : bucketTexts.FutureOne;

        string template = result.IsPast ? bucketTexts.PastMany : bucketTexts.FutureMany;
        return string.Format(template, result.Value);
    }
}
