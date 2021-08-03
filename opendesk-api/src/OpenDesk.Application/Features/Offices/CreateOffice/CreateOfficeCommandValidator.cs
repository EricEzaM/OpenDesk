using FluentValidation;
using OpenDesk.Application.Offices.Common;
using OpenDesk.Application.Persistence;
using System.Linq;

namespace OpenDesk.Application.Features.Offices
{
	public class CreateOfficeCommandValidator : AbstractValidator<CreateOfficeCommand>
	{
		private readonly OpenDeskDbContext _db;

		public CreateOfficeCommandValidator(OpenDeskDbContext db)
		{
			_db = db;

			RuleFor(o => o.Location)
				.NotEmpty();

			RuleFor(o => o.SubLocation)
				.NotEmpty();

			RuleFor(o => o.Name)
				.NotEmpty();

			RuleFor(o => o.ImageBlobId)
				.NotEmpty();

			RuleFor(o => o)
				.Must(c => ValidateNoOfficeWithSameDetails(c))
				.WithMessage(o => $"An Office with the name '{o.Name}' already exists with location '{o.Location}' and sublocation '{o.SubLocation}'.");
		}

		private bool ValidateNoOfficeWithSameDetails(OfficeCommandBase command)
		{
			return _db.Offices
				.Any(o => o.Name == command.Name && o.Location == command.Location && o.SubLocation == command.SubLocation) == false;
		}
	}
}
