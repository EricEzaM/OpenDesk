import Unauthenticated from "components/auth/Unauthenticated";
import Authenticated from "components/auth/Authenticated";

import { ReactComponent as MsSigningDarkButton } from "resources/ms-signin-dark.svg";
import { useAuth } from "hooks/useAuth";

function SignInOut() {
	const { user, signIn, signOut } = useAuth();

	return (
		<div className="sign-in-out">
			<Unauthenticated>
				<p>Sign in is required to use the app.</p>
				<button
					className="sign-in-out__in-btn"
					onClick={() => signIn(window.location.href)}
				>
					<MsSigningDarkButton />
				</button>
			</Unauthenticated>
			<Authenticated>
				<div
					style={{
						display: "flex",
						justifyContent: "center",
						alignItems: "center",
					}}
				>
					<p>Welcome {user?.name}</p>
					<button
						className="sign-in-out__out-btn"
						onClick={() => signOut(window.location.origin)}
					>
						Sign Out
					</button>
				</div>
			</Authenticated>
		</div>
	);
}

export default SignInOut;
