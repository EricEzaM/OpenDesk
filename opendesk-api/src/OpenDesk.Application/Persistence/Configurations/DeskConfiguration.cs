using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenDesk.Application.Entities;

namespace OpenDesk.Application.Persistence.Configurations
{
	public class DeskConfiguration : IEntityTypeConfiguration<Desk>
	{
		public void Configure(EntityTypeBuilder<Desk> builder)
		{
			builder.Property(d => d.Id)
				.ValueGeneratedOnAdd();

			builder.HasKey(x => x.Id);

			builder.Property(p => p.Name)
				.HasMaxLength(100)
				.IsRequired();

			builder
				.OwnsOne(b => b.DiagramPosition);
		}
	}
}
