using Microsoft.AspNetCore.Identity;
using OpenDesk.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDesk.Infrastructure.Identity
{
	public class OpenDeskUser : IdentityUser<string>
	{
		// Add additional user properties if required.
		public OpenDeskUser(string userName) : base(userName)
		{
			Id = Guid.NewGuid().ToString();
		}

		public string DisplayName { get; set; }
		public List<Booking> Bookings { get; set; }
	}
}
