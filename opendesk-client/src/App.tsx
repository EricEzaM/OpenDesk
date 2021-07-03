import { BrowserRouter as Router, Route } from "react-router-dom";

import Offices from "pages/Offices";
import Me from "pages/Me";
import { AuthProvider } from "hooks/useAuth";
import PrivateRoute from "router/PrivateRoute";
import Nav from "components/Nav";
import About from "pages/About";
import Home from "pages/Home";
import Manage from "pages/Manage";
import { OfficeDesksProvider } from "hooks/useOfficeDesks";
import { BookingsProvider } from "hooks/useBookings";
import { OfficesProvider } from "hooks/useOffices";
import ManageOffices from "pages/ManageOffices";
import { PageTitleProvider } from "hooks/usePageTitle";

function App() {
	return (
		<Router>
			<PageTitleProvider>
				<AuthProvider>
					<OfficesProvider>
						<OfficeDesksProvider>
							<BookingsProvider>
								<Nav />

								<div style={{ margin: "20px" }}>
									<Route exact path="/">
										<Home />
									</Route>
									<Route exact path="/offices/:officeId?/:deskId?">
										<Offices />
									</Route>
									<PrivateRoute exact path="/me">
										<Me />
									</PrivateRoute>
									<Route exact path="/about">
										<About />
									</Route>
									<PrivateRoute exact path="/manage">
										<Manage />
									</PrivateRoute>
									<PrivateRoute exact path="/manage/offices">
										<ManageOffices />
									</PrivateRoute>
								</div>
							</BookingsProvider>
						</OfficeDesksProvider>
					</OfficesProvider>
				</AuthProvider>
			</PageTitleProvider>
		</Router>
	);
}

export default App;
