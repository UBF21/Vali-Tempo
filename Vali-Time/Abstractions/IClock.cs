namespace Vali_Time.Abstractions;

/// <summary>
/// Abstraction over the system clock, enabling deterministic testing of time-dependent code.
/// </summary>
public interface IClock
{
    /// <summary>Gets the current local date and time.</summary>
    DateTime Now { get; }

    /// <summary>Gets the current local date with the time component set to midnight.</summary>
    DateTime Today { get; }
}
