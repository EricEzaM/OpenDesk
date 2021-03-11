import React, { useState } from "react";

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

	const [bkStart, setBkStart] = useState<Date>(setHours(setMinutes(new Date(), 0), 6));
	const [bkEnd, setBkEnd] = useState<Date>(setHours(setMinutes(new Date(), 0), 20));

	function handleStartChange(date: Date) {
		// Don't allow starting bookings after 6pm
		if (date.getHours() >= 18) {
			return;
		}
		// If start was moved to after end, adjust end to still be after start
		if (date > bkEnd) {
			setBkEnd(setHours(setMinutes(date, 0), bkEnd.getHours()));
		}

		setBkStart(date);
	}

	function handleEndChange(date: Date) {
		// Don't allow end bookings before 8am
		if (date.getHours() <= 8) {
			return;
		}
		// If end date was moved to before start, adjust start.
		if (date < bkStart) {
			setBkStart(setHours(setMinutes(date, 0), bkStart.getHours()));
		}

		setBkEnd(date)
	}

	function disallowPast(date: Date) {
		let currentDate = setHours(setMinutes(setSeconds(setMilliseconds(new Date(), 0), 0), 0),0);
		let dateOnly = setHours(setMinutes(setSeconds(setMilliseconds(date, 0), 0), 0),0);

		return dateOnly >= currentDate;
	}

	function CustomTimeInput({ value, onChange }: { value: string, onChange: (event: string) => void }) {
		return (
		<select
			value={value}
			onChange={e => onChange(e.target.value)}
		>
			<option value="06:00">6:00 AM</option>
			<option value="08:00">8:00 AM</option>
			<option value="10:00">10:00 AM</option>
			<option value="12:00">12:00 PM</option>
			<option value="14:00">2:00 PM</option>
			<option value="16:00">4:00 PM</option>
			<option value="18:00">6:00 PM</option>
			<option value="20:00">8:00 PM</option>
		</select>
		)
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

			<div style={{ display: "flex", margin: "0.5em", justifyContent: "center" }}>
				<p style={{margin: "0 0.5em"}}>From</p>
				<DatePicker
					placeholderText="Start Date & Time"
					selected={bkStart}
					onChange={(date) =>date && date instanceof Date && handleStartChange(date)}
					dateFormat="MMMM d, yyyy h:mm aa"
					filterDate={disallowPast}
					showTimeInput
					customTimeInput={React.createElement(CustomTimeInput)}
				/>
				<p style={{margin: "0 0.5em"}}>to</p>
				<DatePicker
					placeholderText="End Date & Time"
					selected={bkEnd}
					onChange={(date) => date && date instanceof Date && handleEndChange(date)}
					dateFormat="MMMM d, yyyy h:mm aa"
					filterDate={disallowPast}
					showTimeInput
					customTimeInput={React.createElement(CustomTimeInput)}
				/>
				<button style={{margin: "0 0.5em", padding: "0 0.5em", whiteSpace: "nowrap"}}>Book Desk</button>
			</div>
		</div>
	);
}

export default DeskDetails;
