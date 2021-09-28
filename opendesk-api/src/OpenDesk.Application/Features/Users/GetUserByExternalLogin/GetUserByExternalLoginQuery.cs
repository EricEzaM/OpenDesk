using MediatR;
using Microsoft.AspNetCore.Identity;
using OpenDesk.Application.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.Application.Features.Users
{
	public class GetUserByExternalLoginQuery : IRequest<OpenDeskUser>
	{
		public GetUserByExternalLoginQuery(string provider, string providerKey)
		{
			Provider = provider;
			ProviderKey = providerKey;
		}

		public string Provider { get; set; }
		public string ProviderKey { get; set; }
	}

	public class GetUserByExternalLoginQueryHandler : IRequestHandler<GetUserByExternalLoginQuery, OpenDeskUser>
	{
		private readonly UserManager<OpenDeskUser> _userManager;

		public GetUserByExternalLoginQueryHandler(UserManager<OpenDeskUser> userManager)
		{
			_userManager = userManager;
		}

		public async Task<OpenDeskUser> Handle(GetUserByExternalLoginQuery request, CancellationToken cancellationToken)
		{
			return await _userManager.FindByLoginAsync(request.Provider, request.ProviderKey);
		}
	}
}
