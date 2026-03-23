namespace Vali_Time.Abstractions;

/// <summary>
/// Default production implementation of <see cref="IClock"/> that delegates to
/// <see cref="DateTime.Now"/> and <see cref="DateTime.Today"/>.
/// </summary>
public sealed class SystemClock : IClock
{
    /// <summary>Shared singleton instance for production use.</summary>
    public static readonly IClock Instance = new SystemClock();

    public SystemClock() { }

    /// <inheritdoc/>
    public DateTime Now => DateTime.Now;

    /// <inheritdoc/>
    public DateTime Today => DateTime.Today;
}
