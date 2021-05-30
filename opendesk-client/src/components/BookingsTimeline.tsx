import React, { useEffect, useState } from "react";
import { format, isSameDay, differenceInCalendarDays } from "date-fns";
import { Booking } from "types";

interface BookingsTimelineProps {
	existingBookings: Booking[];
	newStart?: Date;
	newEnd?: Date;
	isNewValid: boolean;
}

function BookingsTimeline({
	existingBookings,
	newStart,
	newEnd,
	isNewValid,
}: BookingsTimelineProps) {
	// =============================================================
	// Hooks and Variables
	// =============================================================

	const [insertBookingAt, setInsertBookingAt] = useState<number>(-1);

	// =============================================================
	// Effects
	// =============================================================

	useEffect(() => {
		// Determine where to insert the new booking in the display, depending on the existing bookings.
		if (newStart && newEnd) {
			for (let i = 0; i < existingBookings.length; i++) {
				const booking = existingBookings[i];

				if (newStart <= booking.startDateTime) {
					setInsertBookingAt(i);
					break;
				} else if (i === existingBookings.length - 1) {
					setInsertBookingAt(i + 1);
				}
			}
		}
	}, [newStart, newEnd, existingBookings]);

	// =============================================================
	// Functions
	// =============================================================

	// =============================================================
	// Render
	// =============================================================

	return (
		<div className="timeline">
			{existingBookings
				.filter((b, i) => i < insertBookingAt)
				.map((b) =>
					getTimelineContent(b.startDateTime, b.endDateTime, b.user.name, true)
				)}
			{newStart &&
				newEnd &&
				getTimelineContent(newStart, newEnd, "You", isNewValid, false)}
			{existingBookings
				.filter((b, i) => i >= insertBookingAt)
				.map((b) =>
					getTimelineContent(b.startDateTime, b.endDateTime, b.user.name, true)
				)}
		</div>
	);
}

export default BookingsTimeline;

function getTimelineContent(
	s: Date,
	e: Date,
	user: string,
	isValid: boolean,
	isExisting: boolean = true
) {
	const dayDiff = differenceInCalendarDays(e, s);
	const sameDay = isSameDay(e, s);

	const longClass = dayDiff > 0 ? "timeline__container--long" : "";

	const existingClass = isExisting
		? "timeline__container--existing"
		: "timeline__container--new";

	const colourClass = isExisting
		? ""
		: isValid
		? "timeline__container--valid"
		: "timeline__container--invalid";

	return (
		<div
			className={`timeline__container ${existingClass} ${colourClass} ${longClass}`}
		>
			<div
				className="timeline__content"
				style={{ display: "flex", justifyContent: "space-between" }}
			>
				<div>
					<h4>
						{format(s, "haaa EEE LLLL d")} to {sameDay ? format(e, "haaa") : ""}
					</h4>
					{!sameDay && (
						<div style={{ display: "flex", alignContent: "flex-end" }}>
							<h4>{format(e, "haaa EEE LLLL d")}</h4>
							<p>&nbsp;(Over {dayDiff + 1} days)</p>
						</div>
					)}
				</div>
				<div style={{ textAlign: "right" }}>
					<p>{isExisting ? `Booked by ${user}` : "Your Current Selection"}</p>
					{!isValid && (
						<p style={{ color: "#b42e2e" }}>Clashes with another booking.</p>
					)}
				</div>
			</div>
		</div>
	);
}
