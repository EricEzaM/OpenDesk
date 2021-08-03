using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenDesk.Application.Entities;
using OpenDesk.Application.Exceptions;
using OpenDesk.Application.Persistence;
using OpenDesk.Application.ValueObjects;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.Application.Features.Desks
{
	public class CreateDeskCommand : IRequest<DeskDTO>
	{
		[JsonIgnore]
		public string OfficeId { get; set; }
		public string Name { get; set; }
		public DiagramPosition DiagramPosition { get; set; }
	}

	public class CreateDeskCommandHandler : IRequestHandler<CreateDeskCommand, DeskDTO>
	{
		private readonly OpenDeskDbContext _db;

		public CreateDeskCommandHandler(OpenDeskDbContext db)
		{
			_db = db;
		}

		public async Task<DeskDTO> Handle(CreateDeskCommand request, CancellationToken cancellationToken)
		{
			var office = await _db
				.Offices
				.Include(o => o.Desks)
				.FirstOrDefaultAsync(o => o.Id == request.OfficeId);

			if (office == null)
			{
				throw new EntityNotFoundException("Office");
			}

			var desk = new Desk()
			{
				Name = request.Name,
				DiagramPosition = request.DiagramPosition,
			};

			await _db.Desks.AddAsync(desk);

			office.Desks.Add(desk);

			await _db.SaveChangesAsync(cancellationToken);

			return new DeskDTO
			{
				Id = desk.Id,
				Name = desk.Name,
				DiagramPosition = desk.DiagramPosition,
			};
		}
	}
}
