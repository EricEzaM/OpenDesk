using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDesk.Application.Common.DataTransferObjects
{
	public class FullBookingDTO
	{

		public string Id { get; set; }
		public DateTimeOffset StartDateTime { get; set; }
		public DateTimeOffset EndDateTime { get; set; }
		public OfficeDTO Office { get; set; }
		public DeskDTO Desk { get; set; }
		public UserDTO User { get; set; }
	}
}
