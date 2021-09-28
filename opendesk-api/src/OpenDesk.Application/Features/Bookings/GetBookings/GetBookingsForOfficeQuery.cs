using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenDesk.Application.Features.Users;
using OpenDesk.Application.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.Application.Features.Bookings
{
	public class GetBookingsForOfficeQuery : IRequest<IEnumerable<BookingDto>>
	{
		public GetBookingsForOfficeQuery(string officeId)
		{
			OfficeId = officeId;
		}

		public string OfficeId { get; }
	}

	public class GetBookingsForOfficeQueryHandler : IRequestHandler<GetBookingsForOfficeQuery, IEnumerable<BookingDto>>
	{
		private readonly OpenDeskDbContext _db;

		public GetBookingsForOfficeQueryHandler(OpenDeskDbContext db)
		{
			_db = db;
		}

		public async Task<IEnumerable<BookingDto>> Handle(GetBookingsForOfficeQuery request, CancellationToken cancellationToken)
		{
			return await _db.Bookings
					.Include(b => b.Desk)
					.ThenInclude(d => d.Office)
					.Where(b => b.Desk.Office.Id == request.OfficeId)
					.Join(_db.Users, b => b.UserId, u => u.Id, (b, u) => new
					{
						Booking = b,
						User = new UserDto
						{
							Id = u.Id,
							UserName = u.UserName,
							DisplayName = u.DisplayName
						}
					}).Select(bu => new BookingDto
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
