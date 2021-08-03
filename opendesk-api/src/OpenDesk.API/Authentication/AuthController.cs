using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenDesk.Application;
using OpenDesk.Application.Authentication;
using OpenDesk.Application.Common;
using OpenDesk.Application.Features.Users;
using OpenDesk.Application.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OpenDesk.API.Authentication
{
	[Route("api/auth")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IMediator _mediator;
		private readonly IExternalAuthenticationService _externalAuthService;
		private readonly IOptions<ApplicationOptions> _options;

		public AuthController(IMediator mediator, IExternalAuthenticationService externalAuthService, IOptions<ApplicationOptions> options)
		{
			_externalAuthService = externalAuthService;
			_mediator = mediator;
			_options = options;
		}

		[AllowAnonymous]
		[HttpPost("external")]
		public async Task<IActionResult> ExternalSignIn([FromBody] ExternalLoginDto loginDto)
		{
			var authResult = await _externalAuthService.Authenticate(loginDto.Provider, loginDto.IdToken);

			if (authResult.Succeeded)
			{
				var cp = await _mediator.Send(new GetUserClaimsPrincipalQuery(authResult.Value.Id));
				await HttpContext.SignInAsync(cp);
			}
			else
			{
				return Unauthorized("Unable to authenticate via external provider.");
			}

			return NoContent();
		}

		[HttpPost("signout")]
		public IActionResult SignOutUser()
		{
			return SignOut();
		}

		// TODO is this route any good? nothing else uses /me... maybe it should be changed.
		[HttpGet("/api/me/permissions")]
		public IActionResult GetUserPermissions()
		{
			return Ok(User.Claims.Where(c => c.Type == CustomClaimTypes.Permission).Select(c => c.Value));
		}

		[AllowAnonymous]
		[HttpPost("demos/{userId}")]
		public async Task<IActionResult> LogInDemoUser(string userId)
		{
			if (!_options.Value.IsDemo)
			{
				throw new ApplicationException("Application is not in demo mode.");
			}

			var demos = await _mediator.Send(new GetDemoUsersQuery());

			if (!demos.Any(u => u.Id == userId))
			{
				return BadRequest("Unable to log in given user.");
			}

			var cpResult = await _mediator.Send(new GetUserClaimsPrincipalQuery(userId));

			if (cpResult != null)
			{
				await HttpContext.SignInAsync(cpResult);
				return NoContent();
			}
			else
			{
				return BadRequest("Unable to log in given user.");
			}
		}
	}
}
