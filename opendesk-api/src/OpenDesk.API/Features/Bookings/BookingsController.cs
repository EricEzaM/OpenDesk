using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Bookings
{
	[Route("api")]
	public class BookingsController : ControllerBase
	{
		private readonly IMediator _mediator;

		public BookingsController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet("bookings")]
		public async Task<IActionResult> Get()
		{
			var result = await _mediator.Send(new GetBookingsCommand());

			return Ok(result);
		}

		[HttpGet("desks/{deskId}/bookings")]
		public async Task<IActionResult> GetForDesk(string deskId)
		{
			var result = await _mediator.Send(new GetBookingsForDeskCommand(deskId));

			return Ok(result);
		}

		[HttpPost("desks/{deskId}/bookings")]
		public async Task<IActionResult> CreateForDesk(string deskId, [FromBody] CreateBookingCommand command)
		{
			command.DeskId = deskId;

			var result = await _mediator.Send(command);

			return Ok(result);
		}
	}
}
