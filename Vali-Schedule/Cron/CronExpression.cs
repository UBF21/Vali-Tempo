namespace Vali_Schedule.Cron;

/// <summary>
/// Holds the 5 parsed <see cref="CronFieldSpec"/>s of a cron expression and determines whether a
/// given <see cref="DateTime"/> matches them, including the standard day-of-month/day-of-week
/// OR-combination rule.
/// </summary>
internal sealed class CronExpression
{
    private readonly CronFieldSpec _minute;
    private readonly CronFieldSpec _hour;
    private readonly CronFieldSpec _dayOfMonth;
    private readonly CronFieldSpec _month;
    private readonly CronFieldSpec _dayOfWeek;

    private CronExpression(CronFieldSpec minute, CronFieldSpec hour, CronFieldSpec dayOfMonth, CronFieldSpec month, CronFieldSpec dayOfWeek)
    {
        _minute = minute;
        _hour = hour;
        _dayOfMonth = dayOfMonth;
        _month = month;
        _dayOfWeek = dayOfWeek;
    }

    /// <summary>The parsed minute field, exposed for the day-then-minute occurrence scan.</summary>
    public CronFieldSpec Minute => _minute;

    /// <summary>The parsed hour field, exposed for the day-then-minute occurrence scan.</summary>
    public CronFieldSpec Hour => _hour;

    /// <summary>
    /// Parses a whitespace-separated 5-field cron expression into a <see cref="CronExpression"/>.
    /// Fields are parsed in order (minute → hour → day-of-month → month → day-of-week); parsing
    /// stops at the first field that fails, so exactly one <see cref="CronFormatException"/> is
    /// thrown per call.
    /// </summary>
    /// <exception cref="CronFormatException">
    /// Thrown when <paramref name="expression"/> does not have exactly 5 whitespace-separated
    /// fields, or when any field contains a token outside that field's supported syntax or range.
    /// </exception>
    public static CronExpression Parse(string expression)
    {
        var fields = expression.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        if (fields.Length != 5)
            throw new CronFormatException(expression, "field-count", expression,
                $"Cron expression '{expression}' has {fields.Length} fields; exactly 5 are required (minute hour day-of-month month day-of-week).");

        var minute = CronFieldParser.Parse(expression, fields[0], CronFieldKind.Minute);
        var hour = CronFieldParser.Parse(expression, fields[1], CronFieldKind.Hour);
        var dayOfMonth = CronFieldParser.Parse(expression, fields[2], CronFieldKind.DayOfMonth);
        var month = CronFieldParser.Parse(expression, fields[3], CronFieldKind.Month);
        var dayOfWeek = CronFieldParser.Parse(expression, fields[4], CronFieldKind.DayOfWeek);

        return new CronExpression(minute, hour, dayOfMonth, month, dayOfWeek);
    }

    /// <summary>
    /// Determines whether <paramref name="candidate"/> matches this cron expression, applying the
    /// standard day-of-month/day-of-week OR-combination rule: when both fields are restricted
    /// (neither is <c>*</c>), a date matches when either matches; otherwise only the restricted
    /// field (if any) is required.
    /// </summary>
    public bool Matches(DateTime candidate)
    {
        if (!_month.Matches(candidate.Month)) return false;
        if (!MatchesDay(candidate)) return false;
        return _hour.Matches(candidate.Hour) && _minute.Matches(candidate.Minute);
    }

    /// <summary>Calendar-only match (month + day-of-month/day-of-week), ignoring minute/hour.</summary>
    public bool MatchesDate(DateTime candidate) =>
        _month.Matches(candidate.Month) && MatchesDay(candidate);

    private bool MatchesDay(DateTime candidate)
    {
        bool domOk = _dayOfMonth.Matches(candidate.Day);
        bool dowOk = _dayOfWeek.Matches((int)candidate.DayOfWeek);

        bool domRestricted = !_dayOfMonth.IsWildcard;
        bool dowRestricted = !_dayOfWeek.IsWildcard;

        return domRestricted && dowRestricted ? domOk || dowOk : domOk && dowOk;
    }
}
