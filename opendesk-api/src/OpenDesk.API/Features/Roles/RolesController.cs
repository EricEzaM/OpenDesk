using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenDesk.API.Attributes;
using OpenDesk.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Roles
{
	[Route("api/roles")]
	[ApiController]
	public class RolesController : ControllerBase
	{
		private readonly IMediator _mediator;

		public RolesController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet]
		[HasPermission(Permissions.Roles.Read)]
		public async Task<IActionResult> GetRoles()
		{
			var result = await _mediator.Send(new GetRolesCommand());

			return Ok(result);
		}

		[HttpGet("{roleId}/permissions")]
		[HasPermission(Permissions.Roles.Read)]
		public async Task<IActionResult> GetRolePermissions(string roleId)
		{
			var result = await _mediator.Send(new GetRolePermissionsCommand(roleId));

			return Ok(result);
		}

		[HttpPut("{roleId}/permissions")]
		[HasPermission(Permissions.Roles.Edit.Permissions)]
		public async Task<IActionResult> SetRolePermissions(string roleId, [FromBody] SetRolePermissionsCommand command)
		{
			command.RoleId = roleId;
			await _mediator.Send(command);

			return NoContent();
		}
	}
}
