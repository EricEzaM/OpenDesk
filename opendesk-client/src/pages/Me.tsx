import { format } from "date-fns";
import { useAuth } from "hooks/useAuth";
import { useEffect, useState } from "react";
import { FullBooking } from "types";
import apiRequest from "utils/requestUtils";

function Me() {
	const { user } = useAuth();
	const [userBookings, setUserBookings] = useState<FullBooking[]>([]);

	useEffect(() => {
		// API Call to get bookings and stuff
		apiRequest(`users/${user?.id}/bookings`).then((res) => {
			if (res.ok) {
				var bookings = JSON.parse(JSON.stringify(res.data), (k, value) => {
					const isDate = k === "startDateTime" || k === "endDateTime";
					return isDate ? new Date(value) : value;
				});

				setUserBookings(bookings);
			}
		});
	}, []);

	return (
		<div>
			<p>You have {userBookings.length} bookings.</p>
			{userBookings.map((fb) => (
				<div
					style={{
						display: "flex",
						gap: "30px",
						backgroundColor: "#f7f7f7",
						margin: "12px 0",
						padding: "5px 10px",
						borderRadius: "10px",
					}}
				>
					<div>
						<label style={{ fontSize: "small", fontWeight: "bold" }}>
							Office
						</label>
						<div>{fb.office.name}</div>
					</div>
					<div>
						<label style={{ fontSize: "small", fontWeight: "bold" }}>
							Desk
						</label>
						<div>{fb.desk.name}</div>
					</div>
					<div>
						<label style={{ fontSize: "small", fontWeight: "bold" }}>
							Start
						</label>
						<div>{format(fb.startDateTime, "dd/MM/yyyy hh:mm a")}</div>
					</div>
					<div>
						<label style={{ fontSize: "small", fontWeight: "bold" }}>End</label>
						<div>{format(fb.endDateTime, "dd/MM/yyyy hh:mm a")}</div>
					</div>
				</div>
			))}
		</div>
	);
}

export default Me;
