using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vali_Duration.Extensions;
using Vali_Duration.Models;
using Vali_Time.Enums;
using Xunit;

namespace Vali_Time.Tests.Tests;

/// <summary>
/// Covers DI registration for <see cref="ServiceCollectionExtensions"/> (Vali-Duration)
/// and the <see cref="TimeSpanExtensions"/> conversion helpers living in the same file.
/// </summary>
public class ServiceCollectionExtensionsDurationTests
{
    [Fact]
    public void AddValiDuration_ReturnsSameServiceCollection()
    {
        var services = new ServiceCollection();
        var result = services.AddValiDuration();
        result.Should().BeSameAs(services);
    }

    [Fact]
    public void ToValiDuration_FromTimeSpan_IsCorrect()
    {
        TimeSpan ts = TimeSpan.FromHours(2);
        ValiDuration duration = ts.ToValiDuration();
        duration.TotalHours.Should().BeApproximately(2m, 0.0001m);
    }

    [Fact]
    public void As_ExtensionMethod_DelegatesToValiDurationAs()
    {
        // Called via the static form: ValiDuration already has an instance method with the
        // same signature, which C# overload resolution prefers over the extension method,
        // so instance-call syntax would never actually hit TimeSpanExtensions.As.
        ValiDuration duration = ValiDuration.FromHours(2m);
        TimeSpanExtensions.As(duration, TimeUnit.Minutes).Should().Be(120m);
    }
}
