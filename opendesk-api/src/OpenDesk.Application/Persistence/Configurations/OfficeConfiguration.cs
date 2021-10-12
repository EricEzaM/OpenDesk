using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenDesk.Application.Entities;

namespace OpenDesk.Application.Persistence.Configurations
{
	public class OfficeConfiguration : EntityBaseConfiguration<Office>
	{
		public override void Configure(EntityTypeBuilder<Office> builder)
		{
			base.Configure(builder);

			builder.Property(o => o.Name)
				.HasMaxLength(100)
				.IsRequired();

			builder.Property(o => o.Location)
				.HasMaxLength(100)
				.IsRequired();

			builder.Property(o => o.SubLocation)
				.HasMaxLength(100)
				.IsRequired();
		}
	}
}
