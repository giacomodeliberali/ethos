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

            // Identity
            builder.ApplyConfiguration(new ApplicationUserConfiguration());
            builder.ApplyConfiguration(new ApplicationRoleConfiguration());

            // Custom tables
            builder.ApplyConfiguration(new BookingDataConfiguration());
            builder.ApplyConfiguration(new ScheduleDataConfiguration());
        }
    }
}
