using System;
using System.Runtime.CompilerServices;
using Ardalis.GuardClauses;

namespace Ethos.Domain.Extensions;

public static class GuardsExtensions
{
    public static DateTimeOffset DifferentTimezone(
        this IGuardClause guardClause, 
        DateTimeOffset dateTime, 
        TimeZoneInfo timeZone,
        [CallerArgumentExpression("dateTime")] string? parameterName = null)
    {

        // if (dateTime.Kind != DateTimeKind.Unspecified)
        // {
        //     throw new ArgumentException($"{parameterName} must not contain timezone kind");
        // }

        // if (dateTime.Offset != timeZone.BaseUtcOffset)
        // {
        //     throw new ArgumentException($"{parameterName} must use the same offset as the specified timezone {timeZone.Id}. Found {dateTime.Offset} instead of {timeZone.BaseUtcOffset}");
        // }

        return dateTime;
    }
}