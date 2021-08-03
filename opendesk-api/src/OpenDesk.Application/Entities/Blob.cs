using System;

namespace OpenDesk.Application.Entities
{
	public class Blob : EntityBase
	{
		public string Uri { get; set; }
		public DateTimeOffset Expiry { get; set; }
	}
}
