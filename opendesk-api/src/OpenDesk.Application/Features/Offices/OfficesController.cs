using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenDesk.Application.Authorization;
using OpenDesk.Application.Common;
using System.Threading.Tasks;

namespace OpenDesk.Application.Features.Offices
{
	[Route("api/offices")]
	[ApiController]
	public class OfficesController : ControllerBase
	{
		private readonly IMediator _mediator;

		public OfficesController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet]
		[HasPermission(Permissions.Offices.Read)]
		public async Task<IActionResult> Get()
		{
			var result = await _mediator.Send(new GetOfficesQuery());

			return Ok(result);
		}

		[HttpPost]
		[HasPermission(Permissions.Offices.Create)]
		public async Task<IActionResult> Create([FromBody] CreateOfficeCommand command)
		{
			var result = await _mediator.Send(command);
			return Ok(result);
		}

		[HttpPut("{officeId}")]
		[HasPermission(Permissions.Offices.Edit)]
		public async Task<IActionResult> Update(string officeId, [FromBody] UpdateOfficeCommand command)
		{
			command.Id = officeId;
			var result = await _mediator.Send(command);
			return Ok(result);
		}
	}
}
