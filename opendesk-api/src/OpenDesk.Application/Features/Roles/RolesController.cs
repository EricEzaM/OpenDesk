using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenDesk.Application.Authorization;
using OpenDesk.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDesk.Application.Features.Roles
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
		public async Task<IActionResult> Get()
		{
			var result = await _mediator.Send(new GetRolesQuery());
			return Ok(result);
		}

		[HttpGet("{roleId}/permissions")]
		[HasPermission(Permissions.Roles.Read)]
		public async Task<IActionResult> GetPermissions(string roleId)
		{
			var result = await _mediator.Send(new GetRolePermissionsQuery(roleId));
			return Ok(result);
		}

		[HttpPost("{roleId}/permissions")]
		[HasPermission(Permissions.Roles.Edit.Permissions)]
		public async Task<IActionResult> SetPermissions(string roleId, [FromBody] IEnumerable<string> permissions)
		{
			var result = await _mediator.Send(new SetRolePermissionsCommand(roleId, permissions));
			return Ok(result);
		}

		[HttpPost]
		[HasPermission(Permissions.Roles.Create)]
		public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand command)
		{
			var result = await _mediator.Send(command);
			return Ok(result);
		}

		[HttpPost("{roleId}")]
		[HasPermission(Permissions.Roles.Edit.Metadata)]
		public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleCommand command)
		{
			var result = await _mediator.Send(command);
			return Ok(result);
		}
	}
}
