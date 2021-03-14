using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using OpenDesk.Domain.Entities;
using OpenDesk.Domain.ValueObjects;
using OpenDesk.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenDesk.Infrastructure.Persistence
{
	public static class OpenDeskDbContextSeed
	{
		public static async Task SeedDefaults(OpenDeskDbContext context, IHostEnvironment env)
		{
			var userEntity = context.Users.Add(new OpenDeskUser("test.user@testuser.com"));

			var existingFile =
				Path.Combine(
					Path.GetDirectoryName(
						Assembly.GetExecutingAssembly().Location),
					"Persistence",
					"Seed",
					"OfficePlanImage2.png");

			var url = await GetImageUrl(existingFile, env);

			context.Offices.Add(new Office
			{
				Name = "Office 1",
				ImageUrl = url,
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
								UserId = userEntity.Entity.Id,
								StartDateTime = DateTimeOffset.Now.AddDays(1).AddHours(-3),
								EndDateTime = DateTimeOffset.Now.AddDays(1).AddHours(2)
							},
							new Booking()
							{
								UserId = userEntity.Entity.Id,
								StartDateTime = DateTimeOffset.Now.AddDays(1).AddHours(3),
								EndDateTime = DateTimeOffset.Now.AddDays(1).AddHours(5)
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
						DiagramPosition = new DiagramPosition(70, 70)
					}
				}
			});

			await context.SaveChangesAsync();
		}

		public static async Task<string> GetImageUrl(string existingFile, IHostEnvironment env)
		{
			// Get existing
			using var imageFS = new FileStream(existingFile, FileMode.Open);

			// Copy to content path with new filename
			string fName = Guid.NewGuid().ToString();
			string path = Path.Combine(env.ContentRootPath, "office-images", fName + ".png"); // TODO fix this! dont hardcode png
			using var stream = new FileStream(path, FileMode.Create);
			await imageFS.CopyToAsync(stream);

			// TODO: Fix this so that its not hardcoded
			// return URL
			return $"https://localhost:5001/office-images/{fName}.png";
		}
	}
}
