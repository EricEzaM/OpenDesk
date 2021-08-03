using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.Application.Interfaces
{
	public interface IBlobSaver
	{
		Task<string> SaveAsync(byte[] bytes, string fileName, CancellationToken cancellationToken = default);
	}
}
