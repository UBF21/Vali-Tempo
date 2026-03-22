using Microsoft.Extensions.DependencyInjection;
using Vali_Range.Core;

namespace Vali_Range.Extensions;

/// <summary>
/// Extension methods for registering Vali-Range services with the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IValiRange"/> as a singleton.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddValiRange(this IServiceCollection services)
    {
        services.AddSingleton<IValiRange, ValiRange>();
        return services;
    }
}
