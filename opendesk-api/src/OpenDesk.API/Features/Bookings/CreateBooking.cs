using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenDesk.Application.Common.DataTransferObjects;
using OpenDesk.Application.Common.Models;
using OpenDesk.Application.Common.Interfaces;
using OpenDesk.Domain.Entities;
using OpenDesk.Infrastructure.Persistence;
using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using OpenDesk.API.Models;
using OpenDesk.API.Errors;

namespace OpenDesk.API.Features.Bookings
{
	public class CreateBookingCommand : IRequest<BookingDTO>
	{
		[JsonIgnore]
		public string UserId { get; set; }
		[JsonIgnore]
		public string DeskId { get; set; }
		public DateTimeOffset StartDateTime { get; set; }
		public DateTimeOffset EndDateTime { get; set; }
	}

	public class CreateBookingHandler : IRequestHandler<CreateBookingCommand, BookingDTO>
	{
		private readonly OpenDeskDbContext _db;

		public CreateBookingHandler(OpenDeskDbContext db)
		{
			_db = db;
		}

		public async Task<BookingDTO> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
		{
			var desk = await _db
				.Desks
				.Include(d => d.Bookings)
				.FirstOrDefaultAsync(d => d.Id == request.DeskId);

			if (desk == null)
			{
				throw new EntityNotFoundException("Desk");
			}

			var user = _db.Users.FirstOrDefault(u => u.Id == request.UserId);

			if (user == null)
			{
				throw new EntityNotFoundException("User");
			}

			var booking = new Booking
			{
				UserId = request.UserId,
				StartDateTime = request.StartDateTime,
				EndDateTime = request.EndDateTime,
			};

			await _db.Bookings.AddAsync(booking);

			desk.Bookings.Add(booking);

			await _db.SaveChangesAsync();

			return new BookingDTO
			{
				Id = booking.Id,
				DeskId = booking.Desk.Id,
				StartDateTime = booking.StartDateTime,
				EndDateTime = booking.EndDateTime,
				User = new UserDTO
				{
					Id = user.Id,
					UserName = user.UserName,
					DisplayName = user.DisplayName
				}
			};
		}
	}

	public class CreateBookingValidator : AbstractValidator<CreateBookingCommand>
	{
		private readonly OpenDeskDbContext _db;

		public CreateBookingValidator(OpenDeskDbContext db)
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
					(command.StartDateTime >= b.StartDateTime && command.StartDateTime < b.EndDateTime) ||
					// End is between existing start and end
					(command.EndDateTime > b.StartDateTime && command.EndDateTime <= b.EndDateTime) ||
					// Start is before existing start and end is after existing end (i.e. fully surrounds existing)
					(command.StartDateTime <= b.StartDateTime && command.EndDateTime >= b.EndDateTime)
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
					(command.StartDateTime >= b.StartDateTime && command.StartDateTime < b.EndDateTime) ||
					// End is between existing start and end
					(command.EndDateTime > b.StartDateTime && command.EndDateTime <= b.EndDateTime) ||
					// Start is before existing start and end is after existing end (i.e. fully surrounds existing)
					(command.StartDateTime <= b.StartDateTime && command.EndDateTime >= b.EndDateTime))
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
