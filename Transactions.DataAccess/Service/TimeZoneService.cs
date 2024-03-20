using GeoTimeZone;

namespace TransactionsAPI.Services;

/// <summary>
/// Working with client location and time zone info by IANA
/// https://www.iana.org/time-zones
/// </summary>
public static class TimeZoneService
{
    /// <summary>
    /// The method convert a client location to current time zone
    /// </summary>
    /// <param name="clientLocation">Specific user location, which contain coordinates (latitude, longitude).</param>
    /// <returns>object of TimeZoneInfo</returns>
    public static TimeZoneInfo ConvertToTimeZoneInfo(string clientLocation)
    {
        var coordinates = clientLocation.Split(",");
        var latitude = double.Parse(coordinates[0]);
        var longitude = double.Parse(coordinates[1]);

        var timeZoneId = TimeZoneLookup.GetTimeZone(latitude, longitude).Result;

        return CreateTimeZoneById(timeZoneId);
    }

    /// <summary>
    /// Create a some time zone by specific id
    /// </summary>
    /// <param name="timeZoneId">Specific time zone id by IANA</param>
    /// <returns>object of TimeZoneInfo</returns>
    public static TimeZoneInfo CreateTimeZoneById(string timeZoneId)
    {
        // Some issue with region Kyiv.
        // Details: https://github.com/dotnet/runtime/issues/83188
        if (timeZoneId is "Europe/Kyiv")
            timeZoneId = "Europe/Kiev";

        if (timeZoneId is "Antarctica/Troll")
            return CreateTrollTimeZone();
        
        return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
    }
    
    // Unfortunately, TimeZoneOnly does not contain a "Antarctica/Troll" and one of solutions are here
    // Issue: https://github.com/mattjohnsonpint/TimeZoneConverter/issues/62
    private static TimeZoneInfo CreateTrollTimeZone() =>
        TimeZoneInfo.CreateCustomTimeZone(
            id: "Antarctica/Troll",
            baseUtcOffset: TimeSpan.Zero,
            displayName: "(UTC+00:00) Troll Station, Antarctica",
            standardDisplayName: "Greenwich Mean Time",
            daylightDisplayName: "Central European Summer Time",
            adjustmentRules: new[]
            {
                // Like IANA, we will approximate with only UTC and CEST (UTC+2).
                // Handling the CET (UTC+1) period would require generating adjustment rules for each individual year.
                TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(
                    dateStart: DateTime.MinValue,
                    dateEnd: DateTime.MaxValue,
                    daylightDelta: TimeSpan.FromHours(2), // Two hours DST gap
                    daylightTransitionStart: TimeZoneInfo.TransitionTime.CreateFloatingDateRule(
                        timeOfDay: new DateTime(1, 1, 1, 1, 0, 0), // 01:00 local, 01:00 UTC
                        month: 3, // March
                        week: 5, // the last week of the month
                        DayOfWeek.Sunday),
                    daylightTransitionEnd: TimeZoneInfo.TransitionTime.CreateFloatingDateRule(
                        timeOfDay: new DateTime(1, 1, 1, 3, 0, 0), // 03:00 local, 01:00 UTC
                        month: 10, // October
                        week: 5, // the last week of the month
                        DayOfWeek.Sunday)
                )
            }
        );
}