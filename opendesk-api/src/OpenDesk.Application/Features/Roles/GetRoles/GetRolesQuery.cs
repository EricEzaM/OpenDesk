using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenDesk.Application.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.Application.Features.Roles
{
	public class GetRolesQuery : IRequest<IEnumerable<RoleDTO>>
	{
	}

	public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, IEnumerable<RoleDTO>>
	{
		private readonly OpenDeskDbContext _db;

		public GetRolesQueryHandler(OpenDeskDbContext db)
		{
			_db = db;
		}

		public async Task<IEnumerable<RoleDTO>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
		{
			return await _db.Roles
				.Select(r => new RoleDTO
				{
					Id = r.Id,
					Name = r.Name,
					Description = r.Description
				})
				.ToListAsync();
		}
	}
}
