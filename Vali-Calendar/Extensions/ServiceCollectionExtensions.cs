using Microsoft.Extensions.DependencyInjection;
using Vali_Calendar.Core;
using Vali_Holiday.Core;
using Vali_Time.Abstractions;
using Vali_Time.Enums;

namespace Vali_Calendar.Extensions;

/// <summary>
/// Extension methods for registering Vali-Calendar services with the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IValiCalendar"/> as a singleton without a holiday provider.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="weekStart">The default first day of the week. Defaults to <see cref="WeekStart.Monday"/>.</param>
    /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
    /// <remarks>
    /// <para>
    /// <see cref="IClock"/> is resolved via <c>sp.GetService&lt;IClock&gt;()</c> with a fallback to
    /// <see cref="SystemClock.Instance"/> when no <see cref="IClock"/> is registered in the container.
    /// </para>
    /// <para>
    /// To use a custom clock (e.g., for deterministic testing), register a custom <see cref="IClock"/>
    /// implementation in the container <em>before</em> calling <c>AddValiCalendar()</c>, or call
    /// <c>AddValiTime()</c> first, which registers <see cref="SystemClock.Instance"/> as the
    /// default <see cref="IClock"/>.
    /// </para>
    /// </remarks>
    public static IServiceCollection AddValiCalendar(this IServiceCollection services, WeekStart weekStart = WeekStart.Monday)
        => services.AddSingleton<IValiCalendar>(sp => new ValiCalendar(weekStart, null, sp.GetService<IClock>()));

    /// <summary>
    /// Registers <see cref="IValiCalendar"/> as a singleton with the specified holiday provider,
    /// so that workday calculations automatically exclude public holidays.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="holidayProvider">The <see cref="IHolidayProvider"/> to integrate with workday logic.</param>
    /// <param name="weekStart">The default first day of the week. Defaults to <see cref="WeekStart.Monday"/>.</param>
    /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
    /// <remarks>
    /// <para>
    /// <see cref="IClock"/> is resolved via <c>sp.GetService&lt;IClock&gt;()</c> with a fallback to
    /// <see cref="SystemClock.Instance"/> when no <see cref="IClock"/> is registered in the container.
    /// </para>
    /// <para>
    /// To use a custom clock (e.g., for deterministic testing), register a custom <see cref="IClock"/>
    /// implementation in the container <em>before</em> calling <c>AddValiCalendar()</c>, or call
    /// <c>AddValiTime()</c> first, which registers <see cref="SystemClock.Instance"/> as the
    /// default <see cref="IClock"/>.
    /// </para>
    /// </remarks>
    public static IServiceCollection AddValiCalendar(this IServiceCollection services, IHolidayProvider holidayProvider, WeekStart weekStart = WeekStart.Monday)
        => services.AddSingleton<IValiCalendar>(sp => new ValiCalendar(weekStart, holidayProvider, sp.GetService<IClock>()));
}
