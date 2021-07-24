using MediatR;
using OpenDesk.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Users
{
	public class GetUserRolesCommand : IRequest<IEnumerable<string>>
	{
		public GetUserRolesCommand(string userId)
		{
			UserId = userId;
		}

		public string UserId { get; set; }
	}

	public class GetUserRolesHandler : IRequestHandler<GetUserRolesCommand, IEnumerable<string>>
	{
		private readonly IIdentityService _identityService;

		public GetUserRolesHandler(IIdentityService identityService)
		{
			_identityService = identityService;
		}

		public async Task<IEnumerable<string>> Handle(GetUserRolesCommand request, CancellationToken cancellationToken)
		{
			var result = await _identityService.GetUserRolesAsync(request.UserId);

			if (result.Succeeded)
			{
				return result.Value;
			}
			else
			{
				// TODO: Fix... don't like how this throws a generic exception.
				throw new Exception("Could not get roles");
			}
		}
	}
}
