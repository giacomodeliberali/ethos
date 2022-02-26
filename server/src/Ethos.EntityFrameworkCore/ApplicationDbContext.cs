using System;
using Ethos.Domain.Entities;
using Ethos.EntityFrameworkCore.Configurations;
using Ethos.EntityFrameworkCore.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ethos.EntityFrameworkCore
{
    /// <summary>
    /// The application context with the default Identity tables.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public DbSet<ScheduleData> Schedules { get; set; } = null!;

        public DbSet<SingleScheduleData> SingleSchedules { get; set; } = null!;

        public DbSet<RecurringScheduleData> RecurringSchedules { get; set; } = null!;

        public DbSet<ScheduleExceptionData> ScheduleExceptions { get; set; } = null!;

        public DbSet<BookingData> Bookings { get; set; } = null!;

        /// <inheritdoc />
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Identity
            builder.ApplyConfiguration(new ApplicationUserConfiguration());
            builder.ApplyConfiguration(new ApplicationRoleConfiguration());

            // Custom tables
            builder.ApplyConfiguration(new BookingDataConfiguration());

            builder.ApplyConfiguration(new ScheduleDataConfiguration());
            builder.ApplyConfiguration(new SingleScheduleDataConfiguration());
            builder.ApplyConfiguration(new RecurringScheduleDataConfiguration());

            builder.ApplyConfiguration(new ScheduleExceptionDataConfiguration());
        }
    }
}
