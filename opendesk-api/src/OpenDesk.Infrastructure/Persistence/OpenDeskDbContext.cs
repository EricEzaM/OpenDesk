using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenDesk.Application.Common.Interfaces;
using OpenDesk.Domain.Entities;
using OpenDesk.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.Infrastructure.Persistence
{
	public class OpenDeskDbContext : IdentityDbContext<OpenDeskUser, IdentityRole, string>, IOpenDeskDbContext
	{
		public OpenDeskDbContext(DbContextOptions options) : base(options)
		{

		}

		public DbSet<Desk> Desks { get; set; }
		public DbSet<Booking> Bookings { get; set; }
		public DbSet<Office> Offices { get; set; }
		public DbSet<Blob> Blobs { get; set; }

		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
		{
			// Do any AuditableEntity stuff here
			// https://github.com/jasontaylordev/CleanArchitecture/blob/main/src/Infrastructure/Persistence/ApplicationDbContext.cs

			return base.SaveChangesAsync(cancellationToken);
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

			base.OnModelCreating(builder);
		}
	}
}
