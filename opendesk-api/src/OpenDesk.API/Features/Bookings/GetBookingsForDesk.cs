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
	public class GetBookingsForDeskCommand : IRequest<IEnumerable<BookingDTO>>
	{
		public GetBookingsForDeskCommand(string deskId)
		{
			DeskId = deskId;
		}

		public string DeskId { get; set; }
	}

	public class GetBookingsForDeskHandler : IRequestHandler<GetBookingsForDeskCommand, IEnumerable<BookingDTO>>
	{
		private readonly OpenDeskDbContext _db;

		public GetBookingsForDeskHandler(OpenDeskDbContext db)
		{
			_db = db;
		}

		public async Task<IEnumerable<BookingDTO>> Handle(GetBookingsForDeskCommand request, CancellationToken cancellationToken)
		{
			return await _db.Bookings
				.Include(b => b.Desk)
				.Where(b => b.Desk.Id == request.DeskId)
				.Select(b => new BookingDTO
				{
					DeskId = b.Desk.Id,
					UserId = b.UserId,
					StartDateTime = b.StartDateTime,
					EndDateTime = b.EndDateTime
				})
				.ToListAsync();

		}
	}
}
