using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vali_Bucket.Core;
using Vali_Bucket.Extensions;
using Xunit;

namespace Vali_Time.Tests.Tests;

/// <summary>
/// Covers DI registration for <see cref="ServiceCollectionExtensions"/> (Vali-Bucket).
/// </summary>
public class ServiceCollectionExtensionsBucketTests
{
    [Fact]
    public void AddValiBucket_ResolvesSingletonIValiBucket()
    {
        var services = new ServiceCollection();
        services.AddValiBucket();
        var provider = services.BuildServiceProvider();

        var first = provider.GetRequiredService<IValiBucket>();
        var second = provider.GetRequiredService<IValiBucket>();

        first.Should().NotBeNull();
        first.Should().BeSameAs(second);
    }
}
