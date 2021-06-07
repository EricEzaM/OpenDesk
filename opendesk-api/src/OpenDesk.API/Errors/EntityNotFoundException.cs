using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenDesk.API.Errors
{
	public class EntityNotFoundException : Exception
	{
		public EntityNotFoundException(string entityName)
		{
			EntityName = entityName;
		}

		public string EntityName { get; }
	}
}
