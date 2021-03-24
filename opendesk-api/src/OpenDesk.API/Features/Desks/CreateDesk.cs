using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenDesk.API.Models;
using OpenDesk.Application.Common.DataTransferObjects;
using OpenDesk.Domain.Entities;
using OpenDesk.Domain.ValueObjects;
using OpenDesk.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Desks
{
	public class CreateDeskCommand : IRequest<ApiResponse<DeskDTO>>
	{
		[JsonIgnore]
		public string OfficeId { get; set; }
		public string Name { get; set; }
		public DiagramPosition DiagramPosition { get; set; }
	}

	public class CreateDeskHandler : IRequestHandler<CreateDeskCommand, ApiResponse<DeskDTO>>
	{
		private readonly OpenDeskDbContext _db;

		public CreateDeskHandler(OpenDeskDbContext db)
		{
			_db = db;
		}

		public async Task<ApiResponse<DeskDTO>> Handle(CreateDeskCommand request, CancellationToken cancellationToken)
		{
			var office = await _db
				.Offices
				.Include(o => o.Desks)
				.FirstOrDefaultAsync(o => o.Id == request.OfficeId);

			if (office == null)
			{
				return new ApiResponse<DeskDTO>(OperationOutcome.ValidationFailure("The office could not be found."));
			}

			var desk = new Desk()
			{
				Name = request.Name,
				DiagramPosition = request.DiagramPosition,
			};

			await _db.Desks.AddAsync(desk);

			office.Desks.Add(desk);

			await _db.SaveChangesAsync();

			return new ApiResponse<DeskDTO>(
				new DeskDTO
				{
					Id = desk.Id,
					Name = desk.Name,
					DiagramPosition = desk.DiagramPosition,
				});
		}
	}
}
