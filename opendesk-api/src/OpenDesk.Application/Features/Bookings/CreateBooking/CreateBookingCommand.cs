using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenDesk.Application.Entities;
using OpenDesk.Application.Exceptions;
using OpenDesk.Application.Features.Users;
using OpenDesk.Application.Persistence;
using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.Application.Features.Bookings
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

	public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, BookingDTO>
	{
		private readonly OpenDeskDbContext _db;

		public CreateBookingCommandHandler(OpenDeskDbContext db)
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

			await _db.SaveChangesAsync(cancellationToken);

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
}
