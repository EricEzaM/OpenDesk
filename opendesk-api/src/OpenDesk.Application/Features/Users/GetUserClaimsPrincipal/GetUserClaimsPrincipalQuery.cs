using MediatR;
using Microsoft.AspNetCore.Identity;
using OpenDesk.Application.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.Application.Features.Users
{
	public class GetUserClaimsPrincipalQuery : IRequest<ClaimsPrincipal>
	{
		public GetUserClaimsPrincipalQuery(string userId)
		{
			UserId = userId;
		}

		public string UserId { get; set; }
	}

	internal class GetUserClaimsPrincipalQueryhandler : IRequestHandler<GetUserClaimsPrincipalQuery, ClaimsPrincipal>
	{
		private readonly UserManager<OpenDeskUser> _userManager;
		private readonly IUserClaimsPrincipalFactory<OpenDeskUser> _userClaimsPrincipalFactory;

		public GetUserClaimsPrincipalQueryhandler(UserManager<OpenDeskUser> userManager, IUserClaimsPrincipalFactory<OpenDeskUser> userClaimsPrincipalFactory)
		{
			_userManager = userManager;
			_userClaimsPrincipalFactory = userClaimsPrincipalFactory;
		}

		public async Task<ClaimsPrincipal> Handle(GetUserClaimsPrincipalQuery request, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByIdAsync(request.UserId);

			// TODO null check?

			return await _userClaimsPrincipalFactory.CreateAsync(user);
		}
	}
}
