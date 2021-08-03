using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDesk.Application.Authentication.Exceptions
{
	public class ExternalAuthenticationSetupException : Exception
	{
		public ExternalAuthenticationSetupException(string provider)
			: base($"External provider {provider} has been misconfigured.")
		{ }
	}
}
