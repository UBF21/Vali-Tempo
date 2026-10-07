namespace Vali_Schedule.Cron;

/// <summary>
/// The day-then-minute occurrence scan engine for <see cref="CronSchedule"/>: rejects
/// non-matching days using only the cheap month/day-of-month/day-of-week masks before ever
/// checking the minute/hour masks, keeping a dense cron expression's enumeration cheap over a
/// multi-year window instead of brute-forcing every minute.
/// </summary>
public sealed partial class CronSchedule
{
    /// <summary>
    /// Counts matching instants in [<paramref name="fromInclusive"/>, <paramref name="toExclusive"/>),
    /// used to keep the <see cref="EndsAfter"/> budget accurate when a search window starts
    /// after <see cref="StartingFrom"/>.
    /// </summary>
    private int CountForward(DateTime fromInclusive, DateTime toExclusive)
    {
        int count = 0;
        foreach (var candidate in ForwardMatches(fromInclusive))
        {
            if (candidate >= toExclusive) break;
            count++;
        }
        return count;
    }

    /// <summary>
    /// Ascending scan for every matching instant on or after <paramref name="from"/>, bounded to
    /// <see cref="MaxScanDays"/> days ahead.
    /// </summary>
    private IEnumerable<DateTime> ForwardMatches(DateTime from)
    {
        var day = from.Date;
        for (int d = 0; d < MaxScanDays; d++, day = day.AddDays(1))
        {
            if (!_expression.MatchesDate(day)) continue;

            bool isFirstDay = day == from.Date;
            foreach (var (hour, minute) in TimesAscending(isFirstDay ? from.Hour : 0, isFirstDay ? from.Minute : 0))
            {
                var candidate = day.AddHours(hour).AddMinutes(minute);
                if (MatchesExtraPredicates(candidate))
                    yield return candidate;
            }
        }
    }

    /// <summary>
    /// Descending scan for the largest qualifying instant strictly before
    /// <paramref name="beforeExclusive"/>, never yielding below <see cref="EffectiveStartDate"/>.
    /// </summary>
    private IEnumerable<DateTime> BackwardMatches(DateTime beforeExclusive)
    {
        var effectiveStart = EffectiveStartDate;
        var day = beforeExclusive.Date;

        for (int d = 0; d < MaxScanDays; d++, day = day.AddDays(-1))
        {
            if (day < effectiveStart.Date) yield break;
            if (!_expression.MatchesDate(day)) continue;

            bool isFirstDay = day == beforeExclusive.Date;
            int fromHour = isFirstDay ? beforeExclusive.Hour : 23;
            int fromMinute = isFirstDay ? beforeExclusive.Minute - 1 : 59;

            foreach (var (hour, minute) in TimesDescending(fromHour, fromMinute))
            {
                var candidate = day.AddHours(hour).AddMinutes(minute);
                if (candidate < effectiveStart) yield break;
                if (MatchesExtraPredicates(candidate))
                    yield return candidate;
            }
        }
    }

    /// <summary>Enumerates (hour, minute) pairs matching the hour/minute masks, ascending from <paramref name="fromHour"/>:<paramref name="fromMinute"/>.</summary>
    private IEnumerable<(int Hour, int Minute)> TimesAscending(int fromHour, int fromMinute)
    {
        for (int hour = fromHour; hour <= 23; hour++)
        {
            if (!_expression.Hour.Matches(hour)) continue;

            int minuteStart = hour == fromHour ? fromMinute : 0;
            for (int minute = minuteStart; minute <= 59; minute++)
                if (_expression.Minute.Matches(minute))
                    yield return (hour, minute);
        }
    }

    /// <summary>Enumerates (hour, minute) pairs matching the hour/minute masks, descending from <paramref name="fromHour"/>:<paramref name="fromMinute"/>.</summary>
    private IEnumerable<(int Hour, int Minute)> TimesDescending(int fromHour, int fromMinute)
    {
        for (int hour = fromHour; hour >= 0; hour--)
        {
            if (!_expression.Hour.Matches(hour)) continue;

            int minuteStart = hour == fromHour ? fromMinute : 59;
            for (int minute = minuteStart; minute >= 0; minute--)
                if (_expression.Minute.Matches(minute))
                    yield return (hour, minute);
        }
    }

    private bool MatchesExtraPredicates(DateTime candidate)
    {
        if (IsSkipExcluded(candidate.Date)) return false;

        foreach (var predicate in _extraPredicates)
            if (!predicate(candidate)) return false;
        return true;
    }
}
