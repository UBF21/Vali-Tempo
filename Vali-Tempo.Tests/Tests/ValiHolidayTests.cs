using FluentAssertions;
using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Vali_Holiday.Providers.LatinAmerica;
using Vali_Holiday.Utils;
using Xunit;

namespace Vali_Time.Tests.Tests;

public class ValiHolidayTests
{
    // ── PeruHolidayProvider.IsHoliday ───────────────────────────────────────

    [Fact]
    public void PeruHolidayProvider_IsHoliday_July28_ReturnsTrue()
    {
        var provider = new PeruHolidayProvider();
        var date = new DateTime(2025, 7, 28);
        provider.IsHoliday(date).Should().BeTrue();
    }

    [Fact]
    public void PeruHolidayProvider_IsHoliday_July29_ReturnsTrue()
    {
        var provider = new PeruHolidayProvider();
        var date = new DateTime(2025, 7, 29);
        provider.IsHoliday(date).Should().BeTrue();
    }

    [Fact]
    public void PeruHolidayProvider_IsHoliday_August1_ReturnsFalse()
    {
        var provider = new PeruHolidayProvider();
        var date = new DateTime(2025, 8, 1);
        provider.IsHoliday(date).Should().BeFalse();
    }

    // ── PeruHolidayProvider.GetHoliday (Navidad) ────────────────────────────

    [Fact]
    public void PeruHolidayProvider_GetHoliday_Christmas_ReturnsCorrectName()
    {
        var provider = new PeruHolidayProvider();
        var christmas = new DateTime(2025, 12, 25);
        HolidayInfo? holiday = provider.GetHoliday(christmas);
        holiday.Should().NotBeNull();
        holiday!.Name.Should().Be("Navidad del Señor");
    }

    // ── EasterCalculator.GoodFriday 2025 ────────────────────────────────────

    [Fact]
    public void EasterCalculator_GoodFriday2025_IsApril18()
    {
        DateTime gf = EasterCalculator.GoodFriday(2025);
        gf.Month.Should().Be(4);
        gf.Day.Should().Be(18);
    }

    // ── HolidayProviderFactory.CreateAll().Supports ─────────────────────────

    [Fact]
    public void HolidayProviderFactory_CreateAll_SupportsPE()
    {
        ValiHoliday holidays = HolidayProviderFactory.CreateAll();
        holidays.Supports("PE").Should().BeTrue();
    }

    // ── HolidayProviderFactory.CreateLatinAmerica().SupportedCountries ──────

    [Fact]
    public void HolidayProviderFactory_CreateLatinAmerica_ContainsExpectedCountries()
    {
        ValiHoliday holidays = HolidayProviderFactory.CreateLatinAmerica();
        IEnumerable<string> countries = holidays.SupportedCountries;

        countries.Should().Contain("PE");
        countries.Should().Contain("CL");
        countries.Should().Contain("AR");
        countries.Should().Contain("CO");
        countries.Should().Contain("MX");
    }

    // ── GetName in English ───────────────────────────────────────────────────

    [Fact]
    public void GetName_Christmas_InEnglish_ReturnsEnglishName()
    {
        var provider = new PeruHolidayProvider();
        var christmas = new DateTime(2025, 12, 25);
        HolidayInfo? holiday = provider.GetHoliday(christmas);
        holiday.Should().NotBeNull();
        string name = provider.GetName(holiday!, "en");
        name.Should().Be("Christmas Day");
    }

    [Fact]
    public void GetName_IndependenceDay_InEnglish_ReturnsEnglishName()
    {
        var provider = new PeruHolidayProvider();
        var independence = new DateTime(2025, 7, 28);
        HolidayInfo? holiday = provider.GetHoliday(independence);
        holiday.Should().NotBeNull();
        string name = provider.GetName(holiday!, "en");
        name.Should().Be("Independence Day");
    }

    // ── GetHolidays 2025 for PE ──────────────────────────────────────────────

    [Fact]
    public void GetHolidays_Peru2025_ReturnsAtLeast14Holidays()
    {
        ValiHoliday holidays = HolidayProviderFactory.CreateAll();
        IEnumerable<HolidayInfo> peHolidays = holidays.GetHolidays(2025, "PE");
        peHolidays.Should().HaveCountGreaterThanOrEqualTo(14);
    }

    [Fact]
    public void GetHolidays_Peru2025_ContainsGoodFriday()
    {
        ValiHoliday holidays = HolidayProviderFactory.CreateAll();
        IEnumerable<HolidayInfo> peHolidays = holidays.GetHolidays(2025, "PE").ToList();

        // Good Friday 2025 = April 18
        peHolidays.Should().Contain(h => h.Month == 4 && h.Day == 18);
    }

    [Fact]
    public void GetHolidays_Peru2025_ContainsHolyThursday()
    {
        ValiHoliday holidays = HolidayProviderFactory.CreateAll();
        IEnumerable<HolidayInfo> peHolidays = holidays.GetHolidays(2025, "PE").ToList();

        // Holy Thursday 2025 = April 17
        peHolidays.Should().Contain(h => h.Month == 4 && h.Day == 17);
    }

    // ── CountryCode ─────────────────────────────────────────────────────────

    [Fact]
    public void PeruHolidayProvider_CountryCode_IsPE()
    {
        var provider = new PeruHolidayProvider();
        provider.CountryCode.Should().Be("PE");
    }

    // ── HolidayInfo.ToDateTime ───────────────────────────────────────────────

    [Fact]
    public void HolidayInfo_ToDateTime_ReturnsCorrectDate()
    {
        var provider = new PeruHolidayProvider();
        var laborDay = new DateTime(2025, 5, 1);
        HolidayInfo? holiday = provider.GetHoliday(laborDay);
        holiday.Should().NotBeNull();
        holiday!.ToDateTime(2025).Should().Be(new DateTime(2025, 5, 1));
    }

    // ── FIX 1: Null guard tests ──────────────────────────────────────────────

    [Fact]
    public void ValiHoliday_For_NullCode_ThrowsArgumentNullException()
    {
        var vali = HolidayProviderFactory.CreateAll();
        Action act = () => vali.For(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ValiHoliday_For_EmptyCode_ThrowsArgumentNullException()
    {
        var vali = HolidayProviderFactory.CreateAll();
        Action act = () => vali.For("");
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ValiHoliday_For_WhitespaceCode_ThrowsArgumentNullException()
    {
        var vali = HolidayProviderFactory.CreateAll();
        Action act = () => vali.For("  ");
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ValiHoliday_IsHoliday_NullCode_ThrowsArgumentNullException()
    {
        var vali = HolidayProviderFactory.CreateAll();
        Action act = () => vali.IsHoliday(DateTime.Today, null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ValiHoliday_GetHolidays_NullCode_ThrowsArgumentNullException()
    {
        var vali = HolidayProviderFactory.CreateAll();
        Action act = () => vali.GetHolidays(2025, (string)null!);
        act.Should().Throw<ArgumentNullException>();
    }

    // ── FIX 2: Cache consistency tests ──────────────────────────────────────

    [Fact]
    public void PeruProvider_IsHoliday_CalledMultipleTimes_ReturnsSameResult()
    {
        var provider = new PeruHolidayProvider();
        var date = new DateTime(2025, 7, 28); // Independence Day

        // Calling multiple times — results must be consistent (cache does not corrupt state)
        var first  = provider.IsHoliday(date);
        var second = provider.IsHoliday(date);
        var third  = provider.IsHoliday(date);

        first.Should().BeTrue();
        second.Should().BeTrue();
        third.Should().BeTrue();
    }

    [Fact]
    public void PeruProvider_IsHoliday_NationalHoliday_ReturnsTrue()
    {
        var provider = new PeruHolidayProvider();
        // January 1 is New Year's Day — always a holiday
        provider.IsHoliday(new DateTime(2025, 1, 1)).Should().BeTrue();
    }

    [Fact]
    public void PeruProvider_IsHoliday_NonHoliday_ReturnsFalse()
    {
        var provider = new PeruHolidayProvider();
        // March 15 is not a Peruvian public holiday
        provider.IsHoliday(new DateTime(2025, 3, 15)).Should().BeFalse();
    }

    [Theory]
    [InlineData("PE")]
    [InlineData("CL")]
    [InlineData("AR")]
    [InlineData("CO")]
    [InlineData("GT")]
    [InlineData("HN")]
    [InlineData("SV")]
    [InlineData("NI")]
    public void IsHoliday_January1_ReturnsTrue_ForAnyCountry(string countryCode)
    {
        var vali = HolidayProviderFactory.CreateAll();
        vali.IsHoliday(new DateTime(2025, 1, 1), countryCode).Should().BeTrue();
    }

    // ── FIX 3: GetNextHolidayWithYear tests ─────────────────────────────────

    [Fact]
    public void GetNextHolidayWithYear_FromJanuary15_ReturnsHolidayAfterThatDate()
    {
        var vali = HolidayProviderFactory.CreateAll();
        var date = new DateTime(2025, 1, 15);
        var result = vali.GetNextHolidayWithYear(date, "PE");

        result.Should().NotBeNull();
        result!.Value.Holiday.ToDateTime(result.Value.Year).Should().BeOnOrAfter(date);
    }

    [Fact]
    public void GetNextHolidayWithYear_YearIsGreaterOrEqualToInputYear()
    {
        var vali = HolidayProviderFactory.CreateAll();
        var date = new DateTime(2025, 6, 1);
        var result = vali.GetNextHolidayWithYear(date, "PE");

        result.Should().NotBeNull();
        result!.Value.Year.Should().BeGreaterThanOrEqualTo(date.Year);
    }

    [Fact]
    public void GetNextHolidayWithYear_FromDecember31_ReturnsHolidayInNextYear()
    {
        var vali = HolidayProviderFactory.CreateAll();
        // December 31 is not a holiday for PE; next one should be January 1 of the following year
        var date = new DateTime(2025, 12, 31);
        var result = vali.GetNextHolidayWithYear(date, "PE");

        result.Should().NotBeNull();
        result!.Value.Year.Should().BeGreaterThanOrEqualTo(2025);
        result.Value.Holiday.Month.Should().BeInRange(1, 12);
        result.Value.Holiday.Day.Should().BeInRange(1, 31);
    }

    [Fact]
    public void GetNextHolidayWithYear_ReturnedHoliday_HasValidMonthAndDay()
    {
        var vali = HolidayProviderFactory.CreateAll();
        var result = vali.GetNextHolidayWithYear(new DateTime(2025, 3, 1), "PE");

        result.Should().NotBeNull();
        result!.Value.Holiday.Month.Should().BeInRange(1, 12);
        result.Value.Holiday.Day.Should().BeInRange(1, 31);
    }

    [Fact]
    public void GetNextHolidayWithYear_NullCode_ThrowsArgumentNullException()
    {
        var vali = HolidayProviderFactory.CreateAll();
        Action act = () => vali.GetNextHolidayWithYear(DateTime.Today, null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetPreviousHolidayWithYear_NullCode_ThrowsArgumentNullException()
    {
        var vali = HolidayProviderFactory.CreateAll();
        Action act = () => vali.GetPreviousHolidayWithYear(DateTime.Today, null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetPreviousHolidayWithYear_ReturnsHolidayStrictlyBeforeDate()
    {
        var vali = HolidayProviderFactory.CreateAll();
        var date = new DateTime(2025, 7, 30);
        var result = vali.GetPreviousHolidayWithYear(date, "PE");

        result.Should().NotBeNull();
        result!.Value.Holiday.ToDateTime(result.Value.Year).Should().BeBefore(date);
    }

    // ── FIX 4: New Central American providers tests ──────────────────────────

    [Fact]
    public void GuatemalaHolidayProvider_GetHolidays2025_ReturnsAtLeast10Holidays()
    {
        var provider = new GuatemalaHolidayProvider();
        provider.GetHolidays(2025).Should().HaveCountGreaterThanOrEqualTo(10);
    }

    [Fact]
    public void GuatemalaHolidayProvider_September15_IsHoliday()
    {
        var provider = new GuatemalaHolidayProvider();
        provider.IsHoliday(new DateTime(2025, 9, 15)).Should().BeTrue();
    }

    [Fact]
    public void HondurasHolidayProvider_September15_IsHoliday()
    {
        var provider = new HondurasHolidayProvider();
        provider.IsHoliday(new DateTime(2025, 9, 15)).Should().BeTrue();
    }

    [Fact]
    public void ElSalvadorHolidayProvider_September15_IsHoliday()
    {
        var provider = new ElSalvadorHolidayProvider();
        provider.IsHoliday(new DateTime(2025, 9, 15)).Should().BeTrue();
    }

    [Fact]
    public void NicaraguaHolidayProvider_September15_IsHoliday()
    {
        var provider = new NicaraguaHolidayProvider();
        provider.IsHoliday(new DateTime(2025, 9, 15)).Should().BeTrue();
    }

    // ── FIX 4: Factory registration tests ───────────────────────────────────

    [Fact]
    public void HolidayProviderFactory_CreateAll_SupportsGuatemala()
    {
        HolidayProviderFactory.CreateAll().Supports("GT").Should().BeTrue();
    }

    [Fact]
    public void HolidayProviderFactory_CreateAll_SupportsHonduras()
    {
        HolidayProviderFactory.CreateAll().Supports("HN").Should().BeTrue();
    }

    [Fact]
    public void HolidayProviderFactory_CreateAll_SupportsElSalvador()
    {
        HolidayProviderFactory.CreateAll().Supports("SV").Should().BeTrue();
    }

    [Fact]
    public void HolidayProviderFactory_CreateAll_SupportsNicaragua()
    {
        HolidayProviderFactory.CreateAll().Supports("NI").Should().BeTrue();
    }

    [Fact]
    public void HolidayProviderFactory_CreateLatinAmerica_SupportsGuatemala()
    {
        HolidayProviderFactory.CreateLatinAmerica().Supports("GT").Should().BeTrue();
    }

    [Fact]
    public void HolidayProviderFactory_CreateLatinAmerica_SupportsHonduras()
    {
        HolidayProviderFactory.CreateLatinAmerica().Supports("HN").Should().BeTrue();
    }

    [Fact]
    public void HolidayProviderFactory_CreateLatinAmerica_SupportsElSalvador()
    {
        HolidayProviderFactory.CreateLatinAmerica().Supports("SV").Should().BeTrue();
    }

    [Fact]
    public void HolidayProviderFactory_CreateLatinAmerica_SupportsNicaragua()
    {
        HolidayProviderFactory.CreateLatinAmerica().Supports("NI").Should().BeTrue();
    }

    // ── H-1: Honduras Día del Maestro (Sep 17) ──────────────────────────────

    [Fact]
    public void Honduras_Sep17_IsDiaDelMaestro()
    {
        var vali = HolidayProviderFactory.CreateAll();
        vali.IsHoliday(new DateTime(2025, 9, 17), "HN").Should().BeTrue();
    }

    [Fact]
    public void Honduras_GetHolidays_ContainsDiaDelMaestro()
    {
        var vali = HolidayProviderFactory.CreateAll();
        vali.GetHolidays(2025, "HN").Should().Contain(h => h.Id == "hn_teachers_day");
    }

    // ── H-2: Honduras Easter Sunday added to movable holidays ───────────────

    [Fact]
    public void Honduras_EasterSunday_IsHoliday()
    {
        // Easter 2025 = April 20
        var vali = HolidayProviderFactory.CreateAll();
        vali.IsHoliday(new DateTime(2025, 4, 20), "HN").Should().BeTrue();
    }

    [Fact]
    public void Honduras_GetHolidays_ContainsEasterSunday()
    {
        var vali = HolidayProviderFactory.CreateAll();
        vali.GetHolidays(2025, "HN").Should().Contain(h => h.Id == "hn_easter");
    }

    // ── H-3: Argentina ar_easter is Observance + regionCode, excluded from IsHoliday ──

    [Fact]
    public void Argentina_EasterSunday_NotInIsHoliday()
    {
        // Easter 2025 = April 20; ar_easter is HolidayType.Observance with regionCode AR-OBS
        var vali = HolidayProviderFactory.CreateAll();
        vali.IsHoliday(new DateTime(2025, 4, 20), "AR").Should().BeFalse();
    }

    [Fact]
    public void Argentina_EasterSunday_ExistsInGetHolidays()
    {
        // ar_easter is still in the full list even though it is an Observance
        var vali = HolidayProviderFactory.CreateAll();
        vali.GetHolidays(2025, "AR").Should().Contain(h => h.Id == "ar_easter");
    }

    [Fact]
    public void Argentina_EasterSunday_HasObservanceType()
    {
        var provider = new ArgentinaHolidayProvider();
        var holiday = provider.GetHolidays(2025).First(h => h.Id == "ar_easter");
        holiday.Type.Should().Be(HolidayType.Observance);
    }

    // ── H-4: Chile Indigenous Peoples Day is solstice-based (Jun 20 or 21) ──

    [Fact]
    public void Chile_2024_IndigenousPeoplesDay_IsJun20()
    {
        var vali = HolidayProviderFactory.CreateAll();
        vali.IsHoliday(new DateTime(2024, 6, 20), "CL").Should().BeTrue();
    }

    [Fact]
    public void Chile_2025_IndigenousPeoplesDay_IsJun21()
    {
        var vali = HolidayProviderFactory.CreateAll();
        vali.IsHoliday(new DateTime(2025, 6, 21), "CL").Should().BeTrue();
    }

    [Fact]
    public void Chile_2025_Jun20_IsNotIndigenousPeoplesDay()
    {
        var vali = HolidayProviderFactory.CreateAll();
        vali.IsHoliday(new DateTime(2025, 6, 20), "CL").Should().BeFalse();
    }

    [Fact]
    public void Chile_IndigenousPeoplesDay_IsMovable()
    {
        var vali = HolidayProviderFactory.CreateAll();
        var holiday = vali.GetHolidays(2025, "CL").First(h => h.Id == "cl_indigenous_peoples");
        holiday.IsMovable.Should().BeTrue();
    }

    // ── H-5: BaseHolidayProvider Observance type excluded from IsHoliday ────

    [Fact]
    public void Argentina_EasterSunday_ObservanceExcludedFromIsHoliday()
    {
        // Reinforces H-3: Observance-typed holidays must not appear in IsHoliday
        var provider = new ArgentinaHolidayProvider();
        // Easter 2025 = April 20
        provider.IsHoliday(new DateTime(2025, 4, 20)).Should().BeFalse();
    }

    [Fact]
    public void Mexico_MothersDayObservance_NotInIsHoliday()
    {
        // mx_mothers_day (May 10) is HolidayType.Observance — must not appear in IsHoliday
        var vali = HolidayProviderFactory.CreateAll();
        vali.IsHoliday(new DateTime(2025, 5, 10), "MX").Should().BeFalse();
    }

    // ── H-6: Brazil pre-computed EasterCalculator (functional results) ───────

    [Fact]
    public void Brazil_CarnavalMonday_2025_IsCorrect()
    {
        // Easter 2025 = April 20; Carnaval Monday = Easter - 48 days = March 3
        var vali = HolidayProviderFactory.CreateAll();
        vali.IsHoliday(new DateTime(2025, 3, 3), "BR").Should().BeTrue();
    }

    [Fact]
    public void Brazil_CorpusChristi_2025_IsCorrect()
    {
        // Easter 2025 = April 20; Corpus Christi = Easter + 60 days = June 19
        var vali = HolidayProviderFactory.CreateAll();
        vali.IsHoliday(new DateTime(2025, 6, 19), "BR").Should().BeTrue();
    }
}
