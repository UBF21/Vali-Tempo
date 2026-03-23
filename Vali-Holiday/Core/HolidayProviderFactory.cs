using Vali_Holiday.Providers.Europe;
using Vali_Holiday.Providers.LatinAmerica;
using Vali_Holiday.Providers.NorthAmerica;
using Vali_Holiday.Providers.Oceania;

namespace Vali_Holiday.Core;

/// <summary>
/// Static factory that assembles <see cref="ValiHoliday"/> instances pre-loaded with
/// all built-in country providers. Use the granular methods to obtain only the subset
/// of providers relevant to your application domain.
/// </summary>
/// <remarks>
/// Example usage:
/// <code>
/// // All built-in providers (Latin America + Europe + Other):
/// var holidays = HolidayProviderFactory.CreateAll();
///
/// // Only European providers:
/// var euroHolidays = HolidayProviderFactory.CreateEurope();
///
/// // Only Latin American providers:
/// var latamHolidays = HolidayProviderFactory.CreateLatinAmerica();
///
/// // Canada and Australia only:
/// var otherHolidays = HolidayProviderFactory.CreateOther();
/// </code>
/// </remarks>
public static class HolidayProviderFactory
{
    /// <summary>
    /// Creates a <see cref="ValiHoliday"/> instance with ALL built-in providers
    /// registered — Latin American (19 countries), European, and Other (Canada, Australia).
    /// </summary>
    /// <returns>
    /// A fully-populated <see cref="ValiHoliday"/> ready for use.
    /// </returns>
    public static ValiHoliday CreateAll() => new ValiHoliday(GetAllProviders());

    /// <summary>
    /// Creates a <see cref="ValiHoliday"/> instance containing only the
    /// built-in Latin American country providers (19 countries).
    /// </summary>
    /// <returns>
    /// A <see cref="ValiHoliday"/> with Latin American providers registered.
    /// </returns>
    public static ValiHoliday CreateLatinAmerica() => new ValiHoliday(GetLatinAmericaProviders());

    /// <summary>
    /// Creates a <see cref="ValiHoliday"/> instance containing only the
    /// built-in European country providers.
    /// </summary>
    /// <returns>
    /// A <see cref="ValiHoliday"/> with European providers registered.
    /// </returns>
    public static ValiHoliday CreateEurope() => new ValiHoliday(GetEuropeProviders());

    /// <summary>
    /// Creates a <see cref="ValiHoliday"/> instance containing only the
    /// built-in Other providers (USA, Canada, and Australia).
    /// </summary>
    /// <returns>
    /// A <see cref="ValiHoliday"/> with USA, Canada, and Australia providers registered.
    /// </returns>
    public static ValiHoliday CreateOther() => new ValiHoliday(GetOtherProviders());

    /// <summary>
    /// Returns the combined enumerable of all built-in providers from
    /// <see cref="GetLatinAmericaProviders"/>, <see cref="GetEuropeProviders"/>,
    /// and <see cref="GetOtherProviders"/>.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of all built-in <see cref="IHolidayProvider"/> instances.
    /// </returns>
    public static IEnumerable<IHolidayProvider> GetAllProviders()
        => GetLatinAmericaProviders()
            .Concat(GetEuropeProviders())
            .Concat(GetOtherProviders());

    /// <summary>
    /// Returns the enumerable of all built-in Latin American country providers.
    /// Covers 19 countries: Peru, Chile, Argentina, Colombia, Mexico, Brazil,
    /// Ecuador, Bolivia, Uruguay, Paraguay, Venezuela, Panama, Costa Rica,
    /// Dominican Republic, Cuba, Guatemala, Honduras, El Salvador, and Nicaragua.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of Latin American <see cref="IHolidayProvider"/> instances.
    /// </returns>
    public static IEnumerable<IHolidayProvider> GetLatinAmericaProviders() => new IHolidayProvider[]
    {
        new PeruHolidayProvider(),
        new ChileHolidayProvider(),
        new ArgentinaHolidayProvider(),
        new ColombiaHolidayProvider(),
        new MexicoHolidayProvider(),
        new BrazilHolidayProvider(),
        new EcuadorHolidayProvider(),
        new BoliviaHolidayProvider(),
        new UruguayHolidayProvider(),
        new ParaguayHolidayProvider(),
        new VenezuelaHolidayProvider(),
        new PanamaHolidayProvider(),
        new CostaRicaHolidayProvider(),
        new DominicanRepublicHolidayProvider(),
        new CubaHolidayProvider(),
        new GuatemalaHolidayProvider(),
        new HondurasHolidayProvider(),
        new ElSalvadorHolidayProvider(),
        new NicaraguaHolidayProvider(),
    };

    /// <summary>
    /// Returns the enumerable of all built-in European country providers.
    /// Covers 16 countries: Spain, United Kingdom, Germany, France, Italy, Portugal,
    /// Netherlands, Belgium, Switzerland, Austria, Poland, Sweden, Norway, Denmark,
    /// Finland, and Ireland.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of European <see cref="IHolidayProvider"/> instances.
    /// </returns>
    public static IEnumerable<IHolidayProvider> GetEuropeProviders() => new IHolidayProvider[]
    {
        new SpainHolidayProvider(),
        new UnitedKingdomHolidayProvider(),
        new GermanyHolidayProvider(),
        new FranceHolidayProvider(),
        new ItalyHolidayProvider(),
        new PortugalHolidayProvider(),
        new NetherlandsHolidayProvider(),
        new BelgiumHolidayProvider(),
        new SwitzerlandHolidayProvider(),
        new AustriaHolidayProvider(),
        new PolandHolidayProvider(),
        new SwedenHolidayProvider(),
        new NorwayHolidayProvider(),
        new DenmarkHolidayProvider(),
        new FinlandHolidayProvider(),
        new IrelandHolidayProvider(),
    };

    /// <summary>
    /// Returns the enumerable of built-in providers for countries outside Latin America
    /// and Europe: USA, Canada, and Australia.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="IHolidayProvider"/> instances for
    /// USA, Canada, and Australia.
    /// </returns>
    public static IEnumerable<IHolidayProvider> GetOtherProviders() => new IHolidayProvider[]
    {
        new UsaHolidayProvider(),
        new CanadaHolidayProvider(),
        new AustraliaHolidayProvider(),
    };
}
