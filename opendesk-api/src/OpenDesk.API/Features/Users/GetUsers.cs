using MediatR;
using OpenDesk.Application.Common.DataTransferObjects;
using OpenDesk.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Users
{
	public class GetUsersCommand : IRequest<IEnumerable<UserDTO>> { }

	public class GetUsersHandler : IRequestHandler<GetUsersCommand, IEnumerable<UserDTO>>
	{
		private readonly IIdentityService _identityService;

		public GetUsersHandler(IIdentityService identityService)
		{
			_identityService = identityService;
		}

		public async Task<IEnumerable<UserDTO>> Handle(GetUsersCommand request, CancellationToken cancellationToken)
		{
			var result = await _identityService.GetUsersAsync();

			if (result.Succeeded)
			{
				return result.Value;
			}
			else
			{
				throw new Exception("Could not get users.");
			}
		}
	}
}
