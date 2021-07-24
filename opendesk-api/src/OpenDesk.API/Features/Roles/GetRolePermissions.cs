using MediatR;
using Microsoft.AspNetCore.Identity;
using OpenDesk.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Roles
{
	public class GetRolePermissionsCommand : IRequest<IEnumerable<string>>
	{
		public string RoleId { get; set; }

		public GetRolePermissionsCommand(string roleId)
		{
			RoleId = roleId;
		}
	}

	public class GetRolePermissionsHandler : IRequestHandler<GetRolePermissionsCommand, IEnumerable<string>>
	{
		private readonly IIdentityService _identityService;

		public GetRolePermissionsHandler(IIdentityService identityService)
		{
			_identityService = identityService;
		}

		public async Task<IEnumerable<string>> Handle(GetRolePermissionsCommand request, CancellationToken cancellationToken)
		{
			var result = await _identityService.GetRolePermissions(request.RoleId);

			if (result.Succeeded)
			{
				return result.Value;
			}
			else
			{
				throw new Exception("Unable to get role permissions.");
			}
		}
	}
}
