using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
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
	public class CreateUserCommand : IRequest<OpenDeskUser>
	{
		public CreateUserCommand(string email, string displayName)
		{
			Email = email;
			DisplayName = displayName;
		}

		public string Email { get; set; }
		public string DisplayName { get; set; }
	}

	public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, OpenDeskUser>
	{
		private readonly UserManager<OpenDeskUser> _userManager;
		private readonly IOptions<ApplicationOptions> _appOptions;

		public CreateUserCommandHandler(UserManager<OpenDeskUser> userManager, IOptions<ApplicationOptions> appOptions)
		{
			_userManager = userManager;
			_appOptions = appOptions;
		}

		public async Task<OpenDeskUser> Handle(CreateUserCommand request, CancellationToken cancellationToken)
		{
			var newUser = new OpenDeskUser(request.Email, request.DisplayName);
			newUser.Email = request.Email;

			// TODO do anything with result?
			var res = await _userManager.CreateAsync(newUser);

			await _userManager.AddToRoleAsync(newUser, RoleStrings.Member);

			if (newUser.Email == _appOptions.Value.SuperAdminEmail)
			{
				await _userManager.AddToRoleAsync(newUser, RoleStrings.SuperAdmin);
			}

			return newUser;
		}
	}
}
