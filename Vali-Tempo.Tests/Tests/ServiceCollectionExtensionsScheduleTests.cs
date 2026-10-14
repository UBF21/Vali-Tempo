using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vali_Schedule.Core;
using Vali_Schedule.Extensions;
using Xunit;

namespace Vali_Time.Tests.Tests;

/// <summary>
/// Covers DI registration for <see cref="ServiceCollectionExtensions"/> (Vali-Schedule).
/// </summary>
public class ServiceCollectionExtensionsScheduleTests
{
    [Fact]
    public void AddValiSchedule_ResolvesTransient_DifferentInstancesPerResolution()
    {
        var services = new ServiceCollection();
        services.AddValiSchedule();
        var provider = services.BuildServiceProvider();

        var first = provider.GetRequiredService<IValiSchedule>();
        var second = provider.GetRequiredService<IValiSchedule>();

        first.Should().NotBeNull();
        first.Should().NotBeSameAs(second); // transient: ValiSchedule is a stateful builder, must not be shared
    }
}
