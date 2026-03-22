using Microsoft.Extensions.DependencyInjection;
using Vali_TimeZone.Core;

namespace Vali_TimeZone.Extensions;

/// <summary>
/// Extension methods for registering Vali-TimeZone services with
/// <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="ValiTimeZone"/> as the <see cref="IValiTimeZone"/>
    /// implementation with a singleton lifetime.
    /// </summary>
    /// <param name="services">The service collection to add to.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddValiTimeZone(this IServiceCollection services)
    {
        services.AddSingleton<IValiTimeZone, ValiTimeZone>();
        return services;
    }
}
