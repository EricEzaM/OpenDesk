using MediatR;
using Microsoft.AspNetCore.Identity;
using OpenDesk.Application.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDesk.Application.Features.Roles
{
	public class UpdateRoleCommand : IRequest<Unit>
	{
		public UpdateRoleCommand(string roleId, string name, string description)
		{
			RoleId = roleId;
			Name = name;
			Description = description;
		}

		public string RoleId { get; }
		public string Name { get; }
		public string Description { get; }
	}

	public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Unit>
	{
		private readonly RoleManager<OpenDeskRole> _roleManager;

		public UpdateRoleCommandHandler(RoleManager<OpenDeskRole> roleManager)
		{
			_roleManager = roleManager;
		}

		public async Task<Unit> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
		{
			var role = await _roleManager.FindByIdAsync(request.RoleId);

			role.Name = request.Name;
			role.Description = request.Description;

			// TODO do anything with the result?
			var result = await _roleManager.UpdateAsync(role);

			return Unit.Value;
		}
	}
}