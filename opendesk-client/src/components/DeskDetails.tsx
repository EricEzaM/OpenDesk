import BookingsTimeline from "components/BookingsTimeline";
import { Desk } from "types";

import "react-datepicker/dist/react-datepicker.css";
import { useBookings } from "hooks/useBookings";

interface DeskDetailsProps {
	desk: Desk;
}

export default function DeskDetails({ desk }: DeskDetailsProps) {
	// =============================================================
	// Hooks and Variables
	// =============================================================

	const {
		bookingsState: [bookings],
		newBookingStartState: [bookingStart],
		newBookingEndState: [bookingEnd],
		isNewBookingValid,
	} = useBookings();

	// =============================================================
	// Effects
	// =============================================================

	// =============================================================
	// Functions
	// =============================================================

	// =============================================================
	// Render
	// =============================================================

	return (
		<div className="desk-details">
			{desk && (
				<div className="desk-details__title">
					<h3>{desk.name}</h3>
				</div>
			)}

			<BookingsTimeline
				existingBookings={bookings}
				newStart={bookingStart}
				newEnd={bookingEnd}
				isNewValid={isNewBookingValid}
			/>
		</div>
	);
}
