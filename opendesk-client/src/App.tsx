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
import ManageDesks from "pages/ManageDesks";
import {
	deskManagementPermissions,
	managementPermissions,
	officeManagementPermissions,
} from "utils/permissions";

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
									<PrivateRoute routeProps={{ exact: true, path: "/me" }}>
										<Me />
									</PrivateRoute>
									<Route exact path="/about">
										<About />
									</Route>
									<PrivateRoute
										routeProps={{ exact: true, path: "/manage" }}
										permissionCheckProps={{
											requiredPermissionsAny: managementPermissions,
										}}
									>
										<Manage />
									</PrivateRoute>
									<PrivateRoute
										routeProps={{ exact: true, path: "/manage/offices" }}
										permissionCheckProps={{
											requiredPermissionsAny: officeManagementPermissions,
										}}
									>
										<ManageOffices />
									</PrivateRoute>
									<PrivateRoute
										routeProps={{ exact: true, path: "/manage/desks" }}
										permissionCheckProps={{
											requiredPermissionsAny: deskManagementPermissions,
										}}
									>
										<ManageDesks />
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
