using System.Collections.Concurrent;
using Vali_Holiday.Models;
using Vali_Holiday.Utils;

namespace Vali_Holiday.Core;

/// <summary>
/// Abstract base class that provides default implementations of <see cref="IHolidayProvider"/>
/// methods. Concrete country providers should extend this class and override
/// <see cref="GetFixedHolidays"/> and optionally <see cref="GetMovableHolidays"/>.
/// </summary>
public abstract class BaseHolidayProvider : IHolidayProvider
{
    private readonly ConcurrentDictionary<int, HashSet<(int Month, int Day)>> _nationalHolidayCache = new();
    private readonly ConcurrentDictionary<int, IReadOnlyList<HolidayInfo>> _holidayListCache = new();
    private readonly ConcurrentDictionary<int, IReadOnlyDictionary<string, DateTime>> _observedDateCache = new();

    /// <inheritdoc/>
    public abstract string CountryCode { get; }

    /// <inheritdoc/>
    public abstract string CountryName { get; }

    /// <inheritdoc/>
    public abstract string PrimaryLanguage { get; }

    /// <summary>
    /// Returns the set of holidays whose date does not change between years
    /// (e.g., fixed calendar dates such as January 1st).
    /// </summary>
    /// <returns>A sequence of fixed <see cref="HolidayInfo"/> entries.</returns>
    protected abstract IEnumerable<HolidayInfo> GetFixedHolidays();

    /// <summary>
    /// Returns the set of movable holidays for the given <paramref name="year"/>.
    /// These include Easter-based feasts and any floating weekday observances.
    /// Override in subclasses that have movable holidays; the default returns an empty sequence.
    /// </summary>
    /// <param name="year">The four-digit calendar year.</param>
    /// <returns>A sequence of movable <see cref="HolidayInfo"/> entries for the year.</returns>
    protected virtual IEnumerable<HolidayInfo> GetMovableHolidays(int year)
        => Enumerable.Empty<HolidayInfo>();

    /// <summary>
    /// Combines fixed and movable holidays and sorts them by month, then by day.
    /// Used internally to build the cache; external callers should use
    /// <see cref="GetHolidays(int)"/> which delegates to the cache.
    /// </summary>
    private IEnumerable<HolidayInfo> BuildHolidays(int year)
        => GetFixedHolidays()
            .Concat(GetMovableHolidays(year))
            .OrderBy(h => h.Month)
            .ThenBy(h => h.Day);

    /// <inheritdoc/>
    /// <remarks>
    /// Returns the cached holiday list for the given year, building and caching it on first access.
    /// </remarks>
    public IEnumerable<HolidayInfo> GetHolidays(int year)
        => GetCachedHolidays(year);

    private IReadOnlyList<HolidayInfo> GetCachedHolidays(int year)
        => _holidayListCache.GetOrAdd(year, y => BuildHolidays(y).ToList().AsReadOnly());

    private HashSet<(int Month, int Day)> GetNationalHolidaySet(int year)
        => _nationalHolidayCache.GetOrAdd(year, y =>
            GetCachedHolidays(y)
                .Where(h => h.RegionCode == null && h.Type != HolidayType.Observance)
                .Select(h => (h.Month, h.Day))
                .ToHashSet());

    /// <inheritdoc/>
    /// <remarks>
    /// Only national holidays (those with a <see langword="null"/> <see cref="HolidayInfo.RegionCode"/>)
    /// are considered by this fast-path check. Regional holidays are intentionally excluded to avoid
    /// false positives for callers who are not in the relevant region (e.g., a holiday that applies
    /// only to Northern Ireland should not return <see langword="true"/> for an English caller).
    /// To perform region-aware holiday checking use
    /// <c>GetHolidays(year).Where(h =&gt; h.RegionCode == null || h.RegionCode == myRegion)</c>.
    /// </remarks>
    public bool IsHoliday(DateTime date)
        => GetNationalHolidaySet(date.Year).Contains((date.Month, date.Day));

    /// <inheritdoc/>
    /// <remarks>
    /// Only national holidays (those with a <see langword="null"/> <see cref="HolidayInfo.RegionCode"/>)
    /// are considered by this fast-path check. Regional holidays are intentionally excluded to avoid
    /// false positives for callers who are not in the relevant region (e.g., a holiday that applies
    /// only to Northern Ireland should not return <see langword="true"/> for an English caller).
    /// To perform region-aware holiday checking use
    /// <c>GetHolidays(year).Where(h =&gt; h.RegionCode == null || h.RegionCode == myRegion)</c>.
    /// </remarks>
    public bool IsHoliday(DateOnly date)
        => GetNationalHolidaySet(date.Year).Contains((date.Month, date.Day));

    /// <inheritdoc/>
    public HolidayInfo? GetHoliday(DateTime date)
        => GetCachedHolidays(date.Year).FirstOrDefault(h => h.Month == date.Month && h.Day == date.Day);

    /// <inheritdoc/>
    /// <remarks>
    /// Iterates all years in the range and filters holidays whose reconstructed date falls
    /// within [<paramref name="from"/>, <paramref name="to"/>] inclusive.
    /// </remarks>
    public IEnumerable<HolidayInfo> GetHolidaysInRange(DateTime from, DateTime to)
    {
        var years = Enumerable.Range(from.Year, to.Year - from.Year + 1);
        return years
            .SelectMany(y => GetCachedHolidays(y).Select(h => (year: y, holiday: h)))
            .Where(t =>
            {
                if (!t.holiday.TryToDateTime(t.year, out var d)) return false;
                return d >= from.Date && d <= to.Date;
            })
            .Select(t => t.holiday);
    }

    /// <inheritdoc/>
    public string GetName(HolidayInfo holiday, string languageCode)
        => holiday.Names.TryGetValue(languageCode, out var name) ? name : holiday.Name;

    /// <inheritdoc/>
    /// <remarks>
    /// Returns <see langword="null"/> when <paramref name="holiday"/> declares
    /// <see cref="ObservedRule.None"/>, when it falls on Feb 29 in a non-leap
    /// <paramref name="year"/> (see <see cref="HolidayInfo.TryToDateTime"/>), or when its literal
    /// calendar date for <paramref name="year"/> does not fall on a weekend. Observed dates for
    /// all holidays declaring a rule in the same <paramref name="year"/> are computed together
    /// (in calendar order) so that colliding substitute days — e.g. the UK's Christmas Day and
    /// Boxing Day both falling on a weekend — resolve to distinct dates.
    /// </remarks>
    public DateTime? GetObservedDate(HolidayInfo holiday, int year)
        => GetObservedDatesForYear(year).TryGetValue(holiday.Id, out var observed) ? observed : null;

    private IReadOnlyDictionary<string, DateTime> GetObservedDatesForYear(int year)
        => _observedDateCache.GetOrAdd(year, BuildObservedDates);

    private IReadOnlyDictionary<string, DateTime> BuildObservedDates(int year)
    {
        var result = new Dictionary<string, DateTime>();

        var actualDates = GetCachedHolidays(year)
            .Select(h => h.TryToDateTime(year, out var d) ? d : (DateTime?)null)
            .Where(d => d.HasValue)
            .Select(d => d!.Value)
            .ToHashSet();

        var unavailable = new HashSet<DateTime>(actualDates);

        var ruleHolidays = GetCachedHolidays(year)
            .Where(h => h.ObservedRule != ObservedRule.None)
            .Select(h => (holiday: h, date: h.TryToDateTime(year, out var d) ? d : (DateTime?)null))
            .Where(t => t.date.HasValue)
            .OrderBy(t => t.date!.Value);

        foreach (var (holiday, date) in ruleHolidays)
        {
            var actual = date!.Value;
            if (actual.DayOfWeek != DayOfWeek.Saturday && actual.DayOfWeek != DayOfWeek.Sunday)
                continue;

            var observed = holiday.ObservedRule switch
            {
                ObservedRule.SaturdayToFriday_SundayToMonday =>
                    ObservedDateCalculator.ShiftSaturdayToFridaySundayToMonday(actual),
                ObservedRule.NextAvailableWeekday =>
                    ObservedDateCalculator.NextAvailableWeekday(actual, unavailable),
                _ => actual
            };

            result[holiday.Id] = observed;
            unavailable.Add(observed);
        }

        return result;
    }

    /// <summary>
    /// Builds a multilingual name dictionary from individual language strings.
    /// Empty or <see langword="null"/> values fall back to the Spanish (<c>es</c>) value for
    /// <c>pt</c>, and to the English (<c>en</c>) value for <c>fr</c> and <c>de</c>.
    /// </summary>
    /// <param name="es">Spanish name.</param>
    /// <param name="en">English name.</param>
    /// <param name="pt">Portuguese name (falls back to <paramref name="es"/> if empty).</param>
    /// <param name="fr">French name (falls back to <paramref name="en"/> if empty).</param>
    /// <param name="de">German name (falls back to <paramref name="en"/> if empty).</param>
    /// <returns>A read-only dictionary keyed by BCP 47 language tag.</returns>
    protected static IReadOnlyDictionary<string, string> Names(
        string es, string en, string pt = "", string fr = "", string de = "")
        => new Dictionary<string, string>
        {
            ["es"] = es,
            ["en"] = en,
            ["pt"] = string.IsNullOrEmpty(pt) ? es : pt,
            ["fr"] = string.IsNullOrEmpty(fr) ? en : fr,
            ["de"] = string.IsNullOrEmpty(de) ? en : de
        };
}
