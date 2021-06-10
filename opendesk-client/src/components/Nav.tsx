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
} from "@material-ui/core";
import {
	Menu,
	CollectionsBookmark,
	Person,
	Settings,
} from "@material-ui/icons";
import { ReactNode, useState } from "react";

export default function Nav() {
	const [drawerOpen, setDrawerOpen] = useState(false);

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
					<Box display="flex" justifyContent="flex-end" width="100%">
						<SignInOut />
					</Box>
				</Toolbar>
			</AppBar>

			<Drawer
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
						{getMenuListItem(
							"Book a Desk",
							"/offices",
							<CollectionsBookmark />
						)}
						<Authenticated>
							{getMenuListItem("My Bookings", "/me", <Person />)}
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
