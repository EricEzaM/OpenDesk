using MediatR;
using Microsoft.AspNetCore.Identity;
using OpenDesk.Application.Identity;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.Application.Features.Users
{
	public class SetUserRolesCommand : IRequest
	{
		public SetUserRolesCommand(string userId, IEnumerable<string> roleNames)
		{
			UserId = userId;
			RoleNames = roleNames;
		}

		public string UserId { get; }
		public IEnumerable<string> RoleNames { get; }
	}

	internal class SetUserRolesCommandHandler : IRequestHandler<SetUserRolesCommand, Unit>
	{
		private readonly UserManager<OpenDeskUser> _userManager;

		public SetUserRolesCommandHandler(UserManager<OpenDeskUser> userManager)
		{
			_userManager = userManager;
		}

		public async Task<Unit> Handle(SetUserRolesCommand request, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByIdAsync(request.UserId);

			var currentRoles = await _userManager.GetRolesAsync(user);

			var removalResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

			var addResult = await _userManager.AddToRolesAsync(user, request.RoleNames);

			return Unit.Value;
		}
	}
}