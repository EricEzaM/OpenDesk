import { ReactNode } from "react";
import { Redirect, Route, RouteProps } from "react-router";
import { useAuth } from "hooks/useAuth";

interface PrivateRouteProps extends RouteProps {
	children: ReactNode;
}

function PrivateRoute({ children, ...rest }: PrivateRouteProps) {
	let auth = useAuth();

	return (
		<Route
			{...rest}
			render={(props) => (auth.user ? children : <Redirect to="/" />)}
		/>
	);
}

export default PrivateRoute;
