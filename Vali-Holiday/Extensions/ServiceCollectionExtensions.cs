using Microsoft.Extensions.DependencyInjection;
using Vali_Holiday.Core;

namespace Vali_Holiday.Extensions;

/// <summary>
/// Extension methods for registering Vali-Holiday services with the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers a <see cref="IValiHoliday"/> singleton pre-loaded with ALL built-in country providers
    /// (Latin America + Europe).
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddValiHoliday(this IServiceCollection services)
        => services.AddSingleton<IValiHoliday>(_ => HolidayProviderFactory.CreateAll());

    /// <summary>
    /// Registers a <see cref="IValiHoliday"/> singleton pre-loaded with only Latin American country providers.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddValiHolidayLatinAmerica(this IServiceCollection services)
        => services.AddSingleton<IValiHoliday>(_ => HolidayProviderFactory.CreateLatinAmerica());

    /// <summary>
    /// Registers a <see cref="IValiHoliday"/> singleton pre-loaded with only European country providers.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddValiHolidayEurope(this IServiceCollection services)
        => services.AddSingleton<IValiHoliday>(_ => HolidayProviderFactory.CreateEurope());

    /// <summary>
    /// Registers a <see cref="IValiHoliday"/> singleton configured via the provided delegate,
    /// allowing callers to register only the providers they need.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configure">
    /// A delegate that receives an empty <see cref="ValiHoliday"/> instance and can call
    /// <see cref="ValiHoliday.Register"/> to add custom or built-in providers.
    /// </param>
    /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddValiHoliday(this IServiceCollection services, Action<ValiHoliday> configure)
    {
        var valiHoliday = new ValiHoliday();
        configure(valiHoliday);
        services.AddSingleton<IValiHoliday>(valiHoliday);
        return services;
    }
}
