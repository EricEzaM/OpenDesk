using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Office
{
	[Route("api/offices")]
	public class OfficesController : ControllerBase
	{
		private readonly IMediator _mediator;

		public OfficesController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet]
		public async Task<IActionResult> Get()
		{
			var result = await _mediator.Send(new GetOfficesCommand());

			return Ok(result);
		}
	}
}
