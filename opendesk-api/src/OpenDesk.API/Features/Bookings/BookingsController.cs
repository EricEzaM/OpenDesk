using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Bookings
{
	[Authorize]
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
			var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

			if (userId == null)
			{
				return BadRequest(new
				{
					Error = "User not found."
				});
			}

			command.UserId = userId;
			var result = await _mediator.Send(command);

			return Ok(result);
		}

		[HttpGet("/api/me/bookings")]
		public async Task<IActionResult> GetForSignInUser()
		{
			var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

			if (userId == null)
			{
				return BadRequest(new
				{
					Error = "Could not find user"
				});
			}

			var result = await _mediator.Send(new GetBookingsForUserCommand(userId));

			return Ok(result);
		}
	}
}
