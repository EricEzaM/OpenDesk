using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenDesk.Application.Identity;

namespace OpenDesk.Application.Persistence.Configurations
{
	class OpenDeskRoleConfiguration : IEntityTypeConfiguration<OpenDeskRole>
	{
		public void Configure(EntityTypeBuilder<OpenDeskRole> builder)
		{
		}
	}
}
