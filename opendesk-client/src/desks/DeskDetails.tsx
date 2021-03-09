import React from "react";
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

	let datebars = [];

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
			{desk && (
				<>
					<p>Id = {desk.id}</p>
					<p>Name = {desk.name}</p>
					<p>Location = {desk.location}</p>
				</>
			)}

			{datebars.map((bar) => {
				let barLength = bar.end.getTime() - bar.start.getTime();

				return (
          <div style={{ display: "flex", alignItems: "center" }}>
            <div style={{margin: "0 10px"}}>
              {bar.start.toLocaleDateString([], {
                month: "numeric",
                day: "numeric",
              })}
            </div>
						<div
							style={{
								width: "500px",
								height: "30px",
								backgroundColor: "whitesmoke",
                position: "relative",
                display: "flex",
                alignItems: "center"
							}}
						>
							<div
								style={{
									display: "flex",
									justifyContent: "space-between",
									position: "absolute",
                  width: "100%",
                  pointerEvents: "none"
								}}
							>
								<div style={{ margin: "0 5px 0 5px", fontSize: "0.7em" }}>
									{bar.start.toLocaleTimeString([], {
										hour: "2-digit",
										minute: "2-digit",
									})}
								</div>
								<div style={{ margin: "0 5px 0 5px", fontSize: "0.7em" }}>
									{bar.end.toLocaleTimeString([], {
										hour: "2-digit",
										minute: "2-digit",
									})}
								</div>
							</div>
							{bookings.map((b) => {
								let offset = 0;
								let width = 0;

								let isInBar =
									b.endDateTime >= bar.start && b.startDateTime <= bar.end;
								let usesBar =
									b.startDateTime <= bar.start && b.endDateTime >= bar.end;
								if (!isInBar && !usesBar) {
									return <></>;
								}

								let startsInside = b.startDateTime >= bar.start;
								let endsInside = b.endDateTime <= bar.end;

								let bookingEndInBar =
									b.endDateTime.getTime() - bar.start.getTime();

								let bookingStartInBar =
									b.startDateTime.getTime() - bar.start.getTime();

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

								return (
									<div
										style={{
											height: "100%",
											width: width + "%",
											marginLeft: offset + "%",
                      backgroundColor: "#bf391f",
                      borderRadius: startsInside ? "999px 0 0 999px" : endsInside ? "0 999px 999px 0" : "0"
                    }}
										title={b.user.name + "\r\nFrom " + b.startDateTime.toLocaleDateString([], {year:"numeric", month: "numeric", day: "numeric", hour: "2-digit", minute: "2-digit"}) + "\r\nTo " + b.endDateTime.toLocaleDateString([], {year:"numeric", month: "numeric", day: "numeric", hour: "2-digit", minute: "2-digit"})}
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

export default DeskDetails;
