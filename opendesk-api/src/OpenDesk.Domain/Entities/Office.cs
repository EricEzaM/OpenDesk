using OpenDesk.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDesk.Domain.Entities
{
	public class Office : BaseEntity
	{
		public string Location { get; set; }
		public string SubLocation { get; set; }
		public string Name { get; set; }
		public List<Desk> Desks { get; set; }
		public Blob Image { get; set; }
	}
}
