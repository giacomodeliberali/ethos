using System;
using Ardalis.GuardClauses;
using Ethos.Domain.Guards;

namespace Ethos.Domain.Common
{
    public class Period
    {
        public DateTime StartDate { get; }

        public DateTime EndDate { get; }

        public int DurationInMinutes => (int)(EndDate - StartDate).TotalMinutes;

        public Period(DateTime startDate, DateTime endDate)
        {
            Guard.Against.NotUtc(startDate, nameof(startDate));
            Guard.Against.NotUtc(endDate, nameof(endDate));

            Guard.Against.Default(startDate, nameof(startDate));
            Guard.Against.Default(endDate, nameof(endDate));

            if (endDate < startDate)
            {
                throw new ArgumentException("End date is before start date!");
            }

            StartDate = startDate;
            EndDate = endDate;
        }

        public Period(DateTime startDate, int durationInMinutes)
            : this(startDate, startDate.AddMinutes(durationInMinutes))
        {
        }

        public bool Overlaps(Period period)
        {
            return StartDate < period.EndDate && period.StartDate < EndDate;
        }
    }
}
