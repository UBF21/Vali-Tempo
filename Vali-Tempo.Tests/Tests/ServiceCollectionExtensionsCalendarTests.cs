using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vali_Calendar.Core;
using Vali_Calendar.Extensions;
using Vali_Holiday.Providers.LatinAmerica;
using Xunit;

namespace Vali_Time.Tests.Tests;

/// <summary>
/// Covers DI registration for <see cref="ServiceCollectionExtensions"/> (Vali-Calendar).
/// </summary>
public class ServiceCollectionExtensionsCalendarTests
{
    [Fact]
    public void AddValiCalendar_WithoutHolidayProvider_ResolvesSingletonIValiCalendar()
    {
        var services = new ServiceCollection();
        services.AddValiCalendar();
        var provider = services.BuildServiceProvider();

        var first = provider.GetRequiredService<IValiCalendar>();
        var second = provider.GetRequiredService<IValiCalendar>();

        first.Should().NotBeNull();
        first.Should().BeSameAs(second);
        first.HasHolidayProvider.Should().BeFalse();
    }

    [Fact]
    public void AddValiCalendar_WithHolidayProvider_ResolvesSingletonWithHolidayProviderConfigured()
    {
        var services = new ServiceCollection();
        services.AddValiCalendar(new PeruHolidayProvider());
        var provider = services.BuildServiceProvider();

        var first = provider.GetRequiredService<IValiCalendar>();
        var second = provider.GetRequiredService<IValiCalendar>();

        first.Should().BeSameAs(second);
        first.HasHolidayProvider.Should().BeTrue();
    }
}
