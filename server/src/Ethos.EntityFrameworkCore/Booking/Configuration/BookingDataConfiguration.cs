using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ethos.EntityFrameworkCore.Booking.Configuration
{
    public class BookingDataConfiguration : IEntityTypeConfiguration<BookingData>
    {
        private readonly ApplicationDbContext _context;

        public BookingDataConfiguration(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<BookingData> builder)
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

            builder.Property(s => s.ScheduleId).IsRequired();
            builder.Property(s => s.UserId).IsRequired();
            builder.Property(s => s.StartDate).IsRequired();
            builder.Property(s => s.EndDate).IsRequired();
        }
    }
}
