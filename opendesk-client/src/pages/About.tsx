import { useAuth } from "hooks/useAuth";

function About() {
	const { user, signIn } = useAuth();

	return (
		<div>
			<p>Hello! {JSON.stringify(user)} Welcome to the about page!</p>

			{!user && (
				<button onClick={() => signIn(window.location.origin)}>Login</button>
			)}
		</div>
	);
}

export default About;
