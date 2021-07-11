using OpenDesk.Application.Common.DataTransferObjects;
using OpenDesk.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OpenDesk.Application.Common.Interfaces
{
	public interface IIdentityService
	{
		Task<Result> AddUserLoginAsync(string userId, string loginProvider, string providerKey, string providerDisplayName);
		Task<Result<string>> CreateUserAsync(string userName);
		Task<Result<ClaimsPrincipal>> GetUserClaimsPrincipal(string userId);
		Task<Result<UserDTO>> GetUserAsync(string loginProvider, string providerKey);
		Task<Result> SetDisplayNameAsync(string userId, string displayName);
		Task<Result<bool>> GetUserIsDemo(string userId);
		Task<IEnumerable<UserDTO>> GetDemoUsers();
		Task<Result> AddUserToRoleAsync(string userId, string roleName);
	}
}
