using System;

namespace OpenDesk.Application.Entities
{
	public interface ISoftDeletable
	{
		string DeletedBy { get; }
		DateTimeOffset? DeletedAt { get; }
	}
}
