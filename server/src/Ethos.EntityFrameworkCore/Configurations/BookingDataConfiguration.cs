using System;
using Ethos.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ethos.EntityFrameworkCore.Configurations
{
    public class BookingDataConfiguration : IEntityTypeConfiguration<BookingData>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<BookingData> builder)
        {
            builder.ToTable("Bookings", schema: "Bookings");
            builder.Property(u => u.Id).ValueGeneratedOnAdd();
            builder.Property(s => s.ScheduleId).IsRequired();
            builder.Property(s => s.UserId).IsRequired();

            builder
                .Property(s => s.StartDate)
                .IsRequired();

            builder
                .Property(s => s.EndDate)
                .IsRequired();
        }
    }
}
