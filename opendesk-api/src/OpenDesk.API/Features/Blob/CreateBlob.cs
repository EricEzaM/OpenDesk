using MediatR;
using Microsoft.AspNetCore.Http;
using OpenDesk.Application.Common.DataTransferObjects;
using OpenDesk.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenDesk.Application.Common.Interfaces;

namespace OpenDesk.API.Features.Blob
{
	public class CreateBlobCommand: IRequest<BlobDTO>
	{
		public IFormFile File { get; set; }
	}

	public class CreateBlobHandler : IRequestHandler<CreateBlobCommand, BlobDTO>
	{
		private readonly OpenDeskDbContext _db;
		private readonly IBlobSaver _blobSaver;

		public CreateBlobHandler(OpenDeskDbContext db, IBlobSaver blobSaver)
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

			var blob = new Domain.Entities.Blob
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
