using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vali_Range.Core;
using Vali_Range.Extensions;
using Xunit;

namespace Vali_Time.Tests.Tests;

public class ServiceCollectionExtensionsRangeTests
{
    [Fact]
    public void AddValiRange_ResolvesIValiRange()
    {
        var services = new ServiceCollection();
        services.AddValiRange();
        var provider = services.BuildServiceProvider();

        var range = provider.GetRequiredService<IValiRange>();

        range.Should().NotBeNull();
    }

    [Fact]
    public void AddValiRange_RegistersAsSingleton()
    {
        var services = new ServiceCollection();
        services.AddValiRange();
        var provider = services.BuildServiceProvider();

        var first  = provider.GetRequiredService<IValiRange>();
        var second = provider.GetRequiredService<IValiRange>();

        first.Should().BeSameAs(second);
    }
}
