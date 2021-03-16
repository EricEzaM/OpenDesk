using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenDesk.API.Models;
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
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private const string MicrosoftAuthName = "Microsoft";

		private readonly HttpClient _httpClient;
		private readonly IIdentityService _identityService;

		public AuthController(HttpClient httpClient, IIdentityService identityService)
		{
			this._httpClient = httpClient;
			this._identityService = identityService;
		}

		[HttpPost(MicrosoftAuthName)]
		public async Task<IActionResult> Authenticate([FromForm]MicrosoftAuthRequestModel authModel)
		{
			// Construct form content
			var content = new FormUrlEncodedContent(new Dictionary<string, string>()
				{
					{ "code", authModel.Code },
					{ "redirect_uri", Url.ActionLink().ToLower()},
					{ "grant_type", "authorization_code" },
					{ "client_id", "961880c5-5302-41fa-9da3-98adada694d9" },
					{ "client_secret", "8Ta.WbRj_2o81mH~Q_Bd6_~4v~4qvz7I8-" }, // TODO: Remove hard coded secret, generate a new one.
				});

			// Build URI
			var query = new FormUrlEncodedContent(new Dictionary<string, string>()
				{
					{ "scope", "openid profile email" },
				});
			var ub = new UriBuilder("https://login.microsoftonline.com/common/oauth2/v2.0/token");
			ub.Query = query.ReadAsStringAsync().Result;

			// Send request
			var res = await _httpClient.SendAsync(new HttpRequestMessage
			{
				RequestUri = ub.Uri,
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

			var (result, userId) = await _identityService.GetUserIdAsync(MicrosoftAuthName, idToken.Subject);

			if (result.Succeeded == false)
			{
				// No user yet, so create one
				var username = idToken.Claims.FirstOrDefault(c => c.Type == "email").Value;
				(result, userId) = await _identityService.CreateUserAsync(username);

				if (result.Succeeded)
				{
					// Creation Success, add external login info
					result = await _identityService.AddUserLoginAsync(userId, MicrosoftAuthName, idToken.Subject, MicrosoftAuthName);

					if (result.Succeeded == false)
					{
						return BadRequest("Could not add external login information to user.");
					}
				}
				else
				{
					return BadRequest($"Could not create user with username '{username}'");
				}
			}

			var (cpResult, cp) = await _identityService.GetUserClaimsPrincipal(userId);

			if (cpResult.Succeeded)
			{
				await HttpContext.SignInAsync(cp);
			}

			return Redirect(authModel.State);
		}

		[HttpGet("test")]
		[Authorize]
		public IActionResult Test()
		{
			return new JsonResult("Yes");
		}
	}
}
