using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenDesk.Application.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.Application.Features.Desks
{
	public class GetDesksQuery : IRequest<IEnumerable<DeskDTO>>
	{
		public GetDesksQuery(string officeId)
		{
			OfficeId = officeId;
		}

		public string OfficeId { get; set; }
	}

	public class GetDesksQueryHandler : IRequestHandler<GetDesksQuery, IEnumerable<DeskDTO>>
	{
		private readonly OpenDeskDbContext _db;

		public GetDesksQueryHandler(OpenDeskDbContext db)
		{
			_db = db;
		}

		public async Task<IEnumerable<DeskDTO>> Handle(GetDesksQuery request, CancellationToken cancellationToken)
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
