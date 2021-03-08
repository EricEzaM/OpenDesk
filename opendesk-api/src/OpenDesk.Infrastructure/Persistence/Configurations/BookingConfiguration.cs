using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenDesk.Domain.Entities;
using OpenDesk.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDesk.Infrastructure.Persistence.Configurations
{
	public class BookingConfiguration : IEntityTypeConfiguration<Booking>
	{
		public void Configure(EntityTypeBuilder<Booking> builder)
		{
			builder.Property(p => p.StartDateTime)
				.IsRequired();

			builder.Property(p => p.EndDateTime)
				.IsRequired();

			builder.Property(p => p.UserId)
				.IsRequired();
		}
	}
}
