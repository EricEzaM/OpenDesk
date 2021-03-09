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

namespace OpenDesk.API.Features.Office
{
	public class GetOfficeDesksQuery : IRequest<IEnumerable<DeskDTO>>
	{
		public GetOfficeDesksQuery(string officeId)
		{
			OfficeId = officeId;
		}

		public string OfficeId { get; set; }
	}

	public class GetOfficeDesksHandler : IRequestHandler<GetOfficeDesksQuery, IEnumerable<DeskDTO>>
	{
		private readonly OpenDeskDbContext _db;

		public GetOfficeDesksHandler(OpenDeskDbContext db)
		{
			_db = db;
		}

		public async Task<IEnumerable<DeskDTO>> Handle(GetOfficeDesksQuery request, CancellationToken cancellationToken)
		{
			return await _db.Desks
				.Include(d => d.Office)
				.Where(d => d.Office.Id == request.OfficeId)
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
