using Vali_Time.Enums;

namespace Vali_Bucket.Models;

/// <summary>
/// Defines the size of a time bucket as an amount of a fixed-length <see cref="TimeUnit"/>.
/// Only fixed-length units are supported (<see cref="TimeUnit.Milliseconds"/> through
/// <see cref="TimeUnit.Weeks"/>) — <see cref="TimeUnit.Months"/> and <see cref="TimeUnit.Years"/>
/// have no constant tick length and cannot be used for bucket alignment.
/// </summary>
/// <param name="Amount">The number of <paramref name="Unit"/> per bucket. Must be greater than zero.</param>
/// <param name="Unit">The fixed-length time unit.</param>
public readonly record struct BucketSize(decimal Amount, TimeUnit Unit);
