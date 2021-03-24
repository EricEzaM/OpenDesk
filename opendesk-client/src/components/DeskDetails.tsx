import React, { useEffect, useState } from "react";
import { set, setHours } from "date-fns";

import BookingsTimeline from "components/BookingsTimeline";
import DatePicker from "components/DatePicker";
import { Desk, Booking } from "types";

import "react-datepicker/dist/react-datepicker.css";
import apiRequest from "utils/requestUtils";

interface DeskDetailsProps {
	desk: Desk;
}

function DeskDetails({ desk }: DeskDetailsProps) {
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
		apiRequest<Booking[]>(`desks/${desk.id}/bookings`).then((res) => {
			if (res.outcome.isSuccess && res.data) {
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

	function SubmitBooking() {
		apiRequest<Booking>(`desks/${desk.id}/bookings`, {
			method: "POST",
			headers: {
				"content-type": "application/json",
			},
			body: JSON.stringify({
				startDateTime: bkStart,
				endDateTime: bkEnd,
			}),
		}).then((res) => {
			if (res.outcome.isSuccess) {
				// TODO: Don't Repeat Yourself... this is a duplicate of above code. Make a way to have it be shared/common.
				apiRequest<Booking[]>(`desks/${desk.id}/bookings`).then((res) => {
					if (res.outcome?.isSuccess) {
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
					onChange={(date) => handleStartChange(date)}
				/>
				<p className="desk-details__datepickers-text">to</p>
				<DatePicker
					placeholderText="End Date & Time"
					selected={bkStart}
					onChange={(date) => handleEndChange(date)}
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
