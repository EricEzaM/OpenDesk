export const permissions = {
	Users: {
		Read: "Permissions.Users.Read",
		Edit: "Permissions.Users.Edit",
	},
	Roles: {
		Create: "Permissions.Roles.Create",
		Read: "Permissions.Roles.Read",
		Delete: "Permissions.Roles.Delete",
		Edit: {
			Metadata: "Permissions.Roles.Edit.Metadata",
			Permissions: "Permissions.Roles.Edit.Permissions",
		},
	},
	Desks: {
		Create: "Permissions.Desks.Create",
		Read: "Permissions.Desks.Read",
		Delete: "Permissions.Desks.Delete",
		Edit: "Permissions.Desks.Edit",
	},
	Offices: {
		Create: "Permissions.Offices.Create",
		Read: "Permissions.Offices.Read",
		Delete: "Permissions.Offices.Delete",
		Edit: "Permissions.Offices.Edit",
	},
	Bookings: {
		Create: "Permissions.Bookings.Create",
		Read: "Permissions.Bookings.Read",
		Delete: "Permissions.Bookings.Delete",
		Edit: "Permissions.Bookings.Edit",
	},
	Blobs: {
		Create: "Permissions.Blobs.Create",
		Read: "Permissions.Blobs.Read",
	},
};

export function getAllPermissionsArray() {
	return flattenObjValues(permissions);
}

function flattenObjValues(obj: any, result: any[] = []) {
	for (let key in obj) {
		if (typeof obj[key] === "object") {
			flattenObjValues(obj[key], result);
		} else {
			result.push(obj[key]);
		}
	}

	return result;
}

export const managementPermissions = [
	permissions.Users.Edit,
	permissions.Roles.Create,
	permissions.Roles.Delete,
	permissions.Roles.Edit.Metadata,
	permissions.Roles.Edit.Permissions,
	permissions.Desks.Create,
	permissions.Desks.Edit,
	permissions.Desks.Delete,
	permissions.Offices.Create,
	permissions.Offices.Edit,
	permissions.Offices.Delete,
];

export const officeManagementPermissions = [
	permissions.Offices.Create,
	permissions.Offices.Edit,
	permissions.Offices.Delete,
];

export const deskManagementPermissions = [
	permissions.Desks.Create,
	permissions.Desks.Edit,
	permissions.Desks.Delete,
];

export const userManagementPermissions = [permissions.Users.Edit];

export const roleManagementPermissions = [
	permissions.Roles.Edit.Metadata,
	permissions.Roles.Edit.Permissions,
];
