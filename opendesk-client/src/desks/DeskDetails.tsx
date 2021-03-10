import React, { useRef, useState } from "react";
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

	const [bkStart, setBkStart] = useState<Date>();
	const [bkEnd, setBkEnd] = useState<Date>();

	const startDateRef = useRef<HTMLInputElement>(null);
	const startTimeRef = useRef<HTMLInputElement>(null);
	const endDateRef = useRef<HTMLInputElement>(null);
	const endTimeRef = useRef<HTMLInputElement>(null);

	function onBookingChanged() {
		setBkStart(
			new Date(startDateRef.current?.value + " " + startTimeRef.current?.value)
		);
		setBkEnd(
			new Date(endDateRef.current?.value + " " + endTimeRef.current?.value)
		);
	}

	return (
		<div>
			{desk && (
				<>
					<p>Id = {desk.id}</p>
					<p>Name = {desk.name}</p>
					<p>Location = {desk.location}</p>
				</>
			)}
			<BookingsTimeline bookings={bookings} prospectiveBookingStart={bkStart} prospectiveBookingEnd={bkEnd}/>

			<input
				ref={startDateRef}
				type="date"
				onChange={(e) => onBookingChanged()}
			/>
			<input
				ref={startTimeRef}
				type="time"
				onChange={(e) => onBookingChanged()}
			/>
			<input
				ref={endDateRef}
				type="date"
				onChange={(e) => onBookingChanged()}
			/>
			<input
				ref={endTimeRef}
				type="time"
				onChange={(e) => onBookingChanged()}
			/>
		</div>
	);
}

export default DeskDetails;
