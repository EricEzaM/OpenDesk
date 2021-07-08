using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OpenDesk.API.Behaviours;
using OpenDesk.API.Errors;
using OpenDesk.API.Features.Offices.Shared;
using OpenDesk.Application.Common.DataTransferObjects;
using OpenDesk.Domain.Entities;
using OpenDesk.Domain.ValueObjects;
using OpenDesk.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Offices
{
	public class UpdateOfficeCommand : OfficeCommandBase, IRequest<OfficeDTO> 
	{
		public string Id { get; set; }
	}

	public class UpdateOfficeHandler : IRequestHandler<UpdateOfficeCommand, OfficeDTO>
	{
		private readonly OpenDeskDbContext _db;

		public UpdateOfficeHandler(OpenDeskDbContext db)
		{
			_db = db;
		}

		public async Task<OfficeDTO> Handle(UpdateOfficeCommand request, CancellationToken cancellationToken)
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

			return new OfficeDTO
			{
				Id = office.Id,
				Location = office.Location,
				SubLocation = office.SubLocation,
				Name = office.Name,
				Image = new BlobDTO(office.Image)
			};
		}
	}

	public class UpdatedOfficeValidator : AbstractValidator<UpdateOfficeCommand>
	{
		private readonly OpenDeskDbContext _db;

		public UpdatedOfficeValidator(OpenDeskDbContext db)
		{
			_db = db;

			RuleFor(o => o)
				.Must(c => ValidateNoOfficeWithSameDetails(c))
				.WithMessage(o => $"An Office with the name '{o.Name}' already exists with location '{o.Location}' and sublocation '{o.SubLocation}'.");
		}

		private bool ValidateNoOfficeWithSameDetails(UpdateOfficeCommand office)
		{
			return _db.Offices
				.Any(o => o.Name == office.Name && o.Location == office.Location && o.SubLocation == office.SubLocation) == false;
		}
	}
}
