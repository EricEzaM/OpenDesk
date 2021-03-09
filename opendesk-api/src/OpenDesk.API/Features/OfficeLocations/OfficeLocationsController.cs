using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.OfficeLocations
{
	[Route("/api/offices")]
	public class OfficeLocationsController : ControllerBase
	{
		private readonly IMediator _mediator;

		public OfficeLocationsController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet("/")]
		public async Task<IActionResult> Get()
		{
			var result = await _mediator.Send(new GetOfficeLocationsQuery());

			return Ok(result);
		}

		[HttpGet("/{officeLocationId}/desks")]
		public async Task<IActionResult> GetDesks(string officeLocationId)
		{
			var result = await _mediator.Send(new GetOfficeLocationDesksQuery(officeLocationId));

			return Ok(result);
		}
	}
}
