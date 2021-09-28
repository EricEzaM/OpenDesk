using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenDesk.Application.Exceptions;
using OpenDesk.Application.Persistence;
using OpenDesk.Application.ValueObjects;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.Application.Features.Desks
{
	public class UpdateDeskCommand : IRequest<DeskDto>
	{
		[JsonIgnore]
		public string DeskId { get; set; }
		public string Name { get; set; }
		public DiagramPosition DiagramPosition { get; set; }

	}

	public class UpdateDeskHandler : IRequestHandler<UpdateDeskCommand, DeskDto>
	{
		private readonly OpenDeskDbContext _db;

		public UpdateDeskHandler(OpenDeskDbContext db)
		{
			_db = db;
		}

		public async Task<DeskDto> Handle(UpdateDeskCommand request, CancellationToken cancellationToken)
		{
			var desk = await _db.Desks.FirstOrDefaultAsync(d => d.Id == request.DeskId, cancellationToken);

			if (desk == null)
			{
				throw new EntityNotFoundException("Desk");
			}

			desk.Name = request.Name;
			desk.DiagramPosition = request.DiagramPosition;

			await _db.SaveChangesAsync(cancellationToken);

			return new DeskDto
			{
				Id = desk.Id,
				Name = desk.Name,
				DiagramPosition = desk.DiagramPosition
			};
		}
	}
}
