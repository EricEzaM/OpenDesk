import { Marker } from "leaflet";
import { Desk } from "types";
import DeskMarkerColoured from "./DeskMarkerColoured";
import OfficeMapBase from "./OfficeMapBase";

interface OfficeMapMovableDesksProps {
	image: string;
	desks: Desk[];
	selectedDesk?: Desk;
	onDeskSelected?: (deskId: string) => void;
	onSelectedDeskMoved?: (x: number, y: number) => void;
}

export default function OfficeMapMovableDesks({
	image,
	desks,
	selectedDesk,
	onDeskSelected,
	onSelectedDeskMoved,
}: OfficeMapMovableDesksProps) {
	const selectedColour = "green";
	const otherColor = "red";

	return (
		<OfficeMapBase
			image={image}
			markers={desks.map((d) => (
				<DeskMarkerColoured
					position={[d.diagramPosition.y, d.diagramPosition.x]}
					fill={d.id === selectedDesk?.id ? selectedColour : otherColor}
					stroke={d.id === selectedDesk?.id ? selectedColour : otherColor}
					draggable={d.id === selectedDesk?.id}
					eventHandlers={{
						click: () => onDeskSelected && onDeskSelected(d.id),
						dragend: (de) => {
							if (onSelectedDeskMoved) {
								const marker = de.target as Marker;
								const newPos = marker.getLatLng();
								onSelectedDeskMoved(newPos.lng, newPos.lat);
							}
						},
					}}
				/>
			))}
		/>
	);
}
