import React, { useEffect, useState } from "react";
import DatePicker from "react-datepicker";
import { set, setHours } from "date-fns";

import BookingsTimeline from "components/BookingsTimeline";
import { Desk, Booking } from "types";

import "react-datepicker/dist/react-datepicker.css";
import apiRequest from "utils/requestUtils";

interface Props {
	desk: Desk;
}

function DeskDetails({ desk }: Props) {
	const defaultDate = set(new Date(), {
		hours: 0,
		minutes: 0,
		seconds: 0,
		milliseconds: 0,
	});

	const [bkStart, setBkStart] = useState<Date>(setHours(defaultDate, 6));
	const [bkEnd, setBkEnd] = useState<Date>(setHours(defaultDate, 20));

	const [bookings, setBookings] = useState<Booking[]>([]);

	// Update bookings when desk is changed.
	useEffect(() => {
		apiRequest(`desks/${desk.id}/bookings`).then((res) => {
			if (res.ok) {
				// Parse the bookings from the data, converting ISO date to JS Date Object when the JSON contains a date.
				var bookings = JSON.parse(JSON.stringify(res.data), (k, value) => {
					const isDate = k === "startDateTime" || k === "endDateTime";
					return isDate ? new Date(value) : value;
				});
				setBookings(bookings);
			}
		});
	}, [desk]);

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

	function SubmitBooking() {
		apiRequest(`desks/${desk.id}/bookings`, {
			method: "POST",
			headers: {
				"content-type": "application/json",
			},
			body: JSON.stringify({
				startDateTime: bkStart,
				endDateTime: bkEnd,
			}),
		}).then((res) => {
			if (res.ok) {
				// TODO: Don't Repeat Yourself... this is a duplicate of above code. Make a way to have it be shared/common.
				apiRequest(`desks/${desk.id}/bookings`).then((res) => {
					if (res.ok) {
						// Parse the bookings from the data, converting ISO date to JS Date Object when the JSON contains a date.
						var bookings = JSON.parse(JSON.stringify(res.data), (k, value) => {
							const isDate = k === "startDateTime" || k === "endDateTime";
							return isDate ? new Date(value) : value;
						});
						setBookings(bookings);
					}
				});
				console.log("submitted");
			}
		});
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
				<button
					className="desk-details__submit"
					onClick={() => SubmitBooking()}
				>
					Book Desk
				</button>
			</div>
		</div>
	);
}

export default DeskDetails;
