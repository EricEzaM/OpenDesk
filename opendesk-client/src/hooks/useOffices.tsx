import { createContext, ReactNode, useContext, useState } from "react";
import { Office } from "types";

interface OfficesContextProps {
	officesState: [Office[], (o: Office[]) => void];
	selectedOfficeState: [Office | undefined, (o: Office) => void];
}

export const OfficesContext = createContext<OfficesContextProps>({
	officesState: [[], () => {}],
	selectedOfficeState: [undefined, () => {}],
});

export function OfficesProvider({ children }: { children: ReactNode }) {
	const offices = useOfficesProvider();

	return (
		<OfficesContext.Provider value={offices}>
			{children}
		</OfficesContext.Provider>
	);
}

function useOfficesProvider(): OfficesContextProps {
	const officesState = useState<Office[]>([]);
	const selectedOfficeState = useState<Office>();

	return {
		officesState,
		selectedOfficeState,
	};
}

export function useOffices() {
	return useContext(OfficesContext);
}
