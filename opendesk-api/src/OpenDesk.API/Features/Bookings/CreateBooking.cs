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

namespace OpenDesk.API.Features.Bookings
{
	public class CreateBookingCommand : IRequest<ValidatableResponse<BookingDTO>>, IValidatable
	{
		[JsonIgnore]
		public string UserId { get; set; }
		[JsonIgnore]
		public string DeskId { get; set; }
		public DateTimeOffset StartDateTime { get; set; }
		public DateTimeOffset EndDateTime { get; set; }
	}

	public class CreateBookingHandler : IRequestHandler<CreateBookingCommand, ValidatableResponse<BookingDTO>>
	{
		private readonly OpenDeskDbContext _db;

		public CreateBookingHandler(OpenDeskDbContext db)
		{
			_db = db;
		}

		public async Task<ValidatableResponse<BookingDTO>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
		{
			var desk = await _db
				.Desks
				.Include(d => d.Bookings)
				.FirstOrDefaultAsync(d => d.Id == request.DeskId);

			if (desk == null)
			{
				throw new Exception("desk not found");
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

			var user = _db.Users.FirstOrDefault(u => u.Id == request.UserId);

			return new ValidatableResponse<BookingDTO>(new BookingDTO
			{
				Id = booking.Id,
				DeskId = booking.Desk.Id,
				StartDateTime = booking.StartDateTime,
				EndDateTime = booking.EndDateTime,
				User = new UserDTO
				{
					Id = user.Id,
					UserName = user.UserName,
					Name = "Name Placeholder"
				}
			});
		}
	}

	public class CreateBookingValidator : AbstractValidator<CreateBookingCommand>
	{
		private readonly OpenDeskDbContext _db;

		public CreateBookingValidator(OpenDeskDbContext db)
		{
			_db = db;

			RuleFor(c => c.DeskId)
				.NotEmpty()
				.WithMessage("Desk must not be empty.")
				.Must((c, deskId) => ValidateNoOverlappingBookingsOnDesk(c))
				.WithMessage("There is already a booking on the desk in this timeslot.");

			RuleFor(c => c.UserId)
				.NotEmpty()
				.WithMessage("User must not be empty.")
				.Must((c, userId) => ValidateNoOverlappingBookingsForUser(c))
				.WithMessage("This user already has a booking in this timeslot. Double booking of desks is not allowed.")
				.Must((c, userId) => ValidateNoBookingsWithDifferentDesksOnSameDayForUser(c))
				.WithMessage("This user already has a booking on another desk on this day.");

			RuleFor(c => c.StartDateTime)
				.NotEmpty()
				.WithMessage("Start must not be empty.")
				.GreaterThanOrEqualTo(DateTimeOffset.Now)
				.WithMessage("Start must be in the future.")
				.Must(d => new int[] { 6, 8, 10, 12, 14, 16, 18, 20 }.Contains(d.Hour))
				.WithMessage("Hours must be 6, 8, 10, 12, 14, 16, 18, 20.")
				.Must(d => d.Minute == 0)
				.WithMessage("Minute must be 0.")
				.Must(d => d.Millisecond == 0)
				.WithMessage("Millisecond must be 0.");

			RuleFor(c => c.EndDateTime)
				.NotEmpty()
				.WithMessage("End must not be empty.")
				.GreaterThanOrEqualTo(DateTimeOffset.Now)
				.WithMessage("End must be in the future.")
				.GreaterThanOrEqualTo(c => c.StartDateTime)
				.WithMessage("End must be in the future from start.")
				.Must(d => new int[] { 6, 8, 10, 12, 14, 16, 18, 20 }.Contains(d.Hour))
				.WithMessage("Hours must be 6, 8, 10, 12, 14, 16, 18, 20.")
				.Must(d => d.Minute == 0)
				.WithMessage("Minute must be 0.")
				.Must(d => d.Millisecond == 0)
				.WithMessage("Millisecond must be 0.");
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
				.Where(b => b.UserId == command.UserId)
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
