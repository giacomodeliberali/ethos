using System;
using Ethos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ethos.EntityFrameworkCore.Identity
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUserConfiguration(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            var propertyBuilder = builder
                .Property(u => u.Id);

            if (_context.Database.IsSqlite())
            {
                propertyBuilder.HasDefaultValue(Guid.NewGuid());
            }
            else
            {
                propertyBuilder.HasDefaultValueSql("newsequentialid()");
            }

            builder.Property(u => u.UserName)
                .IsRequired();
        }
    }
}
