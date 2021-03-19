import { useState } from "react";

import { Office, Desk, Booking } from "types";

import OfficeSelector from "components/OfficeSelector";
import apiRequest from "utils/requestUtils";
import DeskDetails from "components/DeskDetails";
import OfficeMap from "components/map/OfficeMap";
import { useAuth } from "hooks/useAuth";

function Main() {
	const [selectedOffice, setSelectedOffice] = useState<Office | undefined>();

	const [desks, setDesks] = useState<Desk[]>([]);
	const [selectedDesk, setSelectedDesk] = useState<Desk | undefined>();

	function onDeskSelected(desk: Desk) {
		setSelectedDesk(desk);
		console.log("Selected desk " + desk.name);
	}

	function onOfficeSelected(office: Office) {
		setSelectedOffice(office);
		console.log("Selected office " + office.name);

		apiRequest(`offices/${office.id}/desks`).then((res) => {
			if (res.ok) {
				setDesks(res.data);
			}
		});
	}

	return (
		<>
			<OfficeSelector onChange={onOfficeSelected} />
			{selectedOffice && (
				<OfficeMap
					image={selectedOffice.image}
					desks={desks}
					selectedDesk={selectedDesk}
					onDeskSelected={onDeskSelected}
				/>
			)}
			{selectedDesk && <DeskDetails desk={selectedDesk} />}
		</>
	);
}

export default Main;
