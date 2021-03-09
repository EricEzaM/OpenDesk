using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDesk.Application.Common.DataTransferObjects
{
	public class BookingDTO
	{
		public string Id { get; set; }
		public string UserId { get; set; }
		public string DeskId { get; set; }
		public DateTimeOffset StartDateTime { get; set; }
		public DateTimeOffset EndDateTime { get; set; }
	}
}
