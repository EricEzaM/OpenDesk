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

namespace OpenDesk.API.Features.Offices
{
	public class CreateOfficeCommand : IRequest<ApiResponse<OfficeDTO>>
	{
		public string Name { get; set; }
		public IFormFile Image { get; set; }
	}

	public class CreateOfficeHandler : IRequestHandler<CreateOfficeCommand, ApiResponse<OfficeDTO>>
	{
		private readonly OpenDeskDbContext _db;
		private readonly IWebHostEnvironment _env;

		public CreateOfficeHandler(OpenDeskDbContext db, IWebHostEnvironment env)
		{
			_db = db;
			_env = env;
		}

		public async Task<ApiResponse<OfficeDTO>> Handle(CreateOfficeCommand request, CancellationToken cancellationToken)
		{
			// TODO Re-write this to make it more secure. https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-5.0
			// Store in blob storage somewhere and replace ImageUrl with one from a storage provider in the Office database model?

			string fName = Guid.NewGuid().ToString();
			string path = Path.Combine(_env.ContentRootPath, "office-images", fName + ".png"); // TODO fix this! dont hardcode png
			using var stream = new FileStream(path, FileMode.Create);
			await request.Image.CopyToAsync(stream, cancellationToken);

			var size = Image.FromStream(stream).Size;

			var office = new Office
			{
				Name = request.Name,
				Image = new OfficeImage()
				{
					Url = $"https://localhost:5001/office-images/{fName}.png", // TODO fix hardcoded png
					Width = size.Width,
					Height = size.Height
				}
			};

			_db.Offices.Add(office);
			await _db.SaveChangesAsync(cancellationToken);

			return new ApiResponse<OfficeDTO>(new OfficeDTO
			{
				Id = office.Id,
				Name = office.Name,
				Image = office.Image
			});
		}
	}
}
