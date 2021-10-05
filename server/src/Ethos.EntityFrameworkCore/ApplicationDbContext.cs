using System;
using Ethos.Domain.Entities;
using Ethos.EntityFrameworkCore.Booking;
using Ethos.EntityFrameworkCore.Booking.Configuration;
using Ethos.EntityFrameworkCore.Identity;
using Ethos.EntityFrameworkCore.Schedule;
using Ethos.EntityFrameworkCore.Schedule.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ethos.EntityFrameworkCore
{
    /// <summary>
    /// The application context with the default Identity tables.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public DbSet<ScheduleData> Schedules { get; set; }

        public DbSet<BookingData> Bookings { get; set; }

        /// <inheritdoc />
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new ApplicationUserConfiguration(this));
            builder.ApplyConfiguration(new ApplicationRoleConfiguration(this));

            builder.ApplyConfiguration(new BookingDataConfiguration(this));
            builder.ApplyConfiguration(new ScheduleDataConfiguration(this));
            // builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
