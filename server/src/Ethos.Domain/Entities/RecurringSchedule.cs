using System;
using Ardalis.GuardClauses;
using Cronos;
using Ethos.Domain.Common;
using Ethos.Domain.Exceptions;

namespace Ethos.Domain.Entities
{
    public class RecurringSchedule : Schedule
    {
        /// <summary>
        /// The first date where this schedule applies.
        /// </summary>
        public DateTime StartDate { get; private set; }

        /// <summary>
        /// The end date of the schedule. If no value is specified the schedule lasts forever.
        /// </summary>
        public DateTime? EndDate { get; private set; }

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

        public void UpdateDateTime(DateTime startDate, DateTime? endDate, int durationInMinutes, string recurringExpression)
        {
            Guard.Against.NullOrEmpty(recurringExpression, nameof(recurringExpression));
            Guard.Against.NegativeOrZero(durationInMinutes, nameof(durationInMinutes));

            try
            {
                CronExpression.Parse(recurringExpression);
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Invalid CRON expression '{recurringExpression}'", ex);
            }

            Guard.Against.Null(durationInMinutes, nameof(durationInMinutes));

            StartDate = startDate;
            EndDate = endDate;
            DurationInMinutes = durationInMinutes;
            RecurringCronExpressionString = recurringExpression;
        }

        public static class Factory
        {
            public static RecurringSchedule Create(
                Guid guid,
                ApplicationUser organizer,
                string name,
                string description,
                int participantsMaxNumber,
                DateTime startDate,
                DateTime? endDate,
                int duration,
                string recurringExpression)
            {
                Guard.Against.Null(organizer, nameof(organizer));
                Guard.Against.NullOrEmpty(name, nameof(name));
                Guard.Against.NullOrEmpty(description, nameof(description));
                Guard.Against.NegativeOrZero(duration, nameof(duration));

                return new RecurringSchedule()
                {
                    Id = guid,
                    Organizer = organizer,
                    Name = name,
                    Description = description,
                    ParticipantsMaxNumber = participantsMaxNumber,
                    StartDate = startDate,
                    EndDate = endDate,
                    DurationInMinutes = duration,
                    RecurringCronExpressionString = recurringExpression,
                };
            }

            public static RecurringSchedule FromSnapshot(
                Guid id,
                ApplicationUser organizer,
                DateTime startDate,
                DateTime? endDate,
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
                    StartDate = startDate,
                    EndDate = endDate,
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
