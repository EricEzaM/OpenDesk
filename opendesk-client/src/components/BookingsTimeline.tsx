import React, { useEffect, useState } from "react";
import { format, set, setHours } from "date-fns";
import { addDays } from "date-fns/esm";
import { Booking } from "types";
import { useAuth } from "hooks/useAuth";

interface BookingsTimelineProps {
	daysToDisplay: number;
	daysToDisplayOffset: number;
	existingBookings: Booking[];
	newBookingStart?: Date;
	newBookingEnd?: Date;
}

interface DateBar {
	start: Date;
	end: Date;
}

/** Width and Offset are expressed as a percentage (out of 100) */
interface InnerDateBar {
	width: number;
	offset: number;
}

function BookingsTimeline({
	daysToDisplay,
	daysToDisplayOffset,
	existingBookings,
	newBookingStart,
	newBookingEnd,
}: BookingsTimelineProps) {
	// =============================================================
	// Hooks and Variables
	// =============================================================

	const [dateBars, setDateBars] = useState<DateBar[]>([]);
	const [isBookingInvalid, setIsBookingInvalid] = useState<boolean>(false);
	const { user } = useAuth();

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

	useEffect(() => {}, [daysToDisplayOffset]);

	useEffect(() => {
		let datebars: DateBar[] = [];
		let dateBarsStartTime = set(new Date(), {
			hours: 6,
			minutes: 0,
			milliseconds: 0,
		});

		const dateBarsEndTime = setHours(dateBarsStartTime, 20);

		for (let i = 0; i < daysToDisplay; i++) {
			let start = addDays(
				new Date(dateBarsStartTime.getTime()),
				i + daysToDisplayOffset
			);
			let end = addDays(
				new Date(dateBarsEndTime.getTime()),
				i + daysToDisplayOffset
			);

			datebars.push({
				start: start,
				end: end,
			});
		}

		setDateBars(datebars);
	}, [daysToDisplay, daysToDisplayOffset]);

	// =============================================================
	// Render
	// =============================================================

	return (
		<div>
			{dateBars.map((bar) => {
				const {
					width: newBookingWidth,
					offset: newBookingOffset,
				} = getInnerDateBarSizing(
					newBookingStart ?? new Date(),
					newBookingEnd ?? new Date(),
					bar
				);

				return (
					<div className="booking-container">
						{/* Date displayed at start of bar */}
						<div className="booking-container__date">
							{format(bar.start, "EEEEEE dd/MM")}
						</div>

						{/* Timeline Bar itself */}
						<div className="booking-timeline-bar">
							{/* Calculating what should be displayed as the "time taken" on each bar */}
							{existingBookings.map((existingBooking) => {
								const {
									width: existingBookingWidth,
									offset: existingBookingOffset,
								} = getInnerDateBarSizing(
									existingBooking.startDateTime,
									existingBooking.endDateTime,
									bar
								);

								return (
									<div
										className={`booking-timeline-bar__time-booked ${
											existingBooking.user.id === user?.id
												? "booking-timeline-bar__time-booked--owned"
												: ""
										}`}
										style={{
											width: existingBookingWidth + "%",
											marginLeft: existingBookingOffset + "%",
										}}
										title={
											existingBooking.user.name +
											"\r\nFrom " +
											format(
												existingBooking.startDateTime,
												"dd/MM/yyyy h:mm bb"
											) +
											"\r\nTo " +
											format(existingBooking.endDateTime, "dd/MM/yyyy h:mm bb")
										}
									></div>
								);
							})}

							{/* Display of the prospective booking user has selected */}
							{newBookingStart && newBookingEnd && (
								<div
									className={`booking-timeline-bar__time-booked booking-timeline-bar__time-booked--prospective ${
										isBookingInvalid
											? "booking-timeline-bar__time-booked--clashes"
											: ""
									}`}
									style={{
										width: newBookingWidth + "%",
										marginLeft: newBookingOffset + "%",
									}}
									title={
										"\r\nFrom " +
										format(newBookingStart, "dd/MM/yyyy h:mm bb") +
										"\r\nTo " +
										format(newBookingEnd, "dd/MM/yyyy h:mm bb")
									}
								></div>
							)}

							{/* Time Display at each end of bar */}
							<div className="booking-timeline-bar__time-display">
								<span>{format(bar.start, "HH:mm")}</span>
								<span>
									{format(set(new Date(), { hours: 13, minutes: 0 }), "HH:mm")}
								</span>
								<span>{format(bar.end, "HH:mm")}</span>
							</div>
						</div>
					</div>
				);
			})}
		</div>
	);
}

export default BookingsTimeline;

function getInnerDateBarSizing(
	bStart: Date,
	bEnd: Date,
	bar: DateBar
): InnerDateBar {
	let barLength = bar.end.getTime() - bar.start.getTime();

	let offset = 0;
	let width = 0;

	let isInBar = bEnd >= bar.start && bStart <= bar.end;
	let usesBar = bStart <= bar.start && bEnd >= bar.end;
	if (!isInBar && !usesBar) {
		return {
			width: 0,
			offset: 0,
		};
	}

	let startsInside = bStart >= bar.start;
	let endsInside = bEnd <= bar.end;

	let bookingEndInBar = bEnd.getTime() - bar.start.getTime();

	let bookingStartInBar = bStart.getTime() - bar.start.getTime();

	// Booking starts and ends outside of bar.
	if (!startsInside && !endsInside) {
		offset = 0;
		width = 1;
	}

	// Booking Starts outside, ends inside of bar.
	if (!startsInside && endsInside) {
		offset = 0;
		width = bookingEndInBar / barLength;
	}

	// Booking Starts inside, ends outside of bar.
	if (startsInside && !endsInside) {
		offset = bookingStartInBar / barLength;
		width = 1 - offset;
	}

	// Booking starts and ends inside of bar.
	if (startsInside && endsInside) {
		offset = bookingStartInBar / barLength;
		width = bookingEndInBar / barLength - offset;
	}

	width *= 100;
	offset *= 100;

	return {
		width: width,
		offset: offset,
	};
}
