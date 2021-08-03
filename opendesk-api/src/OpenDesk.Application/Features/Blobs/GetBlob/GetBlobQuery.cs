using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenDesk.Application.Exceptions;
using OpenDesk.Application.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.Application.Features.Blobs
{
	public class GetBlobQuery : IRequest<BlobDTO>
	{
		public GetBlobQuery(string id)
		{
			Id = id;
		}

		public string Id { get; set; }
	}

	public class GetBlobQueryHandler : IRequestHandler<GetBlobQuery, BlobDTO>
	{
		private readonly OpenDeskDbContext _db;

		public GetBlobQueryHandler(OpenDeskDbContext db)
		{
			_db = db;
		}

		public async Task<BlobDTO> Handle(GetBlobQuery request, CancellationToken cancellationToken)
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
