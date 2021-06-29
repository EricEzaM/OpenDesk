using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Offices.Shared
{
	public class OfficeCommandBase
	{
		public string Location { get; set; }
		public string SubLocation { get; set; }
		public string Name { get; set; }
		public IFormFile Image { get; set; }
	}
}
