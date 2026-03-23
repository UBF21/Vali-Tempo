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
    /// <remarks>
    /// <para>
    /// <see cref="IClock"/> is resolved via <c>sp.GetService&lt;IClock&gt;()</c> with a fallback to
    /// <see cref="SystemClock.Instance"/> when no <see cref="IClock"/> is registered in the container.
    /// </para>
    /// <para>
    /// To use a custom clock (e.g., for deterministic testing), register a custom <see cref="IClock"/>
    /// implementation in the container <em>before</em> calling <c>AddValiAge()</c>, or call
    /// <c>AddValiTime()</c> first, which registers <see cref="SystemClock.Instance"/> as the
    /// default <see cref="IClock"/>.
    /// </para>
    /// </remarks>
    public static IServiceCollection AddValiAge(this IServiceCollection services)
    {
        services.AddSingleton<IValiAge>(sp => new ValiAge(sp.GetService<IClock>()));
        return services;
    }
}
