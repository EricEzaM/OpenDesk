import React from "react";
import Booking from "../models/Booking";
import {
	Get24hTime,
	GetFullDateWith24hTime,
	GetMonthDayFormatted,
} from "../util/DateUtils";

interface Props {
	bookings: Booking[];
}

interface DateBar {
	start: Date;
	end: Date;
}

/** Width and Offset are expressed as a percentage (out of 100) */
interface InnerDateBar {
	width: number;
	offset: number;
	startsInside: boolean;
	endsInside: boolean;
}

function BookingsTimeline({ bookings }: Props) {
	let currentDate = new Date();

	let dateBarsStartDate = new Date(
		currentDate.getFullYear(),
		currentDate.getMonth(),
		currentDate.getDate(),
		6
	);
	let dateBarsEndDate = new Date(
		currentDate.getFullYear(),
		currentDate.getMonth(),
		currentDate.getDate(),
		21
	);

	let datebars: DateBar[] = [];

	for (let i = 0; i < 7; i++) {
		let start = new Date(dateBarsStartDate.getTime());
		start.setDate(dateBarsStartDate.getDate() + i);

		let end = new Date(dateBarsEndDate.getTime());
		end.setDate(dateBarsEndDate.getDate() + i);

		datebars.push({
			start: start,
			end: end,
		});
	}

	return (
		<div>
			{datebars.map((bar) => {
				return (
					<div style={{ display: "flex", alignItems: "center" }}>
						{/* Date displayed at start of bar */}
						<div style={{ margin: "0 10px" }}>
							{GetMonthDayFormatted(bar.start)}
						</div>

						{/* Timeline Bar itself */}
						<div
							style={{
								width: "500px",
								height: "30px",
								backgroundColor: "whitesmoke",
								position: "relative",
								display: "flex",
								alignItems: "center",
							}}
						>
							{/* Time Display at each end of bar */}
							<div
								style={{
									display: "flex",
									justifyContent: "space-between",
									position: "absolute",
									width: "100%",
									pointerEvents: "none",
								}}
							>
								<div style={{ margin: "0 5px 0 5px", fontSize: "0.7em" }}>
									{Get24hTime(bar.start)}
								</div>
								<div style={{ margin: "0 5px 0 5px", fontSize: "0.7em" }}>
									{Get24hTime(bar.end)}
								</div>
							</div>

							{/* Calculating what should be displayed as the "time taken" on each bar */}
							{bookings.map((b) => {
								const {
									width,
									offset,
									startsInside,
									endsInside,
								} = GetInnerDateBarSizing(b, bar);

								return (
									<div
										style={{
											height: "100%",
											width: width + "%",
											marginLeft: offset + "%",
											backgroundColor: "#bf391f",
											borderRadius: startsInside
												? "999px 0 0 999px"
												: endsInside
												? "0 999px 999px 0"
												: "0",
										}}
										title={
											b.user.name +
											"\r\nFrom " +
											GetFullDateWith24hTime(b.startDateTime) +
											"\r\nTo " +
											GetFullDateWith24hTime(b.endDateTime)
										}
									></div>
								);
							})}
						</div>
					</div>
				);
			})}
		</div>
	);
}

export default BookingsTimeline;

function GetInnerDateBarSizing(booking: Booking, bar: DateBar): InnerDateBar {
	let barLength = bar.end.getTime() - bar.start.getTime();

	let offset = 0;
	let width = 0;

	let isInBar =
		booking.endDateTime >= bar.start && booking.startDateTime <= bar.end;
	let usesBar =
		booking.startDateTime <= bar.start && booking.endDateTime >= bar.end;
	if (!isInBar && !usesBar) {
		return {
			width: 0,
			offset: 0,
			startsInside: true,
			endsInside: true,
		};
	}

	let startsInside = booking.startDateTime >= bar.start;
	let endsInside = booking.endDateTime <= bar.end;

	let bookingEndInBar = booking.endDateTime.getTime() - bar.start.getTime();

	let bookingStartInBar = booking.startDateTime.getTime() - bar.start.getTime();

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
		endsInside: endsInside,
		startsInside: startsInside,
	};
}
