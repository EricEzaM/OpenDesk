using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenDesk.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDesk.Infrastructure.Persistence.Configurations
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
