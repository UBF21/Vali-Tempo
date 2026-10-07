namespace Vali_Schedule.Cron;

/// <summary>
/// Holds the parsed result of one cron field: the inclusive numeric range for its kind, whether
/// the raw field string was exactly <c>"*"</c>, and a presence mask over every value in that range.
/// </summary>
internal sealed class CronFieldSpec
{
    /// <summary>The minimum valid value for this field's kind.</summary>
    public int Min { get; }

    /// <summary>The maximum valid value for this field's kind.</summary>
    public int Max { get; }

    /// <summary>
    /// <c>true</c> only when the raw field string was literally <c>"*"</c> — a list that happens
    /// to cover the whole range (e.g. <c>0-6</c>) still counts as "restricted" for the
    /// day-of-month/day-of-week OR-combination rule.
    /// </summary>
    public bool IsWildcard { get; }

    /// <summary>
    /// Presence mask indexed by <c>value - Min</c>: <c>Mask[value - Min]</c> is <c>true</c> when
    /// <c>value</c> is matched by this field.
    /// </summary>
    public bool[] Mask { get; }

    public CronFieldSpec(int min, int max, bool isWildcard, bool[] mask)
    {
        Min = min;
        Max = max;
        IsWildcard = isWildcard;
        Mask = mask;
    }

    /// <summary>Determines whether <paramref name="value"/> is matched by this field.</summary>
    public bool Matches(int value) => Mask[value - Min];
}
