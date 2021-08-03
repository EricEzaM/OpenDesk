using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenDesk.Application.Entities;

namespace OpenDesk.Application.Persistence.Configurations
{
	public class OfficeConfiguration : IEntityTypeConfiguration<Office>
	{
		public void Configure(EntityTypeBuilder<Office> builder)
		{
			builder.Property(o => o.Id)
				.ValueGeneratedOnAdd();

			builder.HasKey(x => x.Id);

			builder.Property(p => p.Name)
				.IsRequired();
		}
	}
}
