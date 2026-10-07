using FluentAssertions;
using Vali_Range.Core;
using Xunit;

namespace Vali_Time.Tests.Tests;

/// <summary>
/// Contract-level checks for <see cref="IValiRange"/>. Behavioral coverage of each
/// operation lives in <see cref="ValiRangeTests"/>, exercised through the <c>ValiRange</c>
/// implementation.
/// </summary>
public class IValiRangeTests
{
    [Fact]
    public void ValiRange_ImplementsIValiRange()
    {
        typeof(IValiRange).IsAssignableFrom(typeof(ValiRange)).Should().BeTrue();
    }

    [Fact]
    public void Interface_DeclaresSubtract()
    {
        typeof(IValiRange).GetMethod(nameof(IValiRange.Subtract)).Should().NotBeNull();
    }
}
