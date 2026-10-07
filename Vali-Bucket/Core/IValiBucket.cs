using Vali_Bucket.Enums;
using Vali_Bucket.Models;

namespace Vali_Bucket.Core;

/// <summary>
/// Groups timestamps into fixed-size, aligned time buckets for reporting and analytics —
/// e.g. "bucket these 50,000 event timestamps into 15-minute windows for a chart".
/// </summary>
/// <remarks>
/// Buckets are aligned to <c>anchor</c> (defaulting to <see cref="DateTime.MinValue"/>) so that
/// separate calls with the same <c>size</c> and <c>anchor</c> always produce comparable,
/// identically-aligned buckets. v1 only supports fixed-length units (<see cref="Vali_Time.Enums.TimeUnit.Milliseconds"/>
/// through <see cref="Vali_Time.Enums.TimeUnit.Weeks"/>) — no DST/timezone awareness, no
/// Months/Years bucket sizes.
/// </remarks>
public interface IValiBucket
{
    /// <summary>
    /// Groups <paramref name="timestamps"/> into buckets of <paramref name="size"/>, aligned to
    /// <paramref name="anchor"/>. Buckets with zero matching timestamps are omitted.
    /// </summary>
    /// <param name="timestamps">The timestamps to bucketize. Need not be sorted or distinct.</param>
    /// <param name="size">The fixed size of each bucket.</param>
    /// <param name="anchor">
    /// The alignment anchor. When <c>null</c>, <see cref="DateTime.MinValue"/> is used.
    /// </param>
    /// <returns>Non-empty buckets, ordered by <see cref="TimeBucket.Range"/> start, ascending.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="size"/> has a non-positive amount or a non-fixed-length unit
    /// (<see cref="Vali_Time.Enums.TimeUnit.Months"/> or <see cref="Vali_Time.Enums.TimeUnit.Years"/>).
    /// </exception>
    IReadOnlyList<TimeBucket> Bucketize(IEnumerable<DateTime> timestamps, BucketSize size, DateTime? anchor = null);

    /// <summary>
    /// Groups <paramref name="points"/> into buckets of <paramref name="size"/>, aligned to
    /// <paramref name="anchor"/>, aggregating each bucket's values per <paramref name="aggregation"/>.
    /// Buckets with zero matching points are omitted.
    /// </summary>
    /// <typeparam name="T">The numeric type being aggregated.</typeparam>
    /// <param name="points">The (timestamp, value) pairs to bucketize and aggregate.</param>
    /// <param name="size">The fixed size of each bucket.</param>
    /// <param name="aggregation">How to combine the values within each bucket.</param>
    /// <param name="anchor">
    /// The alignment anchor. When <c>null</c>, <see cref="DateTime.MinValue"/> is used.
    /// </param>
    /// <returns>Non-empty buckets, ordered by <see cref="TimeBucket{T}.Range"/> start, ascending.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="size"/> is invalid (see <see cref="Bucketize"/>), or when
    /// <paramref name="aggregation"/> is not a recognised value.
    /// </exception>
    IReadOnlyList<TimeBucket<T>> Aggregate<T>(
        IEnumerable<(DateTime Timestamp, T Value)> points,
        BucketSize size,
        BucketAggregation aggregation,
        DateTime? anchor = null) where T : System.Numerics.INumber<T>;

    /// <summary>
    /// Returns the bucket that <paramref name="timestamp"/> falls into, without bucketizing an
    /// entire collection — useful for an O(1) lookup in a streaming/hot-path scenario.
    /// </summary>
    /// <param name="timestamp">The timestamp to locate.</param>
    /// <param name="size">The fixed size of the bucket grid.</param>
    /// <param name="anchor">
    /// The alignment anchor. When <c>null</c>, <see cref="DateTime.MinValue"/> is used.
    /// </param>
    /// <returns>
    /// The <see cref="TimeBucket"/> containing <paramref name="timestamp"/>, with
    /// <see cref="TimeBucket.Count"/> always <c>1</c> (this overload does not consult any
    /// other data — it only resolves the grid cell).
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="size"/> is invalid (see <see cref="Bucketize"/>).</exception>
    TimeBucket GetBucketFor(DateTime timestamp, BucketSize size, DateTime? anchor = null);
}
