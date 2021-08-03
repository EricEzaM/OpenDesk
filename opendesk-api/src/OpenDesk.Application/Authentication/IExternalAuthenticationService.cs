using OpenDesk.Application.Common;
using OpenDesk.Application.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDesk.Application.Authentication
{
	public interface IExternalAuthenticationService
	{
		Task<Result<OpenDeskUser>> Authenticate(ExternalAuthenticationProvider provider, string idToken);
	}
}
