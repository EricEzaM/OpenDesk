import { ReactNode, useContext, useEffect, useState } from "react";

import { createContext } from "react";
import { User } from "types";
import apiRequest from "utils/requestUtils";

const AUTH_USER_KEY = "auth_user";

interface AuthContextProps {
	user?: User;
	signIn: (returnUrl: string) => void;
	signOut: (returnUrl: string) => void;
}

const AuthContext = createContext<AuthContextProps>({
	user: undefined,
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

	const [user, setUser] = useState<User | undefined>(userObj ?? undefined);

	// On application reload, check the user login status by querying the API (should use the existing cookie)
	// Since the API redirects us to the application, this will be run after the auth Cookie has been set and will be successful.
	// 1. App loads for the first time, calls API, return 401 since cookie is not set.
	// 2. User clicks login and the API auth endpoint is accessed, which redirects to micrsoft login page.
	// 3. After MS login success, the API will handle that and will create a user in the database. Then will redirect to the provided redirectUrl.
	// 4. The redirect url is this application, so the app will reload, trigger this effect, hit the user endpoint successfully and give us the user object!
	useEffect(() => {
		apiRequest("me").then((res) => {
			// Set user & update localStorage value.
			if (res.ok) {
				setUser(res.data);
				localStorage.setItem(AUTH_USER_KEY, JSON.stringify(res.data));
			} else {
				setUser(undefined);
				localStorage.removeItem(AUTH_USER_KEY);
			}
		});
	}, []);

	// Sign in by simply redirecting to the API with the desired auth provider and the return URL.
	function signIn(returnUrl: string) {
		window.location.href = `https://localhost:5001/api/auth/microsoft?returnUrl=${returnUrl}`;
	}

	function signOut(returnUrl: string) {
		apiRequest(`auth/signout`, {
			method: "POST",
		}).then((res) => {
			if (res.ok) {
				window.location.href = returnUrl;
			}
		});
	}

	return {
		user,
		signIn,
		signOut,
	};
}
