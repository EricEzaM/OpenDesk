import Main from "pages/Main";
import About from "pages/About";
import { BrowserRouter as Router, Route, Link } from "react-router-dom";
import { AuthProvider, useAuth } from "hooks/useAuth";
import PrivateRoute from "router/PrivateRoute";
import Unauthenticated from "components/auth/Unauthenticated";
import Authenticated from "components/auth/Authenticated";

function LogInOut() {
	const { signIn, signOut } = useAuth();

	return (
		<>
			<Unauthenticated>
				<button onClick={() => signIn(window.location.href)}>Login</button>
			</Unauthenticated>
			<Authenticated>
				<button onClick={() => signOut(window.location.origin)}>Logout</button>
			</Authenticated>
		</>
	);
}

function App() {
	return (
		<Router>
			<AuthProvider>
				<div className="container">
					<h1 className="main-title">OpenDesk</h1>
					<div>
						<Link to="/">Main</Link>
						<Link to="/about">About</Link>
						<LogInOut />
					</div>

					<PrivateRoute exact path="/">
						<Main />
					</PrivateRoute>
					<Route path="/about">
						<About />
					</Route>
				</div>
			</AuthProvider>
		</Router>
	);
}

export default App;
