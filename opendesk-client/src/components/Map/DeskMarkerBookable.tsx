import { darken, lighten } from "@material-ui/core";
import { differenceInHours, isSameDay } from "date-fns";
import { MarkerProps } from "react-leaflet";
import { Booking, Desk } from "types";
import DeskMarkerColoured from "./DeskMarkerColoured";

type DeskMarkerBookableProps = {
	desk: Desk;
	bookings: Booking[];
	newBookingStart: Date;
	isSelected: boolean;
	onSelected?: (deskId: string) => void;
};

export default function DeskMarkerBookable({
	desk,
	bookings,
	newBookingStart,
	isSelected,
	onSelected,
	...props
}: DeskMarkerBookableProps & Omit<MarkerProps, "position">) {
	const fillFree = "#49A078";
	const fillBooked = "#c42525";
	const fillHalf = "#de9a26";

	const hours = bookings
		?.filter(
			(b) => b.deskId === desk.id && isSameDay(b.startDateTime, newBookingStart)
		)
		.reduce(
			(acc, curr) =>
				(acc += differenceInHours(curr.endDateTime, curr.startDateTime)),
			0
		);

	let fill = fillFree;
	if (hours > 7) {
		fill = fillBooked;
	} else if (hours > 0) {
		fill = fillHalf;
	}

	return (
		<DeskMarkerColoured
			position={[desk.diagramPosition.y, desk.diagramPosition.x]}
			fill={isSelected ? fill : lighten(fill, 0.6)}
			stroke={isSelected ? darken(fill, 0.6) : fill}
			eventHandlers={{
				click: () => {
					onSelected && onSelected(desk.id);
				},
			}}
			{...props}
		/>
	);
}
