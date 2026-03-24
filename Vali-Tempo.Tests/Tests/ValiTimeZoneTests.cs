using FluentAssertions;
using Vali_TimeZone.Core;
using Xunit;

namespace Vali_Time.Tests.Tests;

public class ValiTimeZoneTests
{
    private readonly ValiTimeZone _vali = new();

    // ====================================================================
    // Conversion
    // ====================================================================

    [Fact]
    public void Convert_LimaToMadrid_AddsCorrectOffset()
    {
        // July 15 → Madrid is in DST (UTC+2), Lima is UTC-5 → diff = +7h
        var limaTime = new DateTime(2025, 7, 15, 10, 0, 0);
        var result = _vali.Convert(limaTime, "America/Lima", "Europe/Madrid");
        result.Should().Be(new DateTime(2025, 7, 15, 17, 0, 0));
    }

    [Fact]
    public void Convert_SameZone_ReturnsSameDateTime()
    {
        var dt = new DateTime(2025, 6, 1, 12, 0, 0);
        var result = _vali.Convert(dt, "America/Lima", "America/Lima");
        result.Should().Be(dt);
    }

    [Fact]
    public void Convert_NYToLima_SameOffset()
    {
        // January (standard time) → NY is UTC-5, Lima is UTC-5 → same time
        var nyTime = new DateTime(2025, 1, 15, 10, 0, 0);
        var result = _vali.Convert(nyTime, "America/New_York", "America/Lima");
        result.Should().Be(new DateTime(2025, 1, 15, 10, 0, 0));
    }

    [Fact]
    public void ToUtc_Lima_AddsOffset()
    {
        // Lima is UTC-5, so local 10:00 → UTC 15:00
        var limaLocal = new DateTime(2025, 3, 1, 10, 0, 0);
        var utc = _vali.ToUtc(limaLocal, "America/Lima");
        utc.Should().Be(new DateTime(2025, 3, 1, 15, 0, 0));
    }

    [Fact]
    public void FromUtc_Lima_SubtractsOffset()
    {
        // Lima is UTC-5, so UTC 15:00 → Lima 10:00
        var utc = new DateTime(2025, 3, 1, 15, 0, 0);
        var limaLocal = _vali.FromUtc(utc, "America/Lima");
        limaLocal.Should().Be(new DateTime(2025, 3, 1, 10, 0, 0));
    }

    [Fact]
    public void ToUtc_ThenFromUtc_Roundtrip_ReturnsSameDateTime()
    {
        var original = new DateTime(2025, 8, 20, 14, 30, 0);
        var utc = _vali.ToUtc(original, "America/Lima");
        var roundtrip = _vali.FromUtc(utc, "America/Lima");
        roundtrip.Should().Be(original);
    }

    [Fact]
    public void ConvertOffset_PreservesInstant()
    {
        // Convert a DateTimeOffset from Lima to Madrid — the UTC instant must be identical
        var limaOffset = new DateTimeOffset(2025, 7, 15, 10, 0, 0, TimeSpan.FromHours(-5));
        var madridResult = _vali.ConvertOffset(limaOffset, "Europe/Madrid");

        madridResult.UtcDateTime.Should().Be(limaOffset.UtcDateTime);
    }

    [Fact]
    public void ToDateTimeOffset_AppliesCorrectOffset()
    {
        var dt = new DateTime(2025, 3, 1, 10, 0, 0);
        var result = _vali.ToDateTimeOffset(dt, "America/Lima");

        result.Offset.Should().Be(TimeSpan.FromHours(-5));
        result.DateTime.Should().Be(dt);
    }

    // ====================================================================
    // Timezone info
    // ====================================================================

    [Fact]
    public void GetOffset_Lima_IsMinusFive()
    {
        var offset = _vali.GetOffset("America/Lima", new DateTime(2025, 6, 1));
        offset.Should().Be(TimeSpan.FromHours(-5));
    }

    [Fact]
    public void GetOffset_UTC_IsZero()
    {
        var offset = _vali.GetOffset("UTC", new DateTime(2025, 6, 1));
        offset.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void GetBaseOffset_Lima_IsMinusFive()
    {
        var baseOffset = _vali.GetBaseOffset("America/Lima");
        baseOffset.Should().Be(TimeSpan.FromHours(-5));
    }

    [Fact]
    public void GetBaseOffset_UTC_IsZero()
    {
        var baseOffset = _vali.GetBaseOffset("UTC");
        baseOffset.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void IsDst_Lima_AlwaysFalse()
    {
        // Lima does not observe DST at any time of year
        var summer = new DateTime(2025, 7, 15, 12, 0, 0);
        var winter = new DateTime(2025, 1, 15, 12, 0, 0);

        _vali.IsDst(summer, "America/Lima").Should().BeFalse();
        _vali.IsDst(winter, "America/Lima").Should().BeFalse();
    }

    [Fact]
    public void IsDst_London_SummerIsTrue()
    {
        // July is BST (British Summer Time = UTC+1, DST active)
        var summerDate = new DateTime(2025, 7, 15, 12, 0, 0);
        _vali.IsDst(summerDate, "Europe/London").Should().BeTrue();
    }

    [Fact]
    public void IsDst_London_WinterIsFalse()
    {
        // January is GMT (UTC+0, no DST)
        var winterDate = new DateTime(2025, 1, 15, 12, 0, 0);
        _vali.IsDst(winterDate, "Europe/London").Should().BeFalse();
    }

    [Fact]
    public void OffsetDiff_LimaAndBogota_IsZero()
    {
        // Both Lima and Bogota are UTC-5 standard, no DST
        var instant = new DateTime(2025, 6, 1, 12, 0, 0);
        var diff = _vali.OffsetDiff("America/Lima", "America/Bogota", instant);
        diff.Should().Be(0m);
    }

    [Fact]
    public void OffsetDiff_LimaAndMadrid_IsCorrect()
    {
        // January: Lima UTC-5, Madrid UTC+1 → diff = -5 - 1 = -6
        var winterInstant = new DateTime(2025, 1, 15, 12, 0, 0);
        var diff = _vali.OffsetDiff("America/Lima", "Europe/Madrid", winterInstant);
        diff.Should().Be(-6m);
    }

    // ====================================================================
    // Discovery
    // ====================================================================

    [Fact]
    public void FindZone_Lima_ReturnsCorrectZoneInfo()
    {
        var zone = _vali.FindZone("America/Lima");

        zone.Should().NotBeNull();
        zone!.Id.Should().Be("America/Lima");
    }

    [Fact]
    public void FindZone_Lima_CountryCodeIsPE()
    {
        var zone = _vali.FindZone("America/Lima");

        zone.Should().NotBeNull();
        zone!.CountryCode.Should().Be("PE");
    }

    [Fact]
    public void FindZone_NonExistent_ReturnsNull()
    {
        var zone = _vali.FindZone("Fake/Zone");
        zone.Should().BeNull();
    }

    [Fact]
    public void AllZones_ReturnsAtLeast40Zones()
    {
        var zones = _vali.AllZones();
        zones.Should().HaveCountGreaterThanOrEqualTo(40);
    }

    [Fact]
    public void ZonesForCountry_PE_ReturnsLima()
    {
        var zones = _vali.ZonesForCountry("PE").ToList();

        zones.Should().NotBeEmpty();
        zones.Should().Contain(z => z.Id == "America/Lima");
    }

    [Fact]
    public void ZonesForCountry_US_ReturnsMultipleZones()
    {
        var zones = _vali.ZonesForCountry("US").ToList();
        zones.Should().HaveCountGreaterThan(1);
    }

    [Fact]
    public void ZonesForCountry_CaseInsensitive_PE_vs_pe()
    {
        var upper = _vali.ZonesForCountry("PE").ToList();
        var lower = _vali.ZonesForCountry("pe").ToList();

        upper.Select(z => z.Id).Should().BeEquivalentTo(lower.Select(z => z.Id));
    }

    [Fact]
    public void ZonesForCountry_UnknownCountry_ReturnsEmpty()
    {
        var zones = _vali.ZonesForCountry("ZZ").ToList();
        zones.Should().BeEmpty();
    }

    [Fact]
    public void IsValidZone_Lima_ReturnsTrue()
    {
        _vali.IsValidZone("America/Lima").Should().BeTrue();
    }

    [Fact]
    public void IsValidZone_UTC_ReturnsTrue()
    {
        _vali.IsValidZone("UTC").Should().BeTrue();
    }

    [Fact]
    public void IsValidZone_FakeZone_ReturnsFalse()
    {
        _vali.IsValidZone("Fake/NotAZone").Should().BeFalse();
    }

    // ====================================================================
    // Utilities
    // ====================================================================

    [Fact]
    public void Now_Lima_IsNotDefault()
    {
        var now = _vali.Now("America/Lima");
        now.Should().NotBe(default(DateTime));
    }

    [Fact]
    public void Today_Lima_HasNoTimeComponent()
    {
        var today = _vali.Today("America/Lima");
        today.TimeOfDay.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void IsSameInstant_SameInstantDifferentZones_ReturnsTrue()
    {
        // Lima and Bogota share UTC-5 standard, so 10:00 Lima == 10:00 Bogota
        var limaTime = new DateTime(2025, 6, 1, 10, 0, 0);
        var bogotaTime = new DateTime(2025, 6, 1, 10, 0, 0);

        _vali.IsSameInstant(limaTime, "America/Lima", bogotaTime, "America/Bogota")
             .Should().BeTrue();
    }

    [Fact]
    public void IsSameInstant_DifferentInstants_ReturnsFalse()
    {
        var limaTime = new DateTime(2025, 6, 1, 10, 0, 0);
        var madridTime = new DateTime(2025, 6, 1, 10, 0, 0);

        // Lima UTC-5 vs Madrid UTC+2 (summer) → different UTC instants
        _vali.IsSameInstant(limaTime, "America/Lima", madridTime, "Europe/Madrid")
             .Should().BeFalse();
    }

    [Fact]
    public void FormatWithZone_Lima_ContainsOffset()
    {
        var dt = new DateTime(2025, 3, 1, 10, 0, 0);
        var formatted = _vali.FormatWithZone(dt, "America/Lima");

        formatted.Should().Contain("-05:00");
    }

    [Fact]
    public void FormatWithZone_CustomFormat_AppliesFormat()
    {
        var dt = new DateTime(2025, 3, 1, 10, 0, 0);
        var formatted = _vali.FormatWithZone(dt, "America/Lima", "yyyy/MM/dd");

        formatted.Should().Be("2025/03/01");
    }

    // ====================================================================
    // GetOffset — Z-1 DST-aware fix
    // ====================================================================

    [Fact]
    public void GetOffset_StandardZone_ReturnsCorrectOffset()
    {
        // America/Lima has no DST — offset is always UTC-5
        var offset = _vali.GetOffset("America/Lima", new DateTime(2025, 6, 1));
        offset.Should().Be(TimeSpan.FromHours(-5));
    }

    [Fact]
    public void GetOffset_WithDstActive_ReturnsCorrectOffset()
    {
        // America/New_York during summer (DST active) → UTC-4
        var offset = _vali.GetOffset("America/New_York", new DateTime(2025, 7, 15));
        offset.Should().Be(TimeSpan.FromHours(-4));
    }

    [Fact]
    public void GetOffset_WithDstInactive_ReturnsCorrectOffset()
    {
        // America/New_York during winter (no DST) → UTC-5
        var offset = _vali.GetOffset("America/New_York", new DateTime(2025, 1, 15));
        offset.Should().Be(TimeSpan.FromHours(-5));
    }

    [Fact]
    public void GetOffset_EuropeLondonDst_ReturnsCorrectOffset()
    {
        // Europe/London during summer (BST) → UTC+1
        var offset = _vali.GetOffset("Europe/London", new DateTime(2025, 8, 1));
        offset.Should().Be(TimeSpan.FromHours(1));
    }

    [Fact]
    public void GetOffset_EuropeLondonWinter_ReturnsUtc()
    {
        // Europe/London during winter (GMT) → UTC+0
        var offset = _vali.GetOffset("Europe/London", new DateTime(2025, 1, 15));
        offset.Should().Be(TimeSpan.Zero);
    }
}
