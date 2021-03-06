using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenDesk.Application.Authorization;
using OpenDesk.Application.Common;
using OpenDesk.Application.Exceptions;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OpenDesk.Application.Features.Bookings
{
	[Authorize]
	[Route("api")]
	[ApiController]
	public class BookingsController : ControllerBase
	{
		private readonly IMediator _mediator;

		public BookingsController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet("bookings")]
		[HasPermission(Permissions.Bookings.Read)]
		public async Task<IActionResult> Get()
		{
			var result = await _mediator.Send(new GetBookingsQuery());

			return Ok(result);
		}

		[HttpGet("desks/{deskId}/bookings")]
		[HasPermission(Permissions.Bookings.Read)]
		public async Task<IActionResult> GetForDesk(string deskId)
		{
			var result = await _mediator.Send(new GetBookingsForDeskQuery(deskId));

			return Ok(result);
		}

		[HttpPost("desks/{deskId}/bookings")]
		[HasPermission(Permissions.Bookings.Create)]
		public async Task<IActionResult> CreateForDesk(string deskId, [FromBody] CreateBookingCommand command)
		{
			command.DeskId = deskId;
			var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

			if (userId == null)
			{
				throw new EntityNotFoundException("User");
			}

			command.UserId = userId;
			var result = await _mediator.Send(command);

			return Ok(result);
		}

		[HttpGet("/api/users/{userId}/bookings")]
		[HasPermission(Permissions.Bookings.Read)]
		public async Task<IActionResult> GetForSignInUser(string userId)
		{
			var signedInUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

			if (signedInUserId == null)
			{
				throw new EntityNotFoundException("User");
			}

			if (signedInUserId != userId)
			{
				// Only signed in users can find their own records.
				// TODO: Admin users (or users with some permission) can access these.
				return NotFound();
			}

			var result = await _mediator.Send(new GetBookingsForUserQuery(userId));

			return Ok(result);
		}

		[HttpGet("offices/{officeId}/bookings")]
		[HasPermission(Permissions.Bookings.Read)]
		public async Task<IActionResult> GetForOffice(string officeId)
		{
			var result = await _mediator.Send(new GetBookingsForOfficeQuery(officeId));

			return Ok(result);
		}
	}
}
