using Vali_Duration.Models;
using Vali_Time.Enums;

namespace Vali_Duration.Extensions;

public static class TimeSpanExtensions
{
    /// Converts a TimeSpan to a ValiDuration with full decimal precision.
    public static ValiDuration ToValiDuration(this TimeSpan ts) => ValiDuration.FromTimeSpan(ts);

    /// Converts a ValiDuration to the specified TimeUnit as decimal.
    public static decimal As(this ValiDuration duration, TimeUnit unit) => duration.As(unit);
}
