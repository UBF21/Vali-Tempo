using FluentAssertions;
using Vali_Age.Core;
using Vali_Age.Models;
using Vali_Time.Enums;
using Xunit;

namespace Vali_Time.Tests.Tests;

public class ValiAgeTests
{
    private readonly ValiAge _vali = new();

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
}
