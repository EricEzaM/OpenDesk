using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDesk.Domain.Entities
{
	public class Booking : BaseEntity
	{
		public string UserId { get; set; }
		public Desk Desk { get; set; }
		public DateTime StartDateTime { get; set; }
		public DateTime EndDateTime { get; set; }

	}
}
