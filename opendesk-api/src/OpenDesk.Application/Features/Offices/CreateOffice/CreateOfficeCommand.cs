using MediatR;
using OpenDesk.Application.Entities;
using OpenDesk.Application.Exceptions;
using OpenDesk.Application.Features.Blobs;
using OpenDesk.Application.Interfaces;
using OpenDesk.Application.Offices.Common;
using OpenDesk.Application.Persistence;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.Application.Features.Offices
{
	public class CreateOfficeCommand : OfficeCommandBase, IRequest<OfficeDto> { }

	public class CreateOfficeHandler : IRequestHandler<CreateOfficeCommand, OfficeDto>
	{
		private readonly OpenDeskDbContext _db;
		private readonly IBlobSaver _blobSaver;

		public CreateOfficeHandler(OpenDeskDbContext db, IBlobSaver blobSaver)
		{
			_db = db;
			_blobSaver = blobSaver;
		}

		public async Task<OfficeDto> Handle(CreateOfficeCommand request, CancellationToken cancellationToken)
		{
			var blob = _db.Blobs
				.FirstOrDefault(b => b.Id == request.ImageBlobId);

			if (blob == null)
			{
				throw new EntityNotFoundException("Blob");
			}

			var office = new Office
			{
				Location = request.Location,
				SubLocation = request.SubLocation,
				Name = request.Name,
				Image = blob
			};

			_db.Offices.Add(office);
			await _db.SaveChangesAsync(cancellationToken);

			return new OfficeDto
			{
				Id = office.Id,
				Location = office.Location,
				SubLocation = office.SubLocation,
				Name = office.Name,
				Image = new BlobDto(blob)
			};
		}
	}
}