using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OpenDesk.Application.Persistence;
using System;
using System.Linq;

namespace OpenDesk.Application.Features.Bookings
{
	public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
	{
		private readonly OpenDeskDbContext _db;

		public CreateBookingCommandValidator(OpenDeskDbContext db)
		{
			_db = db;

			var allowedHours = new int[] { 6, 8, 10, 12, 14, 16, 18, 20 };
			var hrsMessage = $"The hours component of {{PropertyName}} must be one of {allowedHours}";
			var minsMessage = "The minutes component of {PropertyName} must be zero (00)";
			var msMessage = "The milliseconds component of {PropertyName} must be zero (000)";

			RuleFor(c => c.DeskId)
				.NotEmpty()
				.Must((c, deskId) => ValidateNoOverlappingBookingsOnDesk(c))
				.WithMessage("The booking clashes with an existing booking on the desk.");

			RuleFor(c => c.UserId)
				.NotEmpty()
				.Must((c, userId) => ValidateNoOverlappingBookingsForUser(c))
				.WithMessage("The booking clashes with an existing booking on another desk for this user. Double booking of desks is not allowed.")
				.Must((c, userId) => ValidateNoBookingsWithDifferentDesksOnSameDayForUser(c))
				.WithMessage("This user already has a booking on another desk on this day. Booking of multiple desks on the same day is not allowed.");

			RuleFor(c => c.StartDateTime)
				.NotEmpty()
				.GreaterThanOrEqualTo(DateTimeOffset.Now)
				.WithMessage("{PropertyName} must be in the future.")
				.Must(d => allowedHours.Contains(d.Hour))
				.WithMessage(hrsMessage)
				.Must(d => d.Minute == 0)
				.WithMessage(minsMessage)
				.Must(d => d.Millisecond == 0)
				.WithMessage(msMessage);

			RuleFor(c => c.EndDateTime)
				.NotEmpty()
				.GreaterThanOrEqualTo(DateTimeOffset.Now)
				.WithMessage("{PropertyName} must be in the future.")
				.GreaterThan(c => c.StartDateTime)
				.WithMessage(c => $"{{PropertyName}} must be after {nameof(c.StartDateTime)}.")
				.Must(d => allowedHours.Contains(d.Hour))
				.WithMessage(hrsMessage)
				.Must(d => d.Minute == 0)
				.WithMessage(minsMessage)
				.Must(d => d.Millisecond == 0)
				.WithMessage(msMessage);
		}

		private bool ValidateNoOverlappingBookingsOnDesk(CreateBookingCommand command)
		{
			var overlappingBookings = _db
				.Bookings
				.Include(b => b.Desk)
				.Where(b => b.Desk.Id == command.DeskId)
				.Where(b =>
					// Start is between existing start and end
					command.StartDateTime >= b.StartDateTime && command.StartDateTime < b.EndDateTime ||
					// End is between existing start and end
					command.EndDateTime > b.StartDateTime && command.EndDateTime <= b.EndDateTime ||
					// Start is before existing start and end is after existing end (i.e. fully surrounds existing)
					command.StartDateTime <= b.StartDateTime && command.EndDateTime >= b.EndDateTime
				)
				.ToList();

			return overlappingBookings.Any() == false;
		}

		private bool ValidateNoOverlappingBookingsForUser(CreateBookingCommand command)
		{
			var overlappingBookings = _db
				.Bookings
				.Include(b => b.Desk)
				.Where(b => b.UserId == command.UserId)
				.Where(b => b.Desk.Id != command.DeskId) // Must be on different desk
				.Where(b =>
					// Start is between existing start and end
					command.StartDateTime >= b.StartDateTime && command.StartDateTime < b.EndDateTime ||
					// End is between existing start and end
					command.EndDateTime > b.StartDateTime && command.EndDateTime <= b.EndDateTime ||
					// Start is before existing start and end is after existing end (i.e. fully surrounds existing)
					command.StartDateTime <= b.StartDateTime && command.EndDateTime >= b.EndDateTime)
				.ToList();

			return overlappingBookings.Any() == false;
		}

		private bool ValidateNoBookingsWithDifferentDesksOnSameDayForUser(CreateBookingCommand command)
		{
			var overlappingBookings = _db
				.Bookings
				.Where(b => b.UserId == command.UserId)
				.Where(b =>
					b.StartDateTime.Date == command.StartDateTime.Date ||
					b.StartDateTime.Date == command.EndDateTime.Date ||
					b.EndDateTime.Date == command.StartDateTime.Date ||
					b.EndDateTime.Date == command.EndDateTime.Date)
				.ToList();

			return overlappingBookings.Any() == false;
		}
	}
}
