import {
	Button,
	createStyles,
	makeStyles,
	Menu,
	MenuItem,
	Theme,
} from "@material-ui/core";
import { useAuth } from "hooks/useAuth";
import { useEffect, useState } from "react";
import { User } from "types";
import apiRequest from "utils/requestUtils";
import Authenticated from "./Authenticated";

export default function DemoSignInOut() {
	const [options, setOptions] = useState<User[]>([]);
	const [selectedOption, setSelectedOption] = useState<User | null>(null);
	const [menuAnchorEl, setMenuAnchorEl] = useState<Element | null>();

	const { user, signInDemo } = useAuth();

	useEffect(() => {
		apiRequest<User[]>("auth/demos").then((res) => {
			if (res.data) {
				setOptions(res.data);
			}
		});
	}, []);

	useEffect(() => {
		selectedOption && signInDemo(selectedOption.id);
	}, [selectedOption]);

	return (
		<>
			<div className="sign-in-out__out-container">
				<Authenticated>
					<p>{user?.displayName}</p>
				</Authenticated>
				<Button
					variant="contained"
					onClick={(e) => setMenuAnchorEl(e.currentTarget)}
					style={{ marginLeft: "1rem" }}
				>
					{user ? "Switch Demo Account" : "Sign In as Demo User"}
				</Button>
				<Menu
					getContentAnchorEl={null}
					anchorOrigin={{
						vertical: "bottom",
						horizontal: "center",
					}}
					keepMounted
					open={Boolean(menuAnchorEl)}
					anchorEl={menuAnchorEl}
					onClose={(e) => setMenuAnchorEl(null)}
				>
					{options.map((o) => (
						<MenuItem
							key={o.id}
							selected={selectedOption?.id === o.id}
							onClick={(e) => {
								setSelectedOption(o);
								setMenuAnchorEl(null);
							}}
						>
							{o.displayName}
						</MenuItem>
					))}
				</Menu>
			</div>
		</>
	);
}
