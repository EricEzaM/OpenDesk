using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDesk.Domain.Common
{
	public interface IAuditableEntity
	{
		string CreatedBy { get; }
		DateTimeOffset CreatedAt { get; }
		string LastModifiedBy { get; }
		DateTimeOffset? LastModifiedAt { get; }
	}
}
