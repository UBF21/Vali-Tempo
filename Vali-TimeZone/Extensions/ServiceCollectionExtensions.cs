using Microsoft.Extensions.DependencyInjection;
using Vali_Time.Abstractions;
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
    /// <remarks>
    /// <para>
    /// <see cref="IClock"/> is resolved via <c>sp.GetService&lt;IClock&gt;()</c> with a fallback to
    /// <see cref="SystemClock.Instance"/> when no <see cref="IClock"/> is registered in the container.
    /// </para>
    /// <para>
    /// To use a custom clock (e.g., for deterministic testing), register a custom <see cref="IClock"/>
    /// implementation in the container <em>before</em> calling <c>AddValiTimeZone()</c>, or call
    /// <c>AddValiTime()</c> first, which registers <see cref="SystemClock.Instance"/> as the
    /// default <see cref="IClock"/>.
    /// </para>
    /// </remarks>
    public static IServiceCollection AddValiTimeZone(this IServiceCollection services)
    {
        services.AddSingleton<IValiTimeZone>(sp => new ValiTimeZone(sp.GetService<IClock>()));
        return services;
    }
}
