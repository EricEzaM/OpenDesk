import { Booking, Desk } from "types";
import DeskMarkerBookable from "./DeskMarkerBookable";
import OfficeMapBase from "./OfficeMapBase";

interface OfficeMapBookingsDisplayProps {
	image: string;
	desks: Desk[];
	bookings: Booking[];
	newBookingStart: Date;
	selectedDesk?: Desk;
	onDeskSelected?: (deskId: string) => void;
}

export default function OfficeMapBookingsDisplay({
	image,
	desks,
	bookings,
	newBookingStart,
	selectedDesk,
	onDeskSelected,
}: OfficeMapBookingsDisplayProps) {
	return (
		<OfficeMapBase
			image={image}
			markers={
				bookings &&
				newBookingStart &&
				desks.map((d) => (
					<DeskMarkerBookable
						desk={d}
						bookings={bookings}
						newBookingStart={newBookingStart}
						isSelected={d.id === selectedDesk?.id}
						onSelected={onDeskSelected}
					/>
				))
			}
		/>
	);
}
