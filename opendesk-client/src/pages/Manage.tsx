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
import { Link } from "react-router-dom";
import { permissions } from "utils/permissions";

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
	usePageTitle("Management");

	const classes = useStyles();

	const iconStyleDefaults = {
		fontSize: 60,
		marginBottom: 10,
	};

	const managementTiles = [
		{
			link: "/manage/offices",
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
			link: "/manage/desks",
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
			link: "/manage/users",
			title: "Users",
			desc: "Manage users and their permissions.",
			icon: <Group style={iconStyleDefaults} />,
			allowedPermissions: [permissions.Users.Edit],
		},
	];

	return (
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
	);
}
