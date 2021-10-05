using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ethos.EntityFrameworkCore.Schedule.Configuration
{
    public class ScheduleDataConfiguration : IEntityTypeConfiguration<ScheduleData>
    {
        private readonly ApplicationDbContext _context;

        public ScheduleDataConfiguration(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<ScheduleData> builder)
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

            builder.Property(s => s.OrganizerId).IsRequired();
            builder.Property(s => s.StartDate).IsRequired();
            builder.Property(s => s.RecurringExpression).HasMaxLength(32);
            builder.Property(s => s.Name).HasMaxLength(255).IsRequired();
            builder.Property(s => s.Description).HasMaxLength(2048).IsRequired();
            builder.Property(s => s.Duration).IsRequired();
        }
    }
}
