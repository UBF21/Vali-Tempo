namespace Vali_BusinessHours.Models;

/// <summary>
/// The result of resolving the simultaneous business-hours overlap between two
/// <see cref="BusinessHoursWindow"/> instances for a given reference date.
/// </summary>
public readonly struct BusinessHoursOverlapResult
{
    /// <summary>
    /// <see langword="true"/> if the two business-hours windows share any overlapping instant;
    /// otherwise <see langword="false"/>. When <see langword="false"/>, every other member is <c>null</c>.
    /// </summary>
    public bool HasOverlap { get; }

    /// <summary>The start of the overlap window, as an absolute UTC instant.</summary>
    public DateTimeOffset? StartUtc { get; }

    /// <summary>The end of the overlap window, as an absolute UTC instant.</summary>
    public DateTimeOffset? EndUtc { get; }

    /// <summary>The start of the overlap window, projected to the first window's local timezone.</summary>
    public DateTimeOffset? StartInZoneA { get; }

    /// <summary>The end of the overlap window, projected to the first window's local timezone.</summary>
    public DateTimeOffset? EndInZoneA { get; }

    /// <summary>The start of the overlap window, projected to the second window's local timezone.</summary>
    public DateTimeOffset? StartInZoneB { get; }

    /// <summary>The end of the overlap window, projected to the second window's local timezone.</summary>
    public DateTimeOffset? EndInZoneB { get; }

    private BusinessHoursOverlapResult(
        bool hasOverlap,
        DateTimeOffset? startUtc, DateTimeOffset? endUtc,
        DateTimeOffset? startInZoneA, DateTimeOffset? endInZoneA,
        DateTimeOffset? startInZoneB, DateTimeOffset? endInZoneB)
    {
        HasOverlap = hasOverlap;
        StartUtc = startUtc;
        EndUtc = endUtc;
        StartInZoneA = startInZoneA;
        EndInZoneA = endInZoneA;
        StartInZoneB = startInZoneB;
        EndInZoneB = endInZoneB;
    }

    /// <summary>Creates a result representing no overlap.</summary>
    public static BusinessHoursOverlapResult None() => new(false, null, null, null, null, null, null);

    /// <summary>Creates a result representing a real overlap window.</summary>
    /// <param name="utc">The overlap window as an absolute UTC instant (Start, End).</param>
    /// <param name="inZoneA">The overlap window projected to the first window's local timezone (Start, End).</param>
    /// <param name="inZoneB">The overlap window projected to the second window's local timezone (Start, End).</param>
    public static BusinessHoursOverlapResult Found(
        (DateTimeOffset Start, DateTimeOffset End) utc,
        (DateTimeOffset Start, DateTimeOffset End) inZoneA,
        (DateTimeOffset Start, DateTimeOffset End) inZoneB) =>
        new(true, utc.Start, utc.End, inZoneA.Start, inZoneA.End, inZoneB.Start, inZoneB.End);
}
