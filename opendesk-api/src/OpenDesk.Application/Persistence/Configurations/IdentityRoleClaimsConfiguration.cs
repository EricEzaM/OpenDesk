using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDesk.Application.Persistence.Configurations
{
	class IdentityRoleClaimsConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<string>>
	{
		public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
		{
			builder.Property(rc => rc.ClaimType)
				.HasMaxLength(256);

			builder.Property(rc => rc.ClaimValue)
				.HasMaxLength(256);
		}
	}
}
