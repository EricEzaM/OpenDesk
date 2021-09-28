import { Link } from "react-router-dom";
import SignInOut from "components/auth/SignInOut";
import Authenticated from "./auth/Authenticated";
import {
	AppBar,
	IconButton,
	Toolbar,
	Drawer,
	List,
	ListItem,
	ListItemIcon,
	ListItemText,
	Typography,
	Box,
	makeStyles,
	createStyles,
	Theme,
	CircularProgress,
} from "@material-ui/core";
import {
	Menu,
	CollectionsBookmark,
	Person,
	Settings,
	Home,
} from "@material-ui/icons";
import { ReactNode, useState } from "react";
import { usePageTitle } from "hooks/usePageTitle";
import DemoSignInOut from "./auth/DemoSignInOut";
import { managementPermissions } from "utils/permissions";
import { AuthLoadingStatus, useAuth } from "hooks/useAuth";

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		drawer: {
			minWidth: 200,
		},
		fastProgress: {
			animationDuration: "0.75s",
		},
	})
);

export default function Nav() {
	const classes = useStyles();

	const [drawerOpen, setDrawerOpen] = useState(false);
	const title = usePageTitle();

	const { loadingStatus } = useAuth();

	const loginElement =
		process.env.REACT_APP_IS_DEMO === "true" ? (
			<Box>
				<DemoSignInOut />
			</Box>
		) : (
			<Box>
				<SignInOut />
			</Box>
		);

	let loginDisplay = loginElement;

	if (
		loadingStatus === AuthLoadingStatus.Verifying ||
		loadingStatus === AuthLoadingStatus.RetrievingUserData
	) {
		const text =
			loadingStatus === AuthLoadingStatus.Verifying
				? "Verifying..."
				: "Getting Data...";

		loginDisplay = (
			<Box display="flex">
				<CircularProgress
					className={classes.fastProgress}
					size={30}
					thickness={5}
					color="secondary"
					disableShrink
				/>
				<Typography style={{ alignSelf: "center", marginLeft: "1em" }}>
					{text}
				</Typography>
			</Box>
		);
	}

	return (
		<>
			<AppBar position="static">
				<Toolbar>
					<IconButton
						onClick={(e) => setDrawerOpen(!drawerOpen)}
						edge="start"
						color="inherit"
						aria-label="menu"
					>
						<Menu />
					</IconButton>
					<Typography variant="h4" component="h1">
						OpenDesk
					</Typography>
					{title && (
						<Box marginLeft={1}>
							<Typography variant="h5" component="h2" noWrap>
								&#47;&#47; {title}
							</Typography>
						</Box>
					)}
					<Box flexGrow="1" /> {/* Spacer */}
					{loginDisplay}
				</Toolbar>
			</AppBar>

			<Drawer
				classes={{
					paper: classes.drawer,
				}}
				anchor="left"
				open={drawerOpen}
				onClose={(e) => setDrawerOpen(false)}
			>
				<div
					onClick={() => {
						setDrawerOpen(false);
					}}
				>
					<List>
						{getMenuListItem("Home", "/", <Home />)}
						<Authenticated>
							{getMenuListItem(
								"Book a Desk",
								"/offices",
								<CollectionsBookmark />
							)}
						</Authenticated>
						<Authenticated>
							{getMenuListItem("My Bookings", "/me", <Person />)}
						</Authenticated>
						<Authenticated requiredPermissionsAny={managementPermissions}>
							{getMenuListItem("Management Portal", "/manage", <Settings />)}
						</Authenticated>
					</List>
				</div>
			</Drawer>
		</>
	);
}

function getMenuListItem(text: string, linkTo: string, icon: ReactNode) {
	return (
		<ListItem button component={Link} to={linkTo}>
			<ListItemIcon>{icon}</ListItemIcon>
			<ListItemText primary={text} />
		</ListItem>
	);
}
