namespace Vali_Schedule.Cron;

/// <summary>
/// Groups the fixed, per-call context (expression, field identity, numeric bounds) threaded
/// through every parsing step for one field, so helper methods take one parameter instead of four.
/// </summary>
internal readonly record struct CronFieldContext(string Expression, string FieldName, int Min, int Max, CronFieldKind Kind);

/// <summary>
/// Tokenizes and parses a single cron field string (per <see cref="CronFieldKind"/>) into a
/// <see cref="CronFieldSpec"/>, per the grammar: <c>field := component ("," component)*</c>,
/// where each component is <c>*</c>, <c>*/step</c>, a value, a range, or a stepped range/value.
/// </summary>
internal static class CronFieldParser
{
    private static readonly Dictionary<string, int> MonthAliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["JAN"] = 1, ["FEB"] = 2, ["MAR"] = 3, ["APR"] = 4, ["MAY"] = 5, ["JUN"] = 6,
        ["JUL"] = 7, ["AUG"] = 8, ["SEP"] = 9, ["OCT"] = 10, ["NOV"] = 11, ["DEC"] = 12
    };

    private static readonly Dictionary<string, int> DayOfWeekAliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["SUN"] = 0, ["MON"] = 1, ["TUE"] = 2, ["WED"] = 3, ["THU"] = 4, ["FRI"] = 5, ["SAT"] = 6
    };

    /// <summary>Parses one whitespace-trimmed field string into a <see cref="CronFieldSpec"/>.</summary>
    /// <param name="expression">The full cron expression, used only to build error messages.</param>
    /// <param name="fieldText">The raw text of this one field.</param>
    /// <param name="kind">Which of the 5 cron fields this text belongs to.</param>
    /// <exception cref="CronFormatException">Thrown when <paramref name="fieldText"/> is malformed for <paramref name="kind"/>.</exception>
    public static CronFieldSpec Parse(string expression, string fieldText, CronFieldKind kind)
    {
        var (min, max) = Bounds(kind);
        var ctx = new CronFieldContext(expression, FieldName(kind), min, max, kind);
        bool isWildcard = fieldText == "*";
        var mask = new bool[max - min + 1];

        foreach (var component in fieldText.Split(','))
        {
            foreach (var value in ParseComponent(ctx, component))
            {
                int folded = kind == CronFieldKind.DayOfWeek && value == 7 ? 0 : value;
                mask[folded - min] = true;
            }
        }

        return new CronFieldSpec(min, max, isWildcard, mask);
    }

    private static (int Min, int Max) Bounds(CronFieldKind kind) => kind switch
    {
        CronFieldKind.Minute => (0, 59),
        CronFieldKind.Hour => (0, 23),
        CronFieldKind.DayOfMonth => (1, 31),
        CronFieldKind.Month => (1, 12),
        CronFieldKind.DayOfWeek => (0, 6),
        _ => throw new ArgumentOutOfRangeException(nameof(kind))
    };

    private static string FieldName(CronFieldKind kind) => kind switch
    {
        CronFieldKind.Minute => "minute",
        CronFieldKind.Hour => "hour",
        CronFieldKind.DayOfMonth => "day-of-month",
        CronFieldKind.Month => "month",
        CronFieldKind.DayOfWeek => "day-of-week",
        _ => throw new ArgumentOutOfRangeException(nameof(kind))
    };

    private static IEnumerable<int> ParseComponent(CronFieldContext ctx, string component)
    {
        if (component.Length == 0)
            throw Fallback(ctx, component);

        var slashParts = component.Split('/');
        if (slashParts.Length > 2)
            throw Fallback(ctx, component);

        string basePart = slashParts[0];
        string? stepPart = slashParts.Length == 2 ? slashParts[1] : null;
        int step = ParseStep(ctx, component, stepPart);

        var (lower, upper) = ParseBaseRange(ctx, basePart, hasStep: stepPart != null);

        for (int v = lower; v <= upper; v += step)
            yield return v;
    }

    /// <summary>Parses the base (pre-<c>/step</c>) portion of a component into an inclusive [lower, upper] range.</summary>
    private static (int Lower, int Upper) ParseBaseRange(CronFieldContext ctx, string basePart, bool hasStep)
    {
        if (basePart == "*")
            return (ctx.Min, ctx.Max);

        if (basePart.Contains('-'))
            return ParseExplicitRange(ctx, basePart);

        int n = ResolveValue(ctx, basePart, basePart);
        // A bare "N/step" steps from N through the field's maximum; a bare "N" with no step
        // is just the single value N.
        return (n, hasStep ? ctx.Max : n);
    }

    private static (int Lower, int Upper) ParseExplicitRange(CronFieldContext ctx, string basePart)
    {
        var rangeParts = basePart.Split('-');
        if (rangeParts.Length != 2)
            throw Fallback(ctx, basePart);

        int n = ResolveValue(ctx, basePart, rangeParts[0]);
        int m = ResolveValue(ctx, basePart, rangeParts[1]);
        if (n > m)
            throw new CronFormatException(ctx.Expression, ctx.FieldName, basePart,
                $"Cron field '{ctx.FieldName}' has a descending range '{basePart}' ({n}-{m}); the start must be less than or equal to the end. Expression: '{ctx.Expression}'.");

        return (n, m);
    }

    private static int ParseStep(CronFieldContext ctx, string component, string? stepPart)
    {
        if (stepPart == null) return 1;

        if (!int.TryParse(stepPart, out int step) || step <= 0)
            throw new CronFormatException(ctx.Expression, ctx.FieldName, component,
                $"Cron field '{ctx.FieldName}' has step '{stepPart}' in '{component}', but step must be a positive integer greater than zero. Expression: '{ctx.Expression}'.");

        return step;
    }

    /// <summary>
    /// Resolves one atomic value token (a plain number, or a month/day-of-week name alias) to its
    /// integer value, validating it against <see cref="CronFieldContext.Min"/>/<see cref="CronFieldContext.Max"/>.
    /// Day-of-week additionally accepts <c>7</c> as an alias for Sunday (<c>0</c>), per standard
    /// cron, folded by the caller when building the mask.
    /// </summary>
    /// <param name="errorToken">The token reported on failure — the enclosing range/component text
    /// when <paramref name="valueText"/> is one side of a range, or the value itself otherwise.</param>
    private static int ResolveValue(CronFieldContext ctx, string errorToken, string valueText)
    {
        bool supportsAlias = ctx.Kind == CronFieldKind.Month || ctx.Kind == CronFieldKind.DayOfWeek;

        if (supportsAlias && TryResolveAlias(valueText, ctx.Kind, out int aliasValue))
            return ValidateRange(ctx, errorToken, aliasValue);

        if (int.TryParse(valueText, out int numeric))
            return ValidateRange(ctx, errorToken, numeric);

        if (supportsAlias && IsAlphabetic(valueText))
            throw new CronFormatException(ctx.Expression, ctx.FieldName, errorToken,
                $"Cron field '{ctx.FieldName}' contains unrecognized name '{valueText}' in range '{errorToken}'. Valid names: {AliasNamesList(ctx.Kind)}. Expression: '{ctx.Expression}'.");

        throw Fallback(ctx, errorToken);
    }

    private static int ValidateRange(CronFieldContext ctx, string errorToken, int value)
    {
        // Day-of-week accepts the extra value 7 as an alias for Sunday (0).
        int effectiveMax = ctx.Kind == CronFieldKind.DayOfWeek ? 7 : ctx.Max;
        if (value < ctx.Min || value > effectiveMax)
            throw new CronFormatException(ctx.Expression, ctx.FieldName, errorToken,
                $"Cron field '{ctx.FieldName}' has value {value}, which is outside the allowed range {ctx.Min}-{ctx.Max}. Expression: '{ctx.Expression}'.");
        return value;
    }

    private static bool TryResolveAlias(string valueText, CronFieldKind kind, out int value)
    {
        var table = kind == CronFieldKind.Month ? MonthAliases : DayOfWeekAliases;
        return table.TryGetValue(valueText, out value);
    }

    private static string AliasNamesList(CronFieldKind kind) =>
        kind == CronFieldKind.Month
            ? "JAN, FEB, MAR, APR, MAY, JUN, JUL, AUG, SEP, OCT, NOV, DEC"
            : "SUN, MON, TUE, WED, THU, FRI, SAT";

    private static bool IsAlphabetic(string text) => text.Length > 0 && text.All(char.IsLetter);

    private static CronFormatException Fallback(CronFieldContext ctx, string token) =>
        new(ctx.Expression, ctx.FieldName, token,
            $"Cron field '{ctx.FieldName}' contains '{token}', which is not '*', a number, a list, a range, or a step expression. Expression: '{ctx.Expression}'.");
}
