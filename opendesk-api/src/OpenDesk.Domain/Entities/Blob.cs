using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDesk.Domain.Entities
{
	public class Blob
	{
		public string Id { get; set; }
		public string Uri { get; set; }
		public DateTimeOffset Expiry { get; set; }
	}
}
