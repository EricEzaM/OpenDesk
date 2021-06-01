using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenDesk.API.Models;
using OpenDesk.Application.Common.DataTransferObjects;
using OpenDesk.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Bookings
{
	public class GetBookingsForOfficeCommand : IRequest<ApiResponse<IEnumerable<BookingDTO>>>
	{
		public GetBookingsForOfficeCommand(string officeId)
		{
			OfficeId = officeId;
		}

		public string OfficeId { get; }
	}

	public class GetBookingsForOfficeHandler : IRequestHandler<GetBookingsForOfficeCommand, ApiResponse<IEnumerable<BookingDTO>>>
	{
		private readonly OpenDeskDbContext _db;

		public GetBookingsForOfficeHandler(OpenDeskDbContext db)
		{
			_db = db;
		}

		public async Task<ApiResponse<IEnumerable<BookingDTO>>> Handle(GetBookingsForOfficeCommand request, CancellationToken cancellationToken)
		{
			return new ApiResponse<IEnumerable<BookingDTO>>(
				await _db.Bookings
					.Include(b => b.Desk)
					.ThenInclude(d => d.Office)
					.Where(b => b.Desk.Office.Id == request.OfficeId)
					.Join(_db.Users, b => b.UserId, u => u.Id, (b, u) => new
					{
						Booking = b,
						User = new UserDTO
						{
							Id = u.Id,
							UserName = u.UserName,
							Name = "Name Placeholder"
						}
					}).Select(bu => new BookingDTO
					{
						Id = bu.Booking.Id,
						DeskId = bu.Booking.Desk.Id,
						StartDateTime = bu.Booking.StartDateTime,
						EndDateTime = bu.Booking.EndDateTime,
						User = bu.User
					})
					.ToListAsync()
				);
		}
	}
}
