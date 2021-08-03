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
	public class GetBookingsForDeskQuery : IRequest<IEnumerable<BookingDTO>>
	{
		public GetBookingsForDeskQuery(string deskId)
		{
			DeskId = deskId;
		}

		public string DeskId { get; set; }
	}

	public class GetBookingsForDeskQueryHandler : IRequestHandler<GetBookingsForDeskQuery, IEnumerable<BookingDTO>>
	{
		private readonly OpenDeskDbContext _db;

		public GetBookingsForDeskQueryHandler(OpenDeskDbContext db)
		{
			_db = db;
		}

		public async Task<IEnumerable<BookingDTO>> Handle(GetBookingsForDeskQuery request, CancellationToken cancellationToken)
		{
			return await _db.Bookings
				.Include(b => b.Desk)
				.Where(b => b.Desk.Id == request.DeskId)
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
