using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenDesk.Application.Entities;

namespace OpenDesk.Application.Persistence.Configurations
{
	public class DeskConfiguration : EntityBaseConfiguration<Desk>
	{
		public override void Configure(EntityTypeBuilder<Desk> builder)
		{
			base.Configure(builder);

			builder.Property(p => p.Name)
				.HasMaxLength(100)
				.IsRequired();

			builder
				.OwnsOne(b => b.DiagramPosition);
		}
	}
}
