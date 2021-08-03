using MediatR;
using OpenDesk.Application.Common;
using OpenDesk.Application.Features.Users;
using OpenDesk.Application.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDesk.Application.Authentication
{
	public class ExternalAuthenticationService : IExternalAuthenticationService
	{
		private readonly IExternalAuthenticationVerifier _verifier;
		private readonly IMediator _mediator;

		public ExternalAuthenticationService(IExternalAuthenticationVerifier verifier, IMediator mediator)
		{
			_verifier = verifier;
			_mediator = mediator;
		}

		public async Task<Result<OpenDeskUser>> Authenticate(ExternalAuthenticationProvider provider, string idToken)
		{
			var verificationResult = await _verifier.Verify(provider, idToken);

			if (!verificationResult.Succeeded)
			{
				return Result<OpenDeskUser>.Failure(verificationResult.Errors);
			}

			var extUserData = verificationResult.Value;

			// Create new user if required.
			var user = await _mediator.Send(new GetUserByExternalLoginQuery(provider.ToString(), extUserData.Subject));

			if (user == null)
			{
				user = await _mediator.Send(new CreateUserCommand(extUserData.Email, extUserData.Displayname));

				if (user == null)
				{
					// TODO fix
					throw new Exception("bad user");
				}

				await _mediator.Send(new AddUserExternalLoginCommand(user.Id, provider.ToString(), extUserData.Subject));
			}

			return Result<OpenDeskUser>.Success(user);
		}
	}
}
