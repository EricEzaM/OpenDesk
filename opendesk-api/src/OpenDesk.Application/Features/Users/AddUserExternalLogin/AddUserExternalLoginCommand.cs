using MediatR;
using Microsoft.AspNetCore.Identity;
using OpenDesk.Application.Common;
using OpenDesk.Application.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.Application.Features.Users
{
	public class AddUserExternalLoginCommand : IRequest<Result>
	{
		public AddUserExternalLoginCommand(string userId, string provider, string providerKey)
		{
			UserId = userId;
			Provider = provider;
			ProviderKey = providerKey;
		}

		public string UserId { get; set; }
		public string Provider { get; set; }
		public string ProviderKey { get; set; }
	}

	public class AddUserExternalLoginCommandHandler : IRequestHandler<AddUserExternalLoginCommand, Result>
	{
		private readonly UserManager<OpenDeskUser> _userManager;

		public AddUserExternalLoginCommandHandler(UserManager<OpenDeskUser> userManager)
		{
			_userManager = userManager;
		}

		public async Task<Result> Handle(AddUserExternalLoginCommand request, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByIdAsync(request.UserId);

			if (user == null)
			{
				return Result.Failure("Cannot find user.");
			}

			var info = new UserLoginInfo(request.Provider, request.ProviderKey, request.Provider);
			var result = await _userManager.AddLoginAsync(user, info);

			return result.ToOpenDeskResult();
		}
	}
}
