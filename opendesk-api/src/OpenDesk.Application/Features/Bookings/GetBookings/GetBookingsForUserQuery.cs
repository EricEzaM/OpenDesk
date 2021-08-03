using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenDesk.Application.Features.Blobs;
using OpenDesk.Application.Features.Desks;
using OpenDesk.Application.Features.Offices;
using OpenDesk.Application.Features.Users;
using OpenDesk.Application.Persistence;
using OpenDesk.Application.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.Application.Features.Bookings
{
	public class GetBookingsForUserQuery : IRequest<IEnumerable<FullBookingDTO>>
	{
		public GetBookingsForUserQuery(string userId)
		{
			UserId = userId;
		}

		public string UserId { get; }

		public class GetBookingsForUserQueryHandler : IRequestHandler<GetBookingsForUserQuery, IEnumerable<FullBookingDTO>>
		{
			private readonly OpenDeskDbContext _db;

			public GetBookingsForUserQueryHandler(OpenDeskDbContext db)
			{
				_db = db;
			}

			public async Task<IEnumerable<FullBookingDTO>> Handle(GetBookingsForUserQuery request, CancellationToken cancellationToken)
			{
				return await _db.Bookings
					.Include(b => b.Desk)
					.ThenInclude(d => d.Office)
					.ThenInclude(o => o.Image)
					.Where(b => b.UserId == request.UserId)
					// TODO: This join is used in many commands/handlers. Make common?
					.Join(_db.Users, b => b.UserId, u => u.Id, (b, u) => new
					{
						Booking = b,
						User = new UserDTO
						{
							Id = u.Id,
							UserName = u.UserName,
							DisplayName = u.DisplayName
						}
					})
					.Select(bu => new FullBookingDTO
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
							Image = new BlobDTO(bu.Booking.Desk.Office.Image)
						},
						User = bu.User
					})
				.AsNoTracking()
				.ToListAsync();
			}
		}
	}
}
