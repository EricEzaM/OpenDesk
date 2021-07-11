using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDesk.Application.Common
{
	public class ApplicationOptions
	{
		public const string SECTION_NAME = "Application";

		public bool IsDemo { get; set; }
		public string SuperAdminEmail { get; set; }
	}
}
