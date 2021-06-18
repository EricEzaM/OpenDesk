using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using OpenDesk.Domain.Entities;
using OpenDesk.Domain.ValueObjects;
using OpenDesk.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Drawing;
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
			var userEntity = context.Users.Add(new OpenDeskUser("test.user@testuser.com") 
			{
				DisplayName = "Test User",
			});

			var existingFile =
				Path.Combine(
					Path.GetDirectoryName(
						Assembly.GetExecutingAssembly().Location),
					"Persistence",
					"Seed",
					"OfficePlanImage2.png");

			var img = await GetOfficeImage(existingFile, env);

			var n = DateTimeOffset.Now;

			context.Offices.Add(new Office
			{
				Location = "Brisbane, Australia",
				SubLocation = "Queen Street",
				Name = "Office 1",
				Image = new OfficeImage()
				{
					Url = img.Url,
					Width = img.Width,
					Height = img.Height
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
								UserId = userEntity.Entity.Id,
								StartDateTime = new DateTimeOffset(n.Year, n.Month, n.Day, 8, 0, 0, TimeSpan.FromHours(10)),
								EndDateTime = new DateTimeOffset(n.Year, n.Month, n.Day, 18, 0, 0, TimeSpan.FromHours(10))
							},
							new Booking()
							{
								UserId = userEntity.Entity.Id,
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
						DiagramPosition = new DiagramPosition(70, 70)
					}
				}
			});

			await context.SaveChangesAsync();
		}

		public static async Task<OfficeImage> GetOfficeImage(string existingFile, IHostEnvironment env)
		{
			// Get existing
			using var imageFS = new FileStream(existingFile, FileMode.Open);

			// Copy to content path with new filename
			string fName = Guid.NewGuid().ToString();
			string path = Path.Combine(env.ContentRootPath, "office-images", fName + ".png"); // TODO fix this! dont hardcode png
			using var stream = new FileStream(path, FileMode.Create);
			await imageFS.CopyToAsync(stream);

			var size = Image.FromStream(stream);

			// TODO: Fix this so that its not hardcoded
			// return URL
			return new OfficeImage()
			{
				Url = $"https://localhost:5001/office-images/{fName}.png",
				Width = size.Width,
				Height = size.Height
			};
		}
	}
}
