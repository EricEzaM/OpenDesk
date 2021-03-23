﻿using MediatR;
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

			if (result.IsValidResponse)
			{
				return Ok(result.Result);
			}
			else
			{
				return BadRequest(result.Errors);
			}

		}

		[HttpGet("/api/users/{userId}/bookings")]
		public async Task<IActionResult> GetForSignInUser(string userId)
		{
			var signedInUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

			if (signedInUserId == null)
			{
				return BadRequest(new
				{
					Error = "User not found."
				});
			}

			if (signedInUserId != userId)
			{
				// Only signed in users can find their own records.
				// TODO: Admin users (or users with some permission) can access these.
				return NotFound();
			}

			var result = await _mediator.Send(new GetBookingsForUserCommand(userId));

			return Ok(result);
		}
	}
}
