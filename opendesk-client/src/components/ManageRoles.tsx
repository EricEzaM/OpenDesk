import {
	Button,
	Checkbox,
	Grid,
	List,
	ListItem,
	ListItemIcon,
	ListItemText,
} from "@material-ui/core";
import { usePageTitle } from "hooks/usePageTitle";
import { useEffect, useMemo, useState } from "react";
import { Role } from "types";
import { getAllPermissionsArray } from "utils/permissions";
import apiRequest from "utils/requestUtils";

export default function ManageRoles() {
	usePageTitle("Manage Roles");

	const [roles, setRoles] = useState<Role[]>([]);
	const [selectedRole, setSelectedRole] = useState<Role | null>(null);

	const permissions = useMemo(() => getAllPermissionsArray(), []);
	const [checkedPermissions, setCheckedPermissions] = useState<string[]>([]);

	useEffect(() => {
		apiRequest<Role[]>("roles").then((res) => {
			if (res.data) {
				setRoles(res.data);
			}
		});
	}, []);

	function onRoleSelected(role: Role) {
		setSelectedRole(role);
		apiRequest<string[]>(`roles/${role.id}/permissions`).then((res) => {
			if (res.data) {
				setCheckedPermissions(res.data);
			}
		});
	}

	function handlePermissionToggle(toggledPerm: string) {
		const currentIndex = checkedPermissions.indexOf(toggledPerm);
		const newChecked = [...checkedPermissions];

		if (currentIndex === -1) {
			newChecked.push(toggledPerm);
		} else {
			newChecked.splice(currentIndex, 1);
		}

		setCheckedPermissions(newChecked);
	}

	function onSave() {
		apiRequest(`roles/${selectedRole?.id}/permissions`, {
			method: "PUT",
			headers: {
				"content-type": "application/json",
			},
			body: JSON.stringify({ permissions: checkedPermissions }),
		});
	}

	return (
		<>
			<Button variant="contained" onClick={() => onSave()}>
				Save
			</Button>
			<Grid container spacing={2}>
				<Grid item xs={6}>
					<List>
						{roles.map((r) => (
							<ListItem
								key={r.id}
								dense
								button
								selected={selectedRole?.id === r.id}
								onClick={() => onRoleSelected(r)}
							>
								<ListItemText primary={r.name} secondary={r.description} />
							</ListItem>
						))}
					</List>
				</Grid>
				<Grid item xs={6}>
					<List>
						{permissions.map((p) => (
							<ListItem
								key={p}
								dense
								button
								onClick={() => handlePermissionToggle(p)}
							>
								<ListItemIcon>
									<Checkbox
										edge="start"
										checked={checkedPermissions.includes(p)}
										tabIndex={-1}
										disableRipple
										inputProps={{ "aria-labelledby": p + "-label" }}
									/>
								</ListItemIcon>
								<ListItemText id={p + "-label"} primary={p} />
							</ListItem>
						))}
					</List>
				</Grid>
			</Grid>
		</>
	);
}
