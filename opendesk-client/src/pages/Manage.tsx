import {
	ButtonBase,
	createStyles,
	Grid,
	makeStyles,
	Theme,
	Typography,
} from "@material-ui/core";
import { Business, Group, Room, Security } from "@material-ui/icons";
import Authenticated from "components/auth/Authenticated";
import { usePageTitle } from "hooks/usePageTitle";
import { Link, useRouteMatch } from "react-router-dom";
import PrivateRoute from "router/PrivateRoute";
import {
	deskManagementPermissions,
	officeManagementPermissions,
	roleManagementPermissions,
	userManagementPermissions,
} from "utils/permissions";
import ManageDesks from "components/ManageDesks";
import ManageOffices from "components/ManageOffices";
import ManageUsers from "components/ManageUsers";
import ManageRoles from "components/ManageRoles";
import { PermissionsCheckProps } from "hooks/useAuthPermissionsCheck";

type ManagementTileInfo = {
	relativePath: string;
	title: string;
	description: string;
	icon: JSX.Element;
	permissionCheckProps: PermissionsCheckProps;
};

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		button: {
			height: "clamp(100px, 40vh , 300px)",
			borderStyle: "solid",
			borderWidth: "1px",
			borderRadius: theme.shape.borderRadius,
			flexDirection: "column",
			width: "100%",
		},
		buttonTitle: {
			fontSize: "1.5rem",
		},
		link: {
			color: "inherit",
			textDecoration: "none",
		},
	})
);

export default function Manage() {
	const { path, url, isExact } = useRouteMatch();
	usePageTitle(isExact ? "Manage" : undefined);

	const classes = useStyles();

	const iconStyleDefaults = {
		fontSize: 60,
		marginBottom: 10,
	};

	const managementTiles: ManagementTileInfo[] = [
		{
			relativePath: `offices`,
			title: "Offices",
			description: "Create and edit offices.",
			icon: <Business color="primary" style={iconStyleDefaults} />,
			permissionCheckProps: {
				requiredPermissionsAny: officeManagementPermissions,
			},
		},
		{
			relativePath: `desks`,
			title: "Desks",
			description:
				"Create and edit desks - including desk location, names and other information.",
			icon: <Room color="secondary" style={iconStyleDefaults} />,
			permissionCheckProps: {
				requiredPermissionsAny: deskManagementPermissions,
			},
		},
		{
			relativePath: `users`,
			title: "Users",
			description: "Manage user roles.",
			icon: <Group style={iconStyleDefaults} />,
			permissionCheckProps: {
				requiredPermissionsAny: userManagementPermissions,
			},
		},
		{
			relativePath: `roles`,
			title: "Roles",
			description: "Manage roles and their permissions.",
			icon: <Security style={iconStyleDefaults} />,
			permissionCheckProps: {
				requiredPermissionsAny: roleManagementPermissions,
			},
		},
	];

	return (
		<>
			{/* Tiles displaying options */}
			<PrivateRoute routeProps={{ exact: true, path: `${path}` }}>
				<Grid container spacing={2} justify="center">
					{managementTiles.map((tileInfo) => (
						<Authenticated
							key={tileInfo.title}
							{...tileInfo.permissionCheckProps}
						>
							<Grid item sm={4} xs={12}>
								<Link
									className={classes.link}
									to={`${url}/${tileInfo.relativePath}`}
								>
									<ButtonBase className={classes.button} focusRipple>
										{tileInfo.icon}
										<Typography className={classes.buttonTitle}>
											{tileInfo.title}
										</Typography>
										<Typography variant="body1">
											{tileInfo.description}
										</Typography>
									</ButtonBase>
								</Link>
							</Grid>
						</Authenticated>
					))}
				</Grid>
			</PrivateRoute>
			<PrivateRoute
				routeProps={{ exact: true, path: `${path}/offices` }}
				permissionCheckProps={{
					requiredPermissionsAny: officeManagementPermissions,
				}}
			>
				<ManageOffices />
			</PrivateRoute>
			<PrivateRoute
				routeProps={{ exact: true, path: `${path}/desks` }}
				permissionCheckProps={{
					requiredPermissionsAny: deskManagementPermissions,
				}}
			>
				<ManageDesks />
			</PrivateRoute>
			<PrivateRoute
				routeProps={{ exact: true, path: `${path}/users` }}
				permissionCheckProps={{
					requiredPermissionsAny: userManagementPermissions,
				}}
			>
				<ManageUsers />
			</PrivateRoute>
			<PrivateRoute
				routeProps={{ exact: true, path: `${path}/roles` }}
				permissionCheckProps={{
					requiredPermissionsAny: roleManagementPermissions,
				}}
			>
				<ManageRoles />
			</PrivateRoute>
		</>
	);
}
