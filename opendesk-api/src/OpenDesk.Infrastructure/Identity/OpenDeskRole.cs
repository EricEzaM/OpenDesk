using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDesk.Infrastructure.Identity
{
	public class OpenDeskRole : IdentityRole<string>
	{
		public string Description { get; set; }

		public OpenDeskRole(string name) : base(name)
		{
			Id = Guid.NewGuid().ToString();
		}

		public OpenDeskRole(string name, string description) : this(name)
		{
			Description = description;
		}
	}
}
