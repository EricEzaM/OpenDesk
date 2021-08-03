using OpenDesk.Application.ValueObjects;
using System.Collections.Generic;

namespace OpenDesk.Application.Entities
{
	public class Desk : EntityBase
	{
		public string Name { get; set; }
		public DiagramPosition DiagramPosition { get; set; }
		public Office Office { get; set; }
		public List<Booking> Bookings { get; set; }
	}
}
