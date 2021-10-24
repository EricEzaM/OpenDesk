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
import { PageTitleProvider } from "hooks/usePageTitle";
import { managementPermissions } from "utils/permissions";
import React, { JSXElementConstructor, ReactNode } from "react";
import SEO from "components/SEO";

function CombineProviders({
	providers,
	children,
	...rest
}: {
	providers: JSXElementConstructor<React.PropsWithChildren<any>>[];
	children: ReactNode;
}) {
	return (
		<>
			{providers.reduceRight(
				(acc, Comp) => (
					<Comp {...rest}>{acc}</Comp>
				),
				children
			)}
		</>
	);
}

function App() {
	const providers = [
		PageTitleProvider,
		AuthProvider,
		OfficesProvider,
		OfficeDesksProvider,
		BookingsProvider,
	];

	return (
		<Router>
			<CombineProviders providers={providers}>
				<SEO />
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
						routeProps={{ path: "/manage" }}
						permissionCheckProps={{
							requiredPermissionsAny: managementPermissions,
						}}
					>
						<Manage />
					</PrivateRoute>
				</div>
			</CombineProviders>
		</Router>
	);
}

export default App;
