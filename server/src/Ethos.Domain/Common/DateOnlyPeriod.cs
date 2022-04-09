using System;
using Ardalis.GuardClauses;

namespace Ethos.Domain.Common
{
    public class DateOnlyPeriod
    {
        public DateOnly StartDate { get; }

        public DateOnly EndDate { get; }

        public DateOnlyPeriod(DateOnly startDate, DateOnly endDate)
        {
            Guard.Against.Default(startDate, nameof(startDate));
            Guard.Against.Default(endDate, nameof(endDate));

            if (endDate < startDate)
            {
                throw new ArgumentException("End date is before start date!");
            }

            StartDate = startDate;
            EndDate = endDate;
        }

        public DateOnlyPeriod(DateTime startDate, DateTime endDate)
        : this(DateOnly.FromDateTime(startDate), DateOnly.FromDateTime(endDate))
        {
        }
        
        public DateOnlyPeriod(DateTime startDate, DateOnly endDate)
            : this(DateOnly.FromDateTime(startDate), endDate)
        {
        }
        
        public DateOnlyPeriod(DateOnly startDate, DateTime endDate)
            : this(startDate, DateOnly.FromDateTime(endDate))
        {
        }
        
        public DateOnlyPeriod(DateTimeOffset startDate, DateTimeOffset endDate)
            : this(startDate.DateTime, endDate.DateTime)
        {
        }

        public DateOnlyPeriod(DateTimeOffset startDate, DateOnly endDate)
        :this(startDate.Date, endDate)
        {
        }

        public DateOnlyPeriod(DateOnly startDate, DateTimeOffset endDate)
        : this(startDate, endDate.DateTime)
        {
        }
    }
}
