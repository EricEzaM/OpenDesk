using System.ComponentModel.DataAnnotations;

namespace OpenDesk.Application.Common
{
	/// <summary>
	/// Contains all Permissions for the application.
	/// </summary>
	public static class Permissions
	{
		private const string Prefix = nameof(Permissions) + ".";

		public static class Users
		{
			private const string Group = Prefix + nameof(Users) + ".";

			[Display(GroupName = "Users", Name = "Read", Description = "Read user data.")] 
			public const string Read = Group + nameof(Read);
			[Display(GroupName = "Users", Name = "Edit", Description = "Edit user data.")]
			public const string Edit = Group + nameof(Edit);
		}

		public static class Roles
		{
			private const string Group = Prefix + nameof(Roles) + ".";

			[Display(GroupName = "Roles", Name = "Create", Description = "Create new roles.")]
			public const string Create = Group + nameof(Create);
			[Display(GroupName = "Roles", Name = "Read", Description = "Read role data.")]
			public const string Read = Group + nameof(Read);
			[Display(GroupName = "Roles", Name = "Delete", Description = "Delete roles.")]
			public const string Delete = Group + nameof(Delete);

			public static class Edit
			{
				private const string SubGroup = Group + nameof(Edit) + ".";

				[Display(GroupName = "Roles", Name = "Edit Metadata", Description = "Edit role metdata.")]
				public const string Metadata = SubGroup + nameof(Metadata);
				[Display(GroupName = "Roles", Name = "Edit Permissions", Description = "Edit which permissions are assigned to each role.")]
				public const string Permissions = SubGroup + nameof(Permissions);
			}
		}

		public static class Desks
		{
			private const string Group = Prefix + nameof(Desks) + ".";

			[Display(GroupName = "Desks", Name = "Create", Description = "Create new desks.")]
			public const string Create = Group + nameof(Create);
			[Display(GroupName = "Desks", Name = "Read", Description = "Read desk data.")]
			public const string Read = Group + nameof(Read);
			[Display(GroupName = "Desks", Name = "Delete", Description = "Delete desks.")]
			public const string Delete = Group + nameof(Delete);
			[Display(GroupName = "Desks", Name = "Edit", Description = "Edit desks.")]
			public const string Edit = Group + nameof(Edit);
		}

		public static class Offices
		{
			private const string Group = Prefix + nameof(Offices) + ".";

			[Display(GroupName = "Offices", Name = "Create", Description = "Create new offices.")]
			public const string Create = Group + nameof(Create);
			[Display(GroupName = "Offices", Name = "Read", Description = "Read office data.")]
			public const string Read = Group + nameof(Read);
			[Display(GroupName = "Offices", Name = "Delete", Description = "Delete offices.")]
			public const string Delete = Group + nameof(Delete);
			[Display(GroupName = "Offices", Name = "Edit", Description = "Edit offices.")]
			public const string Edit = Group + nameof(Edit);
		}

		public static class Bookings
		{
			private const string Group = Prefix + nameof(Bookings) + ".";

			[Display(GroupName = "Bookings", Name = "Create", Description = "Create new bookings.")]
			public const string Create = Group + nameof(Create);
			[Display(GroupName = "Bookings", Name = "Read", Description = "Read booking data.")]
			public const string Read = Group + nameof(Read);
			[Display(GroupName = "Bookings", Name = "Delete", Description = "Delete bookings.")]
			public const string Delete = Group + nameof(Delete);
			[Display(GroupName = "Bookings", Name = "Edit", Description = "Edit bookings.")]
			public const string Edit = Group + nameof(Edit);
		}

		public static class Blobs
		{
			private const string Group = Prefix + nameof(Blobs) + ".";

			public const string Create = Group + nameof(Create);
			public const string Read = Group + nameof(Read);
		}
	}
}
