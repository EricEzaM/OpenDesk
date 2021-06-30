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

namespace OpenDesk.API.Features.Offices
{
	public class CreateOfficeCommand : OfficeCommandBase, IRequest<OfficeDTO> { }

	public class CreateOfficeHandler : IRequestHandler<CreateOfficeCommand, OfficeDTO>
	{
		private readonly OpenDeskDbContext _db;
		private readonly IWebHostEnvironment _env;

		public CreateOfficeHandler(OpenDeskDbContext db, IWebHostEnvironment env)
		{
			_db = db;
			_env = env;
		}

		public async Task<OfficeDTO> Handle(CreateOfficeCommand request, CancellationToken cancellationToken)
		{
			// Unify with UpdateOffice
			// TODO Re-write this to make it more secure. https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-5.0
			// Store in blob storage somewhere and replace ImageUrl with one from a storage provider in the Office database model?

			string fName = Guid.NewGuid().ToString();
			string path = Path.Combine(_env.ContentRootPath, "office-images", fName + ".png"); // TODO fix this! dont hardcode png
			using var stream = new FileStream(path, FileMode.Create);
			await request.Image.CopyToAsync(stream, cancellationToken);

			var office = new Office
			{
				Location = request.Location,
				SubLocation = request.SubLocation,
				Name = request.Name,
				ImageUrl = $"https://localhost:5001/office-images/{fName}.png", // TODO fix hardcoded png
			};

			_db.Offices.Add(office);
			await _db.SaveChangesAsync(cancellationToken);

			return new OfficeDTO
			{
				Id = office.Id,
				Location = office.Location,
				SubLocation = office.SubLocation,
				Name = office.Name,
				ImageUrl = office.ImageUrl
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

			RuleFor(o => o.Image)
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