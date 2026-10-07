namespace Vali_BusinessHours.Models;

/// <summary>
/// Defines a single daily business-hours block for one IANA timezone — e.g.
/// "09:00–18:00 in America/Lima". Assumed to apply identically on every workday
/// (v1 does not support per-day or recurring exceptions).
/// </summary>
/// <param name="ZoneId">The IANA timezone ID, e.g. <c>"America/Lima"</c>.</param>
/// <param name="Start">The inclusive local start time of the business-hours block.</param>
/// <param name="End">The exclusive local end time of the business-hours block. Must be later than <paramref name="Start"/>.</param>
public readonly record struct BusinessHoursWindow(string ZoneId, TimeOnly Start, TimeOnly End);
