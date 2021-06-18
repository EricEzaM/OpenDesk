using Microsoft.AspNetCore.Identity;
using OpenDesk.Application.Common.Interfaces;
using OpenDesk.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OpenDesk.Infrastructure.Identity
{
	public class IdentityService : IIdentityService
	{
		private readonly UserManager<OpenDeskUser> userManager;
		private readonly IUserClaimsPrincipalFactory<OpenDeskUser> userClaimsPrincipalFactory;

		public IdentityService(
			UserManager<OpenDeskUser> userManager,
			IUserClaimsPrincipalFactory<OpenDeskUser> userClaimsPrincipalFactory)
		{
			this.userManager = userManager;
			this.userClaimsPrincipalFactory = userClaimsPrincipalFactory;
		}

		public async Task<Result> AddUserLoginAsync(string userId, string loginProvider, string providerKey, string providerDisplayName)
		{
			var user = await userManager.FindByIdAsync(userId);

			if (user == null)
			{
				return Result.Failure("Could not find user.");
			}

			var result = await userManager.AddLoginAsync(user, new UserLoginInfo(loginProvider, providerKey, providerDisplayName));

			return result.ToOpenDeskResult();
		}

		public async Task<(Result Result, string UserId)> CreateUserAsync(string userName)
		{
			var user = new OpenDeskUser(userName);

			var result = await userManager.CreateAsync(user);

			return (result.ToOpenDeskResult(), user.Id);
		}

		public async Task<(Result Result, ClaimsPrincipal ClaimsPrincipal)> GetUserClaimsPrincipal(string userId)
		{
			var user = await userManager.FindByIdAsync(userId);

			if (user == null)
			{
				return (Result.Failure("Could not find user."), null);
			}

			return (Result.Success(), await userClaimsPrincipalFactory.CreateAsync(user));
		}

		public async Task<(Result Result, string UserId)> GetUserIdAsync(string userName)
		{
			var user = await userManager.FindByNameAsync(userName);

			return user == null
				? (Result.Failure(), string.Empty)
				: (Result.Success(), user.Id);
		}

		public async Task<(Result Result, string UserId)> GetUserIdAsync(string loginProvider, string providerKey)
		{
			var user = await userManager.FindByLoginAsync(loginProvider, providerKey);

			return user == null
				? (Result.Failure(), string.Empty)
				: (Result.Success(), user.Id);
		}

		public async Task<string> GetUserNameAsync(string userId)
		{
			var user = await userManager.FindByIdAsync(userId);

			return user.UserName;
		}

		public async Task<string> GetDisplayNameAsync(string userId)
		{
			var user = await userManager.FindByIdAsync(userId);

			return user.DisplayName;
		}

		public async Task<Result> SetDisplayNameAsync(string userId, string displayName)
		{
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return Result.Failure("Could not find user.");
			}

			user.DisplayName = displayName;
			await userManager.UpdateAsync(user);

			return Result.Success();
		}
	}
}
