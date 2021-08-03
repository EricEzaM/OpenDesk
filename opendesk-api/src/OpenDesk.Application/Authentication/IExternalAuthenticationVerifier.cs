using OpenDesk.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDesk.Application.Authentication
{
	public interface IExternalAuthenticationVerifier
	{
		Task<Result<ExternalUserData>> Verify(ExternalAuthenticationProvider provider, string idToken);
	}
}
