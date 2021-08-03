using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OpenDesk.Application.Persistence;
using System.Linq;

namespace OpenDesk.Application.Features.Desks
{
	public class CreateDeskCommandValidator : AbstractValidator<CreateDeskCommand>
	{
		private readonly OpenDeskDbContext _db;

		public CreateDeskCommandValidator(OpenDeskDbContext db)
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
