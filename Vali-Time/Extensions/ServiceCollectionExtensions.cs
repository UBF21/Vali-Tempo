using Microsoft.Extensions.DependencyInjection;
using Vali_Time.Abstractions;
using Vali_Time.Core;

namespace Vali_Time.Extensions;

/// <summary>
/// Extension methods for registering Vali-Time services with the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IValiTime"/> and <see cref="IValiDate"/> as singletons.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddValiTime(this IServiceCollection services)
    {
        services.AddSingleton<IValiTime, ValiTime>();
        services.AddSingleton<IValiDate, ValiDate>();
        services.AddSingleton<IClock, SystemClock>();
        return services;
    }
}
