using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace OpenDesk.Application.Authorization
{
	class PermissionPolicyProvider : IAuthorizationPolicyProvider
	{
		public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; set; }

		public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
		{
			FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
		}

		public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();

		public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();

		public async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
		{
			if (policyName.StartsWith(CustomClaimTypes.Permission, StringComparison.OrdinalIgnoreCase))
			{
				return new AuthorizationPolicyBuilder()
					.AddRequirements(new PermissionRequirement(policyName))
					.Build();
			}

			return await FallbackPolicyProvider.GetPolicyAsync(policyName);
		}
	}
}
