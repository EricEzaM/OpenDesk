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
				.Select(b => new BookingDTO
				{
					Id = b.Id,
					DeskId = b.Desk.Id,
					UserId = b.UserId,
					StartDateTime = b.StartDateTime,
					EndDateTime = b.EndDateTime
				})
				.ToListAsync();
		}
	}
}
