using System;
using Ethos.Domain.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ethos.EntityFrameworkCore
{
    /// <summary>
    /// The application context with the default Identity tables.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        /// <inheritdoc />
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(b =>
            {
                var propertyBuilder = b.Property(u => u.Id);

                if (Database.IsSqlServer())
                {
                    propertyBuilder.HasDefaultValueSql("newsequentialid()");
                }

                if (Database.IsSqlite())
                {
                    propertyBuilder.HasDefaultValue(Guid.NewGuid());
                }
            });

            builder.Entity<ApplicationRole>(b =>
            {
                var propertyBuilder = b.Property(u => u.Id);

                if (Database.IsSqlServer())
                {
                    propertyBuilder.HasDefaultValueSql("newsequentialid()");
                }

                if (Database.IsSqlite())
                {
                    propertyBuilder.HasDefaultValue(Guid.NewGuid());
                }
            });
        }
    }
}
