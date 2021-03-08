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
