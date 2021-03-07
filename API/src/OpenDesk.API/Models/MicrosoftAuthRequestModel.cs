using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenDesk.API.Models
{
	public class MicrosoftAuthRequestModel
	{
		public string Code { get; set; }
		public string RedirectUri { get; set; }
	}
}
