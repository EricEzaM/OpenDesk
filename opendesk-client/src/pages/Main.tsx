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
	const [selectedDeskBookings, setSelectedDeskBookings] = useState<Booking[]>(
		[]
	);

	function onDeskSelected(desk: Desk) {
		setSelectedDesk(desk);
		console.log("Selected desk " + desk.name);

		apiRequest(`desks/${desk.id}/bookings`).then((res) => {
			if (res.ok) {
				// Parse the bookings from the data, converting ISO date to JS Date Object when the JSON contains a date.
				var bookings = JSON.parse(JSON.stringify(res.data), (k, value) => {
					const isDate = k === "startDateTime" || k === "endDateTime";
					return isDate ? new Date(value) : value;
				});
				setSelectedDeskBookings(bookings);
			}
		});
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
			{selectedDesk && (
				<DeskDetails desk={selectedDesk} bookings={selectedDeskBookings} />
			)}
		</>
	);
}

export default Main;
