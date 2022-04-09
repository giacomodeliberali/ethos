using System;

namespace Ethos.Domain.Common;

public static class TimeZones
{
    public static TimeZoneInfo Amsterdam => TimeZoneInfo.FindSystemTimeZoneById("Europe/Amsterdam");
    public static TimeZoneInfo LosAngeles => TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles");
}