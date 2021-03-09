using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenDesk.Application.Common.DataTransferObjects;
using OpenDesk.Domain.Entities;
using OpenDesk.Domain.ValueObjects;
using OpenDesk.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Desks
{
	public class CreateDesk
	{
		public class Command : IRequest<DeskDTO>
		{
			public string Name { get; set; }
			public DiagramPosition DiagramPosition { get; set; }
			public string OfficeId { get; set; }
		}

		public class Handler : IRequestHandler<Command, DeskDTO>
		{
			private readonly OpenDeskDbContext _db;

			public Handler(OpenDeskDbContext db)
			{
				_db = db;
			}

			public async Task<DeskDTO> Handle(Command request, CancellationToken cancellationToken)
			{
				var office = await _db
					.Offices
					.Include(o => o.Desks)
					.FirstOrDefaultAsync(o => o.Id == request.OfficeId);

				if (office == null)
				{
					// TODO Better error handling?
					throw new Exception("office not found");
				}

				var desk = new Desk()
				{
					Name = request.Name,
					DiagramPosition = request.DiagramPosition,
				};

				await _db.Desks.AddAsync(desk);

				office.Desks.Add(desk);

				await _db.SaveChangesAsync();

				return new DeskDTO
				{
					Id = desk.Id,
					Name = desk.Name,
					DiagramPosition = desk.DiagramPosition,
				};
			}
		}
	}
}
