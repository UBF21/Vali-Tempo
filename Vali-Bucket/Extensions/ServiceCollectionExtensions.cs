using Microsoft.Extensions.DependencyInjection;
using Vali_Bucket.Core;

namespace Vali_Bucket.Extensions;

/// <summary>
/// Extension methods for registering Vali-Bucket services with the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IValiBucket"/> as a singleton.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddValiBucket(this IServiceCollection services)
    {
        services.AddSingleton<IValiBucket, ValiBucket>();
        return services;
    }
}
