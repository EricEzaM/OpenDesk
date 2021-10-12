using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OpenDesk.Application.Common;
using OpenDesk.Application.Entities;
using OpenDesk.Application.Identity;
using OpenDesk.Application.Interfaces;
using OpenDesk.Application.ValueObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OpenDesk.Application.Persistence
{
	public static class OpenDeskDbContextSeed
	{
		/// <summary>
		/// Creates SuperAdmin and Member roles and assigns their permissions.
		/// </summary>
		public static async Task SeedDefaultsAsync(IServiceProvider serviceProvider)
		{
			var rm = serviceProvider.GetRequiredService<RoleManager<OpenDeskRole>>();

			// SuperAdmin
			var superAdminRole = new OpenDeskRole(RoleStrings.SuperAdmin, "Super Admin! Nice.");
			var result = await rm.CreateAsync(superAdminRole);

			if (result.Succeeded)
			{
				foreach (var permissionString in PermissionHelper.GetPermissionsFromClass(typeof(Permissions)))
				{
					await rm.AddClaimAsync(superAdminRole, new Claim(CustomClaimTypes.Permission, permissionString));
				}
			}

			// Member
			var member = new OpenDeskRole(RoleStrings.Member, "Member role, hehe");
			result = await rm.CreateAsync(member);

			if (result.Succeeded)
			{
				await rm.AddClaimAsync(member, new Claim(CustomClaimTypes.Permission, Permissions.Bookings.Read));
				await rm.AddClaimAsync(member, new Claim(CustomClaimTypes.Permission, Permissions.Desks.Read));
				await rm.AddClaimAsync(member, new Claim(CustomClaimTypes.Permission, Permissions.Offices.Read));
				await rm.AddClaimAsync(member, new Claim(CustomClaimTypes.Permission, Permissions.Blobs.Read));
			}
		}

		public static async Task SeedDemoAsync(IServiceProvider serviceProvider)
		{
			await SeedDefaultsAsync(serviceProvider);

			// Get Services
			var ctx = serviceProvider.GetRequiredService<OpenDeskDbContext>();
			var um = serviceProvider.GetRequiredService<UserManager<OpenDeskUser>>();
			var rm = serviceProvider.GetRequiredService<RoleManager<OpenDeskRole>>();

			var bs = serviceProvider.GetRequiredService<IBlobSaver>();

			var demo = new OpenDeskRole(RoleStrings.Demo, "Demo users.");
			var demoResult = await rm.CreateAsync(demo);

			var deskManagerRole = new OpenDeskRole("DeskManager", "Manager of desks.");
			var deskManagerResult = await rm.CreateAsync(deskManagerRole);

			if (deskManagerResult.Succeeded)
			{
				foreach (var permissionString in PermissionHelper.GetPermissionsFromClass(typeof(Permissions.Desks)))
				{
					await rm.AddClaimAsync(deskManagerRole, new Claim(CustomClaimTypes.Permission, permissionString));
				}

				await rm.AddClaimAsync(deskManagerRole, new Claim(CustomClaimTypes.Permission, Permissions.Bookings.Read));
				await rm.AddClaimAsync(deskManagerRole, new Claim(CustomClaimTypes.Permission, Permissions.Offices.Read));
				await rm.AddClaimAsync(deskManagerRole, new Claim(CustomClaimTypes.Permission, Permissions.Blobs.Read));
			}

			var superAdmin = await SeedSuperAdminDemoAsync(um, rm);
			var deskManager = await SeedDeskManagerUserDemoAsync(um, rm);
			var memberUser = await SeedMemberUserDemoAsync(um, rm);

			var existingOfficeImagePath =
				Path.Combine(
					Path.GetDirectoryName(
						Assembly.GetExecutingAssembly().Location),
					"Persistence",
					"Seed",
					"OfficePlanImage2.png");
			var dbOfficeImagePath = await GetOfficeImagePathAsync(existingOfficeImagePath, bs);

			var n = DateTimeOffset.Now;

			ctx.Offices.Add(new Office
			{
				Location = "Brisbane, Australia",
				SubLocation = "Queen Street",
				Name = "Office 1",
				Image = new Blob
				{
					Uri = dbOfficeImagePath,
					Expiry = DateTimeOffset.UtcNow.AddDays(1)
				},
				Desks = new List<Desk>()
				{
					new Desk()
					{
						Name = "Desk 1",
						DiagramPosition = new DiagramPosition(10, 10),
						Bookings = new List<Booking>()
						{
							new Booking()
							{
								UserId = memberUser.Id,
								StartDateTime = new DateTimeOffset(n.Year, n.Month, n.Day, 8, 0, 0, TimeSpan.FromHours(10)),
								EndDateTime = new DateTimeOffset(n.Year, n.Month, n.Day, 18, 0, 0, TimeSpan.FromHours(10))
							},
							new Booking()
							{
								UserId = memberUser.Id,
								StartDateTime = new DateTimeOffset(n.Year, n.Month, n.Day, 8, 0, 0, TimeSpan.FromHours(10)).AddDays(2),
								EndDateTime = new DateTimeOffset(n.Year, n.Month, n.Day, 18, 0, 0, TimeSpan.FromHours(10)).AddDays(3)
							}
						}
					},
					new Desk()
					{
						Name = "Desk 2",
						DiagramPosition = new DiagramPosition(50, 50)
					},
					new Desk()
					{
						Name = "Desk 3",
						DiagramPosition = new DiagramPosition(70, 180)
					}
				}
			});

			await ctx.SaveChangesAsync();
		}

		public static async Task<string> GetOfficeImagePathAsync(string existingFile, IBlobSaver env)
		{
			var bytes = File.ReadAllBytes(existingFile);

			if (bytes.Length == 0)
			{
				throw new Exception("Could not load existing file " + existingFile);
			}

			return await env.SaveAsync(bytes, existingFile);
		}

		private static async Task<OpenDeskUser> SeedSuperAdminDemoAsync(UserManager<OpenDeskUser> um, RoleManager<OpenDeskRole> rm)
		{
			var user = new OpenDeskUser("superadmin@opendeskdemo.com", "SuperAdmin Demo");
			await um.CreateAsync(user);
			await um.AddToRoleAsync(user, RoleStrings.SuperAdmin);
			await um.AddToRoleAsync(user, RoleStrings.Demo);

			return user;
		}

		private static async Task<OpenDeskUser> SeedMemberUserDemoAsync(UserManager<OpenDeskUser> um, RoleManager<OpenDeskRole> rm)
		{
			var user = new OpenDeskUser("member@opendeskdemo.com", "Member Demo");
			await um.CreateAsync(user);
			await um.AddToRoleAsync(user, RoleStrings.Member);
			await um.AddToRoleAsync(user, RoleStrings.Demo);

			return user;
		}

		private static async Task<OpenDeskUser> SeedDeskManagerUserDemoAsync(UserManager<OpenDeskUser> um, RoleManager<OpenDeskRole> rm)
		{
			var user = new OpenDeskUser("deskmanager@opendeskdemo.com", "Desk Manager Demo");
			await um.CreateAsync(user);
			await um.AddToRoleAsync(user, "DeskManager");
			await um.AddToRoleAsync(user, RoleStrings.Demo);

			return user;
		}
	}
}
