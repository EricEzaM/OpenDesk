using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenDesk.Application.Authorization;
using OpenDesk.Application.Common;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenDesk.Application.Features.Users
{
	[Route("api/users")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IMediator _mediator;

		public UsersController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet]
		[HasPermission(Permissions.Users.Read)]
		public async Task<IActionResult> Get()
		{
			var result = await _mediator.Send(new GetUsersQuery());
			return Ok(result);
		}

		[HttpGet("demos")]
		[AllowAnonymous]
		public async Task<IActionResult> GetDemos()
		{
			var result = await _mediator.Send(new GetDemoUsersQuery());
			return Ok(result);
		}

		[HttpGet("{userId}")]
		[HasPermission(Permissions.Users.Read)]
		public async Task<IActionResult> GetUser(string userId)
		{
			// TODO: this should return a DTO which includes roles (and permissions? need permissions to replace the /me/permissions route).
			var result = await _mediator.Send(new GetUserQuery(userId));
			return Ok(result);
		}

		[HttpPost("{userId}/roles")]
		[HasPermission(Permissions.Users.Edit)]
		public async Task<IActionResult> SetUserRoles(string userId, [FromBody] IEnumerable<string> roleNames)
		{
			var result = await _mediator.Send(new SetUserRolesCommand(userId, roleNames));
			return Ok(result);
		}
	}
}
