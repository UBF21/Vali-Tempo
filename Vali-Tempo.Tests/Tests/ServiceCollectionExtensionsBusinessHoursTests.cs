using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vali_BusinessHours.Core;
using Vali_BusinessHours.Extensions;
using Vali_TimeZone.Extensions;
using Xunit;

namespace Vali_Time.Tests.Tests;

/// <summary>
/// Covers DI registration for <see cref="ServiceCollectionExtensions"/> (Vali-BusinessHours).
/// </summary>
public class ServiceCollectionExtensionsBusinessHoursTests
{
    [Fact]
    public void AddValiBusinessHours_ResolvesSingletonIValiBusinessHours()
    {
        var services = new ServiceCollection();
        services.AddValiTimeZone();
        services.AddValiBusinessHours();
        var provider = services.BuildServiceProvider();

        var first = provider.GetRequiredService<IValiBusinessHours>();
        var second = provider.GetRequiredService<IValiBusinessHours>();

        first.Should().NotBeNull();
        first.Should().BeSameAs(second);
    }
}
