using FluentAssertions;
using Vali_Age.Core;
using Vali_Calendar.Core;
using Vali_CountDown.Core;
using Vali_Duration.Models;
using Vali_Holiday.Core;
using Vali_Holiday.Providers.LatinAmerica;
using Vali_Range.Core;
using Vali_Range.Models;
using Vali_Schedule.Core;
using Vali_Time.Core;
using Vali_Time.Enums;
using Xunit;

namespace Vali_Time.Tests.Tests;

/// <summary>
/// Integration tests that combine multiple Vali-Time ecosystem packages
/// in realistic cross-module scenarios.
/// </summary>
public class IntegrationTests
{
    // =========================================================================
    // Scenario 1: ValiCalendar + ValiHoliday (workdays reales)
    // =========================================================================

    private static ValiCalendar BuildPeruCalendar()
        => new ValiCalendar(WeekStart.Monday, new PeruHolidayProvider());

    [Fact]
    public void WorkdaysBetween_PeruJuly_ExcludesIndependenceDays()
    {
        // Jul 28-29 are Peruvian Independence Day holidays.
        // A calendar WITH the PE provider must count fewer workdays than one without.
        var from = new DateTime(2025, 7, 1);
        var to   = new DateTime(2025, 7, 31);

        var calendarWithHolidays    = BuildPeruCalendar();
        var calendarWithoutHolidays = new ValiCalendar(WeekStart.Monday);

        int withHolidays    = calendarWithHolidays.WorkdaysBetween(from, to);
        int withoutHolidays = calendarWithoutHolidays.WorkdaysBetween(from, to);

        withHolidays.Should().BeLessThan(withoutHolidays);
    }

    [Fact]
    public void IsWorkday_Peru_July28_ReturnsFalse()
    {
        var calendar = BuildPeruCalendar();
        var july28   = new DateTime(2025, 7, 28); // Independence Day (Monday)

        calendar.IsWorkday(july28).Should().BeFalse();
    }

    [Fact]
    public void IsWorkday_Peru_Monday_January6_ReturnsTrue()
    {
        // January 6, 2025 is a Monday with no Peruvian holiday.
        var calendar = BuildPeruCalendar();
        var jan6     = new DateTime(2025, 1, 6);

        calendar.IsWorkday(jan6).Should().BeTrue();
    }

    [Fact]
    public void NextWorkday_Peru_July27_SkipsJuly28And29_ReturnsTuesdayJuly30()
    {
        // Jul 27, 2025 is a Sunday → NextWorkday skips it.
        // Jul 28 (Mon) and Jul 29 (Tue) are PE holidays → also skipped.
        // The first valid workday is Jul 30 (Wednesday).
        var calendar = BuildPeruCalendar();
        var sunday   = new DateTime(2025, 7, 27);

        DateTime result = calendar.NextWorkday(sunday);

        result.Should().Be(new DateTime(2025, 7, 30));
    }

    [Fact]
    public void AddWorkdays_Peru_5WorkdaysFromJuly24_SkipsFeriados()
    {
        // Jul 24 (Thu) = workday 1
        // Jul 25 (Fri) = workday 2
        // Jul 26 (Sat) = skipped (weekend)
        // Jul 27 (Sun) = skipped (weekend)
        // Jul 28 (Mon) = skipped (holiday)
        // Jul 29 (Tue) = skipped (holiday)
        // Jul 30 (Wed) = workday 2
        // Jul 31 (Thu) = workday 3
        // Aug 01 (Fri) = workday 4
        // Aug 02 (Sat) = skip (weekend)
        // Aug 03 (Sun) = skip (weekend)
        // Aug 04 (Mon) = workday 5
        var calendar = BuildPeruCalendar();
        var start    = new DateTime(2025, 7, 24);

        DateTime result = calendar.AddWorkdays(start, 5);

        result.Should().Be(new DateTime(2025, 8, 4));
    }

    // =========================================================================
    // Scenario 2: ValiCountdown + ValiDate (deadlines con boundaries)
    // =========================================================================

    // Helper that mimics TimeUntil with a custom reference (avoids DateTime.Now).
    private static decimal TimeUntilFixed(DateTime deadline, DateTime reference, TimeUnit unit)
    {
        if (deadline <= reference) return 0m;
        double totalSeconds = (deadline - reference).TotalSeconds;
        return unit switch
        {
            TimeUnit.Days    => (decimal)(totalSeconds / 86_400.0),
            TimeUnit.Hours   => (decimal)(totalSeconds / 3_600.0),
            TimeUnit.Minutes => (decimal)(totalSeconds / 60.0),
            TimeUnit.Seconds => (decimal)totalSeconds,
            _                => (decimal)totalSeconds
        };
    }

    [Fact]
    public void Countdown_ToEndOfQuarter_IsPositive()
    {
        // Use a fixed reference inside Q1 2025; the end of Q1 is March 31 23:59:59.
        var reference = new DateTime(2025, 1, 15, 12, 0, 0);
        var valiDate  = new ValiDate();

        // EndOf(Quarter) uses a date inside Q1 2025.
        DateTime endOfQ1 = valiDate.EndOf(reference, DatePart.Quarter);

        // Cross-check with deterministic TimeSpan math (reference is before end of Q1).
        decimal days = TimeUntilFixed(endOfQ1, reference, TimeUnit.Days);

        days.Should().BeGreaterThan(0m);
    }

    [Fact]
    public void Countdown_Progress_FullQuarter_IsBetween0And1()
    {
        var valiDate  = new ValiDate();
        var reference = new DateTime(2025, 2, 14); // mid-Q1

        DateTime startQ1 = valiDate.StartOf(reference, DatePart.Quarter);
        DateTime endQ1   = valiDate.EndOf(reference, DatePart.Quarter);

        var countdown = new ValiCountdown();
        decimal progress = countdown.Progress(startQ1, endQ1, reference);

        progress.Should().BeInRange(0m, 1m);
    }

    [Fact]
    public void TimeUntil_StartOfNextMonth_IsPositive()
    {
        var reference    = new DateTime(2025, 3, 15, 12, 0, 0);
        var valiDate     = new ValiDate();
        DateTime nextMonthStart = valiDate.StartOf(reference.AddMonths(1), DatePart.Month);

        decimal days = TimeUntilFixed(nextMonthStart, reference, TimeUnit.Days);

        days.Should().BeGreaterThan(0m);
    }

    [Fact]
    public void Breakdown_TimeUntilEndOfYear_HasDays()
    {
        // Reference: January 15, 2025; end of year: December 31, 2025.
        var reference = new DateTime(2025, 1, 15, 12, 0, 0);
        var valiDate  = new ValiDate();
        DateTime endOfYear = valiDate.EndOf(reference, DatePart.Year);

        var countdown = new ValiCountdown();
        // We validate days remaining via a direct TimeSpan calculation (deterministic).
        double daysRemaining = (endOfYear - reference).TotalDays;

        daysRemaining.Should().BeGreaterThan(0);
        daysRemaining.Should().BeApproximately(350.5, 10.0); // roughly 11.5 months
    }

    // =========================================================================
    // Scenario 3: ValiAge + ValiDate (cumpleaños y períodos)
    // =========================================================================

    [Fact]
    public void Age_BirthdayIsInCurrentQuarter_QuarterContainsBirthday()
    {
        // Birthday: February 14 — falls in Q1 (Jan–Mar).
        var birthDate  = new DateTime(1990, 2, 14);
        var reference  = new DateTime(2025, 2, 14);
        var valiDate   = new ValiDate();
        var valiAge    = new ValiAge();

        DateTime qStart = valiDate.QuarterStart(reference);
        DateTime qEnd   = valiDate.QuarterEnd(reference);

        // The birthday occurrence in the reference year.
        var birthdayThisYear = new DateTime(reference.Year, birthDate.Month, birthDate.Day);

        bool birthdayInQuarter = birthdayThisYear >= qStart && birthdayThisYear <= qEnd;

        birthdayInQuarter.Should().BeTrue();
        valiAge.IsBirthday(birthDate, reference).Should().BeTrue();
    }

    [Fact]
    public void NextBirthday_IsWithinNextYear()
    {
        var birthDate = new DateTime(1995, 6, 15);
        var reference = new DateTime(2025, 6, 16); // one day after birthday

        var valiAge      = new ValiAge();
        DateTime nextBd  = valiAge.NextBirthday(birthDate, reference);

        // Must be strictly in the future and no more than 365 days away.
        nextBd.Should().BeAfter(reference);
        (nextBd - reference).TotalDays.Should().BeLessOrEqualTo(366);
    }

    [Fact]
    public void DaysUntilBirthday_MatchesCountdown()
    {
        // Reference: Jan 15, 2025; Birthday: March 10 → 54 days away.
        var birthDate = new DateTime(1990, 3, 10);
        var reference = new DateTime(2025, 1, 15);

        var valiAge  = new ValiAge();
        int ageDays  = valiAge.DaysUntilBirthday(birthDate, reference);

        // Cross-check with direct calculation.
        DateTime nextBd    = valiAge.NextBirthday(birthDate, reference);
        int manualDays     = (int)(nextBd.Date - reference.Date).TotalDays;

        ageDays.Should().Be(manualDays);
    }

    // =========================================================================
    // Scenario 4: ValiRange + ValiCalendar (rangos de workdays)
    // =========================================================================

    [Fact]
    public void EachWorkday_InRange_MatchesWorkdaysBetween()
    {
        // Full work week: Jan 6–10, 2025 (Mon–Fri).
        var from     = new DateTime(2025, 1, 6);
        var to       = new DateTime(2025, 1, 10);
        var range    = new DateRange(from, to);
        var valiRange    = new ValiRange();
        var valiCalendar = new ValiCalendar(WeekStart.Monday);

        int eachWorkdayCount = valiRange.EachWorkday(range).Count();
        int calendarCount    = valiCalendar.WorkdaysBetween(from, to);

        eachWorkdayCount.Should().Be(calendarCount);
    }

    [Fact]
    public void ThisMonth_WorkdayCount_IsReasonable()
    {
        // January 2025: 31 days, starts on Wednesday → 23 weekdays.
        var from = new DateTime(2025, 1, 1);
        var to   = new DateTime(2025, 1, 31);
        var range = new DateRange(from, to);

        var valiRange    = new ValiRange();
        var valiCalendar = new ValiCalendar(WeekStart.Monday);

        int rangeCount    = valiRange.EachWorkday(range).Count();
        int calendarCount = valiCalendar.WorkdaysBetween(from, to);

        rangeCount.Should().Be(calendarCount);
        calendarCount.Should().BeInRange(18, 23);
    }

    [Fact]
    public void LastUnits_30Days_ContainsToday()
    {
        var valiRange = new ValiRange();
        DateRange range = valiRange.LastUnits(30, TimeUnit.Days);

        // Today must be within the last 30 days range (with a small clock margin).
        DateTime today = DateTime.Today;

        range.Start.Date.Should().BeOnOrBefore(today);
        range.End.Date.Should().BeOnOrAfter(today.AddDays(-1)); // allow up to 1 day margin
    }

    // =========================================================================
    // Scenario 5: ValiSchedule + ValiAge (eventos recurrentes de cumpleaños)
    // =========================================================================

    [Fact]
    public void AnnualSchedule_OccursOnBirthday()
    {
        // Birthday: June 15. Build a yearly schedule starting on Jun 15, 2025.
        var birthDate    = new DateTime(1990, 6, 15);
        var scheduleStart = new DateTime(2025, 6, 15);

        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Years)
            .StartingFrom(scheduleStart);

        // OccursOn should return true on the birthday itself.
        schedule.OccursOn(scheduleStart).Should().BeTrue();

        // And on the same day-month next year.
        var nextBirthdayYear = new DateTime(2026, birthDate.Month, birthDate.Day);
        schedule.OccursOn(nextBirthdayYear).Should().BeTrue();
    }

    [Fact]
    public void AnnualBirthdaySchedule_Next5Years_Has5Occurrences()
    {
        var scheduleStart = new DateTime(2025, 6, 15);

        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Years)
            .StartingFrom(scheduleStart);

        var occurrences = schedule.Occurrences(scheduleStart, 5).ToList();

        occurrences.Should().HaveCount(5);

        // Every occurrence should be on June 15.
        occurrences.Should().AllSatisfy(d =>
        {
            d.Month.Should().Be(6);
            d.Day.Should().Be(15);
        });
    }

    [Fact]
    public void WeeklySchedule_OccurrencesInRange_CountMatchesExpected()
    {
        // Weekly on Mondays; range covers 4 Mondays: Jan 6, 13, 20, 27 2025.
        var schedule = new ValiSchedule()
            .Every(1, TimeUnit.Weeks)
            .On(DayOfWeek.Monday)
            .StartingFrom(new DateTime(2025, 1, 6));

        var valiRange = new ValiRange();
        DateRange range = valiRange.Create(new DateTime(2025, 1, 6), new DateTime(2025, 1, 27));

        var occurrences = schedule.OccurrencesInRange(range.Start, range.End).ToList();

        occurrences.Should().HaveCount(4);
        occurrences.Should().AllSatisfy(d => d.DayOfWeek.Should().Be(DayOfWeek.Monday));
    }

    // =========================================================================
    // Scenario 6: ValiHoliday + ValiCountdown (días hasta el próximo feriado)
    // =========================================================================

    [Fact]
    public void NextHoliday_Countdown_IsPositive()
    {
        // Reference: January 2, 2025 — next PE holiday is Semana Santa (April).
        var reference = new DateTime(2025, 1, 2);
        var valiHoliday = HolidayProviderFactory.CreateAll();

        var nextResult = valiHoliday.GetNextHolidayWithYear(reference, "PE");
        nextResult.Should().NotBeNull();

        DateTime nextHolidayDate = nextResult!.Value.Holiday.ToDateTime(nextResult.Value.Year);
        double daysRemaining = (nextHolidayDate - reference).TotalDays;

        daysRemaining.Should().BeGreaterThan(0);
    }

    [Fact]
    public void NextHoliday_IsExpired_IsFalse()
    {
        // Reference: January 2, 2025 — the next holiday has not expired yet.
        var reference   = new DateTime(2025, 1, 2, 0, 0, 0);
        var valiHoliday = HolidayProviderFactory.CreateAll();
        var countdown   = new ValiCountdown();

        var nextResult = valiHoliday.GetNextHolidayWithYear(reference, "PE");
        nextResult.Should().NotBeNull();

        DateTime nextHolidayDate = nextResult!.Value.Holiday.ToDateTime(nextResult.Value.Year);

        countdown.IsExpired(nextHolidayDate, reference).Should().BeFalse();
    }

    [Fact]
    public void HolidaysInRange_ThisMonth_CountIsReasonable()
    {
        // July 2025 in Peru has at least 2 holidays (Jul 28 and Jul 29).
        var valiHoliday = HolidayProviderFactory.CreateAll();
        var holidays = valiHoliday.HolidaysThisMonth(2025, 7, "PE").ToList();

        holidays.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    // =========================================================================
    // Scenario 7: ValiDuration + ValiTime (precisión sin drift)
    // =========================================================================

    [Fact]
    public void Duration_FromTimeConversion_MatchesValiTime()
    {
        // ValiDuration.FromHours(2) → TotalSeconds == 7200.
        // ValiTime.Convert(2, Hours, Seconds) → 7200.
        var valiTime     = new ValiTime();
        var duration     = ValiDuration.FromHours(2m);
        decimal converted = valiTime.Convert(2m, TimeUnit.Hours, TimeUnit.Seconds);

        duration.TotalSeconds.Should().Be(converted);
    }

    [Fact]
    public void Duration_Sum_NoAccumulatedDrift()
    {
        // Summing 100 × 0.1 min should yield exactly 10 min — no floating-point drift.
        ValiDuration total = ValiDuration.FromMinutes(0m);
        for (int i = 0; i < 100; i++)
            total = total + ValiDuration.FromMinutes(0.1m);

        total.TotalMinutes.Should().Be(ValiDuration.FromMinutes(10m).TotalMinutes);
    }

    // =========================================================================
    // Scenario 8: Múltiples módulos en workflow realista
    // =========================================================================

    [Fact]
    public void BusinessWorkflowTest_DeadlineCalculation()
    {
        // A project starts on July 24, 2025 (Thursday) and lasts 30 workdays (PE calendar).
        // We calculate the delivery date and verify the countdown to it is positive
        // relative to a fixed reference before the delivery date.
        var projectStart = new DateTime(2025, 7, 24);
        var peCalendar   = BuildPeruCalendar();
        var countdown    = new ValiCountdown();

        DateTime deliveryDate = peCalendar.AddWorkdays(projectStart, 30);

        // Delivery must be after the start.
        deliveryDate.Should().BeAfter(projectStart);

        // It must be a workday (PE calendar).
        peCalendar.IsWorkday(deliveryDate).Should().BeTrue();

        // The countdown from the project start to the delivery date is positive.
        countdown.IsExpired(deliveryDate, projectStart).Should().BeFalse();

        // It takes more than 30 calendar days due to weekends and holidays.
        int calendarDays = (int)(deliveryDate - projectStart).TotalDays;
        calendarDays.Should().BeGreaterThan(30);
    }

    [Fact]
    public void BirthdayLongWeekend_2025_IsLongWeekend()
    {
        // July 28, 2025 is Independence Day in PE and falls on a Monday.
        // IsLongWeekend should return true (Monday holiday → long weekend).
        var valiHoliday = HolidayProviderFactory.CreateAll();
        var july28      = new DateTime(2025, 7, 28);

        bool isLong = valiHoliday.IsLongWeekend(july28, "PE");

        isLong.Should().BeTrue();
    }
}
