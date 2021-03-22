import { useState } from "react";

import { Office, Desk } from "types";

import OfficeSelector from "components/OfficeSelector";
import apiRequest from "utils/requestUtils";
import DeskDetails from "components/DeskDetails";
import OfficeMap from "components/map/OfficeMap";
import Authenticated from "components/auth/Authenticated";
import Unauthenticated from "components/auth/Unauthenticated";

function Offices() {
	// =============================================================
	// Hooks and Variables
	// =============================================================

	const [desks, setDesks] = useState<Desk[] | undefined>();
	const [selectedDesk, setSelectedDesk] = useState<Desk | undefined>();
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
			apiRequest(`offices/${office.id}/desks`).then((res) => {
				if (res.ok) {
					setDesks(res.data);
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
				<OfficeSelector onOfficeSelected={onOfficeSelected} />
				{selectedOffice && (
					<OfficeMap
						image={selectedOffice.image}
						desks={desks}
						selectedDesk={selectedDesk}
						onDeskSelected={onDeskSelected}
					/>
				)}
				{selectedDesk && <DeskDetails desk={selectedDesk} />}
			</Authenticated>
		</>
	);
}

export default Offices;
