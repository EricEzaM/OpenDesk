using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenDesk.Application.Common;
using OpenDesk.Application.Features.Users;
using OpenDesk.Application.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.Application.Features.Users
{
	public class GetUsersQuery : IRequest<IEnumerable<UserDto>>
	{
	}

	public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IEnumerable<UserDto>>
	{
		private readonly OpenDeskDbContext _db;

		public GetUsersQueryHandler(OpenDeskDbContext db)
		{
			_db = db;
		}

		public async Task<IEnumerable<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
		{
			return await _db.Users
				.AsNoTracking()
				.Select(u => new UserDto
				{
					Id = u.Id,
					UserName = u.UserName,
					DisplayName = u.DisplayName
				})
				.ToListAsync();
		}
	}
}
