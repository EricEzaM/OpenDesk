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
		Task<(Result Result, string UserId)> CreateUserAsync(string userName);
		Task<(Result Result, ClaimsPrincipal ClaimsPrincipal)> GetUserClaimsPrincipal(string userId);
		Task<(Result Result, string UserId)> GetUserIdAsync(string userName);
		Task<(Result Result, string UserId)> GetUserIdAsync(string loginProvider, string providerKey);
		Task<string> GetUserNameAsync(string userId);
	}
}
