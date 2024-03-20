using GeoTimeZone;

namespace TransactionsAPI.Services;

public static class TimeZoneService
{
    public static TimeZoneInfo ConvertToTimeZoneInfo(string clientLocation)
    {
        var coordinates = clientLocation.Split(",");
        var latitude = double.Parse(coordinates[0]);
        var longitude = double.Parse(coordinates[1]);

        var timeZoneId = TimeZoneLookup.GetTimeZone(latitude, longitude).Result;
    
        // Some issue with region Kyiv.
        // Details: https://github.com/dotnet/runtime/issues/83188
        if (timeZoneId is "Europe/Kyiv")
            timeZoneId = "Europe/Kiev";

        if (timeZoneId is "Antarctica/Troll")
            return timeZoneId.CreateTrollTimeZone();

        return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
    }
    
    private static TimeZoneInfo CreateTrollTimeZone(this string timeZoneId) =>
        TimeZoneInfo.CreateCustomTimeZone(
            id: timeZoneId,
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