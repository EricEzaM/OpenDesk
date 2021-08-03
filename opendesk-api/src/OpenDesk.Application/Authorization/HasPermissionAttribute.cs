using Microsoft.AspNetCore.Authorization;
using System;

namespace OpenDesk.Application.Authorization
{

	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
	public class HasPermissionAttribute : AuthorizeAttribute
	{
		/// <summary>
		/// Attribute for checking a permission from the <see cref="Permissions"/ enum.>
		/// </summary>
		/// <param name="permission">The permission to check</param>
		public HasPermissionAttribute(string permission) : base(permission)
		{

		}
	}
}
