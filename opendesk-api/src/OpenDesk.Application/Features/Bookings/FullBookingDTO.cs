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
		public OfficeDTO Office { get; set; }
		public DeskDTO Desk { get; set; }
		public UserDTO User { get; set; }
	}
}
