import { PublicClientApplication } from "@azure/msal-browser";
import { loginRequest, msalConfig } from "authConfig";
import { ReactNode, useContext, useEffect, useState } from "react";

import { createContext } from "react";
import { User } from "types";
import apiRequest from "utils/requestUtils";

const AUTH_USER_KEY = "auth_user";
const PERMISSIONS_KEY = "permissions";

const msalInstance = new PublicClientApplication(msalConfig);

export enum AuthLoadingStatus {
	Authenticated,
	NotAuthenticated,
	Verifying,
	RetrievingUserData,
}

interface AuthContextProps {
	loadingStatus: AuthLoadingStatus;
	user?: User;
	permissions: string[];
	signIn: (returnUrl: string) => void;
	signOut: (returnUrl: string) => void;
	signInDemo: (userId: string) => void;
}

const AuthContext = createContext<AuthContextProps>({
	loadingStatus: AuthLoadingStatus.NotAuthenticated,
	user: undefined,
	permissions: [],
	signIn: () => {
		console.error(
			"Not Implemented. Method likely called without context provider."
		);
	},
	signOut: () => {
		console.error(
			"Not Implemented. Method likely called without context provider."
		);
	},
	signInDemo: () => {
		console.error(
			"Not Implemented. Method likely called without context provider."
		);
	},
});

/**
 * Wrap any elements with this to allow them to access the authentication context.
 */
export function AuthProvider({ children }: { children: ReactNode }) {
	const auth = useAuthProvider();

	return <AuthContext.Provider value={auth}>{children}</AuthContext.Provider>;
}

/**
 * Allows access to the Auth Context object.
 */
export function useAuth(): AuthContextProps {
	return useContext(AuthContext);
}

/**
 * Internal Use only for the Context Provider value.
 * @returns The actual values of the AuthContextProps
 */
function useAuthProvider(): AuthContextProps {
	// Try use the localStorage stored user before the request to the API finishes.
	const userJson = localStorage.getItem(AUTH_USER_KEY);
	const userObj = userJson && JSON.parse(userJson);

	const permissionsJson = localStorage.getItem(PERMISSIONS_KEY);
	const permissionsObj = permissionsJson && JSON.parse(permissionsJson);

	const [user, setUser] = useState<User | undefined>(userObj ?? undefined);
	const [permissions, setPermissions] = useState<string[]>(
		permissionsObj ?? []
	);

	const [loadingStatus, setLoadingStatus] = useState(
		AuthLoadingStatus.NotAuthenticated
	);

	useEffect(() => {
		let current = loadingStatus;
		setLoadingStatus(AuthLoadingStatus.Verifying);
		msalInstance.handleRedirectPromise().then((tokenResponse) => {
			if (tokenResponse !== null) {
				// Coming back from a valid authentication redirect
				console.log(tokenResponse);
				apiRequest("auth/external", {
					method: "POST",
					headers: {
						"content-type": "application/json",
					},
					body: JSON.stringify({
						provider: 0, // FIXME: Not hardcode provider 0 (microsoft)
						idToken: tokenResponse.idToken,
					}),
				}).then((res) => {
					if (res.success) {
						console.log("Login Success");
						refreshUser();
					} else {
						console.log("Login Failed");
					}
				});
			} else {
				setLoadingStatus(current);
			}
		});
	}, []);

	function refreshUser() {
		setLoadingStatus(AuthLoadingStatus.RetrievingUserData);
		apiRequest<User>("me").then((res) => {
			// Set user & update localStorage value.
			if (res.data) {
				setUser(res.data);
				localStorage.setItem(AUTH_USER_KEY, JSON.stringify(res.data));

				refreshPermissions();
				setLoadingStatus(AuthLoadingStatus.Authenticated);
			} else if (res.problem?.status === 401) {
				setUser(undefined);
				localStorage.removeItem(AUTH_USER_KEY);
				setLoadingStatus(AuthLoadingStatus.NotAuthenticated);
			}
		});
	}

	function refreshPermissions() {
		apiRequest<string[]>("me/permissions").then((res) => {
			if (res.data) {
				setPermissions(res.data);
				localStorage.setItem(PERMISSIONS_KEY, JSON.stringify(res.data));
			} else {
				localStorage.setItem(PERMISSIONS_KEY, JSON.stringify([]));
			}
		});
	}

	function signIn(returnUrl: string) {
		msalInstance.loginRedirect(loginRequest);
	}

	function signInDemo(userId: string) {
		apiRequest<User>(`auth/demos/${userId}`, {
			method: "POST",
		}).then((res) => {
			refreshUser();
		});
	}

	function signOut(returnUrl: string) {
		apiRequest<void>(`auth/signout`, {
			method: "POST",
		}).then(() => {
			window.location.href = returnUrl;
			localStorage.removeItem(AUTH_USER_KEY);
			localStorage.removeItem(PERMISSIONS_KEY);
			msalInstance.logoutRedirect();
		});
	}

	return {
		loadingStatus,
		user,
		permissions,
		signIn,
		signOut,
		signInDemo,
	};
}
