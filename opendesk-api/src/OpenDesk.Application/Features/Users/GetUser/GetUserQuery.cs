using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenDesk.Application.Identity;
using OpenDesk.Application.Persistence;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.Application.Features.Users
{
	public class GetUserQuery : IRequest<UserDto>
	{
		public GetUserQuery(string userId)
		{
			UserId = userId;
		}

		public string UserId { get; set; }
	}

	// TODO: set other command/query handler classes to internal
	internal class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDto>
	{
		private readonly UserManager<OpenDeskUser> _userManager;

		public GetUserQueryHandler(UserManager<OpenDeskUser> userManager)
		{
			_userManager = userManager;
		}

		public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByIdAsync(request.UserId);

			return user == null
				? null
				: new UserDto
				{
					Id = user.Id,
					UserName = user.UserName,
					DisplayName = user.DisplayName
				};
		}
	}
}