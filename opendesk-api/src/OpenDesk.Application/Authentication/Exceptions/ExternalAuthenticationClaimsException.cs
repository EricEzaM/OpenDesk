using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDesk.Application.Authentication.Exceptions
{
	class ExternalAuthenticationClaimsException : Exception
	{
		public ExternalAuthenticationClaimsException(IEnumerable<string> missingClaims) 
			: base($"External provider id token is missing required claims: {string.Join(", ", missingClaims)}")
		{ }
	}
}
