using MediatR;
using Microsoft.AspNetCore.Identity;
using OpenDesk.Application.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.Application.Features.Roles
{
	public class CreateRoleCommand : IRequest<RoleDto>
	{
		public string Name { get; set; }
		public string Description { get; set; }
	}

	public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, RoleDto>
	{
		private readonly RoleManager<OpenDeskRole> _roleManager;

		public CreateRoleCommandHandler(RoleManager<OpenDeskRole> roleManager)
		{
			_roleManager = roleManager;
		}

		public async Task<RoleDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
		{
			var role = new OpenDeskRole(request.Name)
			{
				Description = request.Description
			};

			// TODO: What to do about result?
			var result = await _roleManager.CreateAsync(role);

			return new RoleDto
			{
				Id = role.Id,
				Name = role.Name,
				Description = role.Description
			};
		}
	}
}
