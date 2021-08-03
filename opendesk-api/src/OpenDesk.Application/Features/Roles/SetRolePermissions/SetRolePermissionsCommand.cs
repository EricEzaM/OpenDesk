using MediatR;
using Microsoft.AspNetCore.Identity;
using OpenDesk.Application.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.Application.Features.Roles
{
	public class SetRolePermissionsCommand : IRequest<Unit>
	{
		public SetRolePermissionsCommand(string roleId, IEnumerable<string> permissions)
		{
			RoleId = roleId;
			Permissions = permissions;
		}

		public string RoleId { get; }
		public IEnumerable<string> Permissions { get; }
	}

	internal class SetRolePermissionsCommandHandler : IRequestHandler<SetRolePermissionsCommand, Unit>
	{
		private readonly RoleManager<OpenDeskRole> _roleManager;

		public SetRolePermissionsCommandHandler(RoleManager<OpenDeskRole> roleManager)
		{
			_roleManager = roleManager;
		}

		public async Task<Unit> Handle(SetRolePermissionsCommand request, CancellationToken cancellationToken)
		{
			var role = await _roleManager.FindByIdAsync(request.RoleId);
			var claims = await _roleManager.GetClaimsAsync(role);

			var permissionClaims = claims
				.Where(c => c.Type == CustomClaimTypes.Permission);

			// Not ideal...
			// TODO write extension method to do this in one database hit
			foreach (var pClaim in permissionClaims)
			{
				// Do anything with result...?
				var removeResult = await _roleManager.RemoveClaimAsync(role, pClaim);
			}

			// TODO write extension method to do this in one database hit
			foreach (var newClaim in request.Permissions)
			{
				// Do anything with result...?
				var addResult = await _roleManager.AddClaimAsync(role, new System.Security.Claims.Claim(CustomClaimTypes.Permission, newClaim));
			}

			return Unit.Value;
		}
	}
}