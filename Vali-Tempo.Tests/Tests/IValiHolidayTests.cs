using FluentAssertions;
using Vali_Holiday.Core;
using Xunit;

namespace Vali_Time.Tests.Tests;

/// <summary>
/// Contract-level checks for <see cref="IValiHoliday"/>. Behavioral coverage lives in
/// <see cref="ValiHolidayTests"/>, exercised through the <c>ValiHoliday</c> implementation.
/// </summary>
public class IValiHolidayTests
{
    [Fact]
    public void ValiHoliday_ImplementsIValiHoliday()
    {
        typeof(IValiHoliday).IsAssignableFrom(typeof(ValiHoliday)).Should().BeTrue();
    }

    [Fact]
    public void Interface_DeclaresMultiCountryIsHolidayOverloads()
    {
        typeof(IValiHoliday).GetMethod(nameof(IValiHoliday.IsHolidayInAll)).Should().NotBeNull();
    }
}
