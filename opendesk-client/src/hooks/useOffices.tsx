import {
	createContext,
	ReactNode,
	useContext,
	useEffect,
	useState,
} from "react";
import { Office } from "types";
import apiRequest from "utils/requestUtils";

interface OfficesContextProps {
	officesState: [Office[] | undefined, (o: Office[] | undefined) => void];
	selectedOfficeState: [Office | undefined, (o: Office | undefined) => void];
	refreshOffices: () => void;
}

export const OfficesContext = createContext<OfficesContextProps>({
	officesState: [[], () => {}],
	selectedOfficeState: [undefined, () => {}],
	refreshOffices: () => {},
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
	const [offices, setOffices] = useState<Office[]>();
	const [selectedOffice, setSelectedOffice] = useState<Office>();

	useEffect(() => {
		refreshOffices();
	}, []);

	function refreshOffices() {
		apiRequest<Office[]>("offices").then((res) => {
			if (res.data) {
				setOffices(res.data ?? []);
			}
		});
	}

	return {
		officesState: [offices, setOffices],
		selectedOfficeState: [selectedOffice, setSelectedOffice],
		refreshOffices,
	};
}

export function useOffices() {
	return useContext(OfficesContext);
}
