using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenDesk.API.Errors;
using OpenDesk.Application.Common.DataTransferObjects;
using OpenDesk.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Blob
{
	public class GetBlobCommand : IRequest<BlobDTO>
	{
		public GetBlobCommand(string id)
		{
			Id = id;
		}

		public string Id { get; set; }
	}

	public class GetBlobHandler : IRequestHandler<GetBlobCommand, BlobDTO>
	{
		private readonly OpenDeskDbContext _db;

		public GetBlobHandler(OpenDeskDbContext db)
		{
			_db = db;
		}

		public async Task<BlobDTO> Handle(GetBlobCommand request, CancellationToken cancellationToken)
		{
			var blob = await _db.Blobs.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken: cancellationToken);

			if (blob == null)
			{
				throw new EntityNotFoundException("Blob");
			}

			return new BlobDTO
			{
				Id = blob.Id,
				Uri = blob.Uri,
				Expiry = blob.Expiry,
			};
		}
	}
}
