using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDesk.Application.Authentication
{
	public class AuthenticationOptions
	{
		public const string SECTION_NAME = "Authentication";

		public ExternalAuth External { get; set; }

		public class ExternalAuth
		{
			public ExternalAuthSettings Microsoft { get; set; }
		}

		public class ExternalAuthSettings
		{
			public string ClientId { get; set; }
			public string TenantId { get; set; }
		}
	}
}
