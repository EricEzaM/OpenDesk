using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDesk.Domain.Common
{
	public interface ISoftDeletable
	{
		string DeletedBy { get; }
		DateTimeOffset? DeletedAt { get; }
	}
}
