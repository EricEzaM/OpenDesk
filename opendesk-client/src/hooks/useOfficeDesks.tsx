import {
	createContext,
	ReactNode,
	useContext,
	useEffect,
	useState,
} from "react";
import { Desk } from "types";
import apiRequest from "utils/requestUtils";
import { useOffices } from "./useOffices";

interface OfficeDesksContextProps {
	desksState: [Desk[], (d: Desk[]) => void];
	selectedDeskState: [Desk | undefined, (d: Desk | undefined) => void];
}

export const OfficeDesksContext = createContext<OfficeDesksContextProps>({
	desksState: [[], () => {}],
	selectedDeskState: [undefined, () => {}],
});

export function OfficeDesksProvider({ children }: { children: ReactNode }) {
	const desks = useOfficeDesksProvider();

	return (
		<OfficeDesksContext.Provider value={desks}>
			{children}
		</OfficeDesksContext.Provider>
	);
}

function useOfficeDesksProvider(): OfficeDesksContextProps {
	const [desks, setDesks] = useState<Desk[]>([]);
	const [selectedDesk, setSelectedDesk] = useState<Desk>();

	const {
		selectedOfficeState: [selectedOffice],
	} = useOffices();

	// Update desks when office is changed
	useEffect(() => {
		if (selectedOffice) {
			apiRequest<Desk[]>(`offices/${selectedOffice.id}/desks`).then((res) => {
				if (res.outcome.isSuccess) {
					setDesks(res.data ?? []);
				}
			});
		}
	}, [selectedOffice]);

	return {
		desksState: [desks, setDesks],
		selectedDeskState: [selectedDesk, setSelectedDesk],
	};
}

export function useOfficeDesks() {
	return useContext(OfficeDesksContext);
}
