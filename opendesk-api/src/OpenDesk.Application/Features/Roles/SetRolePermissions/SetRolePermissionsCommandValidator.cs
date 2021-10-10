using FluentValidation;
using Microsoft.AspNetCore.Identity;
using OpenDesk.Application.Common;
using OpenDesk.Application.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDesk.Application.Features.Roles
{
	class SetRolePermissionsCommandValidator : AbstractValidator<SetRolePermissionsCommand>
	{
		private readonly RoleManager<OpenDeskRole> _roleManager;

		public SetRolePermissionsCommandValidator(RoleManager<OpenDeskRole> roleManager)
		{
			_roleManager = roleManager;

			RuleFor(p => p.RoleId)
				.MustAsync((command, _, _) => ValidateRoleExists(command))
				.WithMessage("Role does not exist.")
				.MustAsync((command, _, _) => ValidateNotChangingSuperAdmin(command))
				.WithMessage("SuperAdmin cannot have permissions changed.")
				.NotEmpty();
		}

		private async Task<bool> ValidateRoleExists(SetRolePermissionsCommand command)
		{
			var role = await _roleManager.FindByIdAsync(command.RoleId);

			return role != null;
		}

		private async Task<bool> ValidateNotChangingSuperAdmin(SetRolePermissionsCommand command)
		{
			var role = await _roleManager.FindByIdAsync(command.RoleId);

			return role.Name != RoleStrings.SuperAdmin;
		}
	}
}
