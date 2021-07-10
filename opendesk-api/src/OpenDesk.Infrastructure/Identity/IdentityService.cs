using Microsoft.AspNetCore.Identity;
using OpenDesk.Application.Common.DataTransferObjects;
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
		private readonly UserManager<OpenDeskUser> _userManager;
		private readonly IUserClaimsPrincipalFactory<OpenDeskUser> _userClaimsPrincipalFactory;

		public IdentityService(
			UserManager<OpenDeskUser> userManager,
			IUserClaimsPrincipalFactory<OpenDeskUser> userClaimsPrincipalFactory)
		{
			this._userManager = userManager;
			this._userClaimsPrincipalFactory = userClaimsPrincipalFactory;
		}

		public async Task<Result> AddUserLoginAsync(string userId, string loginProvider, string providerKey, string providerDisplayName)
		{
			var user = await _userManager.FindByIdAsync(userId);

			if (user == null)
			{
				return Result.Failure("Could not find user.");
			}

			var result = await _userManager.AddLoginAsync(user, new UserLoginInfo(loginProvider, providerKey, providerDisplayName));

			return result.ToOpenDeskResult();
		}

		public async Task<Result<string>> CreateUserAsync(string userName)
		{
			var user = new OpenDeskUser(userName);

			var result = await _userManager.CreateAsync(user);

			return result.ToOpenDeskResult(user.Id);
		}

		public async Task<Result<ClaimsPrincipal>> GetUserClaimsPrincipal(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);

			if (user == null)
			{
				return Result<ClaimsPrincipal>.Failure("Could not find user.");
			}

			return Result<ClaimsPrincipal>.Success(await _userClaimsPrincipalFactory.CreateAsync(user));
		}

		public async Task<Result<UserDTO>> GetUserAsync(string loginProvider, string providerKey)
		{
			var user = await _userManager.FindByLoginAsync(loginProvider, providerKey);

			return user == null
				? Result<UserDTO>.Failure()
				: Result<UserDTO>.Success(new UserDTO
				{
					Id = user.Id,
					UserName = user.UserName,
					DisplayName = user.DisplayName
				});
		}

		public async Task<Result> SetDisplayNameAsync(string userId, string displayName)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return Result.Failure("Could not find user.");
			}

			user.DisplayName = displayName;
			await _userManager.UpdateAsync(user);

			return Result.Success();
		}

		public async Task<Result<bool>> GetUserIsDemo(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);

			bool isDemo = await _userManager.IsInRoleAsync(user, "demo");

			return user == null
				? Result<bool>.Failure()
				: Result<bool>.Success(isDemo);
		}

		public async Task<IEnumerable<UserDTO>> GetDemoUsers()
		{
			var users = await _userManager.GetUsersInRoleAsync("demo");

			return users.Select(u => new UserDTO
			{
				Id = u.Id,
				UserName = u.UserName,
				DisplayName	= u.DisplayName
			});
		}
	}
}
