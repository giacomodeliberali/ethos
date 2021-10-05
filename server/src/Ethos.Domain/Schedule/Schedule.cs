using System;
using Ardalis.GuardClauses;
using Cronos;
using Ethos.Domain.Identity;

namespace Ethos.Domain.Schedule
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
        public TimeSpan Duration { get; private set; }

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

        private Schedule()
        {
        }

        public static class Factory
        {
            public static Schedule CreateRecurring(
                ApplicationUser organizer,
                string name,
                string description,
                DateTime startDate,
                DateTime? endDate,
                TimeSpan duration,
                string recurringExpression)
            {
                Guard.Against.Null(organizer, nameof(organizer));
                Guard.Against.NullOrEmpty(name, nameof(name));
                Guard.Against.NullOrEmpty(description, nameof(description));

                return new Schedule()
                {
                    Organizer = organizer,
                    Name = name,
                    Description = description,
                    StartDate = startDate,
                    EndDate = endDate,
                    Duration = duration,
                    RecurringCronExpressionString = recurringExpression,
                };
            }

            public static Schedule CreateNonRecurring(
                ApplicationUser organizer,
                string name,
                string description,
                DateTime startDate,
                DateTime endDate)
            {
                Guard.Against.Null(organizer, nameof(organizer));
                Guard.Against.NullOrEmpty(name, nameof(name));
                Guard.Against.NullOrEmpty(description, nameof(description));

                return new Schedule()
                {
                    Organizer = organizer,
                    Name = name,
                    Description = description,
                    StartDate = startDate,
                    EndDate = endDate,
                    Duration = endDate - startDate,
                };
            }

            public static Schedule FromSnapshot(
                ApplicationUser organizer,
                DateTime startDate,
                DateTime? endDate,
                string recurringExpression,
                TimeSpan duration,
                string name,
                string description)
            {
                return new Schedule()
                {
                    Organizer = organizer,
                    StartDate = startDate,
                    EndDate = endDate,
                    RecurringCronExpressionString = recurringExpression,
                    Duration = duration,
                    Name = name,
                    Description = description,
                };
            }
        }
    }
}
