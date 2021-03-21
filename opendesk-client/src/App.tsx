import { BrowserRouter as Router, Link, Route } from "react-router-dom";

import Main from "pages/Main";
import Me from "pages/Me";
import { AuthProvider } from "hooks/useAuth";
import PrivateRoute from "router/PrivateRoute";
import Nav from "components/Nav";
import About from "pages/About";

function App() {
	return (
		<Router>
			<AuthProvider>
				<div className="container">
					<Nav />

					<Route exact path="/:officeId?/:deskId?">
						<Main />
					</Route>
					<PrivateRoute exact path="/me">
						<Me />
					</PrivateRoute>
					<Route exact path="/about">
						<About />
					</Route>
				</div>
			</AuthProvider>
		</Router>
	);
}

export default App;
