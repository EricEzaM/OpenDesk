using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Desks
{
	[Route("api/offices")]
	public class DesksController : ControllerBase
	{
		private readonly IMediator _mediator;

		public DesksController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet("{officeId}/desks")]
		public async Task<IActionResult> Get(string officeId)
		{
			var result = await _mediator.Send(new GetDesks.Command(officeId));

			return Ok(result);
		}

		[HttpPost("{officeId}/desks")]
		public async Task<IActionResult> Create(string officeId, [FromBody]CreateDesk.Command command)
		{
			command.OfficeId = officeId;

			var result = await _mediator.Send(command);

			return Ok(result);
		}

		[HttpDelete("{officeId}/desks/{deskId}")]
		public async Task<IActionResult> Delete(string officeId, string deskId)
		{
			return Ok();
		}
	}
}
