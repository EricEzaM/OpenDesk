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
	public class GetOfficeLocationDesksQuery : IRequest<IEnumerable<DeskDTO>>
	{
		public GetOfficeLocationDesksQuery(string officeLocationId)
		{
			OfficeLocationId = officeLocationId;
		}

		public string OfficeLocationId { get; set; }
	}

	public class GetOfficeLocationDesksHandler : IRequestHandler<GetOfficeLocationDesksQuery, IEnumerable<DeskDTO>>
	{
		private readonly OpenDeskDbContext _db;

		public GetOfficeLocationDesksHandler(OpenDeskDbContext db)
		{
			_db = db;
		}

		public async Task<IEnumerable<DeskDTO>> Handle(GetOfficeLocationDesksQuery request, CancellationToken cancellationToken)
		{
			return await _db.Desks
				.Include(d => d.OfficeLocation)
				.Where(d => d.OfficeLocation.Id == request.OfficeLocationId)
				.Select(d => new DeskDTO
				{
					Id = d.Id,
					DiagramPosition = d.DiagramPosition,
					Name = d.Name,
				})
				.AsNoTracking()
				.ToArrayAsync();
		}
	}
}
