using Microsoft.EntityFrameworkCore;
using OpenDesk.Domain.Entities;
using OpenDesk.Domain.ValueObjects;
using OpenDesk.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDesk.Infrastructure.Persistence
{
	public static class OpenDeskDbContextSeed
	{
		public static async Task SeedDefaults(OpenDeskDbContext context)
		{
			var userEntity = context.Users.Add(new OpenDeskUser("test.user@testuser.com"));

			context.Offices.Add(new Office
			{
				Name = "Office 1",
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

			context.Offices.Add(new Office
			{
				Name = "Office 2",
				Desks = new List<Desk>()
				{
					new Desk()
					{
						Name = "Desk 1",
						DiagramPosition = new DiagramPosition(100, 100)
					},
					new Desk()
					{
						Name = "Desk 2",
						DiagramPosition = new DiagramPosition(150, 150)
					},
					new Desk()
					{
						Name = "Desk 3",
						DiagramPosition = new DiagramPosition(270, 270)
					}
				}
			});

			await context.SaveChangesAsync();
		}
	}
}
