using System;
using Ardalis.GuardClauses;
using Cronos;
using Ethos.Domain.Common;
using Ethos.Domain.Exceptions;

namespace Ethos.Domain.Entities
{
    public class Schedule : Entity
    {
        /// <summary>
        /// The admin user that manages this schedule.
        /// </summary>
        public ApplicationUser Organizer { get; private set; }

        /// <summary>
        /// The schedule friendly name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The schedule extended description.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// The first date where this schedule applies.
        /// </summary>
        public DateTime StartDate { get; private set; }

        /// <summary>
        /// The end date of the schedule. If no value is specified the schedule lasts forever.
        /// </summary>
        public DateTime? EndDate { get; private set; }

        /// <summary>
        /// Indicates if this schedule is recurring or not.
        /// </summary>
        public bool IsRecurring => RecurringCronExpression != null;

        /// <summary>
        /// The duration of the schedule when it is defined as recurring (or calculated if single).
        /// </summary>
        public int DurationInMinutes { get; private set; }

        /// <summary>
        /// The underlying CRON expression expressed as a string.
        /// See https://github.com/HangfireIO/Cronos/issues/15.
        /// </summary>
        public string RecurringCronExpressionString { get; private set; }

        /// <summary>
        /// The CRON expression. Null if this schedule is not recurring.
        /// <remarks>Do not call the toString() (see https://github.com/HangfireIO/Cronos/issues/15)</remarks>
        /// </summary>
        public CronExpression RecurringCronExpression
        {
            get
            {
                if (string.IsNullOrEmpty(RecurringCronExpressionString))
                {
                    return null;
                }

                return CronExpression.Parse(RecurringCronExpressionString);
            }
        }

        /// <summary>
        /// The max number of people that can participate to each schedule.
        /// </summary>
        public int ParticipantsMaxNumber { get; private set; }

        private Schedule()
        {
        }

        public Schedule UpdateNameAndDescription(string name, string description, int participantsMaxNumber)
        {
            Guard.Against.NullOrEmpty(name, nameof(name));
            Guard.Against.NullOrEmpty(description, nameof(description));
            Name = name;
            Description = description;
            ParticipantsMaxNumber = participantsMaxNumber;
            return this;
        }

        public Schedule UpdateDateTime(DateTime startDate, DateTime? endDate, int? durationInMinutes, string recurringExpression)
        {
            if (!string.IsNullOrEmpty(recurringExpression))
            {
                // recurring
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
                DurationInMinutes = durationInMinutes.Value;
                RecurringCronExpressionString = recurringExpression;
            }
            else
            {
                // non recurring
                Guard.Against.Null(endDate, nameof(endDate));

                StartDate = startDate;
                EndDate = endDate;
                DurationInMinutes = (int)(endDate.Value - startDate).TotalMinutes;
                RecurringCronExpressionString = null;
            }

            return this;
        }

        public Schedule UpdateOrganizer(ApplicationUser organizer)
        {
            Guard.Against.Null(organizer, nameof(organizer));
            Organizer = organizer;
            return this;
        }

        public static class Factory
        {
            public static Schedule CreateRecurring(
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

                return new Schedule()
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

            public static Schedule CreateNonRecurring(
                Guid guid,
                ApplicationUser organizer,
                string name,
                string description,
                int participantsMaxNumber,
                DateTime startDate,
                DateTime endDate)
            {
                Guard.Against.Null(organizer, nameof(organizer));
                Guard.Against.NullOrEmpty(name, nameof(name));
                Guard.Against.NullOrEmpty(description, nameof(description));

                return new Schedule()
                {
                    Id = guid,
                    Organizer = organizer,
                    Name = name,
                    Description = description,
                    ParticipantsMaxNumber = participantsMaxNumber,
                    StartDate = startDate,
                    EndDate = endDate,
                    DurationInMinutes = (int)(endDate - startDate).TotalMinutes,
                };
            }

            public static Schedule FromSnapshot(
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
                return new Schedule()
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
