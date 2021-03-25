using FluentValidation;
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

	public class CreateDeskValidator : AbstractValidator<CreateDeskCommand>
	{
		private readonly OpenDeskDbContext _db;

		public CreateDeskValidator(OpenDeskDbContext db)
		{
			_db = db;

			RuleFor(d => d.Name)
				.NotEmpty()
				.Must((c, name) => ValidateNoDeskWithSameNameInSameOffice(c))
				.WithMessage("A desk with the name '{PropertyValue}' already exists at this office.");

			RuleFor(d => d.DiagramPosition)
				.NotEmpty()
				.Must((c, position) => ValidateNoDeskWithSamePositionInSameOffice(c))
				.WithMessage("A desk with the position {PropertyValue} already exists at this office.");

			RuleFor(d => d.OfficeId)
				.NotEmpty()
				.Must((c, officeId) => ValidateOfficeExists(c))
				.WithMessage("No office was found with the id {PropertyValue}.");
		}

		private bool ValidateNoDeskWithSameNameInSameOffice(CreateDeskCommand command)
		{
			var desksWithSameName = _db.Desks
				.Include(d => d.Office)
				.Where(d => d.Name == command.Name)
				.Where(d => d.Office.Id == command.OfficeId)
				.ToList();

			return desksWithSameName.Any() == false;
		}

		private bool ValidateNoDeskWithSamePositionInSameOffice(CreateDeskCommand command)
		{
			var desksWithSameName = _db.Desks
				.Include(d => d.Office)
				.Where(d => d.DiagramPosition.X == command.DiagramPosition.X)
				.Where(d => d.DiagramPosition.Y == command.DiagramPosition.Y)
				.Where(d => d.Office.Id == command.OfficeId)
				.ToList();

			return desksWithSameName.Any() == false;
		}

		private bool ValidateOfficeExists(CreateDeskCommand command)
		{
			var office = _db.Offices
				.FirstOrDefault(o => o.Id == command.OfficeId);

			return office != null;
		}
	}
}
