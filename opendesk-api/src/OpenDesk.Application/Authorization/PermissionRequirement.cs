using Microsoft.AspNetCore.Authorization;

namespace OpenDesk.Application.Authorization
{
	/// <summary>
	/// Permission requirement passed to the Authorization Handler and used by the PolicyProvider
	/// </summary>
	public class PermissionRequirement : IAuthorizationRequirement
	{
		public string Permission { get; private set; }

		public PermissionRequirement(string permission)
		{
			Permission = permission;
		}
	}
}
