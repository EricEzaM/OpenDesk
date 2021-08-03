using Microsoft.Extensions.Hosting;
using OpenDesk.Application.Interfaces;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.Application.Persistence
{
	public class LocalFileBlobSaver : IBlobSaver
	{
		private readonly IHostEnvironment _env;

		public LocalFileBlobSaver(IHostEnvironment env)
		{
			_env = env;
		}

		public async Task<string> SaveAsync(byte[] bytes, string fileName, CancellationToken cancellationToken = default)
		{
			string extension = Path.GetExtension(fileName);
			string file = Guid.NewGuid().ToString() + extension;
			string folder = Path.Combine(_env.ContentRootPath, "static", "blobs");

			string path = Path.Combine(folder, file);
			using var stream = new FileStream(path, FileMode.Create);

			await stream.WriteAsync(bytes, cancellationToken);

			return $"https://localhost:5001/static/blobs/{file}";
		}
	}
}
