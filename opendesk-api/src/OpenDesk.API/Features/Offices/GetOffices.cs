using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenDesk.API.Models;
using OpenDesk.Application.Common.DataTransferObjects;
using OpenDesk.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Offices
{
	public class GetOfficesCommand : IRequest<IEnumerable<OfficeDTO>> { }

	public class GetOfficesHandler : IRequestHandler<GetOfficesCommand, IEnumerable<OfficeDTO>>
	{
		private readonly OpenDeskDbContext _db;

		public GetOfficesHandler(OpenDeskDbContext db)
		{
			_db = db;
		}

		public async Task<IEnumerable<OfficeDTO>> Handle(GetOfficesCommand request, CancellationToken cancellationToken)
		{
			return await _db.Offices
				.Select(o => new OfficeDTO
				{
					Id = o.Id,
					Location = o.Location,
					SubLocation = o.SubLocation,
					Name = o.Name,
					Image = new BlobDTO(o.Image)
				})
				.AsNoTracking()
				.ToListAsync();
		}
	}
}
