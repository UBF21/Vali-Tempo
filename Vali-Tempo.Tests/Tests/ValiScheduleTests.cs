using FluentAssertions;
using Vali_Schedule.Core;
using Vali_Time.Enums;
using Xunit;

namespace Vali_Time.Tests.Tests;

public class ValiScheduleTests
{
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
}
