using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Offices
{
	[Route("api/offices")]
	public class OfficesController : ControllerBase
	{
		private readonly IMediator _mediator;
		private readonly IWebHostEnvironment _env;

		public OfficesController(IMediator mediator, IWebHostEnvironment env)
		{
			_mediator = mediator;
			_env = env;
		}

		[HttpGet]
		public async Task<IActionResult> Get()
		{
			var result = await _mediator.Send(new GetOfficesCommand());

			return Ok(result);
		}

		[HttpPost]
		public async Task<IActionResult> Create(CreateOfficeCommand command)
		{
			var result = await _mediator.Send(command);
			return Ok(result);
		}

		[HttpGet("{officeId}/image")]
		public async Task<IActionResult> GetImage(string officeId)
		{
			var imageId = await _mediator.Send(new GetOfficeImageCommand(officeId));

			FileStream stream = System.IO.File.Open(
				Path.Combine(_env.ContentRootPath, "office-images", imageId + ".png"),
				FileMode.Open,
				FileAccess.Read);

			// The below closes the stream for us once it's finished sending!
			return File(stream, "image/png");	
		}
	}
}
