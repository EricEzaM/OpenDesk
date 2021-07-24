using MediatR;
using OpenDesk.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Roles
{
	public class SetRolePermissionsCommand : IRequest<Unit>
	{
		public string RoleId { get; set; }
		public IEnumerable<string> Permissions { get; set; }
	}

	public class SetRolePermissionsHandler : IRequestHandler<SetRolePermissionsCommand>
	{
		private readonly IIdentityService _identityService;

		public SetRolePermissionsHandler(IIdentityService identityService)
		{
			_identityService = identityService;
		}

		public async Task<Unit> Handle(SetRolePermissionsCommand request, CancellationToken cancellationToken)
		{
			var result = await _identityService.SetRolePermissionsAsync(request.RoleId, request.Permissions);

			if (result.Succeeded)
			{
				return Unit.Value;
			}
			else
			{
				throw new Exception("Unable to set role permissions.");
			}
		}
	}
}
