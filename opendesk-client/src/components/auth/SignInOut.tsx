import Unauthenticated from "components/auth/Unauthenticated";
import Authenticated from "components/auth/Authenticated";

import { ReactComponent as MsSigningDarkButton } from "resources/ms-signin-dark.svg";
import { useAuth } from "hooks/useAuth";
import { Button, createStyles, makeStyles, Theme } from "@material-ui/core";

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		signOutButton: {
			marginLeft: "1em",
		},
	})
);

function SignInOut() {
	const { user, signIn, signOut } = useAuth();
	const classes = useStyles();

	return (
		<div className="sign-in-out">
			<Unauthenticated>
				<button
					className="sign-in-out__in-btn"
					onClick={() => signIn(window.location.href)}
				>
					<MsSigningDarkButton />
				</button>
			</Unauthenticated>
			<Authenticated>
				<div className="sign-in-out__out-container">
					<p>{user?.displayName}</p>
					<Button
						className={classes.signOutButton}
						variant="contained"
						onClick={() => signOut(window.location.origin)}
					>
						Sign Out
					</Button>
				</div>
			</Authenticated>
		</div>
	);
}

export default SignInOut;
