using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenDesk.Application.Common.DataTransferObjects;
using OpenDesk.Domain.Entities;
using OpenDesk.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Bookings
{
	public class CreateBookingCommand : IRequest<BookingDTO>
	{
		public string UserId { get; set; }
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
			var desk = _db.Desks
				.Include(d => d.Bookings)
				.FirstOrDefault(d => d.Id == request.DeskId);

			if (desk == null)
			{
				throw new Exception("desk not found");
			}

			var booking = new Booking
			{
				// TODO: Get this from currently logged in user, NOT from query params
				UserId = request.UserId,
				StartDateTime = request.StartDateTime,
				EndDateTime = request.EndDateTime
			};

			desk.Bookings.Add(booking);

			_db.Bookings.Add(booking);

			await _db.SaveChangesAsync();

			return new BookingDTO
			{
				Id = booking.Id,
				DeskId = booking.Desk.Id,
				StartDateTime = booking.StartDateTime,
				EndDateTime = booking.EndDateTime,
				User = new UserDTO
				{
					Id = "SomeId",
					Name = "SomeName",
					UserName = "SomeUserName"
				}
			};
		}
	}
}
