using System;
using Ethos.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ethos.EntityFrameworkCore.Configurations
{
    public class SingleScheduleDataConfiguration : IEntityTypeConfiguration<SingleScheduleData>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<SingleScheduleData> builder)
        {
            builder.ToTable("Singles", schema: "Schedules");
            builder.HasKey(s => s.ScheduleId);

            builder
                .Property(s => s.StartDate)
                .IsRequired();

            builder
                .Property(s => s.EndDate)
                .IsRequired();
        }
    }
}
