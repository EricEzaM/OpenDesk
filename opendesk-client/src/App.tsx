import React, { useState } from "react";

import Desk from "./models/Desk";
import OfficeMap from "./map/OfficeMap";
import DeskDetails from "./desks/DeskDetails";
import OfficeSelector from "./OfficeSelector";
import Office from "./models/Office";
import apiRequest from "./utils/requestUtils";
import Booking from "./models/Booking";

function App() {
	const [selectedOffice, setSelectedOffice] = useState<Office | undefined>();

	const [desks, setDesks] = useState<Desk[]>([]);
	const [selectedDesk, setSelectedDesk] = useState<Desk | undefined>();
	const [selectedDeskBookings, setSelectedDeskBookings] = useState<Booking[]>(
		[]
	);

	function onDeskSelected(desk: Desk) {
		setSelectedDesk(desk);
		console.log("Selected desk " + desk.name);

		apiRequest(`desks/${desk.id}/bookings`).then((b) => {
			var bookings = JSON.parse(JSON.stringify(b), (k, value) => {
				const isDate = k === "startDateTime" || k === "endDateTime";
				return isDate ? new Date(value) : value
			});
			setSelectedDeskBookings(bookings);
			console.log(bookings)
		});
	}

	function onOfficeSelected(office: Office) {
		setSelectedOffice(office);
		console.log("Selected office " + office.name);

		apiRequest(`offices/${office.id}/desks`).then((d) => {
			setDesks(d);
			console.log(d);
		});
	}

	return (
		<div className="container">
			<h1 className="main-title">OpenDesk</h1>
			<OfficeSelector onChange={onOfficeSelected} />
			{selectedOffice && (
				<OfficeMap
					image={selectedOffice.image}
					desks={desks}
					selectedDesk={selectedDesk}
					onDeskSelected={onDeskSelected}
				/>
			)}
			{selectedDesk && <DeskDetails desk={selectedDesk} bookings={selectedDeskBookings}/>}
		</div>
	);
}

export default App;
