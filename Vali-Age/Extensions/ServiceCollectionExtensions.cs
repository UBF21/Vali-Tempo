using Microsoft.Extensions.DependencyInjection;
using Vali_Age.Core;
using Vali_Time.Abstractions;

namespace Vali_Age.Extensions;

/// <summary>
/// Extension methods for registering Vali-Age services with the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IValiAge"/> as a singleton.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddValiAge(this IServiceCollection services)
    {
        services.AddSingleton<IValiAge>(sp => new ValiAge(sp.GetService<IClock>()));
        return services;
    }
}
