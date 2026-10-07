using Vali_Schedule.Enums;
using Vali_Schedule.Models;

namespace Vali_Schedule.Core;

/// <summary>
/// Resolves a candidate occurrence date against the <c>SkipWeekends</c>/<c>SkipHolidays</c>
/// rules configured on a <see cref="ScheduleConfig"/>, isolating that concern from
/// <see cref="ValiSchedule"/>'s recurrence-matching and enumeration logic.
/// </summary>
internal sealed class OccurrenceSkipResolver
{
    /// <summary>Maximum number of days <see cref="Resolve"/> searches in either direction.</summary>
    private const int MaxSkipSearchDays = 366;

    private readonly ScheduleConfig _config;

    /// <summary>
    /// Creates a resolver bound to the given schedule configuration.
    /// </summary>
    /// <param name="config">The schedule configuration holding the skip rules to apply.</param>
    public OccurrenceSkipResolver(ScheduleConfig config)
    {
        _config = config;
    }

    /// <summary>
    /// Determines whether <paramref name="date"/> is excluded by <see cref="ScheduleConfig.SkipWeekendsEnabled"/>
    /// and/or <see cref="ScheduleConfig.HolidayProvider"/>, treating both rules as a single union of excluded dates.
    /// </summary>
    private bool IsSkipExcluded(DateTime date) =>
        (_config.SkipWeekendsEnabled && (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday))
        || (_config.HolidayProvider != null && _config.HolidayProvider.IsHoliday(date));

    /// <summary>
    /// Resolves <paramref name="candidate"/> against the configured <c>SkipWeekends</c>/<c>SkipHolidays</c>
    /// rules. Returns the original date unchanged when no skip rule is active or the date is not excluded;
    /// otherwise shifts it per the effective <see cref="HolidaySkipStrategy"/>, or returns <c>null</c> when
    /// the occurrence must be dropped (explicit <see cref="HolidaySkipStrategy.Skip"/>, or no valid
    /// replacement found within <see cref="MaxSkipSearchDays"/> days, or a backward search crossing
    /// <paramref name="effectiveStart"/>).
    /// </summary>
    /// <param name="candidate">The candidate occurrence date to resolve.</param>
    /// <param name="effectiveStart">The schedule's effective start date, used as the backward-search floor.</param>
    public DateTime? Resolve(DateTime candidate, DateTime effectiveStart)
    {
        bool anySkipRuleActive = _config.SkipWeekendsEnabled || _config.HolidayProvider != null;
        if (!anySkipRuleActive || !IsSkipExcluded(candidate))
            return candidate;

        var strategy = EffectiveStrategy();
        if (strategy == HolidaySkipStrategy.Skip)
            return null;

        return SearchReplacement(candidate, strategy == HolidaySkipStrategy.PreviousWorkday ? -1 : 1, effectiveStart);
    }

    /// <summary>
    /// Weekend-only exclusion (no holiday provider configured) always resolves forward;
    /// the strategy knob only applies once a holiday provider is active.
    /// </summary>
    private HolidaySkipStrategy EffectiveStrategy() =>
        _config.HolidayProvider != null ? _config.HolidaySkipStrategy : HolidaySkipStrategy.NextWorkday;

    /// <summary>
    /// Searches up to <see cref="MaxSkipSearchDays"/> days in <paramref name="direction"/> for the first
    /// non-excluded date, returning <c>null</c> if a backward search crosses <paramref name="effectiveStart"/>
    /// or no replacement is found within the search window.
    /// </summary>
    private DateTime? SearchReplacement(DateTime candidate, int direction, DateTime effectiveStart)
    {
        var resolved = candidate;
        for (int i = 0; i < MaxSkipSearchDays; i++)
        {
            resolved = resolved.AddDays(direction);
            if (direction < 0 && resolved.Date < effectiveStart.Date)
                return null;
            if (!IsSkipExcluded(resolved))
                return resolved;
        }
        return null;
    }
}
