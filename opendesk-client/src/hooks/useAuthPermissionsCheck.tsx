import { useAuth } from "./useAuth";

export type PermissionsCheckProps = {
	requiredPermissionsAll?: string[];
	requiredPermissionsAny?: string[];
	permissionsCheckCallback?: (userPermissions: string[]) => boolean;
};

export default function useAuthPermissionsCheck({
	requiredPermissionsAll,
	requiredPermissionsAny,
	permissionsCheckCallback,
}: PermissionsCheckProps) {
	const { permissions } = useAuth();

	const reqAllPass =
		(requiredPermissionsAll &&
			requiredPermissionsAll.every((rp) => permissions.includes(rp))) ??
		true;

	const reqAnyPass =
		(requiredPermissionsAny &&
			requiredPermissionsAny.some((rp) => permissions.includes(rp))) ??
		true;

	const callbackPass =
		(permissionsCheckCallback && permissionsCheckCallback(permissions)) ?? true;

	return reqAllPass && reqAnyPass && callbackPass;
}
