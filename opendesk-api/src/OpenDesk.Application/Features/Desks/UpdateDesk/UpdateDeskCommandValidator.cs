using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OpenDesk.Application.Persistence;
using System.Linq;

namespace OpenDesk.Application.Features.Desks
{
	public class UpdateDeskCommandValidator : AbstractValidator<UpdateDeskCommand>
	{
		private readonly OpenDeskDbContext _db;

		public UpdateDeskCommandValidator(OpenDeskDbContext db)
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
