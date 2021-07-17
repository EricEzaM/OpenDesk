import {
	ButtonBase,
	createStyles,
	Grid,
	makeStyles,
	Theme,
	Typography,
} from "@material-ui/core";
import { Business, Group, Room } from "@material-ui/icons";
import Authenticated from "components/auth/Authenticated";
import { usePageTitle } from "hooks/usePageTitle";
import { Link, useRouteMatch } from "react-router-dom";
import PrivateRoute from "router/PrivateRoute";
import {
	deskManagementPermissions,
	officeManagementPermissions,
	permissions,
} from "utils/permissions";
import ManageDesks from "components/ManageDesks";
import ManageOffices from "components/ManageOffices";

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

	const managementTiles = [
		{
			link: `${url}/offices`,
			title: "Offices",
			desc: "Create and edit offices.",
			icon: <Business color="primary" style={iconStyleDefaults} />,
			allowedPermissions: [
				permissions.Offices.Create,
				permissions.Offices.Edit,
				permissions.Offices.Delete,
			],
		},
		{
			link: `${url}/desks`,
			title: "Desks",
			desc: "Create and edit desks - including desk location, names and other information.",
			icon: <Room color="secondary" style={iconStyleDefaults} />,
			allowedPermissions: [
				permissions.Desks.Create,
				permissions.Desks.Edit,
				permissions.Desks.Delete,
			],
		},
		{
			link: `${url}/users`,
			title: "Users",
			desc: "Manage users and their permissions.",
			icon: <Group style={iconStyleDefaults} />,
			allowedPermissions: [permissions.Users.Edit],
		},
	];

	return (
		<>
			<PrivateRoute routeProps={{ exact: true, path: `${path}` }}>
				<Grid container spacing={2} justify="center">
					{managementTiles.map((tileInfo) => (
						<Authenticated
							key={tileInfo.title}
							requiredPermissionsAny={tileInfo.allowedPermissions}
						>
							<Grid item sm={4} xs={12}>
								<Link className={classes.link} to={tileInfo.link}>
									<ButtonBase className={classes.button} focusRipple>
										{tileInfo.icon}
										<Typography className={classes.buttonTitle}>
											{tileInfo.title}
										</Typography>
										<Typography variant="body1">{tileInfo.desc}</Typography>
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
		</>
	);
}
