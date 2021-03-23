using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Offices
{
	[Authorize]
	[Route("api/offices")]
	[ApiController]
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

		// TODO: Add permissions / roles authentication.
		[HttpPost]
		public async Task<IActionResult> Create(CreateOfficeCommand command)
		{
			var result = await _mediator.Send(command);
			return Ok(result);
		}
	}
}
