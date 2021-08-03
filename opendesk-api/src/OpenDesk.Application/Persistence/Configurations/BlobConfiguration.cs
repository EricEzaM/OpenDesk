using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenDesk.Application.Entities;

namespace OpenDesk.Application.Persistence.Configurations
{
	class BlobConfiguration : IEntityTypeConfiguration<Blob>
	{
		public void Configure(EntityTypeBuilder<Blob> builder)
		{
			builder.Property(p => p.Id)
				.ValueGeneratedOnAdd();

			builder.HasKey(p => p.Id);

			builder.Property(p => p.Uri)
				.IsRequired();

			builder.Property(p => p.Expiry)
				.IsRequired();
		}
	}
}
