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
	public class GetBookingsForUserCommand : IRequest<IEnumerable<BookingDTO>>
	{
		public GetBookingsForUserCommand(string userId)
		{
			UserId = userId;
		}

		public string UserId { get; }

		public class GetBookingsForUserHandler : IRequestHandler<GetBookingsForUserCommand, IEnumerable<BookingDTO>>
		{
			private readonly OpenDeskDbContext _db;

			public GetBookingsForUserHandler(OpenDeskDbContext db)
			{
				_db = db;
			}

			public async Task<IEnumerable<BookingDTO>> Handle(GetBookingsForUserCommand request, CancellationToken cancellationToken)
			{
				return await _db.Bookings
					.Where(b => b.UserId == request.UserId)
					// TODO: This join is used in many commands/handlers. Make common?
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
}
