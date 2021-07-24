using MediatR;
using OpenDesk.Application.Common.DataTransferObjects;
using OpenDesk.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Roles
{
	public class GetRolesCommand : IRequest<IEnumerable<RoleDTO>>
	{
	}

	public class GetRolesHandler : IRequestHandler<GetRolesCommand, IEnumerable<RoleDTO>>
	{
		private readonly IIdentityService _identityService;

		public GetRolesHandler(IIdentityService identityService)
		{
			_identityService = identityService;
		}

		public async Task<IEnumerable<RoleDTO>> Handle(GetRolesCommand request, CancellationToken cancellationToken)
		{
			return await _identityService.GetRolesAsync();
		}
	}
}
