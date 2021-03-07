using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenDesk.API.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OpenDesk.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private const string MicrosoftAuthName = "Microsoft";

		private readonly HttpClient httpClient;
		private readonly UserManager<IdentityUser> userManager;
		private readonly IUserClaimsPrincipalFactory<IdentityUser> userClaimsPrincipalFactory;

		public AuthController(HttpClient httpClient, UserManager<IdentityUser> userManager, IUserClaimsPrincipalFactory<IdentityUser> userClaimsPrincipalFactory)
		{
			this.httpClient = httpClient;
			this.userManager = userManager;
			this.userClaimsPrincipalFactory = userClaimsPrincipalFactory;
		}

		[HttpPost(MicrosoftAuthName)]
		public async Task<IActionResult> Authenticate(MicrosoftAuthRequestModel authModel)
		{
			// Construct form content
			var content = new FormUrlEncodedContent(new Dictionary<string, string>()
				{
					{ "code", authModel.Code },
					{ "redirect_uri", authModel.RedirectUri},
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
			var res = await httpClient.SendAsync(new HttpRequestMessage
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

			var user = await userManager.FindByLoginAsync(MicrosoftAuthName, idToken.Subject);

			if (user == null)
			{
				user = new IdentityUser(idToken.Claims.FirstOrDefault(c => c.Type == "email").Value);
				await userManager.CreateAsync(user);
				await userManager.AddLoginAsync(user, new UserLoginInfo(MicrosoftAuthName, idToken.Subject, MicrosoftAuthName));
			}

			var cp = await userClaimsPrincipalFactory.CreateAsync(user);
			await HttpContext.SignInAsync(cp);

			return NoContent();
		}

		[HttpGet("test")]
		[Authorize]
		public IActionResult Test()
		{
			return new JsonResult("Yes");
		}
	}
}
