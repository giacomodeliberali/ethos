using System;
using Ethos.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ethos.EntityFrameworkCore.Configurations
{
    /// <summary>
    /// Entity config for ApplicationRole.
    /// </summary>
    public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public ApplicationRoleConfiguration(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            var propertyBuilder = builder.Property(u => u.Id);

            if (_context.Database.IsSqlite())
            {
                propertyBuilder.HasDefaultValue(Guid.NewGuid());
            }
            else
            {
                propertyBuilder.HasDefaultValueSql("newsequentialid()");
            }
        }
    }
}
