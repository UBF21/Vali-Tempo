using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vali_Time.Abstractions;
using Vali_Time.Core;
using Vali_Time.Extensions;
using Xunit;

namespace Vali_Time.Tests.Tests;

/// <summary>
/// Covers DI registration for <see cref="ServiceCollectionExtensions"/> (Vali-Time): resolves
/// <see cref="IValiTime"/>, <see cref="IValiDate"/>, <see cref="IValiDateQuarter"/>, and
/// <see cref="IClock"/> as singletons.
/// </summary>
public class ServiceCollectionExtensionsTimeTests
{
    [Fact]
    public void AddValiTime_ResolvesSingletonIValiTime()
    {
        var services = new ServiceCollection();
        services.AddValiTime();
        var provider = services.BuildServiceProvider();

        var first = provider.GetRequiredService<IValiTime>();
        var second = provider.GetRequiredService<IValiTime>();

        first.Should().NotBeNull();
        first.Should().BeSameAs(second);
    }

    [Fact]
    public void AddValiTime_ResolvesSingletonIValiDate()
    {
        var services = new ServiceCollection();
        services.AddValiTime();
        var provider = services.BuildServiceProvider();

        var first = provider.GetRequiredService<IValiDate>();
        var second = provider.GetRequiredService<IValiDate>();

        first.Should().NotBeNull();
        first.Should().BeSameAs(second);
    }

    [Fact]
    public void AddValiTime_IValiDateQuarter_ResolvesToSameInstanceAsIValiDate()
    {
        var services = new ServiceCollection();
        services.AddValiTime();
        var provider = services.BuildServiceProvider();

        var date = provider.GetRequiredService<IValiDate>();
        var quarter = provider.GetRequiredService<IValiDateQuarter>();

        quarter.Should().BeSameAs(date);
    }

    [Fact]
    public void AddValiTime_ResolvesSingletonIClock()
    {
        var services = new ServiceCollection();
        services.AddValiTime();
        var provider = services.BuildServiceProvider();

        var first = provider.GetRequiredService<IClock>();
        var second = provider.GetRequiredService<IClock>();

        first.Should().NotBeNull();
        first.Should().BeSameAs(second);
        first.Should().BeSameAs(SystemClock.Instance);
    }

    [Fact]
    public void AddValiTime_ReturnsSameServiceCollectionForChaining()
    {
        var services = new ServiceCollection();
        var result = services.AddValiTime();
        result.Should().BeSameAs(services);
    }
}
