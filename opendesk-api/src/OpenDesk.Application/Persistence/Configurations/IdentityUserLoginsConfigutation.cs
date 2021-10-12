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
	class IdentityUserLoginsConfigutation : IEntityTypeConfiguration<IdentityUserLogin<string>>
	{
		public void Configure(EntityTypeBuilder<IdentityUserLogin<string>> builder)
		{
			builder.Property(ul => ul.LoginProvider)
				.HasMaxLength(256);

			builder.Property(ul => ul.ProviderKey)
				.HasMaxLength(256);

			builder.Property(ul => ul.ProviderDisplayName)
				.HasMaxLength(256);
		}
	}
}
