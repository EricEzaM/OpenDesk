using MediatR;
using Microsoft.AspNetCore.Identity;
using OpenDesk.Application.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.Application.Features.Roles
{
	public class GetRolePermissionsQuery : IRequest<IEnumerable<string>>
	{
		public GetRolePermissionsQuery(string roleId)
		{
			RoleId = roleId;
		}

		public string RoleId { get; }
	}

	public class GetRolePermissionsQueryHandler : IRequestHandler<GetRolePermissionsQuery, IEnumerable<string>>
	{
		private readonly RoleManager<OpenDeskRole> _roleManager;

		public GetRolePermissionsQueryHandler(RoleManager<OpenDeskRole> roleManager)
		{
			_roleManager = roleManager;
		}

		public async Task<IEnumerable<string>> Handle(GetRolePermissionsQuery request, CancellationToken cancellationToken)
		{
			var role = await _roleManager.FindByIdAsync(request.RoleId);
			var claims = await _roleManager.GetClaimsAsync(role);

			return claims
				.Where(c => c.Type == CustomClaimTypes.Permission)
				.Select(c => c.Value);
		}
	}
}
