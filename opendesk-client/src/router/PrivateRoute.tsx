import { ReactNode } from "react";
import { Redirect, Route, RouteProps } from "react-router";
import useAuthPermissionsCheck, {
	PermissionsCheckProps,
} from "hooks/useAuthPermissionsCheck";
import { useAuth } from "hooks/useAuth";

interface PrivateRouteProps {
	children: ReactNode;
	routeProps: RouteProps;
	permissionCheckProps?: PermissionsCheckProps;
}

function PrivateRoute({
	children,
	routeProps,
	permissionCheckProps,
}: PrivateRouteProps) {
	const { user } = useAuth();
	const passedPermissions = useAuthPermissionsCheck({
		...permissionCheckProps,
	});

	return (
		<Route
			{...routeProps}
			render={(props) =>
				user && passedPermissions ? children : <Redirect to="/" />
			}
		/>
	);
}

export default PrivateRoute;
