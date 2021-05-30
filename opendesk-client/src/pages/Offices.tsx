import { useState } from "react";

import { Office, Desk } from "types";

import OfficeSelector from "components/OfficeSelector";
import apiRequest from "utils/requestUtils";
import DeskDetails from "components/DeskDetails";
import OfficeMap from "components/map/OfficeMap";
import Authenticated from "components/auth/Authenticated";
import Unauthenticated from "components/auth/Unauthenticated";
import BookingSubmissionForm from "components/BookingSubmissionForm";
import { useOfficeDesks } from "hooks/useOfficeDesks";

function Offices() {
	// =============================================================
	// Hooks and Variables
	// =============================================================

	const {
		desksState: [desks, setDesks],
		selectedDeskState: [selectedDesk, setSelectedDesk],
	} = useOfficeDesks();
	const [selectedOffice, setSelectedOffice] = useState<Office | undefined>();

	// =============================================================
	// Effects
	// =============================================================

	// =============================================================
	// Functions
	// =============================================================

	function onDeskSelected(desk?: Desk) {
		setSelectedDesk(desk);
	}

	function onOfficeSelected(office?: Office) {
		setSelectedOffice(office);

		office &&
			apiRequest<Desk[]>(`offices/${office.id}/desks`).then((res) => {
				if (res.outcome.isSuccess) {
					setDesks(res.data ?? []);
				}
			});
	}

	// =============================================================
	// Render
	// =============================================================

	return (
		<>
			<Unauthenticated>
				<p>Sign in is required</p>
			</Unauthenticated>
			<Authenticated>
				<div style={{ display: "flex", justifyContent: "center" }}>
					<p style={{ alignSelf: "center", margin: "0 0.5em" }}>
						Looking for a desk at
					</p>
					<OfficeSelector onOfficeSelected={onOfficeSelected} />
					<BookingSubmissionForm />
				</div>
				{selectedOffice && (
					<>
						<OfficeMap
							image={selectedOffice.image}
							onDeskSelected={onDeskSelected}
						/>
					</>
				)}
				{selectedDesk && <DeskDetails desk={selectedDesk} />}
			</Authenticated>
		</>
	);
}

export default Offices;
