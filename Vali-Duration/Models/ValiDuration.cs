using System.Globalization;
using Vali_Time.Enums;

namespace Vali_Duration.Models;

/// <summary>
/// An immutable value type that represents a duration with full decimal precision.
/// Internally stores the duration as a <see cref="decimal"/> number of seconds,
/// avoiding the floating-point rounding errors inherent in <see cref="TimeSpan"/>.
/// </summary>
/// <remarks>
/// <para>
/// All arithmetic and comparison operators are supported. The struct is implicitly
/// convertible to and from <see cref="TimeSpan"/> for interoperability, though
/// the conversion from <see cref="TimeSpan"/> is limited to the precision of
/// <c>double</c> (100-nanosecond ticks).
/// </para>
/// <para>
/// Month and year conversions use the same average-day constants as <see cref="Vali_Time.Core.ValiTime"/>
/// (365.25 days/year, 30.4375 days/month). For calendar-accurate month arithmetic
/// use <see cref="Vali_Time.Core.ValiDate"/>.
/// </para>
/// </remarks>
public readonly struct ValiDuration : IEquatable<ValiDuration>, IComparable<ValiDuration>
{
    /// <summary>The canonical internal representation: total elapsed seconds as a decimal.</summary>
    private readonly decimal _seconds;

    /// <summary>
    /// Initializes a new <see cref="ValiDuration"/> from a raw seconds value.
    /// </summary>
    /// <param name="seconds">Total duration expressed in seconds.</param>
    private ValiDuration(decimal seconds) => _seconds = seconds;

    // ── Factory methods ──────────────────────────────────────────────────────

    /// <summary>
    /// Creates a <see cref="ValiDuration"/> from a value expressed in milliseconds.
    /// </summary>
    /// <param name="ms">The number of milliseconds.</param>
    /// <returns>A new <see cref="ValiDuration"/> equal to <paramref name="ms"/> milliseconds.</returns>
    public static ValiDuration FromMilliseconds(decimal ms) => new(ms / 1000m);

    /// <summary>
    /// Creates a <see cref="ValiDuration"/> from a value expressed in seconds.
    /// </summary>
    /// <param name="s">The number of seconds.</param>
    /// <returns>A new <see cref="ValiDuration"/> equal to <paramref name="s"/> seconds.</returns>
    public static ValiDuration FromSeconds(decimal s) => new(s);

    /// <summary>
    /// Creates a <see cref="ValiDuration"/> from a value expressed in minutes.
    /// </summary>
    /// <param name="m">The number of minutes.</param>
    /// <returns>A new <see cref="ValiDuration"/> equal to <paramref name="m"/> minutes.</returns>
    public static ValiDuration FromMinutes(decimal m) => new(m * 60m);

    /// <summary>
    /// Creates a <see cref="ValiDuration"/> from a value expressed in hours.
    /// </summary>
    /// <param name="h">The number of hours.</param>
    /// <returns>A new <see cref="ValiDuration"/> equal to <paramref name="h"/> hours.</returns>
    public static ValiDuration FromHours(decimal h) => new(h * 3600m);

    /// <summary>
    /// Creates a <see cref="ValiDuration"/> from a value expressed in days.
    /// </summary>
    /// <param name="d">The number of days.</param>
    /// <returns>A new <see cref="ValiDuration"/> equal to <paramref name="d"/> days.</returns>
    public static ValiDuration FromDays(decimal d) => new(d * 86400m);

    /// <summary>
    /// Creates a <see cref="ValiDuration"/> from a value expressed in weeks.
    /// </summary>
    /// <param name="w">The number of weeks.</param>
    /// <returns>A new <see cref="ValiDuration"/> equal to <paramref name="w"/> weeks.</returns>
    public static ValiDuration FromWeeks(decimal w) => new(w * 604800m);

    /// <summary>Creates a <see cref="ValiDuration"/> from an approximate number of months (1 month ≈ 30.4375 days).</summary>
    public static ValiDuration FromMonths(decimal months) => new(months * 2629800m);

    /// <summary>Creates a <see cref="ValiDuration"/> from an approximate number of years (1 year ≈ 365.25 days).</summary>
    public static ValiDuration FromYears(decimal years) => new(years * 31557600m);

    /// <summary>
    /// Gets a <see cref="ValiDuration"/> representing a zero-length duration.
    /// </summary>
    public static ValiDuration Zero => new(0m);

    // ── Unit conversion ──────────────────────────────────────────────────────

    /// <summary>
    /// Returns the value of this duration expressed in the specified <see cref="TimeUnit"/>.
    /// </summary>
    /// <param name="unit">The target unit for the conversion.</param>
    /// <returns>
    /// The total duration as a <see cref="decimal"/> in <paramref name="unit"/>.
    /// Month and year conversions use averaged constants (30.4375 d/mo, 365.25 d/yr).
    /// </returns>
    /// <exception cref="NotSupportedException">Thrown when <paramref name="unit"/> is not a recognised <see cref="TimeUnit"/>.</exception>
    public decimal As(TimeUnit unit) => unit switch
    {
        TimeUnit.Milliseconds => _seconds * 1000m,
        TimeUnit.Seconds      => _seconds,
        TimeUnit.Minutes      => _seconds / 60m,
        TimeUnit.Hours        => _seconds / 3600m,
        TimeUnit.Days         => _seconds / 86400m,
        TimeUnit.Weeks        => _seconds / 604800m,
        TimeUnit.Months       => _seconds / 2629800m,
        TimeUnit.Years        => _seconds / 31557600m,
        _                     => throw new NotSupportedException($"TimeUnit '{unit}' is not supported.")
    };

    // ── Convenience properties ───────────────────────────────────────────────

    /// <summary>Gets the total duration expressed in milliseconds.</summary>
    public decimal TotalMilliseconds => _seconds * 1000m;

    /// <summary>Gets the total duration expressed in seconds.</summary>
    public decimal TotalSeconds => _seconds;

    /// <summary>Gets the total duration expressed in minutes.</summary>
    public decimal TotalMinutes => _seconds / 60m;

    /// <summary>Gets the total duration expressed in hours.</summary>
    public decimal TotalHours => _seconds / 3600m;

    /// <summary>Gets the total duration expressed in days.</summary>
    public decimal TotalDays => _seconds / 86400m;

    // ── TimeSpan interoperability ────────────────────────────────────────────

    /// <summary>
    /// Converts this <see cref="ValiDuration"/> to a <see cref="TimeSpan"/>.
    /// </summary>
    /// <remarks>
    /// The conversion goes through <see cref="TimeSpan.FromSeconds(double)"/>,
    /// which limits precision to approximately 15–16 significant digits of a
    /// <c>double</c>. For sub-microsecond accuracy, work directly with
    /// <see cref="TotalSeconds"/> or <see cref="As"/>.
    /// </remarks>
    /// <returns>A <see cref="TimeSpan"/> approximating this duration.</returns>
    public TimeSpan ToTimeSpan() => TimeSpan.FromSeconds((double)_seconds);

    /// <summary>
    /// Creates a <see cref="ValiDuration"/> from a <see cref="TimeSpan"/> using tick-level precision.
    /// </summary>
    /// <param name="ts">The <see cref="TimeSpan"/> to convert.</param>
    /// <returns>A <see cref="ValiDuration"/> equal to <paramref name="ts"/>.</returns>
    public static ValiDuration FromTimeSpan(TimeSpan ts)
        => new((decimal)ts.Ticks / TimeSpan.TicksPerSecond);

    // ── Human-readable formatting ────────────────────────────────────────────

    /// <summary>
    /// Formats the duration as a short human-readable string, automatically
    /// choosing the most appropriate unit (weeks, days, hours, minutes, seconds, or milliseconds).
    /// </summary>
    /// <param name="decimalPlaces">
    /// The number of decimal places to include in the formatted value. Defaults to 2.
    /// </param>
    /// <returns>
    /// A string such as <c>"1.50 h"</c>, <c>"45.00 min"</c>, or <c>"500.00 ms"</c>.
    /// </returns>
    public string Format(int decimalPlaces = 2)
    {
        if (decimalPlaces < 0)
            throw new ArgumentOutOfRangeException(nameof(decimalPlaces), "Must be >= 0.");

        if (_seconds < 0m)
        {
            // Format the absolute value, then prepend the minus sign.
            string inner = (-this).Format(decimalPlaces);
            return "-" + inner;
        }

        if (_seconds >= 604800m) return $"{As(TimeUnit.Weeks).ToString($"F{decimalPlaces}", CultureInfo.InvariantCulture)} w";
        if (_seconds >= 86400m)  return $"{As(TimeUnit.Days).ToString($"F{decimalPlaces}", CultureInfo.InvariantCulture)} d";
        if (_seconds >= 3600m)   return $"{As(TimeUnit.Hours).ToString($"F{decimalPlaces}", CultureInfo.InvariantCulture)} h";
        if (_seconds >= 60m)     return $"{As(TimeUnit.Minutes).ToString($"F{decimalPlaces}", CultureInfo.InvariantCulture)} min";
        if (_seconds >= 1m)      return $"{As(TimeUnit.Seconds).ToString($"F{decimalPlaces}", CultureInfo.InvariantCulture)} s";
        return $"{As(TimeUnit.Milliseconds).ToString($"F{decimalPlaces}", CultureInfo.InvariantCulture)} ms";
    }

    // ── Arithmetic operators ─────────────────────────────────────────────────

    /// <summary>Adds two durations together.</summary>
    /// <param name="a">The first duration.</param>
    /// <param name="b">The second duration.</param>
    /// <returns>A new <see cref="ValiDuration"/> equal to the sum of <paramref name="a"/> and <paramref name="b"/>.</returns>
    public static ValiDuration operator +(ValiDuration a, ValiDuration b) => new(a._seconds + b._seconds);

    /// <summary>Subtracts one duration from another.</summary>
    /// <param name="a">The minuend.</param>
    /// <param name="b">The subtrahend.</param>
    /// <returns>A new <see cref="ValiDuration"/> equal to <paramref name="a"/> minus <paramref name="b"/>.</returns>
    public static ValiDuration operator -(ValiDuration a, ValiDuration b) => new(a._seconds - b._seconds);

    /// <summary>Multiplies a duration by a scalar factor.</summary>
    /// <param name="d">The duration to scale.</param>
    /// <param name="factor">The multiplication factor.</param>
    /// <returns>A new <see cref="ValiDuration"/> scaled by <paramref name="factor"/>.</returns>
    public static ValiDuration operator *(ValiDuration d, decimal factor) => new(d._seconds * factor);

    /// <summary>Divides a duration by a scalar divisor.</summary>
    /// <param name="d">The duration to divide.</param>
    /// <param name="divisor">The divisor. Must not be zero.</param>
    /// <returns>A new <see cref="ValiDuration"/> divided by <paramref name="divisor"/>.</returns>
    /// <exception cref="DivideByZeroException">Thrown when <paramref name="divisor"/> is zero.</exception>
    public static ValiDuration operator /(ValiDuration d, decimal divisor) => new(d._seconds / divisor);

    /// <summary>Negates a duration, returning a duration of equal magnitude with the opposite sign.</summary>
    /// <param name="d">The duration to negate.</param>
    /// <returns>A new <see cref="ValiDuration"/> with the negated value of <paramref name="d"/>.</returns>
    public static ValiDuration operator -(ValiDuration d) => new(-d._seconds);

    // ── Comparison operators ─────────────────────────────────────────────────

    /// <summary>Determines whether <paramref name="a"/> is greater than <paramref name="b"/>.</summary>
    public static bool operator >(ValiDuration a, ValiDuration b) => a._seconds > b._seconds;

    /// <summary>Determines whether <paramref name="a"/> is less than <paramref name="b"/>.</summary>
    public static bool operator <(ValiDuration a, ValiDuration b) => a._seconds < b._seconds;

    /// <summary>Determines whether <paramref name="a"/> is greater than or equal to <paramref name="b"/>.</summary>
    public static bool operator >=(ValiDuration a, ValiDuration b) => a._seconds >= b._seconds;

    /// <summary>Determines whether <paramref name="a"/> is less than or equal to <paramref name="b"/>.</summary>
    public static bool operator <=(ValiDuration a, ValiDuration b) => a._seconds <= b._seconds;

    /// <summary>Determines whether two <see cref="ValiDuration"/> values are equal.</summary>
    public static bool operator ==(ValiDuration a, ValiDuration b) => a._seconds == b._seconds;

    /// <summary>Determines whether two <see cref="ValiDuration"/> values are not equal.</summary>
    public static bool operator !=(ValiDuration a, ValiDuration b) => a._seconds != b._seconds;

    // ── Implicit conversions ─────────────────────────────────────────────────

    /// <summary>
    /// Implicitly converts a <see cref="TimeSpan"/> to a <see cref="ValiDuration"/>
    /// using tick-level precision.
    /// </summary>
    /// <param name="ts">The <see cref="TimeSpan"/> to convert.</param>
    public static implicit operator ValiDuration(TimeSpan ts) => FromTimeSpan(ts);

    /// <summary>
    /// Implicitly converts a <see cref="ValiDuration"/> to a <see cref="TimeSpan"/>.
    /// Precision is limited to the <c>double</c> range of <see cref="TimeSpan.FromSeconds"/>.
    /// </summary>
    /// <param name="d">The <see cref="ValiDuration"/> to convert.</param>
    public static implicit operator TimeSpan(ValiDuration d) => d.ToTimeSpan();

    // ── IEquatable<ValiDuration> ─────────────────────────────────────────────

    /// <summary>
    /// Indicates whether this instance is equal to another <see cref="ValiDuration"/>.
    /// Two durations are equal when their internal second values are exactly equal.
    /// </summary>
    /// <param name="other">The duration to compare with this instance.</param>
    /// <returns><c>true</c> if both durations represent the same length of time.</returns>
    public bool Equals(ValiDuration other) => _seconds == other._seconds;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ValiDuration d && Equals(d);

    /// <inheritdoc/>
    public override int GetHashCode() => _seconds.GetHashCode();

    // ── IComparable<ValiDuration> ────────────────────────────────────────────

    /// <summary>
    /// Compares this instance with another <see cref="ValiDuration"/> and returns an integer
    /// that indicates their relative order.
    /// </summary>
    /// <param name="other">The duration to compare with this instance.</param>
    /// <returns>
    /// A negative integer if this instance is shorter than <paramref name="other"/>,
    /// zero if they are equal, or a positive integer if this instance is longer.
    /// </returns>
    public int CompareTo(ValiDuration other) => _seconds.CompareTo(other._seconds);

    // ── Object overrides ─────────────────────────────────────────────────────

    /// <summary>
    /// Returns a human-readable string representation of the duration using
    /// <see cref="Format(int)"/> with 2 decimal places.
    /// </summary>
    /// <returns>A formatted duration string (e.g., <c>"1.50 h"</c>).</returns>
    public override string ToString() => Format();
}
