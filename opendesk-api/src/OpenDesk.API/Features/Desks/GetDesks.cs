using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenDesk.API.Models;
using OpenDesk.Application.Common.DataTransferObjects;
using OpenDesk.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Desks
{
	public class GetDesksCommand : IRequest<IEnumerable<DeskDTO>>
	{
		public GetDesksCommand(string officeId)
		{
			OfficeId = officeId;
		}

		public string OfficeId { get; set; }
	}

	public class GetDesksHandler : IRequestHandler<GetDesksCommand, IEnumerable<DeskDTO>>
	{
		private readonly OpenDeskDbContext _db;

		public GetDesksHandler(OpenDeskDbContext db)
		{
			_db = db;
		}

		public async Task<IEnumerable<DeskDTO>> Handle(GetDesksCommand request, CancellationToken cancellationToken)
		{
			return await _db
				.Desks
				.Where(d => d.Office.Id == request.OfficeId)
				.Select(d => new DeskDTO
				{
					Id = d.Id,
					Name = d.Name,
					DiagramPosition = d.DiagramPosition
				})
				.AsNoTracking() // As No Tracking so that EF does not complain about Diagram Position not having an owner.
				.ToListAsync();
		}
	}
}
