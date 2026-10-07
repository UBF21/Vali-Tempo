using FluentAssertions;
using Vali_Schedule.Core;
using Xunit;

namespace Vali_Time.Tests.Tests;

/// <summary>
/// Contract-level checks for <see cref="IValiSchedule"/>. Behavioral coverage of the
/// fluent builder and query methods lives in <see cref="ValiScheduleTests"/>, exercised
/// through the <c>ValiSchedule</c> implementation.
/// </summary>
public class IValiScheduleTests
{
    [Fact]
    public void ValiSchedule_ImplementsIValiSchedule()
    {
        typeof(IValiSchedule).IsAssignableFrom(typeof(ValiSchedule)).Should().BeTrue();
    }

    [Fact]
    public void Interface_DeclaresSkipWeekendsAndSkipHolidays()
    {
        typeof(IValiSchedule).GetMethod(nameof(IValiSchedule.SkipWeekends)).Should().NotBeNull();
        typeof(IValiSchedule).GetMethod(nameof(IValiSchedule.SkipHolidays)).Should().NotBeNull();
    }
}
