using System;
using Ethos.Domain.Common;

namespace Ethos.Domain.Entities
{
    public class Booking : Entity
    {
        public ApplicationUser User { get; private set; }

        public DateTime StartDate { get; private set; }

        public DateTime EndDate { get; private set; }

        public Schedule Schedule { get; private set; }

        private Booking()
        {
        }

        public static class Factory
        {
            public static Booking Create(
                Guid guid,
                Schedule schedule,
                ApplicationUser user,
                DateTime startDate,
                DateTime endDate)
            {
                return new Booking()
                {
                    Id = guid,
                    User = user,
                    StartDate = startDate,
                    EndDate = endDate,
                    Schedule = schedule,
                };
            }

            public static Booking CreateFromSnapshot(
                Guid id,
                Schedule schedule,
                ApplicationUser user,
                DateTime startDate,
                DateTime endDate)
            {
                return new Booking()
                {
                    Id = id,
                    User = user,
                    StartDate = startDate,
                    EndDate = endDate,
                    Schedule = schedule,
                };
            }
        }
    }
}
