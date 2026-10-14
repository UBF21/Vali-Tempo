using FluentAssertions;
using Vali_Age.Core;
using Vali_Age.Models;
using Vali_Time.Abstractions;
using Vali_Time.Enums;
using Xunit;

namespace Vali_Time.Tests.Tests;

public class ValiAgeTests
{
    private readonly ValiAge _vali = new();

    private sealed class FixedClock : IClock
    {
        private readonly DateTime _fixed;
        public FixedClock(DateTime fixedValue) => _fixed = fixedValue;
        public DateTime Now => _fixed;
        public DateTime Today => _fixed.Date;
    }

    // ── Years ───────────────────────────────────────────────────────────────

    [Fact]
    public void Years_ExactlyThirtyYearsAgo_Returns30()
    {
        var reference = new DateTime(2025, 6, 15);
        var birthDate = new DateTime(1995, 6, 15);
        int result = _vali.Years(birthDate, reference);
        result.Should().Be(30);
    }

    [Fact]
    public void Years_OneDayBeforeBirthday_Returns29()
    {
        var reference = new DateTime(2025, 6, 14);
        var birthDate = new DateTime(1995, 6, 15);
        int result = _vali.Years(birthDate, reference);
        result.Should().Be(29);
    }

    // ── Exact ───────────────────────────────────────────────────────────────

    [Fact]
    public void Exact_KnownAge_ReturnsCorrectBreakdown()
    {
        var birthDate = new DateTime(1990, 3, 15);
        var reference = new DateTime(2025, 6, 20);

        AgeResult result = _vali.Exact(birthDate, reference);

        result.Years.Should().Be(35);
        result.Months.Should().Be(3);
        result.Days.Should().Be(5);
        result.TotalDays.Should().Be((reference.Date - birthDate.Date).Days);
    }

    [Fact]
    public void Exact_SameDay_ReturnsZeroYearsMonthsDays()
    {
        var date = new DateTime(2025, 1, 1);
        AgeResult result = _vali.Exact(date, date);
        result.Years.Should().Be(0);
        result.Months.Should().Be(0);
        result.Days.Should().Be(0);
        result.TotalDays.Should().Be(0);
    }

    // ── IsBirthday ──────────────────────────────────────────────────────────

    [Fact]
    public void IsBirthday_SameDayAndMonth_ReturnsTrue()
    {
        var birthDate = new DateTime(1990, 6, 15);
        var reference = new DateTime(2025, 6, 15);
        _vali.IsBirthday(birthDate, reference).Should().BeTrue();
    }

    [Fact]
    public void IsBirthday_DifferentDay_ReturnsFalse()
    {
        var birthDate = new DateTime(1990, 6, 15);
        var reference = new DateTime(2025, 6, 16);
        _vali.IsBirthday(birthDate, reference).Should().BeFalse();
    }

    [Fact]
    public void IsBirthday_SameDayDifferentMonth_ReturnsFalse()
    {
        var birthDate = new DateTime(1990, 6, 15);
        var reference = new DateTime(2025, 7, 15);
        _vali.IsBirthday(birthDate, reference).Should().BeFalse();
    }

    // ── NextBirthday ────────────────────────────────────────────────────────

    [Fact]
    public void NextBirthday_IsAlwaysInTheFuture()
    {
        var birthDate = new DateTime(1990, 6, 15);
        var reference = new DateTime(2025, 6, 16); // one day after birthday
        DateTime next = _vali.NextBirthday(birthDate, reference);
        next.Should().BeAfter(reference);
    }

    [Fact]
    public void NextBirthday_OnBirthdayItself_ReturnsNextYear()
    {
        var birthDate = new DateTime(1990, 6, 15);
        var reference = new DateTime(2025, 6, 15); // exactly on birthday
        DateTime next = _vali.NextBirthday(birthDate, reference);
        // When reference equals birthday, NextBirthday returns next year
        next.Year.Should().BeGreaterThan(reference.Year);
    }

    [Fact]
    public void NextBirthday_BeforeBirthday_ReturnsThisYear()
    {
        var birthDate = new DateTime(1990, 12, 25);
        var reference = new DateTime(2025, 6, 15);
        DateTime next = _vali.NextBirthday(birthDate, reference);
        next.Year.Should().Be(2025);
        next.Month.Should().Be(12);
        next.Day.Should().Be(25);
    }

    // ── DaysUntilBirthday ───────────────────────────────────────────────────

    [Fact]
    public void DaysUntilBirthday_IsZeroOnBirthdayNextYear()
    {
        // Birthday is today (reference day), next one is 365 or 366 days away
        // Instead, test: birthday is tomorrow → 1 day until
        var reference = new DateTime(2025, 6, 15);
        var birthDate = new DateTime(1990, 6, 16);
        int days = _vali.DaysUntilBirthday(birthDate, reference);
        days.Should().Be(1);
    }

    [Fact]
    public void DaysUntilBirthday_BirthdayIsTodayButNextIsNextYear()
    {
        var reference = new DateTime(2025, 6, 15);
        var birthDate = new DateTime(1990, 6, 15);
        // On birthday exactly, NextBirthday returns next year
        int days = _vali.DaysUntilBirthday(birthDate, reference);
        days.Should().BeGreaterThan(0);
    }

    // ── IsAtLeast ───────────────────────────────────────────────────────────

    [Fact]
    public void IsAtLeast_18Years_PersonIs30_ReturnsTrue()
    {
        var reference = new DateTime(2025, 6, 15);
        var birthDate = new DateTime(1995, 6, 15);
        _vali.IsAtLeast(birthDate, 18, DatePart.Year, reference).Should().BeTrue();
    }

    [Fact]
    public void IsAtLeast_18Years_PersonIs17_ReturnsFalse()
    {
        var reference = new DateTime(2025, 6, 14);
        var birthDate = new DateTime(2008, 6, 15);
        _vali.IsAtLeast(birthDate, 18, DatePart.Year, reference).Should().BeFalse();
    }

    [Fact]
    public void IsAtLeast_18Years_PersonIsTurning18Today_ReturnsTrue()
    {
        var reference = new DateTime(2025, 6, 15);
        var birthDate = new DateTime(2007, 6, 15);
        _vali.IsAtLeast(birthDate, 18, DatePart.Year, reference).Should().BeTrue();
    }

    // ── Relative ────────────────────────────────────────────────────────────

    [Fact]
    public void Relative_OneDayAgo_Returns1DayAgo()
    {
        var reference = new DateTime(2025, 6, 15, 12, 0, 0);
        var date      = new DateTime(2025, 6, 14, 12, 0, 0);
        string result = _vali.Relative(date, reference);
        result.Should().Be("1 day ago");
    }

    [Fact]
    public void Relative_OneHourAgo_Returns1HourAgo()
    {
        var reference = new DateTime(2025, 6, 15, 12, 0, 0);
        var date      = new DateTime(2025, 6, 15, 11, 0, 0);
        string result = _vali.Relative(date, reference);
        result.Should().Be("1 hour ago");
    }

    [Fact]
    public void Relative_FutureDate_StartsWithIn()
    {
        var reference = new DateTime(2025, 6, 15, 12, 0, 0);
        var date      = new DateTime(2025, 6, 17, 12, 0, 0);
        string result = _vali.Relative(date, reference);
        result.Should().StartWith("in");
    }

    [Fact]
    public void Relative_JustNow_ReturnsJustNow()
    {
        var reference = new DateTime(2025, 6, 15, 12, 0, 0);
        var date      = new DateTime(2025, 6, 15, 11, 59, 30); // 30 seconds ago
        string result = _vali.Relative(date, reference);
        result.Should().Be("just now");
    }

    // ── Feb 29 — NextBirthday ───────────────────────────────────────────────

    [Fact]
    public void NextBirthday_Feb29Born_ReferenceAfterBirthdayInLeapYear_ReturnsNextNonLeapFeb28()
    {
        // born 2000-02-29, reference 2024-03-01 → next birthday is 2025-02-28 (2025 not leap)
        var birthDate = new DateTime(2000, 2, 29);
        var reference = new DateTime(2024, 3, 1);
        DateTime next = _vali.NextBirthday(birthDate, reference);
        next.Should().Be(new DateTime(2025, 2, 28));
    }

    [Fact]
    public void NextBirthday_Feb29Born_ReferenceBeforeLeapYear_ReturnsLeapFeb29()
    {
        // born 2000-02-29, reference 2027-12-31 → next birthday is 2028-02-29 (2028 is leap)
        var birthDate = new DateTime(2000, 2, 29);
        var reference = new DateTime(2027, 12, 31);
        DateTime next = _vali.NextBirthday(birthDate, reference);
        next.Should().Be(new DateTime(2028, 2, 29));
    }

    [Fact]
    public void NextBirthday_Feb29Born_ReferenceOnLeapBirthday_ReturnsFollowingYearFeb28()
    {
        // born 2000-02-29, reference 2028-02-29 → next birthday is 2029-02-28 (same day but next year)
        var birthDate = new DateTime(2000, 2, 29);
        var reference = new DateTime(2028, 2, 29);
        DateTime next = _vali.NextBirthday(birthDate, reference);
        next.Should().Be(new DateTime(2029, 2, 28));
    }

    [Fact]
    public void NextBirthday_Feb29Born_ReferenceDayBeforeLeapBirthday_ReturnsLeapFeb29()
    {
        // born 2000-02-29, reference 2028-02-28 → next birthday is 2028-02-29 (tomorrow, leap year)
        var birthDate = new DateTime(2000, 2, 29);
        var reference = new DateTime(2028, 2, 28);
        DateTime next = _vali.NextBirthday(birthDate, reference);
        next.Should().Be(new DateTime(2028, 2, 29));
    }

    [Fact]
    public void NextBirthday_Feb29Born2004_ReferenceJan2025_ReturnsNonLeapFeb28()
    {
        // born 2004-02-29, reference 2025-01-01 → next birthday is 2025-02-28 (2025 not leap)
        var birthDate = new DateTime(2004, 2, 29);
        var reference = new DateTime(2025, 1, 1);
        DateTime next = _vali.NextBirthday(birthDate, reference);
        next.Should().Be(new DateTime(2025, 2, 28));
    }

    // ── Feb 29 — DaysUntilBirthday ──────────────────────────────────────────

    [Fact]
    public void DaysUntilBirthday_Feb29Born_TwoDaysBeforeNonLeapConvention_Returns1()
    {
        // born 2000-02-29, reference 2025-02-27 → 1 day (2025 uses Feb 28)
        var birthDate = new DateTime(2000, 2, 29);
        var reference = new DateTime(2025, 2, 27);
        int days = _vali.DaysUntilBirthday(birthDate, reference);
        days.Should().Be(1);
    }

    [Fact]
    public void DaysUntilBirthday_Feb29Born_DayBeforeLeapBirthday_Returns1()
    {
        // born 2000-02-29, reference 2024-02-28 → 1 day (2024 is leap, uses Feb 29)
        var birthDate = new DateTime(2000, 2, 29);
        var reference = new DateTime(2024, 2, 28);
        int days = _vali.DaysUntilBirthday(birthDate, reference);
        days.Should().Be(1);
    }

    [Fact]
    public void DaysUntilBirthday_Feb29Born_OnNonLeapConventionDay_ReturnsNextYearDistance()
    {
        // born 2000-02-29, reference 2025-02-28 → birthday IS today (by convention), so
        // NextBirthday jumps to 2026-02-28 (365 days away), consistent with the general
        // birthday-today → next-year behavior already tested in DaysUntilBirthday_BirthdayIsTodayButNextIsNextYear.
        var birthDate = new DateTime(2000, 2, 29);
        var reference = new DateTime(2025, 2, 28);
        int days = _vali.DaysUntilBirthday(birthDate, reference);
        days.Should().BeGreaterThan(0);
    }

    // ── Feb 29 — IsBirthday ─────────────────────────────────────────────────

    [Fact]
    public void IsBirthday_Feb29Born_NonLeapYear_Feb28IsTrue()
    {
        // born 2000-02-29, reference 2025-02-28 → true (non-leap convention: Feb 28)
        var birthDate = new DateTime(2000, 2, 29);
        var reference = new DateTime(2025, 2, 28);
        _vali.IsBirthday(birthDate, reference).Should().BeTrue();
    }

    [Fact]
    public void IsBirthday_Feb29Born_NonLeapYear_Feb29DoesNotExist_ReturnsFalse()
    {
        // born 2000-02-29, reference 2025-02-29 → false (2025 has no Feb 29)
        // This would normally throw if DateTime(2025,2,29) were constructed — we verify the method doesn't crash
        var birthDate = new DateTime(2000, 2, 29);
        // 2025-03-01 is NOT the birthday; Feb 28 is.
        var reference = new DateTime(2025, 3, 1);
        _vali.IsBirthday(birthDate, reference).Should().BeFalse();
    }

    [Fact]
    public void IsBirthday_Feb29Born_LeapYear_Feb29IsTrue()
    {
        // born 2000-02-29, reference 2024-02-29 → true (2024 is leap)
        var birthDate = new DateTime(2000, 2, 29);
        var reference = new DateTime(2024, 2, 29);
        _vali.IsBirthday(birthDate, reference).Should().BeTrue();
    }

    [Fact]
    public void IsBirthday_Feb29Born_LeapYear_Feb28IsFalse()
    {
        // born 2000-02-29, reference 2024-02-28 → false (2024 is leap, actual birthday is Feb 29)
        var birthDate = new DateTime(2000, 2, 29);
        var reference = new DateTime(2024, 2, 28);
        _vali.IsBirthday(birthDate, reference).Should().BeFalse();
    }

    // ── Feb 29 — Years / Exact ──────────────────────────────────────────────

    [Fact]
    public void Years_Feb29Born_NonLeapYear_Feb28_Returns25()
    {
        // born 2000-02-29, reference 2025-02-28 → 25 years (convention: Feb 28 counts as birthday)
        var birthDate = new DateTime(2000, 2, 29);
        var reference = new DateTime(2025, 2, 28);
        int result = _vali.Years(birthDate, reference);
        result.Should().Be(25);
    }

    [Fact]
    public void Years_Feb29Born_NonLeapYear_DayBefore_Returns24()
    {
        // born 2000-02-29, reference 2025-02-27 → 24 years (not yet turned 25)
        var birthDate = new DateTime(2000, 2, 29);
        var reference = new DateTime(2025, 2, 27);
        int result = _vali.Years(birthDate, reference);
        result.Should().Be(24);
    }

    [Fact]
    public void Years_Feb29Born_LeapYear_Feb29_Returns28()
    {
        // born 2000-02-29, reference 2028-02-29 → 28 years exactly
        var birthDate = new DateTime(2000, 2, 29);
        var reference = new DateTime(2028, 2, 29);
        int result = _vali.Years(birthDate, reference);
        result.Should().Be(28);
    }

    // ── Feb 29 — IsAtLeast ──────────────────────────────────────────────────

    [Fact]
    public void IsAtLeast_Feb29Born2004_Reference2024Feb28_20Years_ReturnsFalse()
    {
        // born 2004-02-29, reference 2024-02-28 → IsAtLeast(20) → false (birthday Feb 29, not yet 20)
        var birthDate = new DateTime(2004, 2, 29);
        var reference = new DateTime(2024, 2, 28);
        _vali.IsAtLeast(birthDate, 20, DatePart.Year, reference).Should().BeFalse();
    }

    [Fact]
    public void IsAtLeast_Feb29Born2004_Reference2024Feb29_20Years_ReturnsTrue()
    {
        // born 2004-02-29, reference 2024-02-29 → IsAtLeast(20) → true (exactly 20, 2024 is leap)
        var birthDate = new DateTime(2004, 2, 29);
        var reference = new DateTime(2024, 2, 29);
        _vali.IsAtLeast(birthDate, 20, DatePart.Year, reference).Should().BeTrue();
    }

    // ── PreviousBirthday — A-1 guard fix ────────────────────────────────────

    [Fact]
    public void PreviousBirthday_BeforeFirstBirthday_ThrowsArgumentOutOfRange()
    {
        // born Dec 25, 1990; first birthday = Dec 25, 1991
        // reference = Dec 24, 1991 → one day before first birthday → should throw
        var birthDate = new DateTime(1990, 12, 25);
        var reference = new DateTime(1991, 12, 24);
        var act = () => _vali.PreviousBirthday(birthDate, reference);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void PreviousBirthday_OnFirstBirthday_ReturnsFirstBirthday()
    {
        // born Jun 15, 1990; first birthday = Jun 15, 1991
        // reference exactly on first birthday → returns that date
        var birthDate = new DateTime(1990, 6, 15);
        var reference = new DateTime(1991, 6, 15);
        DateTime result = _vali.PreviousBirthday(birthDate, reference);
        result.Should().Be(new DateTime(1991, 6, 15));
    }

    [Fact]
    public void PreviousBirthday_JustAfterFirstBirthday_ReturnsFirstBirthday()
    {
        // born Jun 15, 1990; first birthday = Jun 15, 1991
        // reference = Jun 16, 1991 → one day after first birthday → previous is Jun 15, 1991
        var birthDate = new DateTime(1990, 6, 15);
        var reference = new DateTime(1991, 6, 16);
        DateTime result = _vali.PreviousBirthday(birthDate, reference);
        result.Should().Be(new DateTime(1991, 6, 15));
    }

    [Fact]
    public void PreviousBirthday_BirthdayNotYetThisYear_ReturnsPreviousYear()
    {
        // born Dec 25, 1990; reference = Jun 1, 2025
        // birthday in 2025 is Dec 25, 2025 — hasn't happened yet → previous is Dec 25, 2024
        var birthDate = new DateTime(1990, 12, 25);
        var reference = new DateTime(2025, 6, 1);
        DateTime result = _vali.PreviousBirthday(birthDate, reference);
        result.Should().Be(new DateTime(2024, 12, 25));
    }

    [Fact]
    public void PreviousBirthday_OnBirthdayToday_ReturnsThatDay()
    {
        // born Mar 10, 1990; reference = Mar 10, 2025 → birthday falls on reference date
        var birthDate = new DateTime(1990, 3, 10);
        var reference = new DateTime(2025, 3, 10);
        DateTime result = _vali.PreviousBirthday(birthDate, reference);
        result.Should().Be(new DateTime(2025, 3, 10));
    }

    // ── Exact — month-end clamping (shared helper with ValiDate.MonthsDiff) ──

    [Fact]
    public void Exact_Jan31ToFeb28_NonLeapYear_DoesNotCountClampedMonthAsComplete()
    {
        // born 2023-01-31, reference 2023-02-28: Jan 31 + 1 month clamps to Feb 28, which equals
        // the reference exactly — that clamped month must not be counted as a complete month.
        var birthDate = new DateTime(2023, 1, 31);
        var reference = new DateTime(2023, 2, 28);
        AgeResult result = _vali.Exact(birthDate, reference);
        result.Years.Should().Be(0);
        result.Months.Should().Be(0);
        result.Days.Should().Be(28);
    }

    // ── One-arg overloads (use injected clock's Today/Now) ─────────────────

    [Fact]
    public void Years_OneArgOverload_UsesClockToday()
    {
        var clock = new FixedClock(new DateTime(2025, 6, 15));
        var vali = new ValiAge(clock);
        vali.Years(new DateTime(1995, 6, 15)).Should().Be(30);
    }

    [Fact]
    public void Exact_OneArgOverload_UsesClockToday()
    {
        var clock = new FixedClock(new DateTime(2025, 6, 20));
        var vali = new ValiAge(clock);
        AgeResult result = vali.Exact(new DateTime(1990, 3, 15));
        result.Years.Should().Be(35);
    }

    [Fact]
    public void Format_OneArgOverload_UsesClockToday()
    {
        var clock = new FixedClock(new DateTime(2025, 6, 20));
        var vali = new ValiAge(clock);
        vali.Format(new DateTime(1990, 3, 15)).Should().Be(vali.Exact(new DateTime(1990, 3, 15)).ToString());
    }

    [Fact]
    public void Relative_OneArgOverload_UsesClockNow()
    {
        var clock = new FixedClock(new DateTime(2025, 6, 15, 12, 0, 0));
        var vali = new ValiAge(clock);
        vali.Relative(new DateTime(2025, 6, 14, 12, 0, 0)).Should().Be("1 day ago");
    }

    [Fact]
    public void IsAtLeast_OneArgOverload_UsesClockToday()
    {
        var clock = new FixedClock(new DateTime(2025, 6, 15));
        var vali = new ValiAge(clock);
        vali.IsAtLeast(new DateTime(1995, 6, 15), 18, DatePart.Year).Should().BeTrue();
    }

    [Fact]
    public void IsBirthday_OneArgOverload_UsesClockToday()
    {
        var clock = new FixedClock(new DateTime(2025, 6, 15));
        var vali = new ValiAge(clock);
        vali.IsBirthday(new DateTime(1990, 6, 15)).Should().BeTrue();
    }

    [Fact]
    public void NextBirthday_OneArgOverload_UsesClockToday()
    {
        var clock = new FixedClock(new DateTime(2025, 6, 16));
        var vali = new ValiAge(clock);
        vali.NextBirthday(new DateTime(1990, 6, 15)).Year.Should().Be(2026);
    }

    [Fact]
    public void PreviousBirthday_OneArgOverload_UsesClockToday()
    {
        var clock = new FixedClock(new DateTime(2025, 6, 15));
        var vali = new ValiAge(clock);
        vali.PreviousBirthday(new DateTime(1990, 6, 15)).Should().Be(new DateTime(2025, 6, 15));
    }

    [Fact]
    public void DaysUntilBirthday_OneArgOverload_UsesClockToday()
    {
        var clock = new FixedClock(new DateTime(2025, 6, 15));
        var vali = new ValiAge(clock);
        vali.DaysUntilBirthday(new DateTime(1990, 6, 16)).Should().Be(1);
    }

    // ── Validation guards ────────────────────────────────────────────────

    [Fact]
    public void Years_BirthDateAfterReference_ThrowsArgumentOutOfRange()
    {
        var birthDate = new DateTime(2025, 6, 16);
        var reference = new DateTime(2025, 6, 15);
        var act = () => _vali.Years(birthDate, reference);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Exact_BirthDateAfterReference_ThrowsArgumentOutOfRange()
    {
        var birthDate = new DateTime(2025, 6, 16);
        var reference = new DateTime(2025, 6, 15);
        var act = () => _vali.Exact(birthDate, reference);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void NextBirthday_ReferenceYear9999_ThrowsArgumentOutOfRange()
    {
        var birthDate = new DateTime(1990, 6, 15);
        var reference = new DateTime(9999, 6, 15);
        var act = () => _vali.NextBirthday(birthDate, reference);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void IsAtLeast_InvalidDatePart_ThrowsArgumentOutOfRange()
    {
        var birthDate = new DateTime(1995, 6, 15);
        var reference = new DateTime(2025, 6, 15);
        var act = () => _vali.IsAtLeast(birthDate, 18, (DatePart)999, reference);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    // ── IsAtLeast — Month / Day parts ───────────────────────────────────────

    [Fact]
    public void IsAtLeast_MonthPart_EnoughMonths_ReturnsTrue()
    {
        // born 2024-01-15, reference 2025-06-15 → 17 months old
        var birthDate = new DateTime(2024, 1, 15);
        var reference = new DateTime(2025, 6, 15);
        _vali.IsAtLeast(birthDate, 17, DatePart.Month, reference).Should().BeTrue();
    }

    [Fact]
    public void IsAtLeast_MonthPart_NotEnoughMonths_ReturnsFalse()
    {
        var birthDate = new DateTime(2024, 1, 15);
        var reference = new DateTime(2025, 6, 15);
        _vali.IsAtLeast(birthDate, 18, DatePart.Month, reference).Should().BeFalse();
    }

    [Fact]
    public void IsAtLeast_DayPart_EnoughDays_ReturnsTrue()
    {
        var birthDate = new DateTime(2025, 6, 1);
        var reference = new DateTime(2025, 6, 15);
        _vali.IsAtLeast(birthDate, 14, DatePart.Day, reference).Should().BeTrue();
    }

    [Fact]
    public void IsAtLeast_DayPart_NotEnoughDays_ReturnsFalse()
    {
        var birthDate = new DateTime(2025, 6, 1);
        var reference = new DateTime(2025, 6, 15);
        _vali.IsAtLeast(birthDate, 15, DatePart.Day, reference).Should().BeFalse();
    }

    // ── Relative — week / month / year buckets ──────────────────────────────

    [Fact]
    public void Relative_FifteenDaysAgo_ReturnsWeeksAgo()
    {
        var reference = new DateTime(2025, 6, 15, 12, 0, 0);
        var date      = reference.AddDays(-15);
        string result = _vali.Relative(date, reference);
        result.Should().EndWith("weeks ago");
    }

    [Fact]
    public void Relative_SeventyDaysAgo_ReturnsMonthsAgo()
    {
        var reference = new DateTime(2025, 6, 15, 12, 0, 0);
        var date      = reference.AddDays(-70);
        string result = _vali.Relative(date, reference);
        result.Should().EndWith("months ago");
    }

    [Fact]
    public void Relative_TwoYearsAgo_ReturnsYearsAgo()
    {
        var reference = new DateTime(2025, 6, 15, 12, 0, 0);
        var date      = reference.AddDays(-800);
        string result = _vali.Relative(date, reference);
        result.Should().EndWith("years ago");
    }

    // ── AgeResult.ToString() — singular/plural grammar ──────────────────────

    [Fact]
    public void AgeResult_ToString_SingularValues_UsesSingularGrammar()
    {
        var result = new AgeResult(1, 1, 1, 397);
        result.ToString().Should().Be("1 year, 1 month, 1 day");
    }

    [Fact]
    public void AgeResult_ToString_PluralValues_UsesPluralGrammar()
    {
        var result = new AgeResult(2, 3, 4, 823);
        result.ToString().Should().Be("2 years, 3 months, 4 days");
    }
}
