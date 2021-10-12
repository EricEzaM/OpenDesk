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
	class IdentityUserTokensConfiguration : IEntityTypeConfiguration<IdentityUserToken<string>>
	{
		public void Configure(EntityTypeBuilder<IdentityUserToken<string>> builder)
		{
			builder.Property(ut => ut.LoginProvider)
				.HasMaxLength(256);

			builder.Property(ut => ut.Name)
				.HasMaxLength(256);

			builder.Property(ut => ut.Value)
				.HasMaxLength(256);
		}
	}
}
