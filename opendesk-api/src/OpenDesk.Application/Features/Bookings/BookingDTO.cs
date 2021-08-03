using OpenDesk.Application.Features.Users;
using System;

namespace OpenDesk.Application.Features.Bookings
{
	public class BookingDTO
	{
		public string Id { get; set; }
		public string DeskId { get; set; }
		public DateTimeOffset StartDateTime { get; set; }
		public DateTimeOffset EndDateTime { get; set; }
		public UserDTO User { get; set; }
	}
}
