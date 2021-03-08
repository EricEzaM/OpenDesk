using OpenDesk.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDesk.Domain.Entities
{
	public class Desk : BaseEntity
	{
		public string Name { get; set; }
		public DiagramPosition DiagramPosition { get; set; }
		public OfficeLocation OfficeLocation { get; set; }
		public List<Booking> Bookings { get; set; }
	}
}
