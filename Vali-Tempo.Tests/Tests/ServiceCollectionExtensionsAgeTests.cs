using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vali_Age.Core;
using Vali_Age.Extensions;
using Xunit;

namespace Vali_Time.Tests.Tests;

/// <summary>
/// Covers DI registration for <see cref="ServiceCollectionExtensions"/> (Vali-Age).
/// </summary>
public class ServiceCollectionExtensionsAgeTests
{
    [Fact]
    public void AddValiAge_ReturnsSameServiceCollection()
    {
        var services = new ServiceCollection();
        var result = services.AddValiAge();
        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddValiAge_ResolvesIValiAge_NotNull()
    {
        var services = new ServiceCollection();
        services.AddValiAge();
        using var provider = services.BuildServiceProvider();

        provider.GetService<IValiAge>().Should().NotBeNull();
    }

    [Fact]
    public void AddValiAge_ResolvedAsSingleton_SameInstanceAcrossResolutions()
    {
        var services = new ServiceCollection();
        services.AddValiAge();
        using var provider = services.BuildServiceProvider();

        var first = provider.GetService<IValiAge>();
        var second = provider.GetService<IValiAge>();

        first.Should().BeSameAs(second);
    }
}
