using FluentAssertions;
using Vali_Holiday.Utils;
using Xunit;

namespace Vali_Time.Tests.Tests;

public class EasterCalculatorTests
{
    // ── Easter Sunday dates (historically verified) ─────────────────────────

    [Theory]
    [InlineData(2020, 4, 12)]
    [InlineData(2021, 4,  4)]
    [InlineData(2022, 4, 17)]
    [InlineData(2023, 4,  9)]
    [InlineData(2024, 3, 31)]
    [InlineData(2025, 4, 20)]
    [InlineData(2026, 4,  5)]
    [InlineData(2027, 3, 28)]
    public void Easter_HistoricalYears_ReturnsCorrectDate(int year, int expectedMonth, int expectedDay)
    {
        DateTime easter = EasterCalculator.Easter(year);
        easter.Year.Should().Be(year);
        easter.Month.Should().Be(expectedMonth);
        easter.Day.Should().Be(expectedDay);
    }

    // ── Good Friday = Easter - 2 ────────────────────────────────────────────

    [Theory]
    [InlineData(2020, 4, 10)]
    [InlineData(2021, 4,  2)]
    [InlineData(2022, 4, 15)]
    [InlineData(2023, 4,  7)]
    [InlineData(2024, 3, 29)]
    [InlineData(2025, 4, 18)]
    [InlineData(2026, 4,  3)]
    [InlineData(2027, 3, 26)]
    public void GoodFriday_HistoricalYears_ReturnsEasterMinusTwo(int year, int expectedMonth, int expectedDay)
    {
        DateTime gf = EasterCalculator.GoodFriday(year);
        gf.Year.Should().Be(year);
        gf.Month.Should().Be(expectedMonth);
        gf.Day.Should().Be(expectedDay);
    }

    [Fact]
    public void GoodFriday_EqualsEasterMinusTwo()
    {
        for (int year = 2020; year <= 2027; year++)
        {
            DateTime expected = EasterCalculator.Easter(year).AddDays(-2);
            EasterCalculator.GoodFriday(year).Should().Be(expected);
        }
    }

    // ── Holy Thursday = Easter - 3 ──────────────────────────────────────────

    [Theory]
    [InlineData(2020, 4,  9)]
    [InlineData(2025, 4, 17)]
    public void HolyThursday_ReturnsEasterMinusThree(int year, int expectedMonth, int expectedDay)
    {
        DateTime ht = EasterCalculator.HolyThursday(year);
        ht.Year.Should().Be(year);
        ht.Month.Should().Be(expectedMonth);
        ht.Day.Should().Be(expectedDay);
    }

    [Fact]
    public void HolyThursday_EqualsEasterMinusThree()
    {
        for (int year = 2020; year <= 2027; year++)
        {
            DateTime expected = EasterCalculator.Easter(year).AddDays(-3);
            EasterCalculator.HolyThursday(year).Should().Be(expected);
        }
    }

    // ── Corpus Christi = Easter + 60 ────────────────────────────────────────

    [Fact]
    public void CorpusChristi_2025_ReturnsEasterPlus60()
    {
        DateTime expected = EasterCalculator.Easter(2025).AddDays(60);
        EasterCalculator.CorpusChristi(2025).Should().Be(expected);
    }

    [Fact]
    public void CorpusChristi_EqualsEasterPlusSixty()
    {
        for (int year = 2020; year <= 2027; year++)
        {
            DateTime expected = EasterCalculator.Easter(year).AddDays(60);
            EasterCalculator.CorpusChristi(year).Should().Be(expected);
        }
    }
}
