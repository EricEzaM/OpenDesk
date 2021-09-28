using OpenDesk.Application.Features.Desks;
using OpenDesk.Application.Features.Offices;
using OpenDesk.Application.Features.Users;
using System;

namespace OpenDesk.Application.Features.Bookings
{
	public class FullBookingDTO
	{

		public string Id { get; set; }
		public DateTimeOffset StartDateTime { get; set; }
		public DateTimeOffset EndDateTime { get; set; }
		public OfficeDto Office { get; set; }
		public DeskDto Desk { get; set; }
		public UserDto User { get; set; }
	}
}
