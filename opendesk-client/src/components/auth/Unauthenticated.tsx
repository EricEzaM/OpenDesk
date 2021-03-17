import { ReactNode } from "react";
import { useAuth } from "hooks/useAuth";

interface UnauthenticatedProps {
	children: ReactNode;
}

function Unauthenticated({ children }: UnauthenticatedProps) {
	const { user } = useAuth();

	return user ? null : <> {children} </>;
}

export default Unauthenticated;
