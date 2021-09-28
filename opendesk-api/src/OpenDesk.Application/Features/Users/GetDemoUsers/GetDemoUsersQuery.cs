using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenDesk.Application.Common;
using OpenDesk.Application.Features.Users;
using OpenDesk.Application.Identity;
using OpenDesk.Application.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.Application.Features.Users
{
	public class GetDemoUsersQuery : IRequest<IEnumerable<UserDto>>
	{
	}

	public class GetDemoUsersQueryHandler : IRequestHandler<GetDemoUsersQuery, IEnumerable<UserDto>>
	{
		private readonly UserManager<OpenDeskUser> _userManager;

		public GetDemoUsersQueryHandler(UserManager<OpenDeskUser> db)
		{
			_userManager = db;
		}

		public async Task<IEnumerable<UserDto>> Handle(GetDemoUsersQuery request, CancellationToken cancellationToken)
		{
			var users = await _userManager.GetUsersInRoleAsync(RoleStrings.Demo);
			return users.Select(u => new UserDto
			{
				Id = u.Id,
				UserName = u.UserName,
				DisplayName = u.DisplayName
			});
		}
	}
}
