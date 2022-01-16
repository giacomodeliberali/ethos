using System;
using Ethos.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ethos.EntityFrameworkCore.Configurations
{
    public class ScheduleExceptionDataConfiguration : IEntityTypeConfiguration<ScheduleExceptionData>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<ScheduleExceptionData> builder)
        {
            builder.ToTable("Exceptions", schema: "Schedules");
            builder.Property(u => u.Id).ValueGeneratedOnAdd();
            builder.Property(s => s.ScheduleId).IsRequired();

            builder
                .Property(s => s.StartDate)
                .HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
                .IsRequired();

            builder
                .Property(s => s.EndDate)
                .HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
                .IsRequired();
        }
    }
}
