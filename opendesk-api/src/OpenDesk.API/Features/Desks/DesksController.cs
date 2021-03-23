using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Desks
{
	[Authorize]
	[Route("api/offices")]
	[ApiController]
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
			var result = await _mediator.Send(new GetDesksCommand(officeId));

			return result.Outcome.IsSuccess ? Ok(result) : BadRequest(result);
		}

		// TODO: Add permissions / roles authentication.
		[HttpPost("{officeId}/desks")]
		public async Task<IActionResult> Create(string officeId, [FromBody]CreateDeskCommand command)
		{
			command.OfficeId = officeId;

			var result = await _mediator.Send(command);

			return result.Outcome.IsSuccess ? Ok(result) : BadRequest(result);
		}

		[HttpDelete("{officeId}/desks/{deskId}")]
		public async Task<IActionResult> Delete(string officeId, string deskId)
		{
			throw new NotImplementedException();
		}
	}
}
