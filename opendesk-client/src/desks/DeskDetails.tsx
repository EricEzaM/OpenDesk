import React, { useState } from "react";

import DatePicker from "react-datepicker";

import BookingsTimeline from "../bookings/BookingsTimeline";
import Booking from "../models/Booking";
import Desk from "../models/Desk";

import "react-datepicker/dist/react-datepicker.css";
import { set } from "date-fns";
import apiRequest from "../utils/requestUtils";

interface Props {
	desk: Desk | null;
	bookings: Booking[];
}

function DeskDetails({ desk, bookings }: Props) {
	const [bkStart, setBkStart] = useState<Date>(
		set(new Date(), { hours: 6, minutes: 0, seconds: 0, milliseconds: 0 })
	);
	const [bkEnd, setBkEnd] = useState<Date>(
		set(new Date(), { hours: 20, minutes: 0, seconds: 0, milliseconds: 0 })
	);

	function handleStartChange(date: Date) {
		// Don't allow starting bookings after
		if (date.getHours() >= 20) {
			return;
		}
		// If start was moved to after end, adjust end to still be after start
		if (date > bkEnd) {
			setBkEnd(set(date, { hours: bkEnd.getHours(), minutes: 0 }));
		}

		setBkStart(date);
	}

	function handleEndChange(date: Date) {
		// Don't allow end bookings before
		if (date.getHours() <= 6) {
			return;
		}
		// If end date was moved to before start, adjust start.
		if (date < bkStart) {
			setBkStart(set(date, { hours: bkStart.getHours(), minutes: 0 }));
		}

		setBkEnd(date);
	}

	function disallowPast(date: Date) {
		let currentDate = set(new Date(), {
			hours: 0,
			minutes: 0,
			seconds: 0,
			milliseconds: 0,
		});
		let dateOnly = set(date, {
			hours: 0,
			minutes: 0,
			seconds: 0,
			milliseconds: 0,
		});

		return dateOnly >= currentDate;
	}

	function CustomTimeInput({
		value,
		onChange,
	}: {
		value: string;
		onChange: (event: string) => void;
	}) {
		return (
			<select value={value} onChange={(e) => onChange(e.target.value)}>
				<option value="06:00">6:00 AM</option>
				<option value="08:00">8:00 AM</option>
				<option value="10:00">10:00 AM</option>
				<option value="12:00">12:00 PM</option>
				<option value="14:00">2:00 PM</option>
				<option value="16:00">4:00 PM</option>
				<option value="18:00">6:00 PM</option>
				<option value="20:00">8:00 PM</option>
			</select>
		);
	}

	return (
		<div className="desk-details">
			{desk && (
				<div className="desk-details__title">
					<h3>{desk.name}</h3>
				</div>
			)}
			<BookingsTimeline
				existingBookings={bookings}
				newBookingStart={bkStart}
				newBookingEnd={bkEnd}
			/>

			<div className="desk-details__datepickers">
				<p className="desk-details__datepickers-text">From</p>
				<DatePicker
					placeholderText="Start Date & Time"
					selected={bkStart}
					onChange={(date) =>
						date && date instanceof Date && handleStartChange(date)
					}
					dateFormat="MMMM d, yyyy h:mm aa"
					filterDate={disallowPast}
					showTimeInput
					customTimeInput={React.createElement(CustomTimeInput)}
				/>
				<p className="desk-details__datepickers-text">to</p>
				<DatePicker
					placeholderText="End Date & Time"
					selected={bkEnd}
					onChange={(date) =>
						date && date instanceof Date && handleEndChange(date)
					}
					dateFormat="MMMM d, yyyy h:mm aa"
					filterDate={disallowPast}
					showTimeInput
					customTimeInput={React.createElement(CustomTimeInput)}
				/>
				<button className="desk-details__submit">Book Desk</button>
			</div>
		</div>
	);
}

export default DeskDetails;
