using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenDesk.API.Errors;
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
	public class UpdateDeskCommand : IRequest<DeskDTO>
	{
		[JsonIgnore]
		public string DeskId { get; set; }
		public string Name { get; set; }
		public DiagramPosition DiagramPosition { get; set; }

	}

	public class UpdateDeskHandler : IRequestHandler<UpdateDeskCommand, DeskDTO>
	{
		private readonly OpenDeskDbContext _db;

		public UpdateDeskHandler(OpenDeskDbContext db)
		{
			_db = db;
		}

		public async Task<DeskDTO> Handle(UpdateDeskCommand request, CancellationToken cancellationToken)
		{
			var desk = await _db.Desks.FirstOrDefaultAsync(d => d.Id == request.DeskId, cancellationToken);

			if (desk == null)
			{
				throw new EntityNotFoundException("Desk");
			}

			desk.Name = request.Name;
			desk.DiagramPosition = request.DiagramPosition;

			await _db.SaveChangesAsync(cancellationToken);

			return new DeskDTO
			{
				Id = desk.Id,
				Name = desk.Name,
				DiagramPosition = desk.DiagramPosition
			};
		}
	}

	public class UpdateDeskValidator : AbstractValidator<UpdateDeskCommand>
	{
		private readonly OpenDeskDbContext _db;

		public UpdateDeskValidator(OpenDeskDbContext db)
		{
			_db = db;

			RuleFor(d => d.Name)
				.NotEmpty()
				.Must((c, name) => ValidateNoDeskWithSameNameInSameOffice(c))
				.WithMessage("A desk with the name '{PropertyValue}' already exists in the same office.");

			RuleFor(d => d.DiagramPosition)
				.NotEmpty()
				.Must((c, position) => ValidateNoDeskWithSamePositionInSameOffice(c))
				.WithMessage("A desk with the position {PropertyValue} already exists in the same office.");
		}

		private bool ValidateNoDeskWithSameNameInSameOffice(UpdateDeskCommand command)
		{
			var officeId = _db.Desks
				.Include(d => d.Office)
				.First(d => d.Id == command.DeskId)
				.Office.Id;

			var desksWithSameName = _db.Desks
				.Include(d => d.Office)
				.Where(d => d.Name == command.Name)
				.Where(d => d.Office.Id == officeId)
				.Where(d => d.Id != command.DeskId)
				.ToList();

			return desksWithSameName.Any() == false;
		}

		private bool ValidateNoDeskWithSamePositionInSameOffice(UpdateDeskCommand command)
		{
			var officeId = _db.Desks
				.Include(d => d.Office)
				.First(d => d.Id == command.DeskId)
				.Office.Id;

			var desksWithSamePosition = _db.Desks
				.Include(d => d.Office)
				.Where(d => d.DiagramPosition.X == command.DiagramPosition.X)
				.Where(d => d.DiagramPosition.Y == command.DiagramPosition.Y)
				.Where(d => d.Office.Id == officeId)
				.Where(d => d.Id != command.DeskId)
				.ToList();

			return desksWithSamePosition.Any() == false;
		}
	}
}
