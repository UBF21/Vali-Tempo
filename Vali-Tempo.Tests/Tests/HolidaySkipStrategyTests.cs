using FluentAssertions;
using Vali_Schedule.Enums;
using Xunit;

namespace Vali_Time.Tests.Tests;

public class HolidaySkipStrategyTests
{
    [Fact]
    public void Values_MatchDocumentedUnderlyingIntegers()
    {
        ((int)HolidaySkipStrategy.NextWorkday).Should().Be(0);
        ((int)HolidaySkipStrategy.PreviousWorkday).Should().Be(1);
        ((int)HolidaySkipStrategy.Skip).Should().Be(2);
    }
}
