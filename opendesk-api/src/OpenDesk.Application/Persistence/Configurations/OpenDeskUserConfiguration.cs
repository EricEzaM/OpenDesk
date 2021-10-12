using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenDesk.Application.Identity;

namespace OpenDesk.Application.Persistence.Configurations
{
	class OpenDeskUserConfiguration : IEntityTypeConfiguration<OpenDeskUser>
	{
		public void Configure(EntityTypeBuilder<OpenDeskUser> builder)
		{
			builder.Property(u => u.Id)
				.HasMaxLength(50)
				.ValueGeneratedOnAdd();

			builder.Property(u => u.UserName)
				.HasMaxLength(50)
				.IsRequired();

			builder.Property(r => r.NormalizedUserName)
				.HasMaxLength(50)
				.IsRequired();

			builder.Property(r => r.DisplayName)
				.HasMaxLength(50)
				.IsRequired();

			builder.Property(r => r.PhoneNumber)
				.HasMaxLength(50);
		}
	}
}
