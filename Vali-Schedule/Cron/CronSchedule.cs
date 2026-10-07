using Vali_Holiday.Core;
using Vali_Range.Models;
using Vali_Schedule.Core;
using Vali_Schedule.Enums;
using Vali_Time.Enums;

namespace Vali_Schedule.Cron;

/// <summary>
/// An <see cref="IValiSchedule"/> backed by a parsed 5-field cron expression
/// (minute, hour, day-of-month, month, day-of-week). Produced via
/// <see cref="Parse"/> or <see cref="TryParse"/> — never constructed directly.
/// </summary>
/// <remarks>
/// Cron fires at minute granularity and can produce more than one occurrence per day
/// (e.g. <c>"*/15 9-17 * * MON-FRI"</c> fires 33 times on a matching weekday), unlike
/// <see cref="Vali_Schedule.Core.ValiSchedule"/>, which emits at most one occurrence per day at a
/// single configured time of day. For this reason <see cref="CronSchedule"/> is a separate,
/// independent implementation of <see cref="IValiSchedule"/> rather than a configuration of
/// <see cref="Vali_Schedule.Core.ValiSchedule"/>.
/// <para>
/// <b>DST / wall-clock caveat:</b> <see cref="CronSchedule"/> operates purely on <see cref="DateTime"/>
/// wall-clock values and has no timezone awareness. On a "spring forward" transition, an hour
/// configured in the cron expression may not exist that day; on "fall back", it may be matched
/// twice if evaluated against distinct <see cref="DateTimeKind"/>-ambiguous instants. Convert to
/// UTC (or a fixed zone) before querying if DST-correct evaluation is required.
/// </para>
/// <b>Thread safety:</b> Not thread-safe for the mutating builder calls
/// (<see cref="StartingFrom"/>, <see cref="EndsAfter"/>, <see cref="EndsOn"/>,
/// <see cref="WithCustomPredicate"/>); create one instance per schedule definition.
/// </remarks>
public sealed partial class CronSchedule : IValiSchedule
{
    private const int MaxScanDays = 1825; // 5-year lookahead/lookback window; see ValiSchedule for the same constant, kept independent per the parallel-engine design.

    private readonly CronExpression _expression;
    private readonly List<Func<DateTime, bool>> _extraPredicates = [];

    private DateTime? _startDate;
    private int? _endsAfter;
    private DateTime? _endsOn;
    private bool _skipWeekends;
    private IHolidayProvider? _holidayProvider;

    /// <summary>The original cron expression this schedule was parsed from.</summary>
    public string Expression { get; }

    private CronSchedule(string expression, CronExpression parsed)
    {
        Expression = expression;
        _expression = parsed;
    }

    private DateTime EffectiveStartDate => _startDate ?? DateTime.Today;

    /// <summary>
    /// Parses a 5-field cron expression into a ready-to-query <see cref="CronSchedule"/>.
    /// </summary>
    /// <param name="cronExpression">
    /// A whitespace-separated 5-field cron expression: minute, hour, day-of-month, month,
    /// day-of-week.
    /// </param>
    /// <returns>A parsed, immutable <see cref="CronSchedule"/>.</returns>
    /// <exception cref="CronFormatException">
    /// Thrown when <paramref name="cronExpression"/> does not have exactly 5 fields, or any
    /// field contains a token outside that field's supported syntax or value range.
    /// </exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="cronExpression"/> is null or whitespace.</exception>
    public static CronSchedule Parse(string cronExpression)
    {
        if (string.IsNullOrWhiteSpace(cronExpression))
            throw new ArgumentNullException(nameof(cronExpression));

        var parsed = CronExpression.Parse(cronExpression.Trim());
        return new CronSchedule(cronExpression, parsed);
    }

    /// <summary>
    /// Attempts to parse a 5-field cron expression, without throwing on invalid input.
    /// </summary>
    /// <param name="cronExpression">The cron expression to parse.</param>
    /// <param name="schedule">The parsed schedule, or <c>null</c> if parsing failed.</param>
    /// <param name="error">
    /// A human-readable description of the first parse failure encountered, or <c>null</c> on success.
    /// </param>
    /// <returns><c>true</c> if parsing succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryParse(string cronExpression, out CronSchedule? schedule, out string? error)
    {
        if (string.IsNullOrWhiteSpace(cronExpression))
        {
            schedule = null;
            error = "Cron expression must not be null or whitespace.";
            return false;
        }

        try
        {
            schedule = Parse(cronExpression);
            error = null;
            return true;
        }
        catch (CronFormatException ex)
        {
            schedule = null;
            error = ex.Message;
            return false;
        }
    }

    /// <inheritdoc/>
    /// <remarks>Sets the inclusive lower bound for generated occurrences. Cron expressions have
    /// no inherent start date; defaults to today, same convention as <c>ValiSchedule</c>.</remarks>
    public IValiSchedule StartingFrom(DateTime date)
    {
        _startDate = date.Date;
        return this;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="occurrences"/> is less than 1.</exception>
    public IValiSchedule EndsAfter(int occurrences)
    {
        if (occurrences < 1)
            throw new ArgumentOutOfRangeException(nameof(occurrences), "Occurrences must be greater than zero.");

        _endsAfter = occurrences;
        return this;
    }

    /// <inheritdoc/>
    public IValiSchedule EndsOn(DateTime date)
    {
        _endsOn = date.Date;
        return this;
    }

    /// <summary>
    /// Adds an additional predicate that must also be satisfied, on top of the parsed cron
    /// expression. Evaluated with logical AND against the cron match — e.g., layer a holiday-skip
    /// filter on top of <c>"0 9 * * MON-FRI"</c> without re-parsing or forking the cron string.
    /// Can be called multiple times; predicates accumulate.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="predicate"/> is null.</exception>
    public IValiSchedule WithCustomPredicate(Func<DateTime, bool> predicate)
    {
        _extraPredicates.Add(predicate ?? throw new ArgumentNullException(nameof(predicate)));
        return this;
    }

    /// <inheritdoc/>
    /// <remarks>No-op for <see cref="CronSchedule"/>: the cron expression is already fully
    /// parsed and validated at <see cref="Parse"/> time. Returns the current instance.</remarks>
    public IValiSchedule Build() => this;

    /// <inheritdoc/>
    /// <remarks>
    /// Excludes Saturday/Sunday occurrences — but, unlike <c>ValiSchedule.SkipWeekends</c>, drops
    /// them rather than shifting them to the next workday. A cron schedule can fire more than once
    /// per day (e.g. <c>"*/15 9-17 * * *"</c>), so "shift this occurrence to the next workday,
    /// same time" would independently relocate every one of that day's occurrences onto the same
    /// following day, colliding with that day's own native occurrences at the same times. Dropping
    /// is the only unambiguous behavior for a multi-fire-per-day schedule.
    /// </remarks>
    public IValiSchedule SkipWeekends()
    {
        _skipWeekends = true;
        return this;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Excludes holiday occurrences — but, like <see cref="SkipWeekends"/>, drops rather than
    /// shifts them; <paramref name="strategy"/> is accepted for interface compatibility but has no
    /// effect beyond <see cref="HolidaySkipStrategy.Skip"/>'s drop semantics, for the same
    /// multi-fire-per-day reason documented on <see cref="SkipWeekends"/>.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="provider"/> is null.</exception>
    public IValiSchedule SkipHolidays(IHolidayProvider provider, HolidaySkipStrategy strategy = HolidaySkipStrategy.NextWorkday)
    {
        _holidayProvider = provider ?? throw new ArgumentNullException(nameof(provider));
        return this;
    }

    /// <exception cref="NotSupportedException">
    /// Always thrown. The recurrence interval is already fully determined by the parsed cron
    /// expression's minute/hour/day/month/day-of-week fields — express the interval in the cron
    /// string instead (e.g., <c>"*/15 * * * *"</c> instead of calling <c>Every</c>).
    /// </exception>
    public IValiSchedule Every(int interval, TimeUnit unit) =>
        throw new NotSupportedException(
            "CronSchedule does not support Every(); express the interval directly in the cron expression (e.g. \"*/15 * * * *\").");

    /// <exception cref="NotSupportedException">
    /// Always thrown. Days of week are already expressed in the cron expression's 5th field.
    /// </exception>
    public IValiSchedule On(params DayOfWeek[] days) =>
        throw new NotSupportedException(
            "CronSchedule does not support On(); days of week are already expressed in the cron expression's day-of-week field.");

    /// <exception cref="NotSupportedException">
    /// Always thrown. Time of day is already expressed in the cron expression's minute/hour fields
    /// — unlike <c>ValiSchedule</c>, a cron schedule can fire at more than one time of day.
    /// </exception>
    public IValiSchedule At(TimeOnly time) =>
        throw new NotSupportedException(
            "CronSchedule does not support At(); time of day is already expressed in the cron expression's minute/hour fields, and a cron schedule can fire more than once per day.");

    /// <exception cref="NotSupportedException">
    /// Always thrown. Day of month is already expressed in the cron expression's 3rd field.
    /// </exception>
    public IValiSchedule OnDayOfMonth(int day) =>
        throw new NotSupportedException(
            "CronSchedule does not support OnDayOfMonth(); day of month is already expressed in the cron expression's day-of-month field.");

    /// <summary>
    /// Returns the next occurrence on or after the given reference date, bounded by
    /// <see cref="StartingFrom"/>, <see cref="EndsAfter"/>, <see cref="EndsOn"/>, and any
    /// <see cref="WithCustomPredicate"/> filters. Scans forward up to 5 years.
    /// See the type-level DST caveat regarding wall-clock evaluation.
    /// </summary>
    public DateTime? NextOccurrence(DateTime reference)
    {
        var effectiveStart = EffectiveStartDate;
        var searchStart = reference < effectiveStart ? effectiveStart : reference;
        int preScanCount = _endsAfter.HasValue ? CountForward(effectiveStart, searchStart) : 0;
        int yielded = 0;

        foreach (var candidate in ForwardMatches(searchStart))
        {
            if (_endsOn.HasValue && candidate.Date > _endsOn.Value.Date) return null;

            yielded++;
            if (_endsAfter.HasValue && preScanCount + yielded > _endsAfter.Value) return null;

            return candidate;
        }

        return null;
    }

    /// <summary>
    /// Returns the most recent occurrence strictly before the given reference date, bounded by
    /// <see cref="StartingFrom"/>, <see cref="EndsAfter"/>, <see cref="EndsOn"/>, and any
    /// <see cref="WithCustomPredicate"/> filters. Scans backward up to 5 years.
    /// </summary>
    /// <remarks>
    /// ponytail: re-validates the <see cref="EndsAfter"/> budget per candidate via a fresh forward
    /// count, which is O(n) per candidate instead of an incremental counter — fine at the bounded
    /// 5-year/minute-granularity scale this method already caps itself to; revisit with an
    /// incremental counter if profiling ever shows this method as a hot path.
    /// </remarks>
    public DateTime? PreviousOccurrence(DateTime reference)
    {
        var effectiveStart = EffectiveStartDate;

        foreach (var candidate in BackwardMatches(reference))
        {
            if (_endsOn.HasValue && candidate.Date > _endsOn.Value.Date) continue;

            if (_endsAfter.HasValue)
            {
                int countUpToCandidateInclusive = CountForward(effectiveStart, candidate.AddTicks(1));
                if (countUpToCandidateInclusive > _endsAfter.Value) continue;
            }

            return candidate;
        }

        return null;
    }

    /// <summary>
    /// Determines whether the schedule produces an occurrence on the given date, regardless of
    /// what minute/hour it fires at. Only the calendar fields (month, day-of-month, day-of-week)
    /// are evaluated — equivalent to <c>ValiSchedule.OccursOn</c>'s "regardless of time of day"
    /// contract, adapted to cron: a cron schedule's minute/hour masks are always non-empty by
    /// construction, so the calendar check alone determines whether any minute/hour pair matches.
    /// </summary>
    public bool OccursOn(DateTime date)
    {
        var day = date.Date;
        if (day < EffectiveStartDate.Date) return false;
        if (_endsOn.HasValue && day > _endsOn.Value.Date) return false;
        if (!_expression.MatchesDate(day)) return false;
        if (IsSkipExcluded(day)) return false;

        foreach (var predicate in _extraPredicates)
            if (!predicate(day)) return false;

        return true;
    }

    /// <summary>
    /// Determines whether <paramref name="date"/> is excluded by <see cref="SkipWeekends"/>
    /// and/or <see cref="SkipHolidays"/> — dropped, not shifted; see the remarks on those methods.
    /// </summary>
    private bool IsSkipExcluded(DateTime date) =>
        (_skipWeekends && (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday))
        || (_holidayProvider != null && _holidayProvider.IsHoliday(date));

    /// <summary>
    /// Enumerates up to <paramref name="limit"/> occurrences starting on or after the given
    /// reference date-time, respecting <see cref="StartingFrom"/>, <see cref="EndsAfter"/>,
    /// <see cref="EndsOn"/>, and any <see cref="WithCustomPredicate"/> filters.
    /// </summary>
    public IEnumerable<DateTime> Occurrences(DateTime reference, int limit = 10)
    {
        var effectiveStart = EffectiveStartDate;
        var searchStart = reference < effectiveStart ? effectiveStart : reference;
        int preScanCount = _endsAfter.HasValue ? CountForward(effectiveStart, searchStart) : 0;
        int yielded = 0;
        int emitted = 0;

        foreach (var candidate in ForwardMatches(searchStart))
        {
            if (_endsOn.HasValue && candidate.Date > _endsOn.Value.Date) yield break;

            yielded++;
            if (_endsAfter.HasValue && preScanCount + yielded > _endsAfter.Value) yield break;

            yield return candidate;
            emitted++;
            if (emitted >= limit) yield break;
        }
    }

    /// <summary>
    /// Enumerates all occurrences within the inclusive date range
    /// [<paramref name="from"/>, <paramref name="to"/>] (compared by date — the time-of-day
    /// components of <paramref name="from"/>/<paramref name="to"/> narrow the first/last day but
    /// every matching minute within the range's last day is still included).
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="from"/> is later than <paramref name="to"/>.</exception>
    public IEnumerable<DateTime> OccurrencesInRange(DateTime from, DateTime to)
    {
        if (from.Date > to.Date)
            throw new ArgumentException(
                $"'from' ({from:yyyy-MM-dd}) must be less than or equal to 'to' ({to:yyyy-MM-dd}).",
                nameof(from));

        var effectiveStart = EffectiveStartDate;
        var searchStart = from < effectiveStart ? effectiveStart : from;
        int preScanCount = _endsAfter.HasValue ? CountForward(effectiveStart, searchStart) : 0;
        int yielded = 0;

        foreach (var candidate in ForwardMatches(searchStart))
        {
            if (candidate.Date > to.Date) yield break;
            if (_endsOn.HasValue && candidate.Date > _endsOn.Value.Date) yield break;

            yielded++;
            if (_endsAfter.HasValue && preScanCount + yielded > _endsAfter.Value) yield break;

            yield return candidate;
        }
    }

    /// <summary>
    /// Pairs each occurrence in [<paramref name="from"/>, <paramref name="to"/>] with the
    /// schedule's next real occurrence to produce a sequence of closed periods, mirroring
    /// <c>ValiSchedule.OccurrencePeriods</c>'s contract: periods are never truncated at
    /// <paramref name="to"/>, and the last occurrence produces no period if the schedule ended.
    /// </summary>
    public IEnumerable<DateRange> OccurrencePeriods(DateTime from, DateTime to)
    {
        if (from.Date > to.Date)
            throw new ArgumentException(
                $"'from' ({from:yyyy-MM-dd}) must be less than or equal to 'to' ({to:yyyy-MM-dd}).",
                nameof(from));

        DateTime? previous = null;

        foreach (var occurrence in OccurrencesInRange(from, to))
        {
            if (previous.HasValue)
                yield return new DateRange(previous.Value, occurrence.AddTicks(-1));

            previous = occurrence;
        }

        if (previous.HasValue)
        {
            var next = NextOccurrence(previous.Value.AddTicks(1));
            if (next.HasValue)
                yield return new DateRange(previous.Value, next.Value.AddTicks(-1));
        }
    }

}
