import { ReactNode } from "react";
import { useAuth } from "hooks/useAuth";
import useAuthPermissionsCheck, {
	PermissionsCheckProps,
} from "hooks/useAuthPermissionsCheck";

type AuthenticatedProps = {
	children: ReactNode;
	renderWhenFailed?: ReactNode;
};

function Authenticated({
	children,
	renderWhenFailed,
	...permissionProps
}: AuthenticatedProps & PermissionsCheckProps) {
	const { user } = useAuth();
	const permissionsPassed = useAuthPermissionsCheck({ ...permissionProps });

	return user && permissionsPassed ? (
		<> {children} </>
	) : (
		<> {renderWhenFailed} </>
	);
}

export default Authenticated;
