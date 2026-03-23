using Microsoft.Extensions.DependencyInjection;
using Vali_CountDown.Core;
using Vali_Time.Abstractions;

namespace Vali_CountDown.Extensions;

/// <summary>
/// Extension methods for registering Vali-CountDown services with the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IValiCountdown"/> as a singleton.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddValiCountdown(this IServiceCollection services)
    {
        services.AddSingleton<IValiCountdown>(sp => new ValiCountdown(sp.GetService<IClock>()));
        return services;
    }
}
