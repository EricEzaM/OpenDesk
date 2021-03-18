import { BrowserRouter as Router, Route } from "react-router-dom";

import Main from "pages/Main";
import { AuthProvider } from "hooks/useAuth";
import PrivateRoute from "router/PrivateRoute";
import SignInOut from "components/auth/SignInOut";

function App() {
	return (
		<Router>
			<AuthProvider>
				<div className="container">
					<h1 className="main-title">OpenDesk</h1>
					<SignInOut />

					<PrivateRoute exact path="/">
						<Main />
					</PrivateRoute>
				</div>
			</AuthProvider>
		</Router>
	);
}

export default App;
