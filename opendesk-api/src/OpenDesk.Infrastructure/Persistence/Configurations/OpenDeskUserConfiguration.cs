using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenDesk.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDesk.Infrastructure.Persistence.Configurations
{
	class OpenDeskUserConfiguration : IEntityTypeConfiguration<OpenDeskUser>
	{
		public void Configure(EntityTypeBuilder<OpenDeskUser> builder)
		{
		}
	}
}
