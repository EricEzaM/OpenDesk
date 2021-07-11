using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenDesk.API.Attributes
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
