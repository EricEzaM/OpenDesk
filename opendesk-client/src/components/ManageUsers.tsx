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
import { useEffect, useState } from "react";
import { Role, User } from "types";
import apiRequest from "utils/requestUtils";

export default function ManageUsers() {
	usePageTitle("Manage Users");

	const [users, setUsers] = useState<User[]>([]);
	const [roles, setRoles] = useState<Role[]>([]);
	const [selectedUser, setSelectedUser] = useState<User | null>(null);
	const [checkedRoleNames, setCheckedRoleNames] = useState<string[]>([]);

	useEffect(() => {
		apiRequest<User[]>("users").then((res) => {
			if (res.data) {
				setUsers(res.data);
			}
		});

		apiRequest<Role[]>("roles").then((res) => {
			if (res.data) {
				setRoles(res.data);
			}
		});
	}, []);

	function onUserSelected(user: User) {
		setSelectedUser(user);
		apiRequest<string[]>(`users/${user.id}/roles`).then((res) => {
			if (res.data) {
				setCheckedRoleNames(res.data);
			}
		});
	}

	function handleRoleToggle(toggledRole: Role) {
		const currentIndex = checkedRoleNames.indexOf(toggledRole.name);
		const newChecked = [...checkedRoleNames];

		if (currentIndex === -1) {
			newChecked.push(toggledRole.name);
		} else {
			newChecked.splice(currentIndex, 1);
		}

		setCheckedRoleNames(newChecked);
	}

	function onSave() {
		if (selectedUser) {
			apiRequest(`users/${selectedUser.id}/roles`, {
				method: "PUT",
				headers: {
					"content-type": "application/json",
				},
				body: JSON.stringify({ roles: checkedRoleNames }),
			}).then((res) => {
				console.log(res);
			});
		}
	}

	return (
		<>
			<Button
				variant="contained"
				onClick={() => {
					onSave();
				}}
			>
				Save
			</Button>
			<Grid container spacing={2}>
				<Grid item xs={6}>
					<List>
						{users.map((u) => (
							<ListItem
								key={u.id}
								button
								selected={selectedUser?.id === u.id}
								onClick={() => onUserSelected(u)}
							>
								<ListItemText primary={u.displayName} secondary={u.username} />
							</ListItem>
						))}
					</List>
				</Grid>
				<Grid item xs={6}>
					<List>
						{roles.map((r) => (
							<ListItem
								key={r.id}
								dense
								button
								onClick={() => handleRoleToggle(r)}
							>
								<ListItemIcon>
									<Checkbox
										edge="start"
										checked={checkedRoleNames.includes(r.name)}
										tabIndex={-1}
										disableRipple
										inputProps={{ "aria-labelledby": r + "-label" }}
									/>
								</ListItemIcon>
								<ListItemText
									id={r + "-label"}
									primary={r.name}
									secondary={r.description}
								/>
							</ListItem>
						))}
					</List>
				</Grid>
			</Grid>
		</>
	);
}
