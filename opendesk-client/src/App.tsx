import { BrowserRouter as Router, Link, Route } from "react-router-dom";

import Main from "pages/Main";
import Me from "pages/Me";
import { AuthProvider } from "hooks/useAuth";
import PrivateRoute from "router/PrivateRoute";
import Nav from "components/Nav";

function App() {
	return (
		<Router>
			<AuthProvider>
				<div className="container">
					<Nav />

					<PrivateRoute exact path="/">
						<Main />
					</PrivateRoute>
					<PrivateRoute exact path="/me">
						<Me />
					</PrivateRoute>
				</div>
			</AuthProvider>
		</Router>
	);
}

export default App;
