using FluentAssertions;
using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Range.Models;
using Vali_Schedule.Core;
using Vali_Schedule.Enums;
using Vali_Time.Enums;
using Xunit;

namespace Vali_Time.Tests.Tests;

public class ValiScheduleTests
{
    /// <summary>Test-only holiday provider backed by a fixed set of dates.</summary>
    private sealed class FixedDateHolidayProvider : IHolidayProvider
    {
        private readonly HashSet<DateTime> _holidays;

        public FixedDateHolidayProvider(params DateTime[] holidays) => _holidays = holidays.Select(d => d.Date).ToHashSet();

        public string CountryCode => "ZZ";
        public string CountryName => "Test";
        public string PrimaryLanguage => "en";
        public bool IsHoliday(DateTime date) => _holidays.Contains(date.Date);
        public bool IsHoliday(DateOnly date) => _holidays.Contains(date.ToDateTime(TimeOnly.MinValue));
        public IEnumerable<HolidayInfo> GetHolidays(int year) => throw new NotSupportedException();
        public HolidayInfo? GetHoliday(DateTime date) => throw new NotSupportedException();
        public IEnumerable<HolidayInfo> GetHolidaysInRange(DateTime from, DateTime to) => throw new NotSupportedException();
        public string GetName(HolidayInfo holiday, string languageCode) => throw new NotSupportedException();
        public DateTime? GetObservedDate(HolidayInfo holiday, int year) => throw new NotSupportedException();
    }

    // ── SkipWeekends / SkipHolidays ──────────────────────────────────────────

    [Fact]
    public void SkipWeekends_OccurrenceOnWeekend_MovesToNextMonday()
    {
        // Jan 1 2025 is a Wednesday; Jan 4/5 are Sat/Sun.
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Days)
            .StartingFrom(new DateTime(2025, 1, 1))
            .SkipWeekends();

        var results = schedule.OccurrencesInRange(new DateTime(2025, 1, 4), new DateTime(2025, 1, 6)).ToList();

        results.Should().ContainSingle().Which.Should().Be(new DateTime(2025, 1, 6));
    }

    [Fact]
    public void SkipWeekends_OccurrenceOnWeekday_IsUnchanged()
    {
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Days)
            .StartingFrom(new DateTime(2025, 1, 1))
            .SkipWeekends();

        var result = schedule.NextOccurrence(new DateTime(2025, 1, 1));

        result.Should().Be(new DateTime(2025, 1, 1));
    }

    [Fact]
    public void SkipHolidays_NextWorkday_MovesForwardPastHoliday()
    {
        var holidayProvider = new FixedDateHolidayProvider(new DateTime(2025, 1, 2));
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Days)
            .StartingFrom(new DateTime(2025, 1, 1))
            .SkipHolidays(holidayProvider, HolidaySkipStrategy.NextWorkday);

        var result = schedule.NextOccurrence(new DateTime(2025, 1, 2));

        result.Should().Be(new DateTime(2025, 1, 3));
    }

    [Fact]
    public void SkipHolidays_PreviousWorkday_MovesBackward_DropsIfCrossesStartingFrom()
    {
        var holidayProvider = new FixedDateHolidayProvider(new DateTime(2025, 1, 1));
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Days)
            .StartingFrom(new DateTime(2025, 1, 1))
            .SkipHolidays(holidayProvider, HolidaySkipStrategy.PreviousWorkday);

        // Jan 1 is both the only possible backward replacement and StartingFrom; resolving it
        // crosses the start date immediately, so that single candidate is dropped — but the
        // schedule itself does not stop, it keeps scanning forward and finds Jan 2.
        var result = schedule.NextOccurrence(new DateTime(2025, 1, 1));

        result.Should().Be(new DateTime(2025, 1, 2));
    }

    [Fact]
    public void SkipHolidays_Skip_OmitsOccurrenceWithoutReplacement()
    {
        var holidayProvider = new FixedDateHolidayProvider(new DateTime(2025, 1, 2));
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Days)
            .StartingFrom(new DateTime(2025, 1, 1))
            .SkipHolidays(holidayProvider, HolidaySkipStrategy.Skip);

        var results = schedule.OccurrencesInRange(new DateTime(2025, 1, 1), new DateTime(2025, 1, 3)).ToList();

        results.Should().BeEquivalentTo([new DateTime(2025, 1, 1), new DateTime(2025, 1, 3)]);
    }

    [Fact]
    public void SkipWeekendsAndSkipHolidays_Combined_ResolvesInOneUnifiedSearch()
    {
        // Friday Jan 3 2025 is a holiday; Sat 4th/Sun 5th are weekend; Monday 6th is free.
        var holidayProvider = new FixedDateHolidayProvider(new DateTime(2025, 1, 3));
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Days)
            .StartingFrom(new DateTime(2025, 1, 1))
            .SkipWeekends()
            .SkipHolidays(holidayProvider, HolidaySkipStrategy.NextWorkday);

        var result = schedule.NextOccurrence(new DateTime(2025, 1, 3));

        result.Should().Be(new DateTime(2025, 1, 6));
    }

    [Fact]
    public void SkipWeekends_WithCustomPredicateEquivalent_AppliesAfterResolution()
    {
        // WithCustomPredicate replaces the recurrence evaluator for RecurrenceType.Custom,
        // so this exercises skip resolution combined with a custom recurrence definition.
        var schedule = new ValiSchedule()
            .WithCustomPredicate(d => d.Day is 4 or 5 or 6) // Sat, Sun, Mon
            .StartingFrom(new DateTime(2025, 1, 1))
            .SkipWeekends();

        var results = schedule.OccurrencesInRange(new DateTime(2025, 1, 4), new DateTime(2025, 1, 6)).ToList();

        results.Should().ContainSingle().Which.Should().Be(new DateTime(2025, 1, 6));
    }

    [Fact]
    public void SkipHolidays_NullProvider_ThrowsArgumentNullException()
    {
        var schedule = new ValiSchedule();

        var act = () => schedule.SkipHolidays(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void SkipHolidays_Skip_StillConsumesEndsAfterBudgetSlot()
    {
        var holidayProvider = new FixedDateHolidayProvider(new DateTime(2025, 1, 2));
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Days)
            .StartingFrom(new DateTime(2025, 1, 1))
            .SkipHolidays(holidayProvider, HolidaySkipStrategy.Skip)
            .EndsAfter(3);

        // Budget of 3 covers Jan 1, Jan 2 (dropped), Jan 3 — only 2 dates are actually returned.
        var results = schedule.Occurrences(new DateTime(2025, 1, 1), 10).ToList();

        results.Should().BeEquivalentTo([new DateTime(2025, 1, 1), new DateTime(2025, 1, 3)]);
    }


    // ── Daily recurrence ─────────────────────────────────────────────────────

    [Fact]
    public void DailyRecurrence_Occurrences_ReturnsExactlyFiveDates()
    {
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Days)
            .StartingFrom(new DateTime(2025, 1, 1));

        var results = schedule.Occurrences(new DateTime(2025, 1, 1), 5).ToList();

        results.Should().HaveCount(5);
    }

    [Fact]
    public void DailyRecurrence_Occurrences_DatesAreConsecutive()
    {
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Days)
            .StartingFrom(new DateTime(2025, 1, 1));

        var results = schedule.Occurrences(new DateTime(2025, 1, 1), 5).ToList();

        for (int i = 1; i < results.Count; i++)
            results[i].Should().Be(results[i - 1].AddDays(1));
    }

    // ── Weekly recurrence ────────────────────────────────────────────────────

    [Fact]
    public void WeeklyRecurrence_OnMonday_ReturnsFourConsecutiveMondays()
    {
        // 2025-01-06 is a Monday
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Weeks)
            .On(DayOfWeek.Monday)
            .StartingFrom(new DateTime(2025, 1, 6));

        var results = schedule.Occurrences(new DateTime(2025, 1, 6), 4).ToList();

        results.Should().HaveCount(4);
        results.Should().AllSatisfy(d => d.DayOfWeek.Should().Be(DayOfWeek.Monday));
        for (int i = 1; i < results.Count; i++)
            results[i].Should().Be(results[i - 1].AddDays(7));
    }

    [Fact]
    public void WeeklyRecurrence_EveryTwoWeeks_ReturnsCorrectDates()
    {
        // 2025-01-06 is a Monday
        var schedule = new ValiSchedule()
            .Every(2, TimeUnit.Weeks)
            .On(DayOfWeek.Monday)
            .StartingFrom(new DateTime(2025, 1, 6));

        var results = schedule.Occurrences(new DateTime(2025, 1, 6), 3).ToList();

        results.Should().HaveCount(3);
        results.Should().AllSatisfy(d => d.DayOfWeek.Should().Be(DayOfWeek.Monday));
        for (int i = 1; i < results.Count; i++)
            results[i].Should().Be(results[i - 1].AddDays(14));
    }

    // ── EndsAfter — AfterOccurrences limit ───────────────────────────────────

    [Fact]
    public void EndsAfter_LimitsFiveOccurrences_WhenRequestingTen()
    {
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Days)
            .StartingFrom(new DateTime(2025, 1, 1))
            .EndsAfter(5);

        var results = schedule.Occurrences(new DateTime(2025, 1, 1), 10).ToList();

        results.Should().HaveCount(5);
    }

    [Fact]
    public void EndsAfter_CountIsIdempotent_MultipleCalls()
    {
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Days)
            .StartingFrom(new DateTime(2025, 1, 1))
            .EndsAfter(5);

        var first  = schedule.Occurrences(new DateTime(2025, 1, 1), 10).ToList();
        var second = schedule.Occurrences(new DateTime(2025, 1, 1), 10).ToList();

        first.Should().BeEquivalentTo(second, options => options.WithStrictOrdering());
    }

    // ── NextOccurrence ───────────────────────────────────────────────────────

    [Fact]
    public void NextOccurrence_DailySchedule_ReturnsReferenceWhenItIsValidOccurrence()
    {
        // NextOccurrence searches from reference.Date forward; since every day is valid, it returns the reference itself.
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Days)
            .StartingFrom(new DateTime(2025, 1, 1));

        var result = schedule.NextOccurrence(new DateTime(2025, 1, 5));

        result.Should().Be(new DateTime(2025, 1, 5));
    }

    [Fact]
    public void NextOccurrence_WeeklyOnMonday_ReturnsNextMonday()
    {
        // 2025-01-06 is Monday; reference is 2025-01-07 (Tuesday), next Monday is 2025-01-13
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Weeks)
            .On(DayOfWeek.Monday)
            .StartingFrom(new DateTime(2025, 1, 6));

        var result = schedule.NextOccurrence(new DateTime(2025, 1, 7));

        result.Should().Be(new DateTime(2025, 1, 13));
    }

    // ── OccurrencesInRange ───────────────────────────────────────────────────

    [Fact]
    public void OccurrencesInRange_DailySchedule_SevenDayRange_ReturnsSevenOccurrences()
    {
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Days)
            .StartingFrom(new DateTime(2025, 1, 1));

        var results = schedule.OccurrencesInRange(
            new DateTime(2025, 1, 1),
            new DateTime(2025, 1, 7)).ToList();

        results.Should().HaveCount(7);
    }

    [Fact]
    public void OccurrencesInRange_NoValidDays_ReturnsEmptyList()
    {
        // Weekly on Monday; range covers only Tuesday–Sunday of the same week
        // 2025-01-06 is Monday; range starts Tuesday 2025-01-07, ends Sunday 2025-01-12
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Weeks)
            .On(DayOfWeek.Monday)
            .StartingFrom(new DateTime(2025, 1, 6));

        var results = schedule.OccurrencesInRange(
            new DateTime(2025, 1, 7),
            new DateTime(2025, 1, 12)).ToList();

        results.Should().BeEmpty();
    }

    // ── Custom predicate ─────────────────────────────────────────────────────

    [Fact]
    public void CustomPredicate_WednesdaysOnly_ReturnsThreeWednesdays()
    {
        // WithCustomPredicate also sets RecurrenceType.Custom, activating the predicate branch.
        var schedule = new ValiSchedule()
            .WithCustomPredicate(d => d.DayOfWeek == DayOfWeek.Wednesday)
            .StartingFrom(new DateTime(2025, 1, 1));

        var results = schedule.Occurrences(new DateTime(2025, 1, 1), 3).ToList();

        results.Should().HaveCount(3);
        results.Should().AllSatisfy(d => d.DayOfWeek.Should().Be(DayOfWeek.Wednesday));
    }

    [Fact]
    public void WithCustomPredicate_NullPredicate_ThrowsArgumentNullException()
    {
        var schedule = new ValiSchedule();

        Action act = () => schedule.WithCustomPredicate(null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("predicate");
    }

    // ── PreviousOccurrence ───────────────────────────────────────────────────

    [Fact]
    public void PreviousOccurrence_DailySchedule_ReturnsDayBeforeReference()
    {
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Days)
            .StartingFrom(new DateTime(2025, 1, 1));

        var result = schedule.PreviousOccurrence(new DateTime(2025, 1, 5));

        result.Should().Be(new DateTime(2025, 1, 4));
    }

    // ── OccursOn ─────────────────────────────────────────────────────────────

    [Fact]
    public void OccursOn_WeeklyMonday_OnMondayStartDate_ReturnsTrue()
    {
        // 2025-01-06 is a Monday
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Weeks)
            .On(DayOfWeek.Monday)
            .StartingFrom(new DateTime(2025, 1, 6));

        schedule.OccursOn(new DateTime(2025, 1, 6)).Should().BeTrue();
    }

    [Fact]
    public void OccursOn_WeeklyMonday_OnTuesday_ReturnsFalse()
    {
        // 2025-01-06 is Monday; 2025-01-07 is Tuesday
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Weeks)
            .On(DayOfWeek.Monday)
            .StartingFrom(new DateTime(2025, 1, 6));

        schedule.OccursOn(new DateTime(2025, 1, 7)).Should().BeFalse();
    }

    // ── S-1: Monthly day-31 skip ─────────────────────────────────────────────

    [Fact]
    public void Monthly_Day31_SkipsShortMonths()
    {
        // Start Jan 31 2025, every 1 month on day 31.
        // Months with fewer than 31 days (Feb, Apr, Jun, Sep, Nov) must be skipped.
        // Within 2025 the valid occurrences are: Jan 31, Mar 31, May 31, Jul 31, Aug 31, Oct 31, Dec 31 — 7 total.
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Months)
            .OnDayOfMonth(31)
            .StartingFrom(new DateTime(2025, 1, 31));

        var results = schedule.OccurrencesInRange(
            new DateTime(2025, 1, 1),
            new DateTime(2025, 12, 31)).ToList();

        results.Should().HaveCount(7);
        results.Should().AllSatisfy(d => d.Day.Should().Be(31));
        results.Should().NotContain(d => d.Month == 2);  // Feb has 28 days in 2025
        results.Should().NotContain(d => d.Month == 4);  // Apr has 30 days
        results.Should().NotContain(d => d.Month == 6);  // Jun has 30 days
        results.Should().NotContain(d => d.Month == 9);  // Sep has 30 days
        results.Should().NotContain(d => d.Month == 11); // Nov has 30 days
    }

    [Fact]
    public void Monthly_Day31_NextOccurrence_SkipsApril()
    {
        // From March 31 2025, next monthly occurrence on day 31 should be May 31 (not Apr 30)
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Months)
            .OnDayOfMonth(31)
            .StartingFrom(new DateTime(2025, 1, 31));

        var result = schedule.NextOccurrence(new DateTime(2025, 4, 1));

        result.Should().Be(new DateTime(2025, 5, 31));
    }

    [Fact]
    public void Monthly_Day29_SkipsNonLeapFeb()
    {
        // Start Jan 29 2025, every 1 month on day 29.
        // Feb 2025 has 28 days — not a leap year — so Feb must be skipped.
        // Mar 29 must be the next occurrence after Jan 29.
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Months)
            .OnDayOfMonth(29)
            .StartingFrom(new DateTime(2025, 1, 29));

        var results = schedule.OccurrencesInRange(
            new DateTime(2025, 1, 1),
            new DateTime(2025, 3, 31)).ToList();

        results.Should().NotContain(d => d.Month == 2); // Feb 2025 has only 28 days
        results.Should().Contain(new DateTime(2025, 1, 29));
        results.Should().Contain(new DateTime(2025, 3, 29));
    }

    // ── OccurrencePeriods ─────────────────────────────────────────────────────

    [Fact]
    public void OccurrencePeriods_MonthlySchedule_PairsConsecutiveOccurrencesExactly()
    {
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Months)
            .OnDayOfMonth(1)
            .StartingFrom(new DateTime(2026, 1, 1));

        var periods = schedule.OccurrencePeriods(
            new DateTime(2026, 1, 1), new DateTime(2026, 3, 15)).ToList();

        periods.Should().HaveCount(3);
        periods[0].Start.Should().Be(new DateTime(2026, 1, 1));
        periods[0].End.Should().Be(new DateTime(2026, 2, 1).AddTicks(-1));
        periods[1].Start.Should().Be(new DateTime(2026, 2, 1));
        periods[1].End.Should().Be(new DateTime(2026, 3, 1).AddTicks(-1));
        periods[2].Start.Should().Be(new DateTime(2026, 3, 1));
        // Closes with the real next occurrence (Apr 1), even though it falls beyond `to`.
        periods[2].End.Should().Be(new DateTime(2026, 4, 1).AddTicks(-1));
    }

    [Fact]
    public void OccurrencePeriods_NoOverlapBetweenConsecutivePeriods()
    {
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Months)
            .OnDayOfMonth(1)
            .StartingFrom(new DateTime(2026, 1, 1));

        var periods = schedule.OccurrencePeriods(
            new DateTime(2026, 1, 1), new DateTime(2026, 4, 1)).ToList();

        for (int i = 1; i < periods.Count; i++)
            periods[i - 1].End.Should().BeBefore(periods[i].Start);
    }

    [Fact]
    public void OccurrencePeriods_EndsAfterExhaustedMidRange_ExcludesLastOccurrenceWithoutPeriod()
    {
        // EndsAfter(2): Jan 1 and Feb 1 are valid; Mar 1 is beyond the budget, so Feb 1 has
        // no real next occurrence and must be excluded rather than closed at `to`.
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Months)
            .OnDayOfMonth(1)
            .StartingFrom(new DateTime(2026, 1, 1))
            .EndsAfter(2);

        var periods = schedule.OccurrencePeriods(
            new DateTime(2026, 1, 1), new DateTime(2026, 6, 1)).ToList();

        periods.Should().ContainSingle();
        periods[0].Start.Should().Be(new DateTime(2026, 1, 1));
        periods[0].End.Should().Be(new DateTime(2026, 2, 1).AddTicks(-1));
    }

    [Fact]
    public void OccurrencePeriods_EndsOnExhaustedMidRange_ExcludesLastOccurrenceWithoutPeriod()
    {
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Months)
            .OnDayOfMonth(1)
            .StartingFrom(new DateTime(2026, 1, 1))
            .EndsOn(new DateTime(2026, 2, 15));

        var periods = schedule.OccurrencePeriods(
            new DateTime(2026, 1, 1), new DateTime(2026, 6, 1)).ToList();

        periods.Should().ContainSingle();
        periods[0].Start.Should().Be(new DateTime(2026, 1, 1));
        periods[0].End.Should().Be(new DateTime(2026, 2, 1).AddTicks(-1));
    }

    [Fact]
    public void OccurrencePeriods_NoOccurrencesInRange_ReturnsEmptySequence()
    {
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Months)
            .OnDayOfMonth(1)
            .StartingFrom(new DateTime(2026, 1, 1));

        var periods = schedule.OccurrencePeriods(
            new DateTime(2026, 1, 2), new DateTime(2026, 1, 31)).ToList();

        periods.Should().BeEmpty();
    }

    [Fact]
    public void OccurrencePeriods_SingleOccurrenceInRange_ClosesBeyondTo()
    {
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Months)
            .OnDayOfMonth(1)
            .StartingFrom(new DateTime(2026, 1, 1));

        var periods = schedule.OccurrencePeriods(
            new DateTime(2026, 1, 1), new DateTime(2026, 1, 20)).ToList();

        periods.Should().ContainSingle();
        periods[0].Start.Should().Be(new DateTime(2026, 1, 1));
        periods[0].End.Should().Be(new DateTime(2026, 2, 1).AddTicks(-1));
        periods[0].End.Should().BeAfter(new DateTime(2026, 1, 20));
    }

    [Fact]
    public void OccurrencePeriods_FromAfterTo_ThrowsArgumentException()
    {
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Months)
            .OnDayOfMonth(1)
            .StartingFrom(new DateTime(2026, 1, 1));

        var act = () => schedule.OccurrencePeriods(new DateTime(2026, 2, 1), new DateTime(2026, 1, 1)).ToList();

        act.Should().Throw<ArgumentException>().WithParameterName("from");
    }

    [Fact]
    public void OccurrencePeriods_WithCustomPredicate_PairsCorrectly()
    {
        var schedule = new ValiSchedule()
            .WithCustomPredicate(d => d.Day is 1 or 15)
            .StartingFrom(new DateTime(2026, 1, 1));

        // Only Jan 1 falls in range; it closes with the next real occurrence (Jan 15),
        // which falls beyond `to` (Jan 10) — same "closes beyond to" rule as any other type.
        var periods = schedule.OccurrencePeriods(
            new DateTime(2026, 1, 1), new DateTime(2026, 1, 10)).ToList();

        periods.Should().ContainSingle();
        periods[0].Start.Should().Be(new DateTime(2026, 1, 1));
        periods[0].End.Should().Be(new DateTime(2026, 1, 15).AddTicks(-1));
    }

    // ── VS-1: AfterOccurrences budget off-by-one ─────────────────────────────

    [Fact]
    public void Occurrences_AfterOccurrences_WithPreScanOffset_ReturnsCorrectCount()
    {
        // Start: Jan 1 2025, Every 1 Day, EndsAfter(5).
        // Reference: Jan 3 2025.
        // Pre-scan counts Jan 1 and Jan 2 = 2 occurrences already consumed.
        // Remaining budget = 5 - 2 = 3. So Occurrences(Jan 3, limit:10) should yield exactly 3:
        // Jan 3, Jan 4, Jan 5 — then Jan 6 would push the count to 6 > 5, so it stops.
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Days)
            .StartingFrom(new DateTime(2025, 1, 1))
            .EndsAfter(5);

        var results = schedule.Occurrences(new DateTime(2025, 1, 3), 10).ToList();

        results.Should().HaveCount(3);
        results[0].Should().Be(new DateTime(2025, 1, 3));
        results[1].Should().Be(new DateTime(2025, 1, 4));
        results[2].Should().Be(new DateTime(2025, 1, 5));
    }

    // ── Weekly-without-days validation: unified between Build() and direct queries ──

    [Fact]
    public void Build_WeeklyWithoutDays_ThrowsInvalidOperationException()
    {
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Weeks)
            .StartingFrom(new DateTime(2025, 1, 1));

        var act = () => schedule.Build();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Weekly recurrence requires at least one day of week. Call .On(DayOfWeek) to configure.");
    }

    [Fact]
    public void DirectQuery_WeeklyWithoutDays_WithoutBuild_ThrowsSameInvalidOperationException()
    {
        // No .On(...) and no .Build() call — the query path must enforce the same rule
        // that Build() enforces, since Build() is never mandatory before querying.
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Weeks)
            .StartingFrom(new DateTime(2025, 1, 1));

        var act = () => schedule.NextOccurrence(new DateTime(2025, 1, 1));

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Weekly recurrence requires at least one day of week. Call .On(DayOfWeek) to configure.");
    }

    // ── Builder argument validation ───────────────────────────────────────────

    [Fact]
    public void Every_ZeroInterval_ThrowsArgumentOutOfRangeException()
    {
        var schedule = new ValiSchedule();
        var act = () => schedule.Every(0, TimeUnit.Days);
        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("interval");
    }

    [Fact]
    public void Every_UnsupportedTimeUnit_ThrowsArgumentOutOfRangeException()
    {
        var schedule = new ValiSchedule();
        var act = () => schedule.Every(1, TimeUnit.Hours);
        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("unit");
    }

    [Fact]
    public void EndsAfter_ZeroOccurrences_ThrowsArgumentOutOfRangeException()
    {
        var schedule = new ValiSchedule();
        var act = () => schedule.EndsAfter(0);
        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("occurrences");
    }

    [Fact]
    public void OnDayOfMonth_OutOfRange_ThrowsArgumentOutOfRangeException()
    {
        var schedule = new ValiSchedule();
        var act = () => schedule.OnDayOfMonth(32);
        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("day");
    }

    [Fact]
    public void Build_ValidConfiguration_ReturnsSameInstanceWithoutThrowing()
    {
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Days)
            .StartingFrom(new DateTime(2025, 1, 1));

        var built = schedule.Build();

        built.Should().BeSameAs(schedule);
    }

    // ── Explicit IValiSchedule interface forwarding ───────────────────────────

    [Fact]
    public void IValiSchedule_ExplicitInterfaceMembers_DelegateToTypedImplementation()
    {
        IValiSchedule schedule = new ValiSchedule();

        schedule = schedule
            .Every(1, TimeUnit.Weeks)
            .On(DayOfWeek.Monday)
            .At(new TimeOnly(9, 0))
            .StartingFrom(new DateTime(2025, 1, 6)) // a Monday
            .EndsAfter(5)
            .EndsOn(new DateTime(2025, 12, 31))
            .WithCustomPredicate(d => true)
            .SkipWeekends()
            .SkipHolidays(new FixedDateHolidayProvider())
            .Build();

        schedule.Should().NotBeNull();
        schedule.OccursOn(new DateTime(2025, 1, 6)).Should().BeTrue();
    }

    [Fact]
    public void IValiSchedule_ExplicitOnDayOfMonth_DelegatesToTypedImplementation()
    {
        IValiSchedule schedule = new ValiSchedule()
            .Every(1, TimeUnit.Months)
            .StartingFrom(new DateTime(2025, 1, 15));

        schedule = schedule.OnDayOfMonth(15);

        schedule.OccursOn(new DateTime(2025, 2, 15)).Should().BeTrue();
    }

    // ── NextOccurrence: full-window exhaustion returns null ──────────────────

    [Fact]
    public void NextOccurrence_PredicateNeverMatches_ExhaustsWindow_ReturnsNull()
    {
        var schedule = new ValiSchedule()
            .WithCustomPredicate(_ => false)
            .StartingFrom(new DateTime(2025, 1, 1));

        var result = schedule.NextOccurrence(new DateTime(2025, 1, 1));

        result.Should().BeNull();
    }

    // ── PreviousOccurrence: AfterOccurrences budget already exhausted before reference ──

    [Fact]
    public void PreviousOccurrence_BudgetExhaustedBeforeReference_ReturnsLastValidDayBeforeReference()
    {
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Days)
            .StartingFrom(new DateTime(2025, 1, 1))
            .EndsAfter(1);

        var result = schedule.PreviousOccurrence(new DateTime(2025, 1, 5));

        result.Should().Be(new DateTime(2025, 1, 4));
    }

    [Fact]
    public void PreviousOccurrence_BudgetExhaustedAndBackwardScanCrossesStartDate_ReturnsNull()
    {
        // Jan 1 is a configured holiday with Skip strategy — the only candidate the backward
        // scan can reach is dropped, and the next step backward crosses StartingFrom, so the
        // search returns null instead of a synthetic replacement.
        var holidayProvider = new FixedDateHolidayProvider(new DateTime(2025, 1, 1));
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Days)
            .StartingFrom(new DateTime(2025, 1, 1))
            .SkipHolidays(holidayProvider, HolidaySkipStrategy.Skip)
            .EndsAfter(1);

        var result = schedule.PreviousOccurrence(new DateTime(2025, 1, 2));

        result.Should().BeNull();
    }

    [Fact]
    public void PreviousOccurrence_NoMatchWithinScanWindow_ReturnsNull()
    {
        // Predicate never matches; StartingFrom is far enough back that the backward scan
        // exhausts its MaxScanDays budget before ever crossing the start date.
        var schedule = new ValiSchedule()
            .WithCustomPredicate(_ => false)
            .StartingFrom(new DateTime(2000, 1, 1));

        var result = schedule.PreviousOccurrence(new DateTime(2025, 1, 1));

        result.Should().BeNull();
    }

    // ── OccurrencesInRange: argument validation and AfterOccurrences pre-scan offset ──

    [Fact]
    public void OccurrencesInRange_FromAfterTo_ThrowsArgumentException()
    {
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Days)
            .StartingFrom(new DateTime(2025, 1, 1));

        var act = () => schedule.OccurrencesInRange(new DateTime(2025, 1, 10), new DateTime(2025, 1, 1)).ToList();

        act.Should().Throw<ArgumentException>().WithParameterName("from");
    }

    [Fact]
    public void OccurrencesInRange_AfterOccurrences_WithRangeStartAfterStartDate_AppliesPreScanOffset()
    {
        // Budget of 5; range starts on Jan 3, so Jan 1/Jan 2 are pre-counted (2 already consumed).
        // Remaining budget = 3, so Jan 3, Jan 4, Jan 5 are returned; Jan 6 would exceed the budget.
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Days)
            .StartingFrom(new DateTime(2025, 1, 1))
            .EndsAfter(5);

        var results = schedule.OccurrencesInRange(new DateTime(2025, 1, 3), new DateTime(2025, 1, 10)).ToList();

        results.Should().BeEquivalentTo(
        [
            new DateTime(2025, 1, 3),
            new DateTime(2025, 1, 4),
            new DateTime(2025, 1, 5)
        ], options => options.WithStrictOrdering());
    }

    // ── Yearly recurrence: Feb 29 start, non-leap-year fallback to Feb 28 ─────

    [Fact]
    public void YearlyRecurrence_Feb29Start_NonLeapYear_FallsBackToFeb28()
    {
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Years)
            .StartingFrom(new DateTime(2024, 2, 29)); // 2024 is a leap year

        schedule.OccursOn(new DateTime(2025, 2, 28)).Should().BeTrue();  // 2025 is not a leap year
        schedule.OccursOn(new DateTime(2025, 2, 27)).Should().BeFalse();
    }

    [Fact]
    public void YearlyRecurrence_Feb29Start_LeapYear_MatchesRealAnniversary()
    {
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Years)
            .StartingFrom(new DateTime(2024, 2, 29));

        schedule.OccursOn(new DateTime(2028, 2, 29)).Should().BeTrue(); // 2028 is a leap year
    }
}
