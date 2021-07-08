using MediatR;
using Microsoft.AspNetCore.Http;
using OpenDesk.Application.Common.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenDesk.Domain.Entities;
using OpenDesk.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Drawing;
using OpenDesk.Domain.ValueObjects;
using OpenDesk.API.Models;
using FluentValidation;
using OpenDesk.API.Features.Offices.Shared;
using OpenDesk.Application.Common.Interfaces;
using OpenDesk.API.Errors;

namespace OpenDesk.API.Features.Offices
{
	public class CreateOfficeCommand : OfficeCommandBase, IRequest<OfficeDTO> { }

	public class CreateOfficeHandler : IRequestHandler<CreateOfficeCommand, OfficeDTO>
	{
		private readonly OpenDeskDbContext _db;
		private readonly IWebHostEnvironment _env;
		private readonly IBlobSaver _blobSaver;

		public CreateOfficeHandler(OpenDeskDbContext db, IWebHostEnvironment env, IBlobSaver blobSaver)
		{
			_db = db;
			_env = env;
			_blobSaver = blobSaver;
		}

		public async Task<OfficeDTO> Handle(CreateOfficeCommand request, CancellationToken cancellationToken)
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

			return new OfficeDTO
			{
				Id = office.Id,
				Location = office.Location,
				SubLocation = office.SubLocation,
				Name = office.Name,
				Image = new BlobDTO(blob)
			};
		}
	}

	public class CreateOfficeValidator : AbstractValidator<CreateOfficeCommand>
	{
		private readonly OpenDeskDbContext _db;

		public CreateOfficeValidator(OpenDeskDbContext db)
		{
			_db = db;

			RuleFor(o => o.Location)
				.NotEmpty();

			RuleFor(o => o.SubLocation)
				.NotEmpty();

			RuleFor(o => o.Name)
				.NotEmpty();

			RuleFor(o => o.ImageBlobId)
				.NotEmpty();

			RuleFor(o => o)
				.Must(c => ValidateNoOfficeWithSameDetails(c))
				.WithMessage(o => $"An Office with the name '{o.Name}' already exists with location '{o.Location}' and sublocation '{o.SubLocation}'.");
		}

		private bool ValidateNoOfficeWithSameDetails(OfficeCommandBase command)
		{
			return _db.Offices
				.Any(o => o.Name == command.Name && o.Location == command.Location && o.SubLocation == command.SubLocation) == false;
		}
	}
}