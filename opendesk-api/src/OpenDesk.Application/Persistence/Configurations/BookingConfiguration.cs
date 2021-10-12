using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenDesk.Application.Entities;

namespace OpenDesk.Application.Persistence.Configurations
{
	public class BookingConfiguration : EntityBaseConfiguration<Booking>
	{
		public override void Configure(EntityTypeBuilder<Booking> builder)
		{
			base.Configure(builder);

			builder.Property(p => p.StartDateTime)
				.IsRequired();

			builder.Property(p => p.EndDateTime)
				.IsRequired();

			builder.Property(p => p.UserId)
				.HasMaxLength(50)
				.IsRequired();
		}
	}
}
