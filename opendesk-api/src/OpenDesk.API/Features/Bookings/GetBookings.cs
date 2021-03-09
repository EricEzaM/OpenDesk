using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenDesk.Application.Common.DataTransferObjects;
using OpenDesk.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Bookings
{
	public class GetBookingsCommand : IRequest<IEnumerable<BookingDTO>> { }

	public class GetBookingsHandler : IRequestHandler<GetBookingsCommand, IEnumerable<BookingDTO>>
	{
		private readonly OpenDeskDbContext _db;

		public GetBookingsHandler(OpenDeskDbContext db)
		{
			_db = db;
		}

		public async Task<IEnumerable<BookingDTO>> Handle(GetBookingsCommand request, CancellationToken cancellationToken)
		{
			return await _db.Bookings
				.Include(b => b.Desk)
				.Join(_db.Users, b => b.UserId, u => u.Id, (b, u) => new
				{
					Booking = b,
					User = new UserDTO
					{
						Id = u.Id,
						UserName = u.UserName,
						Name = "Name Placeholder"
					}
				})
				.Select(bu => new BookingDTO
				{
					Id = bu.Booking.Id,
					DeskId = bu.Booking.Desk.Id,
					StartDateTime = bu.Booking.StartDateTime,
					EndDateTime = bu.Booking.EndDateTime,
					User = bu.User
				})
				.ToListAsync();
		}
	}
}
