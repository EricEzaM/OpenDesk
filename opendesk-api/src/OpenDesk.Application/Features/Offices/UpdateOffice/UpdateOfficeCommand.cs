using MediatR;
using OpenDesk.Application.Exceptions;
using OpenDesk.Application.Features.Blobs;
using OpenDesk.Application.Offices.Common;
using OpenDesk.Application.Persistence;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.Application.Features.Offices
{
	public class UpdateOfficeCommand : OfficeCommandBase, IRequest<OfficeDto>
	{
		public string Id { get; set; }
	}

	public class UpdateOfficeHandler : IRequestHandler<UpdateOfficeCommand, OfficeDto>
	{
		private readonly OpenDeskDbContext _db;

		public UpdateOfficeHandler(OpenDeskDbContext db)
		{
			_db = db;
		}

		public async Task<OfficeDto> Handle(UpdateOfficeCommand request, CancellationToken cancellationToken)
		{
			var office = _db.Offices.FirstOrDefault(o => o.Id == request.Id);
			if (office == null)
			{
				throw new EntityNotFoundException("Office");
			}

			var blob = _db.Blobs.FirstOrDefault(b => b.Id == request.ImageBlobId);
			if (office == null)
			{
				throw new EntityNotFoundException("Blob");
			}

			office.Name = request.Name;
			office.Location = request.Location;
			office.SubLocation = request.SubLocation;
			office.Image = blob;

			_db.Offices.Update(office);
			await _db.SaveChangesAsync(cancellationToken);

			return new OfficeDto
			{
				Id = office.Id,
				Location = office.Location,
				SubLocation = office.SubLocation,
				Name = office.Name,
				Image = new BlobDto(office.Image)
			};
		}
	}
}
