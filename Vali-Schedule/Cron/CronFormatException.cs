namespace Vali_Schedule.Cron;

/// <summary>
/// Thrown when a cron expression fails to parse. Always identifies which of the 5 fields
/// failed, the offending token within that field, and why — never a generic "invalid cron
/// expression" message.
/// </summary>
public sealed class CronFormatException : FormatException
{
    /// <summary>The full cron expression that failed to parse.</summary>
    public string Expression { get; }

    /// <summary>
    /// The field that failed: <c>"minute"</c>, <c>"hour"</c>, <c>"day-of-month"</c>,
    /// <c>"month"</c>, <c>"day-of-week"</c>, or <c>"field-count"</c> when the expression does not
    /// have exactly 5 whitespace-separated fields.
    /// </summary>
    public string Field { get; }

    /// <summary>The specific token or sub-expression within <see cref="Field"/> that caused the failure.</summary>
    public string Token { get; }

    /// <summary>
    /// Creates a new <see cref="CronFormatException"/> with an already composed, human-readable
    /// <paramref name="message"/>.
    /// </summary>
    /// <param name="expression">The full cron expression that failed to parse.</param>
    /// <param name="field">The field that failed, or <c>"field-count"</c>.</param>
    /// <param name="token">The specific offending token within <paramref name="field"/>.</param>
    /// <param name="message">A human-readable explanation of the failure.</param>
    public CronFormatException(string expression, string field, string token, string message)
        : base(message)
    {
        Expression = expression;
        Field = field;
        Token = token;
    }
}
