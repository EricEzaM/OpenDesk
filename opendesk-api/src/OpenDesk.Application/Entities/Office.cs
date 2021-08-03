using System.Collections.Generic;

namespace OpenDesk.Application.Entities
{
	public class Office : EntityBase
	{
		public string Location { get; set; }
		public string SubLocation { get; set; }
		public string Name { get; set; }
		public List<Desk> Desks { get; set; }
		public Blob Image { get; set; }
	}
}
