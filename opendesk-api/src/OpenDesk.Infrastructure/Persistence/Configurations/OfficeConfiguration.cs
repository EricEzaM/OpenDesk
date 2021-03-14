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
	public class OfficeConfiguration : IEntityTypeConfiguration<Office>
	{
		public void Configure(EntityTypeBuilder<Office> builder)
		{
			builder.Property(o => o.Id)
				.ValueGeneratedOnAdd();

			builder.OwnsOne(o => o.Image);

			builder.HasKey(x => x.Id);

			builder.Property(p => p.Name)
				.IsRequired();
		}
	}
}
