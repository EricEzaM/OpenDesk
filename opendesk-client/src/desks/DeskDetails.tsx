import React from "react";
import BookingsTimeline from "../bookings/BookingsTimeline";
import Booking from "../models/Booking";
import Desk from "../models/Desk";

interface Props {
	desk: Desk | null;
}

function DeskDetails({ desk }: Props) {
	let bookings: Booking[] = [
		{
			id: "id1",
			deskId: "desk1",
			startDateTime: new Date(2021, 2, 11, 12),
			endDateTime: new Date(2021, 2, 13, 18),
			user: {
				id: "1234",
				name: "John Smith",
				username: "john.smith@email.com",
			},
		},
	];

	return (
		<div>
			{desk && (
				<>
					<p>Id = {desk.id}</p>
					<p>Name = {desk.name}</p>
					<p>Location = {desk.location}</p>
				</>
			)}
			<BookingsTimeline bookings={bookings}/>
		</div>
	);
}

export default DeskDetails;
