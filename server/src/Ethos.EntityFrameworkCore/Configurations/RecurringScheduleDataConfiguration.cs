using Ethos.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ethos.EntityFrameworkCore.Configurations
{
    public class RecurringScheduleDataConfiguration : IEntityTypeConfiguration<RecurringScheduleData>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<RecurringScheduleData> builder)
        {
            builder.ToTable("Recurring", schema: "Schedules");
            builder.HasKey(s => s.ScheduleId);
            builder.Property(s => s.StartDate).IsRequired();
            builder.Property(s => s.EndDate);
            builder.Property(s => s.RecurringExpression).HasMaxLength(32).IsRequired();
        }
    }
}
