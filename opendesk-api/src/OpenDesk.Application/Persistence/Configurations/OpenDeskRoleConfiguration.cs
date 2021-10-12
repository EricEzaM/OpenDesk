using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenDesk.Application.Identity;

namespace OpenDesk.Application.Persistence.Configurations
{
	class OpenDeskRoleConfiguration : IEntityTypeConfiguration<OpenDeskRole>
	{
		public void Configure(EntityTypeBuilder<OpenDeskRole> builder)
		{
			builder.Property(r => r.Id)
				.HasMaxLength(50)
				.ValueGeneratedOnAdd();

			builder.Property(r => r.Name)
				.HasMaxLength(50)
				.IsRequired();

			builder.Property(r => r.NormalizedName)
				.HasMaxLength(50)
				.IsRequired();

			builder.Property(r => r.Description)
				.HasMaxLength(256);
		}
	}
}
