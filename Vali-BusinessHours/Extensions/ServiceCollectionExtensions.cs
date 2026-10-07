using Microsoft.Extensions.DependencyInjection;
using Vali_BusinessHours.Core;
using Vali_TimeZone.Core;

namespace Vali_BusinessHours.Extensions;

/// <summary>
/// Extension methods for registering Vali-BusinessHours services with the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IValiBusinessHours"/> as a singleton.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
    /// <remarks>
    /// Requires an <see cref="IValiTimeZone"/> to already be registered (e.g. via
    /// <c>AddValiTimeZone()</c>) — <see cref="ValiBusinessHours"/> resolves it from the container.
    /// </remarks>
    public static IServiceCollection AddValiBusinessHours(this IServiceCollection services)
    {
        services.AddSingleton<IValiBusinessHours, ValiBusinessHours>();
        return services;
    }
}
