using OpenDesk.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.Application.Common.Interfaces
{
	public interface IBlobSaver
	{
		Task<string> SaveAsync(byte[] bytes, string fileName, CancellationToken cancellationToken = default);
	}
}
