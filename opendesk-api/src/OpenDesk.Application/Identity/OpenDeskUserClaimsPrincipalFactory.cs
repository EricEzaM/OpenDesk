using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OpenDesk.Application.Identity
{
	public class OpenDeskUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<OpenDeskUser, OpenDeskRole>
	{
		public OpenDeskUserClaimsPrincipalFactory(UserManager<OpenDeskUser> userManager, RoleManager<OpenDeskRole> roleManager, IOptions<IdentityOptions> optionsAccessor) : base(userManager, roleManager, optionsAccessor)
		{

		}

		protected override async Task<ClaimsIdentity> GenerateClaimsAsync(OpenDeskUser user)
		{
			var identity = await base.GenerateClaimsAsync(user);
			identity.AddClaim(new Claim(CustomClaimTypes.DisplayName, user.DisplayName));
			return identity;
		}
	}
}
