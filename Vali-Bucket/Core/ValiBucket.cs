using System.Numerics;
using Vali_Bucket.Enums;
using Vali_Bucket.Models;
using Vali_Range.Models;
using Vali_Time.Enums;

namespace Vali_Bucket.Core;

/// <summary>
/// Default <see cref="IValiBucket"/> implementation. Buckets are computed purely from tick
/// arithmetic — no timezone/DST awareness (see interface remarks).
/// </summary>
public class ValiBucket : IValiBucket
{
    /// <inheritdoc/>
    public IReadOnlyList<TimeBucket> Bucketize(IEnumerable<DateTime> timestamps, BucketSize size, DateTime? anchor = null)
    {
        long bucketTicks = ResolveBucketTicks(size);
        long anchorTicks = (anchor ?? DateTime.MinValue).Ticks;

        var counts = new SortedDictionary<long, int>();
        foreach (var timestamp in timestamps)
        {
            long index = FloorDiv(timestamp.Ticks - anchorTicks, bucketTicks);
            counts[index] = counts.TryGetValue(index, out var existing) ? existing + 1 : 1;
        }

        var result = new List<TimeBucket>(counts.Count);
        foreach (var (index, count) in counts)
            result.Add(new TimeBucket(BucketRange(anchorTicks, index, bucketTicks), count));

        return result;
    }

    /// <inheritdoc/>
    public IReadOnlyList<TimeBucket<T>> Aggregate<T>(
        IEnumerable<(DateTime Timestamp, T Value)> points,
        BucketSize size,
        BucketAggregation aggregation,
        DateTime? anchor = null) where T : INumber<T>
    {
        long bucketTicks = ResolveBucketTicks(size);
        long anchorTicks = (anchor ?? DateTime.MinValue).Ticks;

        var buckets = new SortedDictionary<long, List<T>>();
        foreach (var (timestamp, value) in points)
        {
            long index = FloorDiv(timestamp.Ticks - anchorTicks, bucketTicks);
            if (!buckets.TryGetValue(index, out var values))
                buckets[index] = values = [];
            values.Add(value);
        }

        var result = new List<TimeBucket<T>>(buckets.Count);
        foreach (var (index, values) in buckets)
        {
            T aggregated = Aggregate(values, aggregation);
            result.Add(new TimeBucket<T>(BucketRange(anchorTicks, index, bucketTicks), values.Count, aggregated));
        }

        return result;
    }

    /// <inheritdoc/>
    public TimeBucket GetBucketFor(DateTime timestamp, BucketSize size, DateTime? anchor = null)
    {
        long bucketTicks = ResolveBucketTicks(size);
        long anchorTicks = (anchor ?? DateTime.MinValue).Ticks;
        long index = FloorDiv(timestamp.Ticks - anchorTicks, bucketTicks);
        return new TimeBucket(BucketRange(anchorTicks, index, bucketTicks), 1);
    }

    private static T Aggregate<T>(List<T> values, BucketAggregation aggregation) where T : INumber<T> =>
        aggregation switch
        {
            BucketAggregation.Sum => Sum(values),
            BucketAggregation.Average => Sum(values) / T.CreateChecked(values.Count),
            BucketAggregation.Count => T.CreateChecked(values.Count),
            BucketAggregation.Min => values.Aggregate((a, b) => a < b ? a : b),
            BucketAggregation.Max => values.Aggregate((a, b) => a > b ? a : b),
            _ => throw new ArgumentOutOfRangeException(nameof(aggregation), aggregation, "Unrecognised BucketAggregation value.")
        };

    private static T Sum<T>(List<T> values) where T : INumber<T>
    {
        T total = T.Zero;
        foreach (var value in values) total += value;
        return total;
    }

    /// <summary>
    /// Resolves a <see cref="BucketSize"/> to a strictly positive tick count, validating that the
    /// unit is fixed-length and the amount is positive.
    /// </summary>
    private static long ResolveBucketTicks(BucketSize size)
    {
        long unitTicks = size.Unit switch
        {
            TimeUnit.Milliseconds => TimeSpan.TicksPerMillisecond,
            TimeUnit.Seconds => TimeSpan.TicksPerSecond,
            TimeUnit.Minutes => TimeSpan.TicksPerMinute,
            TimeUnit.Hours => TimeSpan.TicksPerHour,
            TimeUnit.Days => TimeSpan.TicksPerDay,
            TimeUnit.Weeks => TimeSpan.TicksPerDay * 7,
            _ => throw new ArgumentOutOfRangeException(nameof(size), size.Unit,
                "Vali-Bucket only supports fixed-length units (Milliseconds..Weeks); Months and Years have no constant tick length.")
        };

        decimal ticks = size.Amount * unitTicks;
        if (ticks <= 0)
            throw new ArgumentOutOfRangeException(nameof(size), "Bucket size amount must be greater than zero.");
        if (ticks > long.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(size), "Bucket size is too large to represent.");

        return (long)ticks;
    }

    /// <summary>Floor division that rounds toward negative infinity for negative dividends.</summary>
    private static long FloorDiv(long dividend, long divisor)
    {
        long quotient = dividend / divisor;
        long remainder = dividend % divisor;
        return remainder != 0 && (remainder < 0) != (divisor < 0) ? quotient - 1 : quotient;
    }

    private static DateRange BucketRange(long anchorTicks, long bucketIndex, long bucketTicks)
    {
        long startTicks = anchorTicks + bucketIndex * bucketTicks;
        long endTicks = startTicks + bucketTicks - 1;
        return new DateRange(new DateTime(startTicks), new DateTime(endTicks));
    }
}
