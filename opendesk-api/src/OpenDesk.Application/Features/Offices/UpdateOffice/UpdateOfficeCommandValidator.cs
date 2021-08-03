using FluentValidation;
using OpenDesk.Application.Persistence;
using System.Linq;

namespace OpenDesk.Application.Features.Offices
{
	public class UpdateOfficeCommandValidator : AbstractValidator<UpdateOfficeCommand>
	{
		private readonly OpenDeskDbContext _db;

		public UpdateOfficeCommandValidator(OpenDeskDbContext db)
		{
			_db = db;

			RuleFor(o => o)
				.Must(c => ValidateNoOfficeWithSameDetails(c))
				.WithMessage(o => $"An Office with the name '{o.Name}' already exists with location '{o.Location}' and sublocation '{o.SubLocation}'.");
		}

		private bool ValidateNoOfficeWithSameDetails(UpdateOfficeCommand office)
		{
			return _db.Offices
				.Any(o => o.Name == office.Name && o.Location == office.Location && o.SubLocation == office.SubLocation) == false;
		}
	}
}
