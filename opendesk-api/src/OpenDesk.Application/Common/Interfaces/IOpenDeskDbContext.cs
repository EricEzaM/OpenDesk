using Microsoft.EntityFrameworkCore;
using OpenDesk.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.Application.Common.Interfaces
{
	public interface IOpenDeskDbContext
	{
		DbSet<Desk> Desks { get; set; }
		DbSet<Booking> Bookings { get; set; }
		DbSet<Office> Offices { get; set; }
		DbSet<Blob> Blobs { get; set; }

		Task<int> SaveChangesAsync(CancellationToken cancellationToken);
	}
}
