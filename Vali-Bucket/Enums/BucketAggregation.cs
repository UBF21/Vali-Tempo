namespace Vali_Bucket.Enums;

/// <summary>
/// Specifies how numeric values are combined within a single time bucket.
/// </summary>
public enum BucketAggregation
{
    /// <summary>Sum of all values in the bucket.</summary>
    Sum,

    /// <summary>Arithmetic mean of all values in the bucket.</summary>
    Average,

    /// <summary>Number of values in the bucket (the values themselves are ignored).</summary>
    Count,

    /// <summary>Smallest value in the bucket.</summary>
    Min,

    /// <summary>Largest value in the bucket.</summary>
    Max
}
