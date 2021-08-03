using System;

namespace OpenDesk.Application.Entities
{
	public class Booking : EntityBase
	{
		public string UserId { get; set; }
		public Desk Desk { get; set; }
		public DateTimeOffset StartDateTime { get; set; }
		public DateTimeOffset EndDateTime { get; set; }	
	}
}
