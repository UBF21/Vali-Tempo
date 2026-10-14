using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vali_CountDown.Core;
using Vali_CountDown.Extensions;
using Xunit;

namespace Vali_Time.Tests.Tests;

/// <summary>
/// Covers DI registration for <see cref="ServiceCollectionExtensions"/> (Vali-CountDown).
/// </summary>
public class ServiceCollectionExtensionsCountdownTests
{
    [Fact]
    public void AddValiCountdown_ResolvesSingletonIValiCountdown()
    {
        var services = new ServiceCollection();
        services.AddValiCountdown();
        var provider = services.BuildServiceProvider();

        var first = provider.GetRequiredService<IValiCountdown>();
        var second = provider.GetRequiredService<IValiCountdown>();

        first.Should().NotBeNull();
        first.Should().BeSameAs(second);
    }
}
