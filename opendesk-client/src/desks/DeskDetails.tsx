import React, { useRef, useState } from "react";

import DatePicker from "react-datepicker";

import BookingsTimeline from "../bookings/BookingsTimeline";
import Booking from "../models/Booking";
import Desk from "../models/Desk";

import "react-datepicker/dist/react-datepicker.css";
import {
	setHours,
	setMilliseconds,
	setMinutes,
	setSeconds,
} from "date-fns";

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

	function disallowPast(date: Date) {
		let currentDate = setHours(setMinutes(setSeconds(setMilliseconds(new Date(), 0), 0), 0),0);
		let dateOnly = setHours(setMinutes(setSeconds(setMilliseconds(date, 0), 0), 0),0);

		return dateOnly >= currentDate;
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
			<BookingsTimeline
				bookings={bookings}
				prospectiveBookingStart={bkStart}
				prospectiveBookingEnd={bkEnd}
			/>

			<div style={{ display: "flex", margin: "0.5em 0" }}>
				<div style={{ marginRight: "1em" }}>
					<p>Start</p>
					<DatePicker
						placeholderText="Click to select a Date & Time"
						selected={bkStart}
						onChange={(date) =>date && date instanceof Date && setBkStart(date)}
						showTimeSelect
						timeIntervals={15}
						minTime={setHours(setMinutes(new Date(), 0), 6)}
						maxTime={setHours(setMinutes(new Date(), 0), 21)}
						dateFormat="MMMM d, yyyy h:mm aa"
						filterDate={disallowPast}
					/>
				</div>
				<div>
					<p>End</p>
					<DatePicker
						placeholderText="Click to select a Date & Time"
						selected={bkEnd}
						onChange={(date) => date && date instanceof Date && setBkEnd(date)}
						showTimeSelect
						timeIntervals={15}
						minTime={setHours(setMinutes(new Date(), 0), 6)}
						maxTime={setHours(setMinutes(new Date(), 0), 21)}
						dateFormat="MMMM d, yyyy h:mm aa"
						filterDate={disallowPast}
					/>
				</div>
			</div>
		</div>
	);
}

export default DeskDetails;
