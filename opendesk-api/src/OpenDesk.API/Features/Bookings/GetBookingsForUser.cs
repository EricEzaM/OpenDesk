using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenDesk.Application.Common.DataTransferObjects;
using OpenDesk.Domain.ValueObjects;
using OpenDesk.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Bookings
{
	public class GetBookingsForUserCommand : IRequest<IEnumerable<UserBookingDTO>>
	{
		public GetBookingsForUserCommand(string userId)
		{
			UserId = userId;
		}

		public string UserId { get; }

		public class GetBookingsForUserHandler : IRequestHandler<GetBookingsForUserCommand, IEnumerable<UserBookingDTO>>
		{
			private readonly OpenDeskDbContext _db;

			public GetBookingsForUserHandler(OpenDeskDbContext db)
			{
				_db = db;
			}

			public async Task<IEnumerable<UserBookingDTO>> Handle(GetBookingsForUserCommand request, CancellationToken cancellationToken)
			{
				return await _db.Bookings
					.Include(b => b.Desk)
					.ThenInclude(d => d.Office)
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
					.Select(bu => new UserBookingDTO
					{
						Id = bu.Booking.Id,
						StartDateTime = bu.Booking.StartDateTime,
						EndDateTime = bu.Booking.EndDateTime,
						Desk = new DeskDTO
						{
							Id = bu.Booking.Desk.Id,
							Name = bu.Booking.Desk.Name,
							// EF throws an errow if they are directly set to one another (cannot cast Int to String) ???
							// Duplicating the value works ok.
							DiagramPosition = new DiagramPosition(bu.Booking.Desk.DiagramPosition)
						},
						Office = new OfficeDTO
						{
							Id = bu.Booking.Desk.Office.Id,
							Name = bu.Booking.Desk.Office.Name,
							Image = bu.Booking.Desk.Office.Image
						},
						User = bu.User
					})
				.AsNoTracking()
				.ToListAsync();
			}
		}
	}
}
