using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenDesk.API.Attributes;
using OpenDesk.Application.Common;
using OpenDesk.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Users
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
		public async Task<IActionResult> GetAllUsers()
		{
			var result = await _mediator.Send(new GetUsersCommand());

			return Ok(result);
		}

		[HttpGet("{userId}/roles")]
		[HasPermission(Permissions.Users.Read)]
		public async Task<IActionResult> GetUserRoles(string userId)
		{
			var result = await _mediator.Send(new GetUserRolesCommand(userId));

			return Ok(result);
		}

		[HttpPut("{userId}/roles")]
		[HasPermission(Permissions.Users.Edit)]
		public async Task<IActionResult> SetUserRoles(string userId, SetUserRolesCommand command)
		{
			command.UserId = userId;

			await _mediator.Send(command);

			return NoContent(); 
		}
	}
}
