using FluentAssertions;
using Vali_Time.Core;
using Vali_Time.Enums;
using Xunit;

namespace Vali_Time.Tests.Tests;

public class ValiTimeTests
{
    private readonly ValiTime _vali = new();

    // ── Convert: basic unit chains ──────────────────────────────────────────

    [Theory]
    [InlineData(1000, TimeUnit.Milliseconds, TimeUnit.Seconds, 1)]
    [InlineData(60,   TimeUnit.Seconds,      TimeUnit.Minutes, 1)]
    [InlineData(60,   TimeUnit.Minutes,      TimeUnit.Hours,   1)]
    [InlineData(24,   TimeUnit.Hours,        TimeUnit.Days,    1)]
    [InlineData(7,    TimeUnit.Days,         TimeUnit.Weeks,   1)]
    public void Convert_BasicChains_ReturnsExpected(decimal input, TimeUnit from, TimeUnit to, decimal expected)
    {
        decimal result = _vali.Convert(input, from, to);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(1, TimeUnit.Seconds,      TimeUnit.Milliseconds, 1000)]
    [InlineData(1, TimeUnit.Minutes,      TimeUnit.Seconds,      60)]
    [InlineData(1, TimeUnit.Hours,        TimeUnit.Minutes,      60)]
    [InlineData(1, TimeUnit.Days,         TimeUnit.Hours,        24)]
    [InlineData(1, TimeUnit.Weeks,        TimeUnit.Days,         7)]
    public void Convert_Reverses_ReturnsExpected(decimal input, TimeUnit from, TimeUnit to, decimal expected)
    {
        decimal result = _vali.Convert(input, from, to);
        result.Should().Be(expected);
    }

    [Fact]
    public void Convert_ZeroValue_ReturnsZero()
    {
        decimal result = _vali.Convert(0, TimeUnit.Hours, TimeUnit.Minutes);
        result.Should().Be(0);
    }

    [Fact]
    public void Convert_WithDecimalPlaces_RoundsCorrectly()
    {
        // 1 hour = 0.0166... days; rounded to 4 places
        decimal result = _vali.Convert(1, TimeUnit.Hours, TimeUnit.Days, decimalPlaces: 4);
        result.Should().Be(Math.Round(1m / 24m, 4, MidpointRounding.ToEven));
    }

    [Fact]
    public void Convert_NegativeTime_ThrowsArgumentException()
    {
        Action act = () => _vali.Convert(-1, TimeUnit.Seconds, TimeUnit.Minutes);
        act.Should().Throw<ArgumentException>().WithMessage("*negative*");
    }

    [Fact]
    public void Convert_MonthsToSeconds_UsesApproximation()
    {
        // 1 month ≈ 2629800 seconds
        decimal result = _vali.Convert(1, TimeUnit.Months, TimeUnit.Seconds);
        result.Should().BeApproximately(2629800m, 1m);
    }

    [Fact]
    public void Convert_YearsToSeconds_UsesApproximation()
    {
        // 1 year ≈ 31557600 seconds
        decimal result = _vali.Convert(1, TimeUnit.Years, TimeUnit.Seconds);
        result.Should().BeApproximately(31557600m, 1m);
    }

    // ── SumTimes ────────────────────────────────────────────────────────────

    [Fact]
    public void SumTimes_MixedUnits_ReturnsSumInSeconds()
    {
        var times = new List<(decimal, TimeUnit)>
        {
            (1,    TimeUnit.Hours),
            (30,   TimeUnit.Minutes),
            (30,   TimeUnit.Seconds)
        };
        decimal result = _vali.SumTimes(TimeUnit.Seconds, times);
        result.Should().Be(5430m);
    }

    [Fact]
    public void SumTimes_SingleValue_ReturnsConvertedValue()
    {
        var times = new List<(decimal, TimeUnit)> { (2, TimeUnit.Minutes) };
        decimal result = _vali.SumTimes(TimeUnit.Seconds, times);
        result.Should().Be(120m);
    }

    [Fact]
    public void SumTimes_EmptyList_ThrowsArgumentException()
    {
        Action act = () => _vali.SumTimes(TimeUnit.Seconds, new List<(decimal, TimeUnit)>());
        act.Should().Throw<ArgumentException>();
    }

    // ── SubtractTimes ───────────────────────────────────────────────────────

    [Fact]
    public void SubtractTimes_ValidSubtraction_ReturnsCorrectResult()
    {
        var times = new List<(decimal, TimeUnit)>
        {
            (2,  TimeUnit.Hours),
            (30, TimeUnit.Minutes)
        };
        decimal result = _vali.SubtractTimes(TimeUnit.Minutes, times);
        result.Should().Be(90m);
    }

    [Fact]
    public void SubtractTimes_NegativeResult_ThrowsWhenAllowNegativeFalse()
    {
        var times = new List<(decimal, TimeUnit)>
        {
            (1,  TimeUnit.Minutes),
            (2,  TimeUnit.Minutes)
        };
        Action act = () => _vali.SubtractTimes(TimeUnit.Seconds, times, allowNegative: false);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SubtractTimes_NegativeResult_AllowedWhenFlagIsTrue()
    {
        var times = new List<(decimal, TimeUnit)>
        {
            (1, TimeUnit.Minutes),
            (2, TimeUnit.Minutes)
        };
        decimal result = _vali.SubtractTimes(TimeUnit.Seconds, times, allowNegative: true);
        result.Should().Be(-60m);
    }

    // ── FormatTime ──────────────────────────────────────────────────────────

    [Fact]
    public void FormatTime_Hours_ReturnsCorrectString()
    {
        string result = _vali.FormatTime(1.5m, TimeUnit.Hours);
        result.Should().Be("1.50 h");
    }

    [Fact]
    public void FormatTime_Milliseconds_ReturnsCorrectString()
    {
        string result = _vali.FormatTime(500m, TimeUnit.Milliseconds);
        result.Should().Be("500.00 ms");
    }

    [Fact]
    public void FormatTime_Seconds_ReturnsCorrectString()
    {
        string result = _vali.FormatTime(90m, TimeUnit.Seconds);
        result.Should().Be("90.00 s");
    }

    // ── GetBestUnit ─────────────────────────────────────────────────────────

    [Theory]
    [InlineData(0.5,     TimeUnit.Milliseconds)]
    [InlineData(1,       TimeUnit.Seconds)]
    [InlineData(59,      TimeUnit.Seconds)]
    [InlineData(60,      TimeUnit.Minutes)]
    [InlineData(3599,    TimeUnit.Minutes)]
    [InlineData(3600,    TimeUnit.Hours)]
    [InlineData(86399,   TimeUnit.Hours)]
    [InlineData(86400,   TimeUnit.Days)]
    [InlineData(604799,  TimeUnit.Days)]
    [InlineData(604800,  TimeUnit.Weeks)]
    public void GetBestUnit_VariousSeconds_ReturnsCorrectUnit(double seconds, TimeUnit expectedUnit)
    {
        (_, TimeUnit unit) = _vali.GetBestUnit((decimal)seconds);
        unit.Should().Be(expectedUnit);
    }

    // ── ParseTime ───────────────────────────────────────────────────────────

    [Theory]
    [InlineData("1h 30m",    5400)]
    [InlineData("1:30:00",   5400)]
    [InlineData("90 minutes", 5400)]
    [InlineData("500ms",     0.5)]
    public void ParseTime_KnownFormats_ReturnsCorrectSeconds(string input, double expectedSeconds)
    {
        decimal result = _vali.ParseTime(input);
        result.Should().BeApproximately((decimal)expectedSeconds, 0.001m);
    }

    [Fact]
    public void ParseTime_TwoDaysFourHours_ReturnsCorrectSeconds()
    {
        // 2d 4h = 2*86400 + 4*3600 = 172800 + 14400 = 187200
        decimal result = _vali.ParseTime("2d 4h");
        result.Should().Be(187200m);
    }

    // ── TryParseTime ────────────────────────────────────────────────────────

    [Fact]
    public void TryParseTime_InvalidInput_ReturnsFalse()
    {
        bool success = _vali.TryParseTime("not a time", out decimal seconds);
        success.Should().BeFalse();
        seconds.Should().Be(0);
    }

    [Fact]
    public void TryParseTime_ValidInput_ReturnsTrueAndValue()
    {
        bool success = _vali.TryParseTime("2h", out decimal seconds);
        success.Should().BeTrue();
        seconds.Should().Be(7200m);
    }

    [Fact]
    public void TryParseTime_NullInput_ReturnsFalseAndZero()
    {
        bool success = _vali.TryParseTime(null!, out decimal seconds);
        success.Should().BeFalse();
        seconds.Should().Be(0);
    }

    [Fact]
    public void TryParseTime_EmptyInput_ReturnsFalseAndZero()
    {
        bool success = _vali.TryParseTime("", out decimal seconds);
        success.Should().BeFalse();
        seconds.Should().Be(0);
    }

    [Fact]
    public void TryParseTime_NullInput_DoesNotThrow()
    {
        Action act = () => _vali.TryParseTime(null!, out _);
        act.Should().NotThrow();
    }

    [Fact]
    public void TryParseTime_EmptyInput_DoesNotThrow()
    {
        Action act = () => _vali.TryParseTime("", out _);
        act.Should().NotThrow();
    }

    // ── TryConvert ──────────────────────────────────────────────────────────

    [Fact]
    public void TryConvert_NegativeTime_ReturnsFalse()
    {
        bool success = _vali.TryConvert(-1, TimeUnit.Hours, TimeUnit.Seconds, out decimal result);
        success.Should().BeFalse();
        result.Should().Be(0);
    }

    [Fact]
    public void TryConvert_ValidInput_ReturnsTrueAndValue()
    {
        bool success = _vali.TryConvert(1, TimeUnit.Hours, TimeUnit.Minutes, out decimal result);
        success.Should().BeTrue();
        result.Should().Be(60m);
    }

    [Fact]
    public void TryConvert_InvalidFromUnit_ReturnsFalseAndZero()
    {
        bool success = _vali.TryConvert(1, (TimeUnit)999, TimeUnit.Seconds, out decimal result);
        success.Should().BeFalse();
        result.Should().Be(0);
    }

    [Fact]
    public void TryConvert_InvalidToUnit_ReturnsFalseAndZero()
    {
        bool success = _vali.TryConvert(1, TimeUnit.Seconds, (TimeUnit)999, out decimal result);
        success.Should().BeFalse();
        result.Should().Be(0);
    }

    [Fact]
    public void TryConvert_InvalidFromUnit_DoesNotThrow()
    {
        Action act = () => _vali.TryConvert(1, (TimeUnit)999, TimeUnit.Seconds, out _);
        act.Should().NotThrow();
    }

    // ── FromTimeSpan ────────────────────────────────────────────────────────

    [Fact]
    public void FromTimeSpan_OneAndHalfHours_ReturnsCorrectHours()
    {
        TimeSpan ts = TimeSpan.FromHours(1.5);
        decimal result = _vali.FromTimeSpan(ts, TimeUnit.Hours);
        result.Should().BeApproximately(1.5m, 0.0001m);
    }

    [Fact]
    public void FromTimeSpan_OneHour_ReturnsCorrectMinutes()
    {
        TimeSpan ts = TimeSpan.FromHours(1);
        decimal result = _vali.FromTimeSpan(ts, TimeUnit.Minutes);
        result.Should().Be(60m);
    }

    // ── Breakdown ───────────────────────────────────────────────────────────

    [Fact]
    public void Breakdown_3665Seconds_ReturnsCorrectComponents()
    {
        // 3665s = 1h 1min 5s 0ms
        var breakdown = _vali.Breakdown(3665m);

        breakdown[TimeUnit.Hours].Should().Be(1m);
        breakdown[TimeUnit.Minutes].Should().Be(1m);
        breakdown[TimeUnit.Seconds].Should().Be(5m);
        breakdown[TimeUnit.Milliseconds].Should().Be(0m);
    }

    [Fact]
    public void Breakdown_NegativeSeconds_ThrowsArgumentException()
    {
        Action act = () => _vali.Breakdown(-1m);
        act.Should().Throw<ArgumentException>();
    }

    // ── Additional Convert tests ─────────────────────────────────────────────

    [Fact]
    public void Convert_Milliseconds_To_Seconds()
    {
        decimal result = _vali.Convert(1000m, TimeUnit.Milliseconds, TimeUnit.Seconds);
        result.Should().Be(1m);
    }

    [Fact]
    public void Convert_Hours_To_Minutes()
    {
        decimal result = _vali.Convert(2m, TimeUnit.Hours, TimeUnit.Minutes);
        result.Should().Be(120m);
    }

    [Fact]
    public void Convert_Days_To_Hours()
    {
        decimal result = _vali.Convert(1m, TimeUnit.Days, TimeUnit.Hours);
        result.Should().Be(24m);
    }

    [Fact]
    public void Convert_Weeks_To_Days()
    {
        decimal result = _vali.Convert(2m, TimeUnit.Weeks, TimeUnit.Days);
        result.Should().Be(14m);
    }

    [Fact]
    public void Convert_Months_To_Days()
    {
        // 1 month = SecondsInMonth / SecondsInDay = 2629800 / 86400 = 30.4375
        decimal result = _vali.Convert(1m, TimeUnit.Months, TimeUnit.Days);
        result.Should().BeApproximately(30.4375m, 0.0001m);
    }

    // ── Additional SumTimes tests ────────────────────────────────────────────

    [Fact]
    public void SumTimes_MixedUnits_OneHourPlusThirtyMin_Returns90Minutes()
    {
        var times = new List<(decimal, TimeUnit)>
        {
            (1m,  TimeUnit.Hours),
            (30m, TimeUnit.Minutes)
        };
        decimal result = _vali.SumTimes(TimeUnit.Minutes, times);
        result.Should().Be(90m);
    }

    // ── Additional FormatTime tests ──────────────────────────────────────────

    [Fact]
    public void FormatTime_Milliseconds_ContainsMs()
    {
        string result = _vali.FormatTime(1500m, TimeUnit.Milliseconds);
        result.Should().Contain("ms");
    }

    [Fact]
    public void FormatTime_Days_ContainsD()
    {
        string result = _vali.FormatTime(2m, TimeUnit.Days);
        result.Should().Contain("d");
    }

    [Fact]
    public void FormatTime_Weeks_ContainsW()
    {
        string result = _vali.FormatTime(3m, TimeUnit.Weeks);
        result.Should().Contain("w");
    }

    // ── Clamp ───────────────────────────────────────────────────────────────

    [Fact]
    public void Clamp_BelowMin_ReturnsMin()
    {
        decimal result = _vali.Clamp(5m, TimeUnit.Minutes, 10m, 60m);
        result.Should().Be(10m);
    }

    [Fact]
    public void Clamp_AboveMax_ReturnsMax()
    {
        decimal result = _vali.Clamp(90m, TimeUnit.Minutes, 10m, 60m);
        result.Should().Be(60m);
    }

    [Fact]
    public void Clamp_WithinRange_ReturnsSameValue()
    {
        decimal result = _vali.Clamp(30m, TimeUnit.Minutes, 10m, 60m);
        result.Should().Be(30m);
    }

    // ── Compare ─────────────────────────────────────────────────────────────

    [Fact]
    public void Compare_SmallerTime_ReturnsNegative()
    {
        int result = _vali.Compare(30m, TimeUnit.Minutes, 1m, TimeUnit.Hours);
        result.Should().BeNegative();
    }

    [Fact]
    public void Compare_EqualTimes_ReturnsZero()
    {
        int result = _vali.Compare(60m, TimeUnit.Minutes, 1m, TimeUnit.Hours);
        result.Should().Be(0);
    }

    // ── MultiConvert ─────────────────────────────────────────────────────────

    [Fact]
    public void MultiConvert_Returns_CorrectDictionary()
    {
        var result = _vali.MultiConvert(3600m, TimeUnit.Seconds, TimeUnit.Hours, TimeUnit.Minutes);
        result[TimeUnit.Hours].Should().Be(1m);
        result[TimeUnit.Minutes].Should().Be(60m);
    }

    // ── Additional ParseTime tests ───────────────────────────────────────────

    [Fact]
    public void ParseTime_LabelFormat_HoursMinutes_Returns150Minutes()
    {
        // "2h 30min" should parse to 2*3600 + 30*60 = 9000 seconds = 150 minutes
        decimal seconds = _vali.ParseTime("2h 30min");
        decimal minutes = _vali.Convert(seconds, TimeUnit.Seconds, TimeUnit.Minutes);
        minutes.Should().Be(150m);
    }

    // ── Additional GetBestUnit tests ─────────────────────────────────────────

    [Fact]
    public void GetBestUnit_SmallValue_ReturnsMilliseconds()
    {
        (_, TimeUnit unit) = _vali.GetBestUnit(0.1m);
        unit.Should().Be(TimeUnit.Milliseconds);
    }

    [Fact]
    public void GetBestUnit_LargeValue_ReturnsHoursOrDays()
    {
        (_, TimeUnit unit) = _vali.GetBestUnit(86400m); // exactly 1 day
        unit.Should().BeOneOf(TimeUnit.Hours, TimeUnit.Days);
    }

    // ── T-2: ToTimeSpan overflow guard ───────────────────────────────────────

    [Fact]
    public void ToTimeSpan_ExceedsMaxValue_ThrowsOverflowException()
    {
        // 999_999_999_999 seconds is far beyond TimeSpan.MaxValue (~29,227 years)
        Action act = () => _vali.ToTimeSpan(999_999_999_999m, TimeUnit.Seconds);
        act.Should().Throw<OverflowException>();
    }

    [Fact]
    public void ToTimeSpan_ExactlyAtMaxValue_DoesNotThrow()
    {
        // 100 years in seconds ≈ 3_155_760_000s — well within TimeSpan range (~29,227 years)
        decimal hundredYearsInSeconds = 100m * 365.25m * 24m * 3600m;
        Action act = () => _vali.ToTimeSpan(hundredYearsInSeconds, TimeUnit.Seconds);
        act.Should().NotThrow();
    }

    [Fact]
    public void ToTimeSpan_NegativeValue_ThrowsArgumentException()
    {
        // Negative time is always invalid — existing behavior must still hold
        Action act = () => _vali.ToTimeSpan(-1m, TimeUnit.Seconds);
        act.Should().Throw<ArgumentException>().WithMessage("*negative*");
    }

    // ── VT-2: ParseColonSeparated validates minutes/seconds in [0,59] ─────────

    [Fact]
    public void ParseTime_ColonFormat_OutOfRangeMinutes_ThrowsFormatException()
    {
        // "0:99" — seconds value 99 is out of range [0,59]
        Action act = () => _vali.ParseTime("0:99");
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void ParseTime_ColonFormat_OutOfRangeSeconds_ThrowsFormatException()
    {
        // "1:30:99" — seconds value 99 is out of range [0,59]
        Action act = () => _vali.ParseTime("1:30:99");
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void ParseTime_ColonFormat_ValidBoundary_DoesNotThrow()
    {
        // "59:59" — both minutes and seconds are exactly at boundary 59; result = 59*60+59 = 3599
        decimal result = _vali.ParseTime("59:59");
        result.Should().Be(3599m);
    }

    // ── VT-3: ParseLabelledTokens rejects garbage between tokens ─────────────

    [Fact]
    public void ParseTime_LabelledTokens_GarbageBetweenTokens_ThrowsFormatException()
    {
        // "abc 5h xyz" contains unrecognised tokens — must throw FormatException
        Action act = () => _vali.ParseTime("abc 5h xyz");
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void ParseTime_LabelledTokens_ValidInput_DoesNotThrow()
    {
        // "2h 30min" is a valid labelled-token string — returns 2*3600 + 30*60 = 9000
        decimal result = _vali.ParseTime("2h 30min");
        result.Should().Be(9000m);
    }
}
