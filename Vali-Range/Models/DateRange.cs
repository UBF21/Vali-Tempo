using Vali_Time.Enums;

namespace Vali_Range.Models;

/// <summary>
/// Represents an immutable range of time between two <see cref="DateTime"/> values.
/// The range is inclusive on both ends and provides utilities to query duration and containment.
/// </summary>
public readonly struct DateRange
{
    /// <summary>
    /// Gets the start boundary of the range (inclusive).
    /// </summary>
    public DateTime Start { get; }

    /// <summary>
    /// Gets the end boundary of the range (inclusive).
    /// </summary>
    public DateTime End { get; }

    /// <summary>
    /// Gets a value indicating whether the range is valid, i.e. <see cref="Start"/> is less than or equal to <see cref="End"/>.
    /// </summary>
    public bool IsValid => Start <= End;

    /// <summary>
    /// Initializes a new instance of <see cref="DateRange"/> with the specified start and end dates.
    /// </summary>
    /// <param name="start">The start boundary of the range.</param>
    /// <param name="end">The end boundary of the range.</param>
    public DateRange(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }

    /// <summary>
    /// Calculates the duration of the range expressed in the specified <see cref="TimeUnit"/>.
    /// Uses <see cref="TimeSpan.Ticks"/> internally for maximum precision.
    /// </summary>
    /// <param name="unit">The unit of time in which to express the duration.</param>
    /// <returns>
    /// A <see cref="decimal"/> representing the length of the range in the given unit.
    /// Returns <c>0</c> for an invalid range where <see cref="Start"/> is greater than <see cref="End"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="unit"/> is not a recognised <see cref="TimeUnit"/> value.</exception>
    public decimal Duration(TimeUnit unit)
    {
        long ticks = End >= Start ? End.Ticks - Start.Ticks : 0L;

        return unit switch
        {
            TimeUnit.Milliseconds => (decimal)ticks / TimeSpan.TicksPerMillisecond,
            TimeUnit.Seconds      => (decimal)ticks / TimeSpan.TicksPerSecond,
            TimeUnit.Minutes      => (decimal)ticks / TimeSpan.TicksPerMinute,
            TimeUnit.Hours        => (decimal)ticks / TimeSpan.TicksPerHour,
            TimeUnit.Days         => (decimal)ticks / TimeSpan.TicksPerDay,
            TimeUnit.Weeks        => (decimal)ticks / (TimeSpan.TicksPerDay * 7),
            TimeUnit.Months       => (decimal)ticks / (TimeSpan.TicksPerDay * 30.4375m),
            TimeUnit.Years        => (decimal)ticks / (TimeSpan.TicksPerDay * 365.25m),
            _                     => throw new ArgumentOutOfRangeException(nameof(unit), unit, "Unrecognised TimeUnit value.")
        };
    }

    /// <summary>
    /// Determines whether the specified <paramref name="date"/> falls within the range,
    /// using inclusive comparison on both the <see cref="Start"/> and <see cref="End"/> boundaries.
    /// </summary>
    /// <param name="date">The date to test.</param>
    /// <returns><c>true</c> if <paramref name="date"/> is within [<see cref="Start"/>, <see cref="End"/>]; otherwise <c>false</c>.</returns>
    public bool Contains(DateTime date) => date >= Start && date <= End;

    /// <summary>
    /// Returns a human-readable representation of the range in the format
    /// <c>yyyy-MM-dd → yyyy-MM-dd</c>.
    /// </summary>
    /// <returns>A string such as <c>"2025-01-01 → 2025-03-31"</c>.</returns>
    public override string ToString() =>
        $"{Start:yyyy-MM-dd} \u2192 {End:yyyy-MM-dd}";
}
