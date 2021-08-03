using System;

namespace OpenDesk.Application.Exceptions
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
