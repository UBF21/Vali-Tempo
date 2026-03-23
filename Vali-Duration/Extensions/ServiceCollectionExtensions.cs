using Microsoft.Extensions.DependencyInjection;
using Vali_Duration.Models;
using Vali_Time.Enums;

namespace Vali_Duration.Extensions;

public static class TimeSpanExtensions
{
    /// Converts a TimeSpan to a ValiDuration with full decimal precision.
    public static ValiDuration ToValiDuration(this TimeSpan ts) => ValiDuration.FromTimeSpan(ts);

    /// Converts a ValiDuration to the specified TimeUnit as decimal.
    public static decimal As(this ValiDuration duration, TimeUnit unit) => duration.As(unit);
}

/// <summary>
/// Extension methods for registering Vali-Duration services with the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers Vali-Duration with the dependency injection container.
    /// </summary>
    /// <remarks>
    /// <see cref="ValiDuration"/> is a value type with static factory methods and requires no
    /// service registration. This method is provided for consistency with the Vali-Tempo
    /// meta-package registration pattern.
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> for fluent chaining.</returns>
    public static IServiceCollection AddValiDuration(this IServiceCollection services)
    {
        return services;
    }
}
