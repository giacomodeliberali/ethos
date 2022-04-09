using System;

namespace Ethos.Domain.UnitTest;

public static class DateTimeExtensions
{
    public static DateTimeOffset ToTimeZone(this DateTimeOffset dateTimeOffset, TimeZoneInfo destinationTimeZone)
    {
        return TimeZoneInfo.ConvertTime(dateTimeOffset, destinationTimeZone);
    }
}