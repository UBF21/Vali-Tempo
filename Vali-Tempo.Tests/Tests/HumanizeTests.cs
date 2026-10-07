using FluentAssertions;
using Vali_Age.Core;
using Vali_Age.Extensions;
using Vali_Time.Abstractions;
using Vali_Time.Enums;
using Xunit;

namespace Vali_Time.Tests.Tests;

public class HumanizeTests
{
    private static readonly DateTime Now = new(2026, 6, 15, 12, 0, 0);

    private sealed class FixedClock : IClock
    {
        private readonly DateTime _fixed;
        public FixedClock(DateTime fixedValue) => _fixed = fixedValue;
        public DateTime Now => _fixed;
        public DateTime Today => _fixed.Date;
    }

    private static ValiAge Vali() => new();

    // ── Moment boundary (4.1) ──────────────────────────────────────────────

    [Fact]
    public void Humanize_Moment_59SecondsAgo_ReturnsAMomentAgo()
    {
        Vali().Humanize(Now.AddSeconds(-59), Now).Should().Be("a moment ago");
    }

    [Fact]
    public void Humanize_Moment_60SecondsAgo_ReturnsAMinuteAgo_NotMoment()
    {
        Vali().Humanize(Now.AddSeconds(-60), Now).Should().Be("a minute ago");
    }

    [Fact]
    public void Humanize_Moment_59SecondsFuture_ReturnsInAMoment()
    {
        Vali().Humanize(Now.AddSeconds(59), Now).Should().Be("in a moment");
    }

    [Fact]
    public void Humanize_Moment_Tie_TreatedAsPast()
    {
        Vali().Humanize(Now, Now).Should().Be("a moment ago");
    }

    // ── Minute boundary (4.2) ──────────────────────────────────────────────

    [Fact]
    public void Humanize_Minute_OneMinuteAgo_ReturnsAMinuteAgo()
    {
        Vali().Humanize(Now.AddMinutes(-1), Now).Should().Be("a minute ago");
    }

    [Fact]
    public void Humanize_Minute_TwoMinutesAgo_ReturnsTwoMinutesAgo()
    {
        Vali().Humanize(Now.AddMinutes(-2), Now).Should().Be("2 minutes ago");
    }

    [Fact]
    public void Humanize_Minute_59Minutes59SecondsAgo_ReturnsFiftyNineMinutesAgo()
    {
        Vali().Humanize(Now.AddSeconds(-3599), Now).Should().Be("59 minutes ago");
    }

    [Fact]
    public void Humanize_Minute_3600SecondsAgo_ReturnsAnHourAgo_NotSixtyMinutes()
    {
        Vali().Humanize(Now.AddSeconds(-3600), Now).Should().Be("an hour ago");
    }

    [Fact]
    public void Humanize_Minute_TwoMinutesFuture_ReturnsInTwoMinutes()
    {
        Vali().Humanize(Now.AddMinutes(2), Now).Should().Be("in 2 minutes");
    }

    // ── Hour boundary (4.3) ─────────────────────────────────────────────────

    [Fact]
    public void Humanize_Hour_OneHourAgo_ReturnsAnHourAgo()
    {
        Vali().Humanize(Now.AddHours(-1), Now).Should().Be("an hour ago");
    }

    [Fact]
    public void Humanize_Hour_TwoHoursAgo_ReturnsTwoHoursAgo()
    {
        Vali().Humanize(Now.AddHours(-2), Now).Should().Be("2 hours ago");
    }

    [Fact]
    public void Humanize_Hour_23Hours59Minutes59SecondsAgo_ReturnsTwentyThreeHoursAgo()
    {
        Vali().Humanize(Now.AddSeconds(-86399), Now).Should().Be("23 hours ago");
    }

    [Fact]
    public void Humanize_Hour_86400SecondsAgo_ReturnsYesterday_NotTwentyFourHours()
    {
        Vali().Humanize(Now.AddSeconds(-86400), Now).Should().Be("yesterday");
    }

    [Fact]
    public void Humanize_Hour_TwoHoursFuture_ReturnsInTwoHours()
    {
        Vali().Humanize(Now.AddHours(2), Now).Should().Be("in 2 hours");
    }

    // ── Day boundary / yesterday-tomorrow (4.4) ─────────────────────────────

    [Fact]
    public void Humanize_Day_OneDayAgo_ReturnsYesterday()
    {
        Vali().Humanize(Now.AddDays(-1), Now).Should().Be("yesterday");
    }

    [Fact]
    public void Humanize_Day_OneDayFuture_ReturnsTomorrow()
    {
        Vali().Humanize(Now.AddDays(1), Now).Should().Be("tomorrow");
    }

    [Fact]
    public void Humanize_Day_TwoDaysAgo_ReturnsTwoDaysAgo()
    {
        Vali().Humanize(Now.AddDays(-2), Now).Should().Be("2 days ago");
    }

    [Fact]
    public void Humanize_Day_SixDaysAgo_ReturnsSixDaysAgo()
    {
        Vali().Humanize(Now.AddDays(-6), Now).Should().Be("6 days ago");
    }

    [Fact]
    public void Humanize_Day_604799SecondsAgo_ReturnsSixDaysAgo()
    {
        Vali().Humanize(Now.AddSeconds(-604799), Now).Should().Be("6 days ago");
    }

    [Fact]
    public void Humanize_Day_604800SecondsAgo_ReturnsLastWeek_NotSevenDays()
    {
        Vali().Humanize(Now.AddSeconds(-604800), Now).Should().Be("last week");
    }

    // ── Week boundary / last-next-week (4.5) ────────────────────────────────

    [Fact]
    public void Humanize_Week_SevenDaysAgo_ReturnsLastWeek()
    {
        Vali().Humanize(Now.AddDays(-7), Now).Should().Be("last week");
    }

    [Fact]
    public void Humanize_Week_SevenDaysFuture_ReturnsNextWeek()
    {
        Vali().Humanize(Now.AddDays(7), Now).Should().Be("next week");
    }

    [Fact]
    public void Humanize_Week_FourteenDaysAgo_ReturnsTwoWeeksAgo()
    {
        Vali().Humanize(Now.AddDays(-14), Now).Should().Be("2 weeks ago");
    }

    [Fact]
    public void Humanize_Week_TwentyEightDaysAgo_ReturnsFourWeeksAgo()
    {
        Vali().Humanize(Now.AddDays(-28), Now).Should().Be("4 weeks ago");
    }

    [Fact]
    public void Humanize_Week_2629799SecondsAgo_ReturnsFourWeeksAgo()
    {
        Vali().Humanize(Now.AddSeconds(-2629799), Now).Should().Be("4 weeks ago");
    }

    [Fact]
    public void Humanize_Week_2629800SecondsAgo_ReturnsLastMonth_NotFourOrFiveWeeks()
    {
        Vali().Humanize(Now.AddSeconds(-2629800), Now).Should().Be("last month");
    }

    // ── Month boundary / last-next-month (4.6) ──────────────────────────────

    [Fact]
    public void Humanize_Month_2629800SecondsFuture_ReturnsNextMonth()
    {
        Vali().Humanize(Now.AddSeconds(2629800), Now).Should().Be("next month");
    }

    [Fact]
    public void Humanize_Month_TwoMonthsAgo_ReturnsTwoMonthsAgo()
    {
        Vali().Humanize(Now.AddSeconds(-2629800 * 2), Now).Should().Be("2 months ago");
    }

    [Fact]
    public void Humanize_Month_ElevenMonthsAgo_ReturnsElevenMonthsAgo()
    {
        Vali().Humanize(Now.AddSeconds(-2629800 * 11), Now).Should().Be("11 months ago");
    }

    [Fact]
    public void Humanize_Month_31557599SecondsAgo_ReturnsElevenMonthsAgo()
    {
        Vali().Humanize(Now.AddSeconds(-31557599), Now).Should().Be("11 months ago");
    }

    [Fact]
    public void Humanize_Month_31557600SecondsAgo_ReturnsLastYear_NotTwelveMonths()
    {
        Vali().Humanize(Now.AddSeconds(-31557600), Now).Should().Be("last year");
    }

    // ── Year boundary / last-next-year (4.7) ────────────────────────────────

    [Fact]
    public void Humanize_Year_31557600SecondsFuture_ReturnsNextYear()
    {
        Vali().Humanize(Now.AddSeconds(31557600), Now).Should().Be("next year");
    }

    [Fact]
    public void Humanize_Year_TwoYearsAgo_ReturnsTwoYearsAgo()
    {
        Vali().Humanize(Now.AddSeconds(-31557600 * 2), Now).Should().Be("2 years ago");
    }

    [Fact]
    public void Humanize_Year_TenYearsAgo_ReturnsTenYearsAgo()
    {
        Vali().Humanize(Now.AddSeconds(-31557600 * 10), Now).Should().Be("10 years ago");
    }

    // ── Spanish parity (4.8) ────────────────────────────────────────────────

    [Fact]
    public void Humanize_Es_Moment_ReturnsHaceUnMomento()
    {
        Vali().Humanize(Now.AddSeconds(-30), Now, HumanizeCulture.Es).Should().Be("hace un momento");
    }

    [Fact]
    public void Humanize_Es_Minute_ReturnsHaceUnMinuto()
    {
        Vali().Humanize(Now.AddMinutes(-1), Now, HumanizeCulture.Es).Should().Be("hace un minuto");
    }

    [Fact]
    public void Humanize_Es_Hour_ReturnsHaceUnaHora()
    {
        Vali().Humanize(Now.AddHours(-1), Now, HumanizeCulture.Es).Should().Be("hace una hora");
    }

    [Fact]
    public void Humanize_Es_Yesterday_ReturnsAyer()
    {
        Vali().Humanize(Now.AddDays(-1), Now, HumanizeCulture.Es).Should().Be("ayer");
    }

    [Fact]
    public void Humanize_Es_Tomorrow_ReturnsManana()
    {
        Vali().Humanize(Now.AddDays(1), Now, HumanizeCulture.Es).Should().Be("mañana");
    }

    [Fact]
    public void Humanize_Es_LastWeek_ReturnsLaSemanaPasada()
    {
        Vali().Humanize(Now.AddDays(-7), Now, HumanizeCulture.Es).Should().Be("la semana pasada");
    }

    [Fact]
    public void Humanize_Es_NextWeek_ReturnsLaProximaSemana()
    {
        Vali().Humanize(Now.AddDays(7), Now, HumanizeCulture.Es).Should().Be("la próxima semana");
    }

    [Fact]
    public void Humanize_Es_LastMonth_ReturnsElMesPasado()
    {
        Vali().Humanize(Now.AddSeconds(-2629800), Now, HumanizeCulture.Es).Should().Be("el mes pasado");
    }

    [Fact]
    public void Humanize_Es_LastYear_ReturnsElAnoPasado()
    {
        Vali().Humanize(Now.AddSeconds(-31557600), Now, HumanizeCulture.Es).Should().Be("el año pasado");
    }

    [Fact]
    public void Humanize_Es_TwoYearsAgo_ReturnsHaceDosAnos()
    {
        Vali().Humanize(Now.AddSeconds(-31557600 * 2), Now, HumanizeCulture.Es).Should().Be("hace 2 años");
    }

    // ── maxPrecision (4.9) ───────────────────────────────────────────────────

    [Fact]
    public void Humanize_MaxPrecisionHours_30SecondsAgo_ReturnsLessThanAnHourAgo()
    {
        Vali().Humanize(Now.AddSeconds(-30), Now, maxPrecision: TimeUnit.Hours)
            .Should().Be("less than an hour ago");
    }

    [Fact]
    public void Humanize_MaxPrecisionHours_30MinutesAgo_ReturnsLessThanAnHourAgo()
    {
        Vali().Humanize(Now.AddMinutes(-30), Now, maxPrecision: TimeUnit.Hours)
            .Should().Be("less than an hour ago");
    }

    [Fact]
    public void Humanize_MaxPrecisionHours_TwoHoursAgo_ReturnsTwoHoursAgo_SameAsUncapped()
    {
        Vali().Humanize(Now.AddHours(-2), Now, maxPrecision: TimeUnit.Hours).Should().Be("2 hours ago");
    }

    [Fact]
    public void Humanize_MaxPrecisionDays_FiveMinutesAgo_ReturnsLessThanADayAgo()
    {
        Vali().Humanize(Now.AddMinutes(-5), Now, maxPrecision: TimeUnit.Days)
            .Should().Be("less than a day ago");
    }

    [Fact]
    public void Humanize_MaxPrecisionMinutes_30SecondsFuture_ReturnsInLessThanAMinute()
    {
        Vali().Humanize(Now.AddSeconds(30), Now, maxPrecision: TimeUnit.Minutes)
            .Should().Be("in less than a minute");
    }

    [Fact]
    public void Humanize_MaxPrecisionMilliseconds_Throws()
    {
        Action act = () => Vali().Humanize(Now.AddDays(-1), Now, maxPrecision: TimeUnit.Milliseconds);
        act.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void Humanize_MaxPrecisionSeconds_Throws()
    {
        Action act = () => Vali().Humanize(Now.AddDays(-1), Now, maxPrecision: TimeUnit.Seconds);
        act.Should().Throw<NotSupportedException>();
    }

    [Theory]
    [InlineData(TimeUnit.Weeks)]
    [InlineData(TimeUnit.Months)]
    [InlineData(TimeUnit.Years)]
    public void Humanize_MaxPrecision_OtherUnits_CollapseWhenBelowThreshold(TimeUnit unit)
    {
        // A one-minute difference is below every coarse-grained floor.
        string result = Vali().Humanize(Now.AddMinutes(-1), Now, maxPrecision: unit);
        result.Should().StartWith("less than");
    }

    // ── Unsupported culture (4.10) ──────────────────────────────────────────

    [Fact]
    public void Humanize_UnsupportedCulture_Throws()
    {
        Action act = () => Vali().Humanize(Now.AddDays(-1), Now, (HumanizeCulture)999);
        NotSupportedException ex = act.Should().Throw<NotSupportedException>().Which;
        ex.Message.Should().Contain("En").And.Contain("Es");
    }

    // ── Past vs future — explicit sign per bucket (4.11) ────────────────────
    // Per-bucket sign coverage (yesterday/tomorrow, last week/next week, last month/next
    // month, last year/next year, "{0} X ago" vs "in {0} X") already lives in the boundary
    // tests above; this just confirms minute-level past/future never collide.

    [Fact]
    public void Humanize_PastFuture_MinutesSignDiffers()
    {
        string past = Vali().Humanize(Now.AddMinutes(-5), Now);
        string future = Vali().Humanize(Now.AddMinutes(5), Now);
        past.Should().Contain("ago");
        future.Should().StartWith("in ");
        past.Should().NotBe(future);
    }

    // ── Extension method vs instance method parity (4.12) ───────────────────

    [Theory]
    [InlineData(-59, HumanizeCulture.En)]
    [InlineData(-3600, HumanizeCulture.En)]
    [InlineData(-86400, HumanizeCulture.En)]
    [InlineData(-604800, HumanizeCulture.En)]
    [InlineData(-2629800, HumanizeCulture.En)]
    [InlineData(-31557600, HumanizeCulture.En)]
    [InlineData(-59, HumanizeCulture.Es)]
    [InlineData(-3600, HumanizeCulture.Es)]
    [InlineData(31557600, HumanizeCulture.Es)]
    public void Humanize_ExtensionMethod_MatchesInstanceMethod(int offsetSeconds, HumanizeCulture culture)
    {
        DateTime date = Now.AddSeconds(offsetSeconds);

        string instanceResult = Vali().Humanize(date, Now, culture);
        string extensionResult = date.Humanize(Now, culture);

        extensionResult.Should().Be(instanceResult);
    }

    [Fact]
    public void Humanize_ExtensionMethod_MatchesInstanceMethod_WithMaxPrecision()
    {
        DateTime date = Now.AddMinutes(-5);

        string instanceResult = Vali().Humanize(date, Now, HumanizeCulture.En, TimeUnit.Hours);
        string extensionResult = date.Humanize(Now, HumanizeCulture.En, TimeUnit.Hours);

        extensionResult.Should().Be(instanceResult);
    }

    // ── Default args / injected clock (4.13) ────────────────────────────────

    [Fact]
    public void Humanize_DefaultRelativeTo_UsesInjectedClock()
    {
        var clock = new FixedClock(Now);
        var vali = new ValiAge(clock);

        string withExplicitReference = vali.Humanize(Now.AddDays(-1), Now);
        string withDefaultReference = vali.Humanize(Now.AddDays(-1));

        withDefaultReference.Should().Be(withExplicitReference);
        withDefaultReference.Should().Be("yesterday");
    }

    [Fact]
    public void Humanize_NoCultureOrMaxPrecision_DefaultsToEnglishUncapped()
    {
        Vali().Humanize(Now.AddDays(-1), Now).Should().Be("yesterday");
    }

    // ── Extremes — no overflow ───────────────────────────────────────────────

    [Fact]
    public void Humanize_MinValue_DoesNotThrowOverflow_AndReturnsYearsAgo()
    {
        Action act = () => Vali().Humanize(DateTime.MinValue, Now);
        act.Should().NotThrow<OverflowException>();
        Vali().Humanize(DateTime.MinValue, Now).Should().EndWith("years ago");
    }

    [Fact]
    public void Humanize_MaxValue_DoesNotThrowOverflow_AndReturnsInYears()
    {
        Action act = () => Vali().Humanize(DateTime.MaxValue, Now);
        act.Should().NotThrow<OverflowException>();
        Vali().Humanize(DateTime.MaxValue, Now).Should().StartWith("in ").And.EndWith("years");
    }
}
