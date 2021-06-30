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

	public class UpdatedOffice
	{
		public string Id { get; set; }
		public string Location { get; set; }
		public string SubLocation { get; set; }
		public string Name { get; set; }
	}

	public class UpdateOfficeHandler : IRequestHandler<UpdateOfficeCommand, OfficeDTO>
	{
		private readonly OpenDeskDbContext _db;
		private readonly IWebHostEnvironment _env;
		private readonly IValidator<UpdatedOffice> _validator;

		public UpdateOfficeHandler(OpenDeskDbContext db, IWebHostEnvironment env, IValidator<UpdatedOffice> validator)
		{
			_db = db;
			_env = env;
			_validator = validator;
		}

		public async Task<OfficeDTO> Handle(UpdateOfficeCommand request, CancellationToken cancellationToken)
		{
			// Unify with CreateOffice
			// TODO Re-write this to make it more secure. https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-5.0
			// Store in blob storage somewhere and replace ImageUrl with one from a storage provider in the Office database model?

			var office = _db.Offices.FirstOrDefault(o => o.Id == request.Id);
			if (office == null)
			{
				throw new EntityNotFoundException("Office");
			}

			// Hacked-together "patch" implementation, but it works...
			var updatedOffice = GetUpdatedOffice(request, office);
			if (updatedOffice != null)
			{
				_validator.ValidateAndThrow(updatedOffice);

				office.Name = updatedOffice.Name;
				office.Location = updatedOffice.Location;
				office.SubLocation = updatedOffice.SubLocation;
			}

			if (request.Image != null)
			{
				string fName = Guid.NewGuid().ToString();
				string path = Path.Combine(_env.ContentRootPath, "office-images", fName + ".png"); // TODO fix this! dont hardcode png
				using var stream = new FileStream(path, FileMode.Create);
				await request.Image.CopyToAsync(stream, cancellationToken);

				office.ImageUrl = $"https://localhost:5001/office-images/{fName}.png"; // TODO fix hardcoded png
			}

			_db.Offices.Update(office);
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

		/// <summary>
		/// Gets an object representing the new state of the office details.
		/// </summary>
		/// <returns><see cref="UpdateOffice "/> instance if any details are different, otherwise null.</returns>
		private UpdatedOffice GetUpdatedOffice(UpdateOfficeCommand command, Office existingOffice)
		{
			var nameChanged = !string.IsNullOrWhiteSpace(command.Name) && command.Name != existingOffice.Name;
			var locationChanged = !string.IsNullOrWhiteSpace(command.Location) && command.Location != existingOffice.Location;
			var subLocationChanged = !string.IsNullOrWhiteSpace(command.Name) && command.Name != existingOffice.Name;

			if (nameChanged || locationChanged || subLocationChanged)
			{
				return new UpdatedOffice()
				{
					Id = existingOffice.Id,
					Name = nameChanged ? command.Name : existingOffice.Name,
					Location = locationChanged ? command.Location : existingOffice.Location,
					SubLocation = subLocationChanged ? command.SubLocation : existingOffice.SubLocation
				};
			}
			else
			{
				return null;
			}
		}
	}

	public class UpdatedOfficeValidator : AbstractValidator<UpdatedOffice>
	{
		private readonly OpenDeskDbContext _db;

		public UpdatedOfficeValidator(OpenDeskDbContext db)
		{
			_db = db;

			RuleFor(o => o)
				.Must(c => ValidateNoOfficeWithSameDetails(c))
				.WithMessage(o => $"An Office with the name '{o.Name}' already exists with location '{o.Location}' and sublocation '{o.SubLocation}'.");
		}

		private bool ValidateNoOfficeWithSameDetails(UpdatedOffice office)
		{
			return _db.Offices
				.Any(o => o.Name == office.Name && o.Location == office.Location && o.SubLocation == office.SubLocation) == false;
		}
	}
}
