using OpenDesk.Application.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenDesk.API.Authentication
{
	public class ExternalLoginDto
	{
		public ExternalAuthenticationProvider Provider { get; set; }
		public string IdToken { get; set; }
	}
}
