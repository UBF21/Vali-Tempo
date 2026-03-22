namespace Vali_Holiday.Models;

/// <summary>
/// Classifies the nature of a public holiday entry.
/// </summary>
public enum HolidayType
{
    /// <summary>National public holiday — everyone has the day off.</summary>
    National,

    /// <summary>Religious observance recognized as a public holiday.</summary>
    Religious,

    /// <summary>Civic or patriotic commemoration.</summary>
    Civic,

    /// <summary>Only applicable to certain regions or states.</summary>
    Regional,

    /// <summary>Observed culturally but not a mandatory day off.</summary>
    Observance,

    /// <summary>Optional bank holiday.</summary>
    Optional
}
