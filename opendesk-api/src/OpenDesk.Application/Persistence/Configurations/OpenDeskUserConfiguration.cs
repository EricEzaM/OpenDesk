using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenDesk.Application.Identity;

namespace OpenDesk.Application.Persistence.Configurations
{
	class OpenDeskUserConfiguration : IEntityTypeConfiguration<OpenDeskUser>
	{
		public void Configure(EntityTypeBuilder<OpenDeskUser> builder)
		{
		}
	}
}
