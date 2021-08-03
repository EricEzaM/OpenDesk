using System;

namespace OpenDesk.Application.Entities
{
	public interface IAuditableEntity
	{
		string CreatedBy { get; }
		DateTimeOffset CreatedAt { get; }
		string LastModifiedBy { get; }
		DateTimeOffset? LastModifiedAt { get; }
	}
}
