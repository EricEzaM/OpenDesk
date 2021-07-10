using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenDesk.API.Models;
using OpenDesk.Application;
using OpenDesk.Application.Common.DataTransferObjects;
using OpenDesk.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Authentication
{
	[Route("api/auth")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private const string MicrosoftAuthName = "microsoft";

		private readonly HttpClient _httpClient;
		private readonly IIdentityService _identityService;

		public AuthController(HttpClient httpClient, IIdentityService identityService)
		{
			_httpClient = httpClient;
			_identityService = identityService;
		}

		[HttpGet(MicrosoftAuthName)]
		public async Task<IActionResult> Authenticate(string returnUrl)
		{
			if (string.IsNullOrEmpty(returnUrl))
			{
				return BadRequest("Return URL cannot be empty.");
			}

			var queryContent = new FormUrlEncodedContent(new Dictionary<string, string>()
				{
					{ "client_id", "961880c5-5302-41fa-9da3-98adada694d9" },
					{ "redirect_uri", Url.ActionLink(nameof(Callback)).ToLower()},
					{ "response_mode", "form_post" },
					{ "response_type", "code" },
					{ "state", returnUrl },
					{ "scope", "openid profile email" },
				});

			var ub = new UriBuilder("https://login.microsoftonline.com/common/oauth2/v2.0/authorize");
			ub.Query = await queryContent.ReadAsStringAsync();

			return Redirect(ub.Uri.AbsoluteUri);
		}

		[HttpPost(MicrosoftAuthName + "/callback")]
		public async Task<IActionResult> Callback([FromForm] MicrosoftAuthRequestModel authModel)
		{
			// Construct form content
			var content = new FormUrlEncodedContent(new Dictionary<string, string>()
				{
					{ "code", authModel.Code },
					{ "redirect_uri", Url.ActionLink()},
					{ "grant_type", "authorization_code" },
					{ "client_id", "961880c5-5302-41fa-9da3-98adada694d9" },
					{ "client_secret", "8Ta.WbRj_2o81mH~Q_Bd6_~4v~4qvz7I8-" }, // TODO: Remove hard coded secret, generate a new one.
					{ "scope", "openid profile email" },
				});

			// Send request
			var res = await _httpClient.SendAsync(new HttpRequestMessage
			{
				RequestUri = new Uri("https://login.microsoftonline.com/common/oauth2/v2.0/token"),
				Method = HttpMethod.Post,
				Content = content
			});

			if (res.IsSuccessStatusCode == false)
			{
				return BadRequest("Something went wrong. Please try again.");
			}

			var tokens = await res.Content.ReadFromJsonAsync<TokenReturnModel>();
			var idToken = new JwtSecurityTokenHandler().ReadJwtToken(tokens.IdToken);

			// Can use custom tenant filtering logic here
			// << Tenant Filtering Logic here >>

			var getUserResult = await _identityService.GetUserAsync(MicrosoftAuthName, idToken.Subject);
			var userId = getUserResult.Value.Id;

			if (getUserResult.Succeeded == false)
			{
				// No user yet, so create one
				var username = idToken.Claims.FirstOrDefault(c => c.Type == "email").Value;
				var createResult = await _identityService.CreateUserAsync(username);

				if (createResult.Succeeded)
				{
					// Creation Success, add external login info
					var addLoginResult = await _identityService.AddUserLoginAsync(createResult.Value, MicrosoftAuthName, idToken.Subject, MicrosoftAuthName);

					if (addLoginResult.Succeeded == false)
					{
						return BadRequest("Could not add external login information to user.");
					}

					userId = createResult.Value;
					var display = idToken.Claims.FirstOrDefault(c => c.Type == "name").Value;
					await _identityService.SetDisplayNameAsync(createResult.Value, display);
				}
				else
				{
					return BadRequest($"Could not create user with username '{username}'");
				}
			}

			var cpResult = await _identityService.GetUserClaimsPrincipal(userId);

			if (cpResult.Succeeded)
			{
				await HttpContext.SignInAsync(cpResult.Value);
			}

			return Redirect(authModel.State);
		}

		[HttpPost("signout")]
		public IActionResult SignOutUser()
		{
			return SignOut();
		}

		[HttpGet("/api/me")] // Don't use the controller prefix for this route.
		[Authorize]
		public IActionResult GetUser()
		{
			return Ok(new UserDTO()
			{
				Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
				UserName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
				DisplayName = User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.DisplayName)?.Value,
			});
		}

		[HttpPost("demos/{userId}")]
		public async Task<IActionResult> LogInDemoUser(string userId)
		{
			var demoResult = await _identityService.GetUserIsDemo(userId);

			if (!demoResult.Value)
			{
				return BadRequest("Unable to log in given user.");
			}

			var cpResult = await _identityService.GetUserClaimsPrincipal(userId);

			if (cpResult.Succeeded)
			{
				await HttpContext.SignInAsync(cpResult.Value);
				return NoContent();
			}
			else
			{
				return BadRequest("Unable to log in given user.");
			}
		}

		[HttpGet("demos")]
		public async Task<IActionResult> GetDemoUsers()
		{
			var users = await _identityService.GetDemoUsers();

			return Ok(users);
		}
	}
}
