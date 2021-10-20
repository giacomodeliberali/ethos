using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;
using Cronos;
using Ethos.Domain.Common;
using Ethos.Domain.Exceptions;
using Ethos.Domain.Guards;

namespace Ethos.Domain.Entities
{
    public class RecurringSchedule : Schedule
    {
        /// <summary>
        /// The period of this schedule.
        /// </summary>
        public Period Period { get; private set; }

        /// <summary>
        /// The underlying CRON expression expressed as a string.
        /// See https://github.com/HangfireIO/Cronos/issues/15.
        /// </summary>
        public string RecurringCronExpressionString { get; private set; }

        /// <summary>
        /// The recurring CRON expression.
        /// <remarks>Do not call the toString() (see https://github.com/HangfireIO/Cronos/issues/15)</remarks>
        /// </summary>
        public CronExpression RecurringCronExpression => CronExpression.Parse(RecurringCronExpressionString);

        private RecurringSchedule()
        {
        }

        public void UpdateDate(Period period, int durationInMinutes, string recurringExpression)
        {
            Guard.Against.NullOrEmpty(recurringExpression, nameof(recurringExpression));
            Guard.Against.NegativeOrZero(durationInMinutes, nameof(durationInMinutes));
            Guard.Against.Null(period, nameof(period));

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
        }

        public IEnumerable<Period> GetOccurrences(Period period)
        {
            return RecurringCronExpression
                .GetOccurrences(period.StartDate, period.EndDate, fromInclusive: true, toInclusive: true)
                .Select(nextStartDate => new Period(nextStartDate, nextStartDate.AddMinutes(DurationInMinutes)));
        }

        public IEnumerable<Period> GetOccurrences()
        {
            return GetOccurrences(Period);
        }

        public static class Factory
        {
            public static RecurringSchedule Create(
                Guid guid,
                ApplicationUser organizer,
                string name,
                string description,
                int participantsMaxNumber,
                Period period,
                int duration,
                string recurringExpression)
            {
                Guard.Against.Null(organizer, nameof(organizer));
                Guard.Against.NullOrEmpty(name, nameof(name));
                Guard.Against.NullOrEmpty(description, nameof(description));
                Guard.Against.NegativeOrZero(duration, nameof(duration));
                Guard.Against.Null(period, nameof(period));

                return new RecurringSchedule()
                {
                    Id = guid,
                    Organizer = organizer,
                    Name = name,
                    Description = description,
                    ParticipantsMaxNumber = participantsMaxNumber,
                    // do not consider time
                    Period = new Period(period.StartDate.Date, period.EndDate.Date.AddDays(1).AddTicks(-1)),
                    DurationInMinutes = duration,
                    RecurringCronExpressionString = recurringExpression,
                };
            }

            public static RecurringSchedule FromSnapshot(
                Guid id,
                ApplicationUser organizer,
                DateTime startDate,
                DateTime endDate,
                string recurringExpression,
                int duration,
                string name,
                string description,
                int participantsMaxNumber)
            {
                return new RecurringSchedule()
                {
                    Id = id,
                    Organizer = organizer,
                    Period = new Period(startDate, endDate),
                    RecurringCronExpressionString = recurringExpression,
                    DurationInMinutes = duration,
                    Name = name,
                    Description = description,
                    ParticipantsMaxNumber = participantsMaxNumber,
                };
            }
        }
    }
}
