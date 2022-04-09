using Ethos.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ethos.EntityFrameworkCore.Configurations
{
    public class ScheduleDataConfiguration : IEntityTypeConfiguration<ScheduleData>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<ScheduleData> builder)
        {
            builder.ToTable("Schedules", schema: "Schedules");
            builder.Property(u => u.Id).ValueGeneratedOnAdd();
            builder.Property(s => s.OrganizerId).IsRequired();
            builder.Property(s => s.Name).HasMaxLength(255).IsRequired();
            builder.Property(s => s.Description).HasMaxLength(2048).IsRequired();
            builder.Property(s => s.DurationInMinutes).IsRequired();
            builder.Property(s => s.ParticipantsMaxNumber).IsRequired();
            builder.Property(s => s.TimeZone).IsRequired();
        }
    }
}
