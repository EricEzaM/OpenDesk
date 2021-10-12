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
	class IdentityUserClaimsConfiguration : IEntityTypeConfiguration<IdentityUserClaim<string>>
	{
		public void Configure(EntityTypeBuilder<IdentityUserClaim<string>> builder)
		{
			builder.Property(uc => uc.ClaimType)
				.HasMaxLength(256);

			builder.Property(uc => uc.ClaimValue)
				.HasMaxLength(256);
		}
	}
}
