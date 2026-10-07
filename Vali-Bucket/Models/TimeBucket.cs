using Vali_Range.Models;

namespace Vali_Bucket.Models;

/// <summary>
/// A single time bucket produced by <c>IValiBucket.Bucketize</c> or <c>GetBucketFor</c> —
/// a fixed-size, aligned time range together with how many timestamps fell inside it.
/// </summary>
public readonly struct TimeBucket
{
    /// <summary>The bucket's time range (inclusive on both ends).</summary>
    public DateRange Range { get; }

    /// <summary>The number of timestamps that fall within <see cref="Range"/>.</summary>
    public int Count { get; }

    /// <summary>Initializes a new <see cref="TimeBucket"/>.</summary>
    public TimeBucket(DateRange range, int count)
    {
        Range = range;
        Count = count;
    }
}

/// <summary>
/// A single time bucket produced by <c>IValiBucket.Aggregate{T}</c> — a fixed-size, aligned
/// time range together with the count of points and their aggregated <typeparamref name="T"/> value.
/// </summary>
/// <typeparam name="T">The numeric type being aggregated.</typeparam>
public readonly struct TimeBucket<T>
{
    /// <summary>The bucket's time range (inclusive on both ends).</summary>
    public DateRange Range { get; }

    /// <summary>The number of points that fall within <see cref="Range"/>.</summary>
    public int Count { get; }

    /// <summary>The aggregated value for this bucket, per the requested <c>BucketAggregation</c>.</summary>
    public T Value { get; }

    /// <summary>Initializes a new <see cref="TimeBucket{T}"/>.</summary>
    public TimeBucket(DateRange range, int count, T value)
    {
        Range = range;
        Count = count;
        Value = value;
    }
}
