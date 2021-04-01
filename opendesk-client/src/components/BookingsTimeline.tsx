import React, { useEffect, useState } from "react";
import { format, set, setHours, addDays, getHours } from "date-fns";
import { Booking } from "types";

interface BookingsTimelineProps {
	existingBookings: Booking[];
	newBookingStart?: Date;
	newBookingEnd?: Date;
}

function BookingsTimeline({
	existingBookings,
	newBookingStart,
	newBookingEnd,
}: BookingsTimelineProps) {
	// =============================================================
	// Hooks and Variables
	// =============================================================

	const [datesOffset, setDatesOffset] = useState<number>(0);
	const [dates, setDates] = useState<Date[]>([]);

	const [isBookingInvalid, setIsBookingInvalid] = useState(false);

	// =============================================================
	// Effects
	// =============================================================

	useEffect(() => {
		let bookingInvalid = false;
		if (newBookingStart && newBookingEnd) {
			for (let i = 0; i < existingBookings.length; i++) {
				const booking = existingBookings[i];

				bookingInvalid =
					// Start is between existing start and end
					(newBookingStart >= booking.startDateTime &&
						newBookingStart < booking.endDateTime) ||
					// End is between existing start and end
					(newBookingEnd > booking.startDateTime &&
						newBookingEnd <= booking.endDateTime) ||
					// Start is before existing start and end is after existing end (i.e. fully surrounds existing)
					(newBookingStart <= booking.startDateTime &&
						newBookingEnd >= booking.endDateTime);

				if (bookingInvalid) {
					break;
				}
			}
		}
		setIsBookingInvalid(bookingInvalid);
	}, [newBookingStart, newBookingEnd, existingBookings]);

	useEffect(() => {
		let datesDisplay: Date[] = [];

		for (let i = 0; i < 7; i++) {
			datesDisplay.push(
				addDays(
					set(new Date(), {
						hours: 0,
						minutes: 0,
						seconds: 0,
						milliseconds: 0,
					}),
					i + datesOffset
				)
			);
		}

		setDates(datesDisplay);
	}, [datesOffset]);

	// =============================================================
	// Functions
	// =============================================================

	function getDateDisplay(
		start: Date,
		end: Date,
		date: Date,
		isProspective: boolean
	): JSX.Element {
		const startTime = format(start, "haaa");
		const endTime = format(end, "haaa");

		const isAllDay = start <= setHours(date, 6) && end >= setHours(date, 20); // starts <= 6am and ends >= 8pm

		const startsToday = start >= date && start < addDays(date, 1);

		const endsToday = end >= date && end < addDays(date, 1);

		let display = "";
		if (isAllDay) {
			display = "All Day";
		} else if (startsToday && endsToday) {
			if (getHours(start) === 6) {
				display = `Until ${endTime}`;
			} else if (getHours(end) === 20) {
				display = `Until ${endTime}`;
			} else {
				display = `${startTime} to ${endTime}`;
			}
		} else if (startsToday && !endsToday) {
			display = `From ${startTime}`;
		} else if (!startsToday && endsToday) {
			display = `Until ${endTime}`;
		}

		let bookingClasses = ["bookings__booking"];

		if (isProspective) {
			const bookingClassSuffix = isBookingInvalid
				? "--invalid"
				: "--prospective";
			bookingClasses.push("bookings__booking" + bookingClassSuffix);
		}

		return display !== "" ? (
			<div className={bookingClasses.join(" ")}>{display}</div>
		) : (
			<></>
		);
	}

	// =============================================================
	// Render
	// =============================================================

	return (
		<div className="timeline">
			{dates.map((d, i) => (
				<>
					{i === 0 && (
						<button
							className="timeline__scroll-btn"
							onClick={() => setDatesOffset(datesOffset - 1)}
						>
							&lt;
						</button>
					)}
					<div className="bookings">
						<div className="bookings__title">{format(d, "dd/MM")}</div>
						{existingBookings.map((b) =>
							getDateDisplay(b.startDateTime, b.endDateTime, d, false)
						)}
						{newBookingStart &&
							newBookingEnd &&
							getDateDisplay(newBookingStart, newBookingEnd, d, true)}
					</div>
					{i === dates.length - 1 && (
						<button
							className="timeline__scroll-btn"
							onClick={() => setDatesOffset(datesOffset + 1)}
						>
							&gt;
						</button>
					)}
				</>
			))}
		</div>
	);
}

export default BookingsTimeline;
