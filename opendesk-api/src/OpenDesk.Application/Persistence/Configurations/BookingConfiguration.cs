using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenDesk.Application.Entities;

namespace OpenDesk.Application.Persistence.Configurations
{
	public class BookingConfiguration : IEntityTypeConfiguration<Booking>
	{
		public void Configure(EntityTypeBuilder<Booking> builder)
		{
			builder.Property(p => p.Id)
				.ValueGeneratedOnAdd();

			builder.HasKey(p => p.Id);

			builder.Property(p => p.StartDateTime)
				.IsRequired();

			builder.Property(p => p.EndDateTime)
				.IsRequired();

			builder.Property(p => p.UserId)
				.IsRequired();
		}
	}
}
