using System.Collections.Concurrent;
using FluentAssertions;
using Vali_Holiday.Core;
using Vali_Holiday.Models;
using Xunit;

namespace Vali_Time.Tests.Tests;

/// <summary>
/// Caso C stress tests (in-process concurrency, no I/O) — exercises the shared mutable
/// state that <see cref="BaseHolidayProvider"/> and <see cref="ValiHoliday"/> carry.
/// Both are registered as DI singletons in production (<c>AddValiHoliday()</c>), so every
/// concurrent request in a real app hits the same <c>ConcurrentDictionary</c> caches and
/// provider registry this test fires concurrency at directly. BenchmarkDotNet would only
/// measure how fast these are — it cannot detect whether the shared state comes out
/// corrupted after a race. Races are probabilistic, so each test repeats <c>Iterations</c>
/// times with a fresh provider per iteration and fails if any single iteration fails, not
/// just the last one.
/// </summary>
public class HolidayConcurrencyStressTests
{
    private const int Parallelism = 32;
    private const int TimeoutSeconds = 30;

    /// <summary>
    /// Fires concurrent IsHoliday/GetHolidays/GetObservedDate calls across many (country, year)
    /// combinations against a single shared <see cref="IValiHoliday"/> instance — the same
    /// object every concurrent request shares in production — and asserts the fast-path cache
    /// (<see cref="BaseHolidayProvider.IsHoliday(DateTime)"/>) never disagrees with the full
    /// list it is derived from, even while many threads race to populate that cache for the
    /// first time on the same year.
    /// </summary>
    [Fact]
    public async Task ConcurrentAccess_AcrossManyYearsAndCountries_FastPathNeverDisagreesWithFullList()
    {
        const int Iterations = 8;
        const int OpsPerWorker = 300;
        var years = Enumerable.Range(2000, 60).ToArray(); // wide range maximizes first-populate races
        string[] countries = ["PE", "CL", "AR", "CO", "MX", "US", "GB", "CA", "AU", "BR"];

        for (int iteration = 0; iteration < Iterations; iteration++)
        {
            // Fresh instance per iteration: forces every worker to race on cold (not already-built) cache entries.
            var holidays = HolidayProviderFactory.CreateAll();
            var failures = new ConcurrentBag<string>();

            await RunConcurrentlyOrFail(iteration, "fast-path", () =>
            {
                var rng = new Random(unchecked(iteration * 104729 + Task.CurrentId.GetHashCode()));
                for (int i = 0; i < OpsPerWorker; i++)
                {
                    string country = countries[rng.Next(countries.Length)];
                    int year = years[rng.Next(years.Length)];
                    var date = new DateTime(year, rng.Next(1, 13), rng.Next(1, 29));

                    bool fastPath = holidays.IsHoliday(date, country);
                    // Mirrors the fast-path's own documented filter exactly (BaseHolidayProvider.
                    // GetNationalHolidaySet): national AND not a mere Observance (e.g. MX's Feb 14
                    // "Dia de San Valentin" is listed but intentionally excluded from IsHoliday).
                    bool inFullList = holidays.GetHolidays(year, country)
                        .Any(h => h.Month == date.Month && h.Day == date.Day
                            && h.RegionCode == null && h.Type != HolidayType.Observance);

                    if (fastPath != inFullList)
                        failures.Add($"{country} {date:yyyy-MM-dd}: IsHoliday={fastPath} vs GetHolidays-derived={inFullList}");
                }
            });

            failures.Should().BeEmpty($"iteration {iteration} produced fast-path/full-list disagreements under concurrency");
        }
    }

    /// <summary>
    /// Targets <see cref="BaseHolidayProvider.GetObservedDate"/> specifically: concurrently
    /// forces the first build of the UK's 2021 observed-date cache entry (Christmas Day falls
    /// on Saturday, Boxing Day on Sunday that year) from many threads at once, and asserts the
    /// two holidays always resolve to their known distinct substitute days — never to the same
    /// date, and never a value that flips between calls once built.
    /// </summary>
    [Fact]
    public async Task ConcurrentObservedDateBuilds_UkChristmasAndBoxingDay2021_NeverCollideOrFlip()
    {
        const int Iterations = 15;
        var christmasExpected = new DateTime(2021, 12, 27);
        var boxingDayExpected = new DateTime(2021, 12, 28);

        for (int iteration = 0; iteration < Iterations; iteration++)
        {
            var holidays = HolidayProviderFactory.CreateAll(); // fresh cache per iteration
            var provider = holidays.For("GB");

            // Resolving the HolidayInfo instances themselves does not touch the observed-date
            // cache (that only happens inside GetObservedDate) — so the cache below is still
            // cold going into the concurrent burst.
            var ukHolidays2021 = provider.GetHolidays(2021).ToList();
            var christmasHoliday = ukHolidays2021.First(h => h.Month == 12 && h.Day == 25);
            var boxingDayHoliday = ukHolidays2021.First(h => h.Month == 12 && h.Day == 26);

            var observedChristmas = new ConcurrentBag<DateTime?>();
            var observedBoxingDay = new ConcurrentBag<DateTime?>();
            var failures = new ConcurrentBag<string>();

            await RunConcurrentlyOrFail(iteration, "observed-date", () =>
            {
                for (int i = 0; i < 50; i++)
                {
                    var christmas = provider.GetObservedDate(christmasHoliday, 2021);
                    var boxingDay = provider.GetObservedDate(boxingDayHoliday, 2021);

                    observedChristmas.Add(christmas);
                    observedBoxingDay.Add(boxingDay);

                    if (christmas is not null && boxingDay is not null && christmas == boxingDay)
                        failures.Add("Christmas and Boxing Day resolved to the same observed date");
                }
            });

            failures.Should().BeEmpty($"iteration {iteration}: observed-date collision detected");
            observedChristmas.Should().OnlyContain(d => d == christmasExpected,
                $"iteration {iteration}: Christmas observed date flipped under concurrent cache builds");
            observedBoxingDay.Should().OnlyContain(d => d == boxingDayExpected,
                $"iteration {iteration}: Boxing Day observed date flipped under concurrent cache builds");
        }
    }

    /// <summary>
    /// Exercises <see cref="ValiHoliday"/>'s provider registry (<c>_providers</c>) under
    /// concurrent <see cref="ValiHoliday.Register"/> calls racing against concurrent
    /// <see cref="ValiHoliday.For"/>/<see cref="ValiHoliday.IsHoliday(DateTime, string)"/> reads
    /// — the registry is a <c>ConcurrentDictionary</c>, but nothing short of running it proves
    /// a reader never observes a torn/partial registration.
    /// </summary>
    [Fact]
    public async Task ConcurrentRegisterAndQuery_RegistryResolvesEveryCountryAfterAllRegistrationsComplete()
    {
        const int Iterations = 8;
        var allProviders = HolidayProviderFactory.CreateAll().SupportedCountries.ToArray();

        for (int iteration = 0; iteration < Iterations; iteration++)
        {
            var vali = new ValiHoliday();
            var source = HolidayProviderFactory.CreateAll();
            var failures = new ConcurrentBag<string>();

            ThreadPool.SetMinThreads(Parallelism, Parallelism);
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(TimeoutSeconds));

            // Half the workers register providers concurrently; the other half hammer reads
            // (Supports/For/IsHoliday) for the whole duration, including countries not yet registered.
            var registerTasks = allProviders.Select(code => Task.Run(() =>
            {
                try { vali.Register(source.For(code)); }
                catch (Exception ex) { failures.Add($"Register({code}) threw: {ex.Message}"); }
            }));

            var readTasks = Enumerable.Range(0, Parallelism).Select(_ => Task.Run(() =>
            {
                var rng = new Random(Task.CurrentId.GetHashCode());
                for (int i = 0; i < 200; i++)
                {
                    string code = allProviders[rng.Next(allProviders.Length)];
                    try
                    {
                        if (vali.Supports(code))
                        {
                            vali.IsHoliday(new DateTime(2025, 1, 1), code);
                            if (vali.For(code).CountryCode != code)
                                failures.Add($"For({code}) returned a provider for a different country");
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        // Supports(code) was true at check-time but the race window closed before
                        // For(code) ran — not expected (registry only grows, never removes), but if
                        // it ever happens it's a real finding, not swallowed as "fine".
                        failures.Add($"For({code}) threw InvalidOperationException right after Supports({code}) returned true");
                    }
                }
            }));

            try
            {
                await Task.WhenAll(registerTasks.Concat(readTasks)).WaitAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                Assert.Fail($"registry: timeout after {TimeoutSeconds}s on iteration {iteration} — possible deadlock.");
            }

            failures.Should().BeEmpty($"iteration {iteration} found registry inconsistencies under concurrency");
            foreach (var code in allProviders)
                vali.Supports(code).Should().BeTrue($"'{code}' should be registered after all Register calls completed (iteration {iteration})");
        }
    }

    /// <summary>
    /// Runs <paramref name="work"/> on <see cref="Parallelism"/> concurrent workers, raising the
    /// ThreadPool's minimum thread count first (.NET injects pool threads gradually otherwise,
    /// which would mask the very race this is trying to provoke), and fails loudly on timeout
    /// instead of hanging — a timeout here is a deadlock/stuck-lock signal, not a slow run.
    /// </summary>
    private static async Task RunConcurrentlyOrFail(int iteration, string label, Action work)
    {
        ThreadPool.SetMinThreads(Parallelism, Parallelism);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(TimeoutSeconds));
        var tasks = Enumerable.Range(0, Parallelism).Select(_ => Task.Run(work));

        try
        {
            await Task.WhenAll(tasks).WaitAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            Assert.Fail($"{label}: timeout after {TimeoutSeconds}s on iteration {iteration} — possible deadlock/stuck lock.");
        }
    }
}
