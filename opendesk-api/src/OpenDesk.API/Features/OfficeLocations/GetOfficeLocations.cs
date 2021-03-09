using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenDesk.Application.Common.DataTransferObjects;
using OpenDesk.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.OfficeLocations
{
	public class GetOfficeLocationsQuery : IRequest<IEnumerable<OfficeLocationDTO>> { }

	public class GetOfficeLocationsHandler : IRequestHandler<GetOfficeLocationsQuery, IEnumerable<OfficeLocationDTO>>
	{
		private readonly OpenDeskDbContext _db;

		public GetOfficeLocationsHandler(OpenDeskDbContext db)
		{
			_db = db;
		}

		public async Task<IEnumerable<OfficeLocationDTO>> Handle(GetOfficeLocationsQuery request, CancellationToken cancellationToken)
		{
			return await _db.OfficeLocations
				.Select(ol => new OfficeLocationDTO
				{
					Id = ol.Id,
					Name = ol.Name
				})
				.ToListAsync();
		}
	}
}
