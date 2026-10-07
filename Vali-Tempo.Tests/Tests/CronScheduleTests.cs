using FluentAssertions;
using Vali_Schedule.Core;
using Vali_Schedule.Cron;
using Vali_Time.Enums;
using Xunit;

namespace Vali_Time.Tests.Tests;

public class CronScheduleTests
{
    // ── Parsing: typical expressions ─────────────────────────────────────────

    [Fact]
    public void Parse_BusinessHoursWeekdays_NextOccurrence_SkipsWeekend()
    {
        // Jan 1 2025 is a Wednesday; Jan 4 is Saturday; next weekday is Monday Jan 6.
        var schedule = CronSchedule.Parse("0 9 * * MON-FRI").StartingFrom(new DateTime(2025, 1, 1));

        var next = schedule.NextOccurrence(new DateTime(2025, 1, 4));

        next.Should().Be(new DateTime(2025, 1, 6, 9, 0, 0));
    }

    [Fact]
    public void Parse_BusinessHoursWeekdays_DoesNotOccurOnSaturday()
    {
        var schedule = CronSchedule.Parse("0 9 * * MON-FRI").StartingFrom(new DateTime(2025, 1, 1));

        schedule.OccursOn(new DateTime(2025, 1, 4)).Should().BeFalse();
        schedule.OccursOn(new DateTime(2025, 1, 7)).Should().BeTrue();
    }

    [Fact]
    public void Parse_EveryFifteenMinutesBusinessHours_NextOccurrence_StepsCorrectly()
    {
        var schedule = CronSchedule.Parse("*/15 9-17 * * MON-FRI").StartingFrom(new DateTime(2025, 1, 1));

        var next = schedule.NextOccurrence(new DateTime(2025, 1, 6, 9, 5, 0));

        next.Should().Be(new DateTime(2025, 1, 6, 9, 15, 0));
    }

    [Fact]
    public void Parse_Quarterly_NextOccurrence_JumpsToNextQuarterMonth()
    {
        var schedule = CronSchedule.Parse("0 0 1 1,4,7,10 *").StartingFrom(new DateTime(2025, 1, 1));

        var next = schedule.NextOccurrence(new DateTime(2025, 1, 2));

        next.Should().Be(new DateTime(2025, 4, 1, 0, 0, 0));
    }

    [Fact]
    public void Parse_Payroll_Occurrences_ReturnsFirstAndFifteenth()
    {
        var schedule = CronSchedule.Parse("0 6 1,15 * *").StartingFrom(new DateTime(2025, 1, 1));

        var results = schedule.Occurrences(new DateTime(2025, 1, 1), 4).ToList();

        results.Should().BeEquivalentTo(
        [
            new DateTime(2025, 1, 1, 6, 0, 0),
            new DateTime(2025, 1, 15, 6, 0, 0),
            new DateTime(2025, 2, 1, 6, 0, 0),
            new DateTime(2025, 2, 15, 6, 0, 0)
        ], options => options.WithStrictOrdering());
    }

    [Fact]
    public void Parse_NameAliases_CaseInsensitive_BehaveLikeNumericEquivalent()
    {
        var byName = CronSchedule.Parse("0 9 * jan,JUL mon-FRI").StartingFrom(new DateTime(2025, 1, 1));
        var byNumber = CronSchedule.Parse("0 9 * 1,7 1-5").StartingFrom(new DateTime(2025, 1, 1));

        var byNameResults = byName.OccurrencesInRange(new DateTime(2025, 1, 1), new DateTime(2025, 1, 10)).ToList();
        var byNumberResults = byNumber.OccurrencesInRange(new DateTime(2025, 1, 1), new DateTime(2025, 1, 10)).ToList();

        byNameResults.Should().BeEquivalentTo(byNumberResults);
    }

    // ── Semantics: day-of-month / day-of-week OR-rule ────────────────────────

    [Fact]
    public void DomDowOrRule_BothRestricted_MatchesEitherSide()
    {
        // 0 0 1,15 * MON over January 2025: 1st, 15th, AND every Monday (6, 13, 20, 27).
        var schedule = CronSchedule.Parse("0 0 1,15 * MON").StartingFrom(new DateTime(2025, 1, 1));

        var results = schedule.OccurrencesInRange(new DateTime(2025, 1, 1), new DateTime(2025, 1, 31)).ToList();

        results.Should().BeEquivalentTo(
        [
            new DateTime(2025, 1, 1),
            new DateTime(2025, 1, 6),
            new DateTime(2025, 1, 13),
            new DateTime(2025, 1, 15),
            new DateTime(2025, 1, 20),
            new DateTime(2025, 1, 27)
        ]);
    }

    [Fact]
    public void DomDowOrRule_OnlyDomRestricted_RequiresOnlyDom()
    {
        var schedule = CronSchedule.Parse("0 0 1 * *").StartingFrom(new DateTime(2025, 1, 1));

        schedule.OccursOn(new DateTime(2025, 1, 1)).Should().BeTrue();
        schedule.OccursOn(new DateTime(2025, 1, 6)).Should().BeFalse(); // a Monday, but day-of-week is wildcard-only, not OR'd in
    }

    // ── Edge cases: Feb 29 and never-matching expressions ────────────────────

    [Fact]
    public void Feb29Cron_OnlyMatchesLeapYears_SkipsToNextLeapYear()
    {
        var schedule = CronSchedule.Parse("0 0 29 2 *");

        var next = schedule.NextOccurrence(new DateTime(2025, 1, 1));

        next.Should().Be(new DateTime(2028, 2, 29));
    }

    [Fact]
    public void NeverMatchingExpression_Feb30_ReturnsNullWithoutInfiniteLoop()
    {
        var schedule = CronSchedule.Parse("30 2 30 2 *");

        var next = schedule.NextOccurrence(new DateTime(2025, 1, 1));
        var occurrences = schedule.Occurrences(new DateTime(2025, 1, 1), 5).ToList();

        next.Should().BeNull();
        occurrences.Should().BeEmpty();
    }

    // ── StartingFrom / EndsAfter / EndsOn ─────────────────────────────────────

    [Fact]
    public void EndsAfter_LimitsOccurrenceCount()
    {
        var schedule = CronSchedule.Parse("0 9 * * *")
            .StartingFrom(new DateTime(2025, 1, 1))
            .EndsAfter(3);

        var results = schedule.Occurrences(new DateTime(2025, 1, 1), 10).ToList();

        results.Should().HaveCount(3);
        results[2].Should().Be(new DateTime(2025, 1, 3, 9, 0, 0));
    }

    [Fact]
    public void EndsOn_StopsAtConfiguredDate()
    {
        var schedule = CronSchedule.Parse("0 9 * * *")
            .StartingFrom(new DateTime(2025, 1, 1))
            .EndsOn(new DateTime(2025, 1, 3));

        var next = schedule.NextOccurrence(new DateTime(2025, 1, 4));

        next.Should().BeNull();
    }

    // ── WithCustomPredicate: AND-combined, chainable ─────────────────────────

    [Fact]
    public void WithCustomPredicate_ChainedTwice_NarrowsFurtherEachTime()
    {
        var schedule = CronSchedule.Parse("0 9 * * *")
            .StartingFrom(new DateTime(2025, 1, 1))
            .WithCustomPredicate(d => d.Day % 2 == 1)
            .WithCustomPredicate(d => d.Month == 1);

        var results = schedule.Occurrences(new DateTime(2025, 1, 1), 5).ToList();

        results.Should().BeEquivalentTo(
        [
            new DateTime(2025, 1, 1, 9, 0, 0),
            new DateTime(2025, 1, 3, 9, 0, 0),
            new DateTime(2025, 1, 5, 9, 0, 0),
            new DateTime(2025, 1, 7, 9, 0, 0),
            new DateTime(2025, 1, 9, 9, 0, 0)
        ]);
    }

    // ── Unsupported builder members throw NotSupportedException ──────────────

    [Fact]
    public void Every_ThrowsNotSupportedException()
    {
        var schedule = CronSchedule.Parse("0 9 * * *");
        var act = () => schedule.Every(1, TimeUnit.Days);
        act.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void On_ThrowsNotSupportedException()
    {
        var schedule = CronSchedule.Parse("0 9 * * *");
        var act = () => schedule.On(DayOfWeek.Monday);
        act.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void At_ThrowsNotSupportedException()
    {
        var schedule = CronSchedule.Parse("0 9 * * *");
        var act = () => schedule.At(new TimeOnly(9, 0));
        act.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void OnDayOfMonth_ThrowsNotSupportedException()
    {
        var schedule = CronSchedule.Parse("0 9 * * *");
        var act = () => schedule.OnDayOfMonth(1);
        act.Should().Throw<NotSupportedException>();
    }

    // ── SkipWeekends: drop semantics (documented divergence from ValiSchedule) ──

    [Fact]
    public void SkipWeekends_DropsWeekendOccurrencesWithoutShifting()
    {
        var schedule = CronSchedule.Parse("0 9 * * *")
            .StartingFrom(new DateTime(2025, 1, 1))
            .SkipWeekends();

        // Jan 4/5 2025 are Sat/Sun — dropped entirely, not shifted to Jan 6.
        var results = schedule.OccurrencesInRange(new DateTime(2025, 1, 3), new DateTime(2025, 1, 6)).ToList();

        results.Should().BeEquivalentTo(
        [
            new DateTime(2025, 1, 3, 9, 0, 0),
            new DateTime(2025, 1, 6, 9, 0, 0)
        ]);
    }

    // ── Error path: CronFormatException field/token attribution ──────────────

    [Fact]
    public void Parse_HourOutOfRange_ThrowsWithHourFieldAndToken()
    {
        var act = () => CronSchedule.Parse("0 25 * * *");

        act.Should().Throw<CronFormatException>()
            .Which.Should().Match<CronFormatException>(e => e.Field == "hour" && e.Token == "25");
    }

    [Fact]
    public void Parse_UnrecognizedDayOfWeekName_ThrowsWithDayOfWeekField()
    {
        var act = () => CronSchedule.Parse("* * * * MON-XYZ");

        act.Should().Throw<CronFormatException>()
            .Which.Should().Match<CronFormatException>(e => e.Field == "day-of-week" && e.Token == "MON-XYZ");
    }

    [Fact]
    public void Parse_ZeroStep_ThrowsWithMinuteField()
    {
        var act = () => CronSchedule.Parse("*/0 * * * *");

        act.Should().Throw<CronFormatException>()
            .Which.Should().Match<CronFormatException>(e => e.Field == "minute");
    }

    [Fact]
    public void Parse_WrongFieldCount_ThrowsWithFieldCountField()
    {
        var act = () => CronSchedule.Parse("0 9 * *");

        act.Should().Throw<CronFormatException>()
            .Which.Should().Match<CronFormatException>(e => e.Field == "field-count");
    }

    [Fact]
    public void Parse_GarbageToken_ThrowsWithMinuteFieldAndToken()
    {
        var act = () => CronSchedule.Parse("a b c d e");

        act.Should().Throw<CronFormatException>()
            .Which.Should().Match<CronFormatException>(e => e.Field == "minute" && e.Token == "a");
    }

    [Fact]
    public void Parse_DescendingRange_Throws()
    {
        var act = () => CronSchedule.Parse("* 17-9 * * *");

        act.Should().Throw<CronFormatException>()
            .Which.Field.Should().Be("hour");
    }

    [Fact]
    public void Parse_NullOrWhitespace_ThrowsArgumentNullException()
    {
        var act = () => CronSchedule.Parse("   ");
        act.Should().Throw<ArgumentNullException>();
    }

    // ── TryParse: never throws, surfaces error text ──────────────────────────

    [Fact]
    public void TryParse_ValidExpression_ReturnsTrueWithSchedule()
    {
        bool ok = CronSchedule.TryParse("0 9 * * MON-FRI", out var schedule, out var error);

        ok.Should().BeTrue();
        schedule.Should().NotBeNull();
        error.Should().BeNull();
    }

    [Fact]
    public void TryParse_InvalidExpression_ReturnsFalseWithError()
    {
        bool ok = CronSchedule.TryParse("0 25 * * *", out var schedule, out var error);

        ok.Should().BeFalse();
        schedule.Should().BeNull();
        error.Should().NotBeNullOrWhiteSpace();
    }

    // ── Parity with ValiSchedule where semantics overlap ─────────────────────

    [Fact]
    public void Parity_CronWeeklyMonday_MatchesEquivalentValiSchedule()
    {
        var cron = CronSchedule.Parse("0 9 * * 1").StartingFrom(new DateTime(2025, 1, 1));
        var native = new ValiSchedule()
            .Every(1, TimeUnit.Weeks)
            .On(DayOfWeek.Monday)
            .At(new TimeOnly(9, 0))
            .StartingFrom(new DateTime(2025, 1, 1));

        var cronResults = cron.OccurrencesInRange(new DateTime(2025, 1, 1), new DateTime(2025, 1, 31)).ToList();
        var nativeResults = native.OccurrencesInRange(new DateTime(2025, 1, 1), new DateTime(2025, 1, 31)).ToList();

        cronResults.Should().BeEquivalentTo(nativeResults);
    }
}
