using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using OpenDesk.Application.Authentication.Exceptions;
using OpenDesk.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OpenDesk.Application.Authentication
{
	public class ExternalAuthenticationVerifier : IExternalAuthenticationVerifier
	{
		private readonly IOptions<AuthenticationOptions> _authOptions;

		public ExternalAuthenticationVerifier(IOptions<AuthenticationOptions> authOptions)
		{
			_authOptions = authOptions;
		}

		public async Task<Result<ExternalUserData>> Verify(ExternalAuthenticationProvider provider, string idToken)
			=> provider switch
			{
				ExternalAuthenticationProvider.Microsoft => await AuthenticateMicrosoftToken(idToken),
				_ => throw new ArgumentException($"Support for provider '{provider}' is not implemented.")
			};

		private async Task<Result<ExternalUserData>> AuthenticateMicrosoftToken(string idToken)
		{
			var clientId = _authOptions.Value.External.Microsoft.ClientId;
			var tenantId = _authOptions.Value.External.Microsoft.TenantId;

			if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(tenantId))
			{
				throw new ExternalAuthenticationSetupException("Microsoft");
			}

			// Get Open ID config for tenantId
			var metadataAddress = $"https://login.microsoftonline.com/{tenantId}/v2.0/.well-known/openid-configuration";
			var cm = new ConfigurationManager<OpenIdConnectConfiguration>(metadataAddress, new OpenIdConnectConfigurationRetriever());
			var openIdConfig = await cm.GetConfigurationAsync();

			// Set up validation based on the open ID configuration and then validate.
			var jsonWebTokenHandler = new JsonWebTokenHandler();
			var validationParams = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
			{
				ValidAudience = clientId,
				ValidIssuer = openIdConfig.Issuer,
				IssuerSigningKeys = openIdConfig.SigningKeys
			};
			var validationResult = jsonWebTokenHandler.ValidateToken(idToken, validationParams);

			if (validationResult.IsValid)
			{
				var email = validationResult.ClaimsIdentity.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
				var sub = validationResult.ClaimsIdentity.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
				var name = validationResult.ClaimsIdentity.Claims.FirstOrDefault(c => c.Type == "name")?.Value;

				if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(sub) || string.IsNullOrWhiteSpace(name))
				{
					var missingClaims = new List<string>();
					if (string.IsNullOrWhiteSpace(email)) missingClaims.Add("email");
					if (string.IsNullOrWhiteSpace(sub)) missingClaims.Add("sub");
					if (string.IsNullOrWhiteSpace(name)) missingClaims.Add("name");

					throw new ExternalAuthenticationClaimsException(missingClaims);
				}

				var userData = new ExternalUserData
				{
					Subject = sub,
					Email = email,
					Displayname = name
				};

				return Result<ExternalUserData>.Success(userData);
			}
			else
			{
				return Result<ExternalUserData>.Failure("External token validation failed.");
			}
		}
	}
}
