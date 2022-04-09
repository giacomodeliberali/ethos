using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Ardalis.GuardClauses;
using Cronos;
using Ethos.Domain.Common;
using Ethos.Domain.Exceptions;

namespace Ethos.Domain.Entities
{
    public class RecurringSchedule : Schedule
    {
        /// <summary>
        /// The period of this schedule.
        /// </summary>
        public DateOnlyPeriod Period { get; private set; }

        /// <summary>
        /// The underlying CRON expression expressed as a string.
        /// See https://github.com/HangfireIO/Cronos/issues/15.
        /// </summary>
        public string RecurringCronExpressionString { get; private set; }

        /// <summary>
        /// The recurring CRON expression.
        /// <remarks>Do not call the toString() (see https://github.com/HangfireIO/Cronos/issues/15)</remarks>
        /// </summary>
        private CronExpression RecurringCronExpression => CronExpression.Parse(RecurringCronExpressionString);

        private RecurringSchedule(
            Guid id,
            ApplicationUser organizer,
            DateOnlyPeriod period,
            string recurringExpression,
            int duration,
            string name,
            string description,
            int participantsMaxNumber,
            TimeZoneInfo timeZone)
            : base(id, organizer, name, description, participantsMaxNumber, duration, timeZone)
        {
            Period = period;
            RecurringCronExpressionString = recurringExpression;
        }

        public void UpdateDate(DateOnlyPeriod period, int durationInMinutes, string recurringExpression, TimeZoneInfo timeZone)
        {
            Guard.Against.NullOrEmpty(recurringExpression, nameof(recurringExpression));
            Guard.Against.NegativeOrZero(durationInMinutes, nameof(durationInMinutes));
            Guard.Against.Null(period, nameof(period));
            Guard.Against.Null(timeZone, nameof(timeZone));

            try
            {
                CronExpression.Parse(recurringExpression);
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Invalid CRON expression '{recurringExpression}'", ex);
            }
            
            Period = period; 
            DurationInMinutes = durationInMinutes;
            RecurringCronExpressionString = recurringExpression;
            TimeZone = timeZone;
        }

        private static DateOnly Max(DateOnly first, DateOnly second)
        {
            if (first >= second)
            {
                return first;
            }

            return second;
        }
        
        private static DateOnly Min(DateOnly first, DateOnly second)
        {
            if (first <= second)
            {
                return first;
            }

            return second;
        }

        public IEnumerable<(DateTimeOffset StartDate, DateTimeOffset EndDate)> GetOccurrences(DateOnlyPeriod requestedPeriod, TimeZoneInfo timeZone)
        {
            if (requestedPeriod.EndDate < Period.StartDate || requestedPeriod.StartDate > Period.EndDate)
            {
                return Enumerable.Empty<(DateTimeOffset StartDate, DateTimeOffset EndDate)>();
            }
            
            DateOnlyPeriod safePeriod = new DateOnlyPeriod(
                Max(Period.StartDate, requestedPeriod.StartDate),
                Min(Period.EndDate, requestedPeriod.EndDate));

            var from = new DateTimeOffset(safePeriod.StartDate.Year, safePeriod.StartDate.Month, safePeriod.StartDate.Day, 0, 0, 0, timeZone.BaseUtcOffset);
            var to = new DateTimeOffset(safePeriod.EndDate.Year, safePeriod.EndDate.Month, safePeriod.EndDate.Day, 23, 59, 59, timeZone.BaseUtcOffset);

            return RecurringCronExpression
                .GetOccurrences(
                    from,
                    to, 
                    timeZone,
                    fromInclusive: true,
                    toInclusive: true)
                .Select(nextStartDate =>
                {
                    return (nextStartDate, nextStartDate.AddMinutes(DurationInMinutes));
                });
        }

        public (DateTimeOffset StartDate, DateTimeOffset EndDate) GetFirstOccurrence(TimeZoneInfo timeZone)
        {
            var occ = GetOccurrences(Period, timeZone);
            return occ.Select(s => (s.StartDate.DateTime, s.EndDate.DateTime)).First();
        }

        public static class Factory
        {
            public static RecurringSchedule Create(
                Guid id,
                ApplicationUser organizer,
                string name,
                string description,
                int participantsMaxNumber,
                DateOnlyPeriod period,
                int duration,
                string recurringExpression,
                TimeZoneInfo timeZone)
            {
                Guard.Against.Null(organizer, nameof(organizer));
                Guard.Against.NullOrEmpty(name, nameof(name));
                Guard.Against.NullOrEmpty(description, nameof(description));
                Guard.Against.NullOrEmpty(recurringExpression, nameof(recurringExpression));
                Guard.Against.NegativeOrZero(duration, nameof(duration));
                Guard.Against.Null(period, nameof(period));
                Guard.Against.Null(timeZone, nameof(timeZone));

                try
                {
                    CronExpression.Parse(recurringExpression);
                }
                catch (Exception ex)
                {
                    throw new BusinessException($"Invalid CRON expression '{recurringExpression}'. {ex.Message}", ex);
                }

                return new RecurringSchedule(
                    id,
                    organizer,
                    period,
                    recurringExpression,
                    duration,
                    name,
                    description,
                    participantsMaxNumber,
                    timeZone);
            }

            public static RecurringSchedule FromSnapshot(
                Guid id,
                ApplicationUser organizer,
                DateOnly startDate,
                DateOnly endDate,
                string recurringExpression,
                int duration,
                string name,
                string description,
                int participantsMaxNumber,
                TimeZoneInfo timeZone)
            {
                return new RecurringSchedule(
                    id,
                    organizer,
                    new DateOnlyPeriod(startDate, endDate),
                    recurringExpression,
                    duration,
                    name,
                    description,
                    participantsMaxNumber,
                    timeZone);
            }
        }
    }
}
