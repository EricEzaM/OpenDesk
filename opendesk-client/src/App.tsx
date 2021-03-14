import React, { useEffect, useState } from "react";

import Desk from "./models/Desk";
import OfficeMap from "./map/OfficeMap";
import DeskDetails from "./desks/DeskDetails";
import OfficeSelector from "./OfficeSelector";
import Office from "./models/Office";

function App() {
	const [selectedDesk, setSelectedDesk] = useState<Desk | undefined>();
	const [selectedOffice, setSelectedOffice] = useState<Office | undefined>();

	let desks: Desk[] = [
		{
			id: "1",
			location: [366, 215],
			name: "Desk 1",
		},
		{
			id: "2",
			location: [366, 365],
			name: "Desk 2",
		},
		{
			id: "3",
			location: [363, 497],
			name: "Desk 3",
		},
		{
			id: "4",
			location: [422, 220],
			name: "Desk 4",
		},
	];

	function onDeskSelected(desk: Desk) {
		setSelectedDesk(desk);
		console.log("Selected desk " + desk.name);
	}

	function onOfficeSelected(office: Office) {
		setSelectedOffice(office);
		console.log("Selected office " + office.name);
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
			{selectedDesk && <DeskDetails desk={selectedDesk} />}
		</div>
	);
}

export default App;
