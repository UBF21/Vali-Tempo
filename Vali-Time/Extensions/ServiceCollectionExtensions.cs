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
    /// Registers <see cref="IValiTime"/> and <see cref="IValiDate"/> as singletons. Also
    /// registers <see cref="IValiDateQuarter"/>, resolving to the same <see cref="IValiDate"/>
    /// singleton instance, so consumers that only need quarter operations can depend on the
    /// narrower interface without pulling in the full <see cref="IValiDate"/> surface.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddValiTime(this IServiceCollection services)
    {
        services.AddSingleton<IValiTime, ValiTime>();
        services.AddSingleton<IValiDate, ValiDate>();
        services.AddSingleton<IValiDateQuarter>(sp => sp.GetRequiredService<IValiDate>());
        services.AddSingleton<IClock>(_ => SystemClock.Instance);
        return services;
    }
}
