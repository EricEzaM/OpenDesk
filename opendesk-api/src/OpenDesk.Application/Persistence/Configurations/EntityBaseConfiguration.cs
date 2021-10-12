using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenDesk.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDesk.Application.Persistence.Configurations
{
	public class EntityBaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
		where TEntity : EntityBase
	{
		public virtual void Configure(EntityTypeBuilder<TEntity> builder)
		{
			builder.HasKey(p => p.Id);

			builder.Property(e => e.Id)
				.HasMaxLength(50)
				.ValueGeneratedOnAdd();

			builder.Property(e => e.CreatedBy)
				.HasMaxLength(50);

			builder.Property(e => e.DeletedBy)
				.HasMaxLength(50);

			builder.Property(e => e.LastModifiedBy)
				.HasMaxLength(50);
		}
	}
}
