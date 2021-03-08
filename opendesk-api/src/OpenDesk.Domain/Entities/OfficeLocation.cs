using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDesk.Domain.Entities
{
	public class OfficeLocation : BaseEntity
	{
		public string Name { get; set; }
		public List<Desk> Desks { get; set; }
	}
}
