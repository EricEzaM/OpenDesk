using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenDesk.Application.Entities;

namespace OpenDesk.Application.Persistence.Configurations
{
	class BlobConfiguration : EntityBaseConfiguration<Blob>
	{
		public override void Configure(EntityTypeBuilder<Blob> builder)
		{
			base.Configure(builder);

			builder.Property(p => p.Uri)
				.IsRequired();

			builder.Property(p => p.Expiry)
				.IsRequired();
		}
	}
}
