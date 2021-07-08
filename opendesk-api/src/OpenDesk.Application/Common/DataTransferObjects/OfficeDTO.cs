using OpenDesk.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDesk.Application.Common.DataTransferObjects
{
	public class OfficeDTO
	{
		public string Id { get; set; }
		public string Location { get; set; }
		public string SubLocation { get; set; }
		public string Name { get; set; }
		public BlobDTO Image { get; set;}
	}
}
