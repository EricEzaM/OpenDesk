using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDesk.Domain.Entities
{
	public class Office : BaseEntity
	{
		public string Name { get; set; }
		public string ImageUrl { get; set; }
		public List<Desk> Desks { get; set; }
	}
}
