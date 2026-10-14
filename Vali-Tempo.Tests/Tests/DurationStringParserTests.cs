using FluentAssertions;
using Vali_Time.Utils;
using Xunit;

namespace Vali_Time.Tests.Tests;

/// <summary>
/// Direct tests for <see cref="DurationStringParser"/>, the shared grammar used by both
/// <see cref="Vali_Time.Core.IValiTime.ParseTime"/> and Vali-Duration's <c>IParsable</c>.
/// </summary>
public class DurationStringParserTests
{
    // ── Parse: null / empty guards ──────────────────────────────────────────

    [Fact]
    public void Parse_NullInput_ThrowsArgumentNullException()
    {
        Action act = () => DurationStringParser.Parse(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Parse_EmptyInput_ThrowsFormatException()
    {
        Action act = () => DurationStringParser.Parse("");
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Parse_WhitespaceInput_ThrowsFormatException()
    {
        Action act = () => DurationStringParser.Parse("   ");
        act.Should().Throw<FormatException>();
    }

    // ── Colon-separated: mm:ss ───────────────────────────────────────────────

    [Fact]
    public void Parse_MmSs_ReturnsCorrectSeconds()
    {
        DurationStringParser.Parse("45:30").Should().Be(45m * 60m + 30m);
    }

    [Fact]
    public void Parse_MmSs_InvalidNumber_ThrowsFormatException()
    {
        Action act = () => DurationStringParser.Parse("ab:30");
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Parse_MmSs_SecondsOutOfRange_ThrowsFormatException()
    {
        Action act = () => DurationStringParser.Parse("10:60");
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Parse_MmSs_MinutesOutOfRange_ThrowsFormatException()
    {
        Action act = () => DurationStringParser.Parse("60:00");
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Parse_MmSs_NegativeMinutes_ThrowsFormatException()
    {
        Action act = () => DurationStringParser.Parse("-1:00");
        act.Should().Throw<FormatException>();
    }

    // ── Colon-separated: hh:mm:ss ────────────────────────────────────────────

    [Fact]
    public void Parse_HhMmSs_ReturnsCorrectSeconds()
    {
        DurationStringParser.Parse("1:30:00").Should().Be(1m * 3600m + 30m * 60m);
    }

    [Fact]
    public void Parse_HhMmSs_InvalidNumber_ThrowsFormatException()
    {
        Action act = () => DurationStringParser.Parse("1:xx:00");
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Parse_HhMmSs_MinutesOutOfRange_ThrowsFormatException()
    {
        Action act = () => DurationStringParser.Parse("1:60:00");
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Parse_TooManyColonParts_ThrowsFormatException()
    {
        Action act = () => DurationStringParser.Parse("1:2:3:4");
        act.Should().Throw<FormatException>();
    }

    // ── Labelled tokens ──────────────────────────────────────────────────────

    [Theory]
    [InlineData("500ms", 0.5)]
    [InlineData("1s", 1)]
    [InlineData("90 minutes", 5400)]
    [InlineData("1h", 3600)]
    [InlineData("2d", 172800)]
    [InlineData("1w", 604800)]
    [InlineData("1mo", 2629800)]
    [InlineData("1yr", 31557600)]
    public void Parse_SingleLabelledToken_ReturnsCorrectSeconds(string input, double expectedSeconds)
    {
        DurationStringParser.Parse(input).Should().Be((decimal)expectedSeconds);
    }

    [Fact]
    public void Parse_CombinedLabelledTokens_SumsAllComponents()
    {
        decimal result = DurationStringParser.Parse("2d 4h 30m 15s");
        decimal expected = 2 * 86400m + 4 * 3600m + 30 * 60m + 15m;
        result.Should().Be(expected);
    }

    [Fact]
    public void Parse_NoRecognisedTokens_ThrowsFormatException()
    {
        Action act = () => DurationStringParser.Parse("hello world");
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Parse_GarbageBetweenValidTokens_ThrowsFormatException()
    {
        Action act = () => DurationStringParser.Parse("1h garbage 2m");
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Parse_NumericValueOverflows_ThrowsFormatException()
    {
        Action act = () => DurationStringParser.Parse("99999999999999999999999999999999999d");
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Parse_IsCaseInsensitive()
    {
        DurationStringParser.Parse("1H").Should().Be(3600m);
    }

    // ── TryParse ─────────────────────────────────────────────────────────────

    [Fact]
    public void TryParse_ValidInput_ReturnsTrueAndValue()
    {
        bool ok = DurationStringParser.TryParse("1h", out decimal seconds);
        ok.Should().BeTrue();
        seconds.Should().Be(3600m);
    }

    [Fact]
    public void TryParse_NullInput_ReturnsFalseAndZero()
    {
        bool ok = DurationStringParser.TryParse(null, out decimal seconds);
        ok.Should().BeFalse();
        seconds.Should().Be(0m);
    }

    [Fact]
    public void TryParse_InvalidInput_ReturnsFalseAndZero()
    {
        bool ok = DurationStringParser.TryParse("garbage", out decimal seconds);
        ok.Should().BeFalse();
        seconds.Should().Be(0m);
    }
}
