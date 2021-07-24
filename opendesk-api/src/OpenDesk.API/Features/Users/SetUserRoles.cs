using FluentValidation;
using MediatR;
using OpenDesk.Application.Common.DataTransferObjects;
using OpenDesk.Application.Common.Interfaces;
using OpenDesk.Application.Common.Models;
using OpenDesk.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.API.Features.Users
{
	public class SetUserRolesCommand : IRequest<Unit>
	{
		public string UserId { get; set; }
		public List<string> Roles { get; set; }
	}
	
	public class SetUserRolesHandler : IRequestHandler<SetUserRolesCommand>
	{
		private readonly IIdentityService _identityService;

		public SetUserRolesHandler(IIdentityService identityService)
		{
			_identityService = identityService;
		}

		public async Task<Unit> Handle(SetUserRolesCommand request, CancellationToken cancellationToken)
		{
			await _identityService.SetUserRolesAsync(request.UserId, request.Roles);
			return Unit.Value;
		}
	}

	public class SetUserRolesValidator : AbstractValidator<SetUserRolesCommand>
	{
		private readonly OpenDeskDbContext _db;

		public SetUserRolesValidator(OpenDeskDbContext db)
		{
			_db = db;

			RuleFor(rc => rc.UserId)
				.NotEmpty();

			RuleFor(rc => rc.Roles)
				.ForEach(r => r.NotEmpty()
					.Must(r => ValidateRoleExists(r))
					.WithMessage("Role {PropertyValue} does not exist."));
		}

		private bool ValidateRoleExists(string roleName)
		{
			return _db.Roles.Any(r => r.Name == roleName);
		}
	}
}
