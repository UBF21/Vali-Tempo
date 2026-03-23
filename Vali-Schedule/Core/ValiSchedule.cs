using Vali_Schedule.Enums;
using Vali_Schedule.Models;
using Vali_Time.Enums;

namespace Vali_Schedule.Core;

/// <summary>
/// Fluent builder for defining and querying recurring time schedules.
/// Supports daily, weekly, monthly, yearly, and custom recurrence patterns
/// with configurable intervals, days of week, time of day, and end conditions.
/// </summary>
/// <example>
/// <code>
/// var schedule = new ValiSchedule()
///     .Every(1, TimeUnit.Weeks)
///     .On(DayOfWeek.Monday, DayOfWeek.Wednesday)
///     .At(new TimeOnly(9, 0))
///     .StartingFrom(new DateTime(2025, 1, 1));
///
/// DateTime? next = schedule.NextOccurrence(DateTime.Today);
/// </code>
/// </example>
/// <remarks>
/// <b>Thread safety:</b> A single <see cref="ValiSchedule"/> instance is NOT thread-safe.
/// The fluent builder methods mutate internal state. Create a new instance per schedule definition.
/// Do not register as a singleton in a DI container.
/// </remarks>
public class ValiSchedule : IValiSchedule
{
    private const int MaxScanDays = 1825; // 5-year lookahead/lookback window

    private readonly ScheduleConfig _config = new();

    /// <summary>
    /// Returns the effective start date, resolving <see cref="DateTime.MinValue"/> (the sentinel
    /// set when <c>StartingFrom</c> is never called) to today's date at access time.
    /// </summary>
    private DateTime EffectiveStartDate =>
        _config.StartDate == DateTime.MinValue ? DateTime.Today : _config.StartDate;

    // ── Fluent builder ───────────────────────────────────────────────────────

    /// <summary>
    /// Sets the recurrence interval and its time unit, determining the recurrence type.
    /// For example, <c>Every(2, TimeUnit.Weeks)</c> configures a biweekly schedule.
    /// </summary>
    /// <param name="interval">The number of units between occurrences. Must be greater than zero.</param>
    /// <param name="unit">
    /// The time unit that qualifies <paramref name="interval"/>.
    /// Supported values: <see cref="TimeUnit.Days"/>, <see cref="TimeUnit.Weeks"/>,
    /// <see cref="TimeUnit.Months"/>, <see cref="TimeUnit.Years"/>.
    /// </param>
    /// <returns>The current <see cref="ValiSchedule"/> instance for fluent chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="interval"/> is less than 1.</exception>
    public ValiSchedule Every(int interval, TimeUnit unit)
    {
        if (interval < 1)
            throw new ArgumentOutOfRangeException(nameof(interval), "Interval must be greater than zero.");

        _config.Interval = interval;
        _config.Type = unit switch
        {
            TimeUnit.Days   => RecurrenceType.Daily,
            TimeUnit.Weeks  => RecurrenceType.Weekly,
            TimeUnit.Months => RecurrenceType.Monthly,
            TimeUnit.Years  => RecurrenceType.Yearly,
            _               => RecurrenceType.Custom
        };
        return this;
    }

    /// <summary>
    /// Restricts a weekly schedule to the specified days of the week.
    /// Only relevant when the recurrence type is <see cref="RecurrenceType.Weekly"/>.
    /// </summary>
    /// <param name="days">One or more days of the week on which the schedule occurs.</param>
    /// <returns>The current <see cref="ValiSchedule"/> instance for fluent chaining.</returns>
    public ValiSchedule On(params DayOfWeek[] days)
    {
        _config.DaysOfWeek = days.ToList().AsReadOnly();
        return this;
    }

    /// <summary>
    /// Sets the time of day at which each occurrence takes place.
    /// </summary>
    /// <param name="time">The time of day for each occurrence.</param>
    /// <returns>The current <see cref="ValiSchedule"/> instance for fluent chaining.</returns>
    public ValiSchedule At(TimeOnly time)
    {
        _config.TimeOfDay = time;
        return this;
    }

    /// <summary>
    /// Sets the date from which the schedule starts generating occurrences.
    /// No occurrence is produced before this date.
    /// </summary>
    /// <param name="date">The inclusive start date of the schedule.</param>
    /// <returns>The current <see cref="ValiSchedule"/> instance for fluent chaining.</returns>
    public ValiSchedule StartingFrom(DateTime date)
    {
        _config.StartDate = date.Date;
        return this;
    }

    /// <summary>
    /// Configures the schedule to end after a fixed number of occurrences.
    /// </summary>
    /// <param name="occurrences">The maximum number of occurrences to generate. Must be greater than zero.</param>
    /// <returns>The current <see cref="ValiSchedule"/> instance for fluent chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="occurrences"/> is less than 1.</exception>
    public ValiSchedule EndsAfter(int occurrences)
    {
        if (occurrences < 1)
            throw new ArgumentOutOfRangeException(nameof(occurrences), "Occurrences must be greater than zero.");

        _config.EndType = RecurrenceEnd.AfterOccurrences;
        _config.MaxOccurrences = occurrences;
        return this;
    }

    /// <summary>
    /// Configures the schedule to end on a specific date.
    /// No occurrence is produced after this date.
    /// </summary>
    /// <param name="date">The inclusive end date of the schedule.</param>
    /// <returns>The current <see cref="ValiSchedule"/> instance for fluent chaining.</returns>
    public ValiSchedule EndsOn(DateTime date)
    {
        _config.EndType = RecurrenceEnd.OnDate;
        _config.EndDate = date.Date;
        return this;
    }

    /// <summary>
    /// Sets the day of the month on which monthly occurrences fall.
    /// Only relevant when the recurrence type is <see cref="RecurrenceType.Monthly"/>.
    /// </summary>
    /// <param name="day">The day of the month (1–31).</param>
    /// <returns>The current <see cref="ValiSchedule"/> instance for fluent chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="day"/> is not between 1 and 31.</exception>
    public ValiSchedule OnDayOfMonth(int day)
    {
        if (day is < 1 or > 31)
            throw new ArgumentOutOfRangeException(nameof(day), "Day of month must be between 1 and 31.");

        _config.DayOfMonth = day;
        return this;
    }

    /// <summary>
    /// Sets a custom predicate for <see cref="RecurrenceType.Custom"/> recurrence.
    /// The predicate receives a candidate date and returns whether it is a valid occurrence.
    /// </summary>
    /// <param name="predicate">A function that determines whether a given date is a valid occurrence.</param>
    /// <returns>The current <see cref="ValiSchedule"/> instance for fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="predicate"/> is <c>null</c>.</exception>
    public ValiSchedule WithCustomPredicate(Func<DateTime, bool> predicate)
    {
        _config.CustomPredicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        _config.Type = RecurrenceType.Custom;
        return this;
    }

    /// <summary>
    /// Validates the current configuration and returns the schedule, ready for use.
    /// Throws if the configuration is invalid (e.g., weekly recurrence with no days configured).
    /// </summary>
    /// <returns>The current <see cref="ValiSchedule"/> instance.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <see cref="RecurrenceType.Weekly"/> is set but no days of week have been configured.
    /// </exception>
    public ValiSchedule Build()
    {
        if (_config.Type == RecurrenceType.Weekly
            && (_config.DaysOfWeek == null || _config.DaysOfWeek.Count == 0))
            throw new InvalidOperationException(
                "Weekly recurrence requires at least one day of week. Call .On(DayOfWeek) to configure.");
        return this;
    }

    // ── IValiSchedule explicit implementation (delegates to typed methods) ───

    /// <inheritdoc/>
    IValiSchedule IValiSchedule.Every(int interval, TimeUnit unit) => Every(interval, unit);

    /// <inheritdoc/>
    IValiSchedule IValiSchedule.On(params DayOfWeek[] days) => On(days);

    /// <inheritdoc/>
    IValiSchedule IValiSchedule.At(TimeOnly time) => At(time);

    /// <inheritdoc/>
    IValiSchedule IValiSchedule.StartingFrom(DateTime date) => StartingFrom(date);

    /// <inheritdoc/>
    IValiSchedule IValiSchedule.EndsAfter(int occurrences) => EndsAfter(occurrences);

    /// <inheritdoc/>
    IValiSchedule IValiSchedule.EndsOn(DateTime date) => EndsOn(date);

    /// <inheritdoc/>
    IValiSchedule IValiSchedule.OnDayOfMonth(int day) => OnDayOfMonth(day);

    /// <inheritdoc/>
    IValiSchedule IValiSchedule.WithCustomPredicate(Func<DateTime, bool> predicate) => WithCustomPredicate(predicate);

    /// <inheritdoc/>
    IValiSchedule IValiSchedule.Build() => Build();

    // ── Query API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the next occurrence on or after the given reference date.
    /// Scans forward up to 5 years (1825 days) from <paramref name="reference"/>.
    /// </summary>
    /// <param name="reference">The date from which to search forward.</param>
    /// <returns>
    /// The next occurrence as a <see cref="DateTime"/> (including the configured
    /// <see cref="ScheduleConfig.TimeOfDay"/> when set), or <c>null</c> if no
    /// further occurrence exists within the end conditions or the 5-year window.
    /// </returns>
    public DateTime? NextOccurrence(DateTime reference)
    {
        var candidate = reference.Date;
        int count = 0;
        int occurrenceCount = 0;

        // Pre-count occurrences from StartDate up to (but not including) reference
        // so that the AfterOccurrences budget stays accurate when reference > StartDate.
        if (_config.EndType == RecurrenceEnd.AfterOccurrences && _config.MaxOccurrences.HasValue)
        {
            var preScan = EffectiveStartDate.Date;
            while (preScan < reference.Date)
            {
                if (IsValidOccurrence(preScan)) occurrenceCount++;
                preScan = preScan.AddDays(1);
            }
        }

        while (count < MaxScanDays) // max 5 years lookahead
        {
            if (IsValidOccurrence(candidate))
            {
                // Design note: when the end condition is exceeded (end date passed or occurrence
                // budget exhausted), this method returns null rather than skipping to look further.
                // The caller receives no occurrence — the schedule intentionally stops here.
                // Callers should not assume that null means "no occurrence ever"; it means the
                // schedule has ended or the lookahead window was exhausted.
                if (!IsWithinEnd(candidate)) return null;

                occurrenceCount++;
                if (_config.EndType == RecurrenceEnd.AfterOccurrences
                    && _config.MaxOccurrences.HasValue
                    && occurrenceCount > _config.MaxOccurrences.Value)
                    return null;

                return _config.TimeOfDay.HasValue
                    ? candidate.Add(_config.TimeOfDay.Value.ToTimeSpan())
                    : candidate;
            }
            candidate = candidate.AddDays(1);
            count++;
        }
        return null;
    }

    /// <summary>
    /// Returns the most recent occurrence strictly before the given reference date.
    /// Scans backward up to 5 years (1825 days) from <paramref name="reference"/>.
    /// </summary>
    /// <param name="reference">The date from which to search backward.</param>
    /// <returns>
    /// The previous occurrence as a <see cref="DateTime"/> (including the configured
    /// <see cref="ScheduleConfig.TimeOfDay"/> when set), or <c>null</c> if no occurrence
    /// exists before <paramref name="reference"/> within the schedule's start date
    /// or the 5-year window.
    /// </returns>
    public DateTime? PreviousOccurrence(DateTime reference)
    {
        var candidate = reference.Date.AddDays(-1);
        int count = 0;
        int occurrenceCount = 0;

        // Pre-count occurrences from StartDate up to (but not including) reference
        // so that the AfterOccurrences budget stays accurate when reference > StartDate.
        if (_config.EndType == RecurrenceEnd.AfterOccurrences && _config.MaxOccurrences.HasValue)
        {
            var preScan = EffectiveStartDate.Date;
            while (preScan < reference.Date)
            {
                if (IsValidOccurrence(preScan)) occurrenceCount++;
                preScan = preScan.AddDays(1);
            }
            // If the budget is already exhausted before reference, the last valid occurrence
            // must be within the counted range — find it by scanning backward from reference.
            if (occurrenceCount >= _config.MaxOccurrences.Value)
            {
                // Scan backward to find the last occurrence within the budget
                var backScan = reference.Date.AddDays(-1);
                int backCount = 0;
                while (backCount < MaxScanDays)
                {
                    if (backScan < EffectiveStartDate.Date) return null;
                    if (IsValidOccurrence(backScan) && IsWithinEnd(backScan))
                    {
                        return _config.TimeOfDay.HasValue
                            ? backScan.Add(_config.TimeOfDay.Value.ToTimeSpan())
                            : backScan;
                    }
                    backScan = backScan.AddDays(-1);
                    backCount++;
                }
                return null;
            }
        }

        while (count < MaxScanDays) // max 5 years lookback
        {
            if (candidate < EffectiveStartDate.Date) return null;
            if (IsValidOccurrence(candidate) && IsWithinEnd(candidate))
            {
                return _config.TimeOfDay.HasValue
                    ? candidate.Add(_config.TimeOfDay.Value.ToTimeSpan())
                    : candidate;
            }
            candidate = candidate.AddDays(-1);
            count++;
        }
        return null;
    }

    /// <summary>
    /// Determines whether the schedule produces an occurrence on the given date,
    /// regardless of the configured time of day.
    /// </summary>
    /// <param name="date">The date to test.</param>
    /// <returns><c>true</c> if the schedule occurs on <paramref name="date"/>; otherwise, <c>false</c>.</returns>
    public bool OccursOn(DateTime date)
        => IsValidOccurrence(date.Date) && IsWithinEnd(date.Date);

    /// <summary>
    /// Enumerates up to <paramref name="limit"/> occurrences starting on or after the given reference date.
    /// Respects the schedule's end conditions and the 5-year lookahead window.
    /// </summary>
    /// <param name="reference">The date from which to start enumerating occurrences.</param>
    /// <param name="limit">The maximum number of occurrences to return. Defaults to 10.</param>
    /// <returns>A sequence of up to <paramref name="limit"/> occurrence dates.</returns>
    public IEnumerable<DateTime> Occurrences(DateTime reference, int limit = 10)
    {
        var candidate = reference.Date;
        int yielded = 0;
        int count = 0;
        int occurrenceCount = 0;

        // Pre-count occurrences from StartDate up to (but not including) reference
        // so that the AfterOccurrences budget stays accurate when reference > StartDate.
        if (_config.EndType == RecurrenceEnd.AfterOccurrences && _config.MaxOccurrences.HasValue)
        {
            var preScan = EffectiveStartDate.Date;
            while (preScan < reference.Date)
            {
                if (IsValidOccurrence(preScan)) occurrenceCount++;
                preScan = preScan.AddDays(1);
            }
        }

        while (yielded < limit && count < MaxScanDays)
        {
            if (IsValidOccurrence(candidate))
            {
                if (!IsWithinEnd(candidate)) yield break;

                occurrenceCount++;

                if (_config.EndType == RecurrenceEnd.AfterOccurrences
                    && _config.MaxOccurrences.HasValue
                    && occurrenceCount > _config.MaxOccurrences.Value)
                    yield break;

                yield return _config.TimeOfDay.HasValue
                    ? candidate.Add(_config.TimeOfDay.Value.ToTimeSpan())
                    : candidate;

                yielded++;
            }
            candidate = candidate.AddDays(1);
            count++;
        }
    }

    /// <summary>
    /// Enumerates all occurrences within the inclusive date range
    /// [<paramref name="from"/>, <paramref name="to"/>].
    /// The scan is limited to <see cref="MaxScanDays"/> * 10 days to prevent runaway iteration
    /// on unbounded or extremely large ranges.
    /// </summary>
    /// <param name="from">The inclusive start of the date range.</param>
    /// <param name="to">The inclusive end of the date range.</param>
    /// <returns>A sequence of all occurrences within the specified range.</returns>
    public IEnumerable<DateTime> OccurrencesInRange(DateTime from, DateTime to)
    {
        var candidate = from.Date < EffectiveStartDate ? EffectiveStartDate : from.Date;
        int occurrenceCount = 0;
        int scanCount = 0;

        // Pre-count occurrences from StartDate up to (but not including) the range start
        // so that the AfterOccurrences budget stays accurate when from > StartDate.
        if (_config.EndType == RecurrenceEnd.AfterOccurrences && _config.MaxOccurrences.HasValue)
        {
            var preScan = EffectiveStartDate.Date;
            while (preScan < from.Date)
            {
                if (IsValidOccurrence(preScan)) occurrenceCount++;
                preScan = preScan.AddDays(1);
            }
        }

        while (candidate <= to.Date && scanCount < MaxScanDays * 10)
        {
            if (IsValidOccurrence(candidate) && IsWithinEnd(candidate))
            {
                occurrenceCount++;

                if (_config.EndType == RecurrenceEnd.AfterOccurrences
                    && _config.MaxOccurrences.HasValue
                    && occurrenceCount > _config.MaxOccurrences.Value)
                    yield break;

                yield return _config.TimeOfDay.HasValue
                    ? candidate.Add(_config.TimeOfDay.Value.ToTimeSpan())
                    : candidate;
            }
            candidate = candidate.AddDays(1);
            scanCount++;
        }
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    /// <summary>
    /// Determines whether <paramref name="date"/> is a structurally valid occurrence
    /// given the current recurrence configuration, without considering end conditions.
    /// </summary>
    /// <param name="date">The candidate date to evaluate.</param>
    /// <returns><c>true</c> if the date satisfies the recurrence pattern; otherwise, <c>false</c>.</returns>
    private bool IsValidOccurrence(DateTime date)
    {
        var effectiveStart = EffectiveStartDate;
        if (date < effectiveStart.Date) return false;

        return _config.Type switch
        {
            RecurrenceType.Daily =>
                (date - effectiveStart.Date).Days % _config.Interval == 0,

            RecurrenceType.Weekly =>
                (_config.DaysOfWeek == null || _config.DaysOfWeek.Count == 0)
                    ? throw new InvalidOperationException("Weekly recurrence requires at least one day of week. Call .On(DayOfWeek) to configure.")
                    : _config.DaysOfWeek.Contains(date.DayOfWeek)
                      && (int)((date - effectiveStart.Date).Days / 7) % _config.Interval == 0,

            RecurrenceType.Monthly =>
                date.Day == Math.Min(_config.DayOfMonth ?? effectiveStart.Day,
                                     DateTime.DaysInMonth(date.Year, date.Month))
                && MonthDiff(effectiveStart, date) % _config.Interval == 0,

            RecurrenceType.Yearly =>
                IsValidYearlyOccurrence(date, effectiveStart),

            RecurrenceType.Custom => _config.CustomPredicate?.Invoke(date) ?? false,

            _ => false
        };
    }

    /// <summary>
    /// Evaluates yearly recurrence, with a fallback for Feb 29 start dates in non-leap years.
    /// </summary>
    private bool IsValidYearlyOccurrence(DateTime date, DateTime effectiveStart)
    {
        bool yearMatches = (date.Year - effectiveStart.Year) % _config.Interval == 0;
        if (!yearMatches) return false;
        // Feb 29 start: in non-leap years, use Feb 28 as the anniversary
        if (effectiveStart.Month == 2 && effectiveStart.Day == 29 && !DateTime.IsLeapYear(date.Year))
            return date.Month == 2 && date.Day == 28;
        return date.Month == effectiveStart.Month && date.Day == effectiveStart.Day;
    }

    /// <summary>
    /// Determines whether <paramref name="date"/> falls within the configured end condition.
    /// Always returns <c>true</c> when <see cref="RecurrenceEnd.Never"/> or
    /// <see cref="RecurrenceEnd.AfterOccurrences"/> is set (the occurrence budget
    /// is evaluated separately during enumeration).
    /// </summary>
    /// <param name="date">The candidate date to evaluate.</param>
    /// <returns>
    /// <c>false</c> when <see cref="RecurrenceEnd.OnDate"/> is active and <paramref name="date"/>
    /// exceeds <see cref="ScheduleConfig.EndDate"/>; otherwise, <c>true</c>.
    /// </returns>
    private bool IsWithinEnd(DateTime date)
    {
        if (_config.EndType == RecurrenceEnd.OnDate && _config.EndDate.HasValue)
            return date <= _config.EndDate.Value;
        return true;
    }

    /// <summary>
    /// Counts how many valid occurrences exist between <paramref name="from"/> and
    /// <paramref name="to"/> (both inclusive) using a forward linear scan.
    /// </summary>
    /// <param name="from">The start of the counting window.</param>
    /// <param name="to">The end of the counting window.</param>
    /// <returns>The number of valid occurrences in the given window.</returns>
    /// <remarks>
    /// This method is no longer used internally. <see cref="Occurrences"/> and
    /// <see cref="OccurrencesInRange"/> now maintain an incremental counter to avoid the
    /// O(n²) complexity that resulted from calling this method inside the iteration loop.
    /// </remarks>
    [Obsolete("CountOccurrencesBetween causes O(n²) when called inside an iteration loop. " +
              "Use an incremental counter instead. This method will be removed in a future version.")]
    private int CountOccurrencesBetween(DateTime from, DateTime to)
    {
        int count = 0;
        var candidate = from;
        while (candidate <= to)
        {
            if (IsValidOccurrence(candidate)) count++;
            candidate = candidate.AddDays(1);
        }
        return count;
    }

    /// <summary>
    /// Computes the total number of whole months between two dates.
    /// </summary>
    /// <param name="from">The earlier date.</param>
    /// <param name="to">The later date.</param>
    /// <returns>The difference in months as a non-negative integer.</returns>
    private static int MonthDiff(DateTime from, DateTime to)
        => (to.Year - from.Year) * 12 + (to.Month - from.Month);
}
