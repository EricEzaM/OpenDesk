using MediatR;
using OpenDesk.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Offices
{
	public class GetOfficeImageCommand : IRequest<string>
	{
		public GetOfficeImageCommand(string officeId)
		{
			OfficeId = officeId;
		}

		public string OfficeId { get; set; }
	}

	public class GetOfficeImageHandler : IRequestHandler<GetOfficeImageCommand, string>
	{
		private readonly OpenDeskDbContext _db;

		public GetOfficeImageHandler(OpenDeskDbContext db)
		{
			_db = db;
		}

		public Task<string> Handle(GetOfficeImageCommand request, CancellationToken cancellationToken)
		{
			var office = _db.Offices
				.FirstOrDefault(o => o.Id == request.OfficeId);

			if (office == null)
			{
				throw new Exception("office not found");
			}

			return Task.FromResult(office.ImageId);
		}
	}
}
