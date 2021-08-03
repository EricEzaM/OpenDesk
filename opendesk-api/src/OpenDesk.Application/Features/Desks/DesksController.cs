using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenDesk.Application.Authorization;
using OpenDesk.Application.Common;
using System;
using System.Threading.Tasks;

namespace OpenDesk.Application.Features.Desks
{
	[Authorize]
	[Route("api")]
	[ApiController]
	public class DesksController : ControllerBase
	{
		private readonly IMediator _mediator;

		public DesksController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet("offices/{officeId}/desks")]
		[HasPermission(Permissions.Desks.Read)]
		public async Task<IActionResult> Get(string officeId)
		{
			var result = await _mediator.Send(new GetDesksQuery(officeId));

			return Ok(result);
		}

		[HttpPost("offices/{officeId}/desks")]
		[HasPermission(Permissions.Desks.Create)]
		public async Task<IActionResult> Create(string officeId, [FromBody] CreateDeskCommand command)
		{
			command.OfficeId = officeId;

			var result = await _mediator.Send(command);

			return Ok(result);
		}

		[HttpPut("desks/{deskId}")]
		[HasPermission(Permissions.Desks.Edit)]
		public async Task<IActionResult> Update(string deskId, [FromBody] UpdateDeskCommand command)
		{
			command.DeskId = deskId;
			var result = await _mediator.Send(command);
			return Ok(result);
		}

		[HttpDelete("{officeId}/desks/{deskId}")]
		[HasPermission(Permissions.Desks.Delete)]
		public async Task<IActionResult> Delete(string officeId, string deskId)
		{
			throw new NotImplementedException();
		}
	}
}
