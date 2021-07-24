using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenDesk.Application;
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
		private readonly RoleManager<OpenDeskRole> _roleManager;
		private readonly IUserClaimsPrincipalFactory<OpenDeskUser> _userClaimsPrincipalFactory;

		public IdentityService(
			UserManager<OpenDeskUser> userManager,
			RoleManager<OpenDeskRole> roleManager,
			IUserClaimsPrincipalFactory<OpenDeskUser> userClaimsPrincipalFactory)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_userClaimsPrincipalFactory = userClaimsPrincipalFactory;
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

		public async Task<Result<IEnumerable<UserDTO>>> GetUsersAsync()
		{
			var users = await _userManager.Users.ToListAsync();

			return Result<IEnumerable<UserDTO>>.Success(users.Select(u => new UserDTO
			{
				Id = u.Id,
				UserName = u.UserName,
				DisplayName = u.DisplayName
			}));
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

			return user == null
				? Result<bool>.Failure("Unable to find user")
				: Result<bool>.Success(await _userManager.IsInRoleAsync(user, "Demo"));
		}

		public async Task<IEnumerable<UserDTO>> GetDemoUsers()
		{
			var users = await _userManager.GetUsersInRoleAsync("Demo");

			return users.Select(u => new UserDTO
			{
				Id = u.Id,
				UserName = u.UserName,
				DisplayName = u.DisplayName
			});
		}

		public async Task<Result<IEnumerable<string>>> GetUserRoles(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);

			return user == null
				? Result<IEnumerable<string>>.Failure("Unable to find user")
				: Result<IEnumerable<string>>.Success(await _userManager.GetRolesAsync(user));
		}

		public async Task<Result> AddUserToRoleAsync(string userId, string roleName)
		{
			var user = await _userManager.FindByIdAsync(userId);

			if (user == null)
			{
				return Result.Failure("Unable to find user");
			}

			return (await _userManager.AddToRoleAsync(user, roleName)).ToOpenDeskResult();
		}

		public async Task<Result> SetUserRolesAsync(string userId, IEnumerable<string> roleNames)
		{
			var user = await _userManager.FindByIdAsync(userId);

			if (user == null)
			{
				return Result.Failure("Unable to find user");
			}

			var currentRoles = await _userManager.GetRolesAsync(user);

			var result = await _userManager.RemoveFromRolesAsync(user, currentRoles);

			var nonexistantRoles = new List<string>();
			if (result.Succeeded)
			{
				foreach (var roleName in roleNames)
				{
					bool exists = await _roleManager.RoleExistsAsync(roleName);

					if (exists)
					{
						result = await _userManager.AddToRoleAsync(user, roleName);
						if (!result.Succeeded)
						{
							break;
						}
					}
					else
					{
						nonexistantRoles.Add(roleName);
					}
				}
			}

			if (!result.Succeeded)
			{
				return result.ToOpenDeskResult();
			}
			else if (nonexistantRoles.Any())
			{
				return Result.Failure(nonexistantRoles.Select(rn => $"Role '{rn}' does not exist."));
			}
			else
			{
				return Result.Success();
			}
		}

		public async Task<IEnumerable<RoleDTO>> GetRolesAsync()
		{
			return await _roleManager.Roles.Select(r => new RoleDTO
			{
				Id = r.Id,
				Name = r.Name,
				Description = r.Description
			}).ToListAsync();
		}

		public async Task<Result<IEnumerable<string>>> GetRolePermissions(string roleId)
		{
			var role = await _roleManager.FindByIdAsync(roleId);

			return role == null
				? Result<IEnumerable<string>>.Failure("Unable to find role.")
				: Result<IEnumerable<string>>.Success(
					(await _roleManager.GetClaimsAsync(role))
						.Where(c => c.Type == CustomClaimTypes.Permission)
						.Select(c => c.Value)
					);
		}

		public async Task<Result> SetRolePermissionsAsync(string roleId, IEnumerable<string> permissions)
		{
			var role = await _roleManager.FindByIdAsync(roleId);

			if (role == null)
			{
				return Result.Failure("Unable to find role.");
			}

			var claims = await _roleManager.GetClaimsAsync(role);
			var permissionClaims = claims.Where(c => c.Type == CustomClaimTypes.Permission);

			var tasks = new List<Task<IdentityResult>>();
			foreach (var item in permissionClaims)
			{
				tasks.Add(_roleManager.RemoveClaimAsync(role, item));
			}

			await Task.WhenAll(tasks);
			tasks.Clear();

			foreach (var item in permissions)
			{
				tasks.Add(_roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permission, item)));
			}

			await Task.WhenAll(tasks);

			return Result.Success();
		}
	}
}
