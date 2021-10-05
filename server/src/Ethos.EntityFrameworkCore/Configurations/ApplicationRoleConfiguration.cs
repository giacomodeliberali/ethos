using Ethos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ethos.EntityFrameworkCore.Configurations
{
    /// <summary>
    /// Entity config for ApplicationRole.
    /// </summary>
    public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.Property(u => u.Id).ValueGeneratedOnAdd();
        }
    }
}
