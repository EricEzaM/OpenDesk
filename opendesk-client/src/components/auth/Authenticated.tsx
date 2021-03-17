import { ReactNode } from "react";
import { useAuth } from "hooks/useAuth";

interface AuthenticatedProps {
	children: ReactNode;
}

function Authenticated({ children }: AuthenticatedProps) {
	const { user } = useAuth();

	return user ? <> {children} </> : null;
}

export default Authenticated;
