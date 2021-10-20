using System;
using Ardalis.GuardClauses;
using JetBrains.Annotations;

namespace Ethos.Domain.Guards
{
    public static class DateTimeGuardExtensions
    {
        public static DateTime NotUtc(
            [NotNull] this IGuardClause guardClause,
            DateTime input,
            [NotNull] string parameterName,
            string message = null)
        {
            if (input.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException($"DateTime {parameterName} is not in UTC!");
            }

            return input;
        }

        public static DateTime? NotUtc(
            [NotNull] this IGuardClause guardClause,
            DateTime? input,
            [NotNull] string parameterName,
            string message = null)
        {
            if (input.HasValue && input.Value.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException($"DateTime {parameterName} is not in UTC!");
            }

            return input;
        }
    }
}
