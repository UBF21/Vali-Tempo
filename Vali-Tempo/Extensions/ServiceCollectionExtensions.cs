using Microsoft.Extensions.DependencyInjection;
using Vali_Age.Extensions;
using Vali_Calendar.Extensions;
using Vali_CountDown.Extensions;
using Vali_Holiday.Extensions;
using Vali_Range.Extensions;
using Vali_Time.Extensions;
using Vali_TimeZone.Extensions;

namespace Vali_Tempo.Extensions;

/// <summary>
/// Extension methods for registering all Vali-Tempo libraries with the dependency injection container
/// in a single call. This is the entry point for applications that want to use the full
/// Vali-Tempo suite — Vali-Time, Vali-Range, Vali-Calendar, Vali-CountDown, Vali-Age,
/// Vali-Holiday, and Vali-TimeZone — without configuring each library individually.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all Vali-Tempo library services with the dependency injection container.
    /// </summary>
    /// <remarks>
    /// This method calls the following individual registration methods in order:
    /// <list type="bullet">
    ///   <item><description><c>AddValiTime()</c> — registers <c>IValiTime</c> and <c>IValiDate</c> as singletons.</description></item>
    ///   <item><description><c>AddValiRange()</c> — registers <c>IValiRange</c> as a singleton.</description></item>
    ///   <item><description><c>AddValiCalendar()</c> — registers <c>IValiCalendar</c> as a singleton without a holiday provider.</description></item>
    ///   <item><description><c>AddValiCountdown()</c> — registers <c>IValiCountdown</c> as a singleton.</description></item>
    ///   <item><description><c>AddValiAge()</c> — registers <c>IValiAge</c> as a singleton.</description></item>
    ///   <item><description><c>AddValiHoliday()</c> — registers <c>ValiHoliday</c> as a singleton pre-loaded with all built-in country providers (Latin America + Europe + Other).</description></item>
    ///   <item><description><c>AddValiTimeZone()</c> — registers <c>IValiTimeZone</c> as a singleton.</description></item>
    /// </list>
    /// To customise individual library registrations (e.g., using only specific holiday providers),
    /// call each library's own <c>AddVali*</c> extension method directly instead of this helper.
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> for fluent chaining.</returns>
    public static IServiceCollection AddValiTempo(this IServiceCollection services)
    {
        services.AddValiTime();
        services.AddValiRange();
        services.AddValiCalendar();
        services.AddValiCountdown();
        services.AddValiAge();
        services.AddValiHoliday();
        services.AddValiTimeZone();
        return services;
    }

    /// <summary>
    /// Registers all Vali-Tempo library services with the dependency injection container.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> for fluent chaining.</returns>
    [Obsolete("Use AddValiTempo() instead.")]
    public static IServiceCollection AddValiAll(this IServiceCollection services) => services.AddValiTempo();
}
