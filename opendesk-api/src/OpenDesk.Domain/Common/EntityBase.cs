using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDesk.Domain.Common
{
	public abstract class EntityBase : IEntity, ISoftDeletable, IAuditableEntity
	{
		public string Id { get; private set; }

		public string DeletedBy { get; private set; }

		public DateTimeOffset? DeletedAt { get; private set; }

		public string CreatedBy { get; private set; }

		public DateTimeOffset CreatedAt { get; private set; }

		public string LastModifiedBy { get; private set; }

		public DateTimeOffset? LastModifiedAt { get; private set; }
	}
}
