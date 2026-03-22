using Microsoft.Extensions.DependencyInjection;
using Vali_Schedule.Core;

namespace Vali_Schedule.Extensions;

/// <summary>
/// Provides extension methods for registering Vali-Schedule services
/// with the Microsoft Dependency Injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="ValiSchedule"/> as a transient implementation of <see cref="IValiSchedule"/>
    /// in the dependency injection container.
    /// </summary>
    /// <remarks>
    /// <see cref="ValiSchedule"/> is registered as <b>transient</b> because it acts as a stateful
    /// builder: each instance carries its own <c>ScheduleConfig</c> and must not be shared
    /// across consumers or requests.
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for fluent chaining.</returns>
    public static IServiceCollection AddValiSchedule(this IServiceCollection services)
    {
        services.AddTransient<IValiSchedule, ValiSchedule>();
        return services;
    }
}
