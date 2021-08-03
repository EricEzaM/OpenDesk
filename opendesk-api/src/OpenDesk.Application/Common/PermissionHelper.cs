using System;
using System.Collections.Generic;
using System.Reflection;

namespace OpenDesk.Application.Common
{
	public class PermissionHelper
	{
		public static List<string> GetPermissionsFromClass(Type permissionClass)
		{
			var results = new List<string>();
			GetNestedPermissions(permissionClass, ref results);
			return results;
		}

		private static List<string> GetNestedPermissions(Type type, ref List<string> results)
		{
			// Get any permissions which reside in the provided class.
			results.AddRange(GetPermissionsInClass(type));

			// And check nested classes too.
			var types = type.GetNestedTypes(BindingFlags.Public | BindingFlags.Static);
			foreach (var nestedType in types)
			{
				GetNestedPermissions(nestedType, ref results);
			}

			return results;
		}

		private static List<string> GetPermissionsInClass(Type type)
		{
			var results = new List<string>();
			var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
			foreach (var field in fields)
			{
				// Don't display obsolete permissions.
				if (field.GetCustomAttribute<ObsoleteAttribute>() != null)
				{
					continue;
				}

				results.Add(field.GetValue(null).ToString());
			}

			return results;
		}
	}
}
