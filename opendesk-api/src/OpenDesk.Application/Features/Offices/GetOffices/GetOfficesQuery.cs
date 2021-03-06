using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenDesk.Application.Features.Blobs;
using OpenDesk.Application.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.Application.Features.Offices
{
	public class GetOfficesQuery : IRequest<IEnumerable<OfficeDto>> { }

	public class GetOfficesQueryHandler : IRequestHandler<GetOfficesQuery, IEnumerable<OfficeDto>>
	{
		private readonly OpenDeskDbContext _db;

		public GetOfficesQueryHandler(OpenDeskDbContext db)
		{
			_db = db;
		}

		public async Task<IEnumerable<OfficeDto>> Handle(GetOfficesQuery request, CancellationToken cancellationToken)
		{
			return await _db.Offices
				.Select(o => new OfficeDto
				{
					Id = o.Id,
					Location = o.Location,
					SubLocation = o.SubLocation,
					Name = o.Name,
					Image = new BlobDto(o.Image)
				})
				.AsNoTracking()
				.ToListAsync();
		}
	}
}
