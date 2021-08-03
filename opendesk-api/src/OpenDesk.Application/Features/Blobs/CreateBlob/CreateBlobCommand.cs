using MediatR;
using Microsoft.AspNetCore.Http;
using OpenDesk.Application.Entities;
using OpenDesk.Application.Interfaces;
using OpenDesk.Application.Persistence;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.Application.Features.Blobs
{
	public class CreateBlobCommand : IRequest<BlobDTO>
	{
		// TODO: is the dependency on Microsoft.AspNetCore.Http acceptable?
		public IFormFile File { get; set; }
	}

	public class CreateBlobCommandHandler : IRequestHandler<CreateBlobCommand, BlobDTO>
	{
		private readonly OpenDeskDbContext _db;
		private readonly IBlobSaver _blobSaver;

		public CreateBlobCommandHandler(OpenDeskDbContext db, IBlobSaver blobSaver)
		{
			_db = db;
			_blobSaver = blobSaver;
		}

		public async Task<BlobDTO> Handle(CreateBlobCommand request, CancellationToken cancellationToken)
		{
			// TODO Re-write this to make it more secure. https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-5.0
			using var ms = new MemoryStream();
			await request.File.CopyToAsync(ms, cancellationToken);

			var uri = await _blobSaver.SaveAsync(ms.ToArray(), request.File.FileName, cancellationToken);

			var blob = new Blob
			{
				Uri = uri,
				Expiry = DateTimeOffset.UtcNow.AddDays(1)
			};

			_db.Blobs.Add(blob);
			await _db.SaveChangesAsync(cancellationToken);

			return new BlobDTO
			{
				Id = blob.Id,
				Uri = blob.Uri,
				Expiry = blob.Expiry
			};
		}
	}
}
