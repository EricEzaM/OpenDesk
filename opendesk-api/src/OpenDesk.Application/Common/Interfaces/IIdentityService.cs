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
		Task<Result<ClaimsPrincipal>> GetUserClaimsPrincipalAsync(string userId);
		Task<Result<UserDTO>> GetUserAsync(string loginProvider, string providerKey);
		Task<Result<IEnumerable<UserDTO>>> GetUsersAsync();
		Task<Result> SetDisplayNameAsync(string userId, string displayName);
		Task<Result<bool>> GetUserIsDemoAsync(string userId);
		Task<IEnumerable<UserDTO>> GetDemoUsersAsync();
		Task<IEnumerable<RoleDTO>> GetRolesAsync();
		Task<Result<IEnumerable<string>>> GetUserRolesAsync(string userId);
		Task<Result> AddUserToRoleAsync(string userId, string roleName);
		Task<Result> SetUserRolesAsync(string userId, IEnumerable<string> roleNames);
		Task<Result<IEnumerable<string>>> GetRolePermissionsAsync(string roleId);
		Task<Result> SetRolePermissionsAsync(string roleId, IEnumerable<string> permissions);
	}
}
