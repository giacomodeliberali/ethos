using System;

namespace Ethos.Domain.UnitTest;

public static class DateTimeExtensions
{
    public static DateTimeOffset ToDateTimeOffset(this DateTime dateTime, TimeZoneInfo destinationTimeZone)
    {
        return new DateTimeOffset(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, destinationTimeZone.GetUtcOffset(dateTime));
    }
}