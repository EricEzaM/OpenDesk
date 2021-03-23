using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenDesk.API.Models;
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
	public class GetBookingsCommand : IRequest<ApiResponse<IEnumerable<FullBookingDTO>>> { }

	public class GetBookingsHandler : IRequestHandler<GetBookingsCommand, ApiResponse<IEnumerable<FullBookingDTO>>>
	{
		private readonly OpenDeskDbContext _db;

		public GetBookingsHandler(OpenDeskDbContext db)
		{
			_db = db;
		}

		public async Task<ApiResponse<IEnumerable<FullBookingDTO>>> Handle(GetBookingsCommand request, CancellationToken cancellationToken)
		{
			return new ApiResponse<IEnumerable<FullBookingDTO>>(
				await _db.Bookings
				.Include(b => b.Desk)
				.ThenInclude(d => d.Office)
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
				.Select(bu => new FullBookingDTO
				{
					Id = bu.Booking.Id,
					StartDateTime = bu.Booking.StartDateTime,
					EndDateTime = bu.Booking.EndDateTime,
					Desk = new DeskDTO
					{
						Id = bu.Booking.Desk.Id,
						Name = bu.Booking.Desk.Name,
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
				.ToListAsync());
		}
	}
}
