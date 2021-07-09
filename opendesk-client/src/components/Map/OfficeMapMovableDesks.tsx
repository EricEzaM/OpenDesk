import { darken } from "@material-ui/core/styles";
import { LeafletMouseEvent, map, Marker } from "leaflet";
import { Desk } from "types";
import DeskMarkerColoured from "./DeskMarkerColoured";
import DeskMarkerDraggableWithinBounds from "./DeskMarkerDraggableWithinBounds";
import OfficeMapBase, { SharedOfficeMapProps } from "./OfficeMapBase";

type OfficeMapMovableDesksProps = {
	image: string;
	desks: Desk[];
	selectedDesk?: Desk;
	onDeskSelected: (deskId: string) => void;
	onSelectedDeskMoved: (x: number, y: number) => void;
};

export default function OfficeMapMovableDesks({
	image,
	desks,
	selectedDesk,
	onDeskSelected,
	onSelectedDeskMoved,
	...props
}: OfficeMapMovableDesksProps & SharedOfficeMapProps) {
	return (
		<OfficeMapBase
			{...props}
			image={image}
			markers={desks.map((d) => (
				<DeskMarkerDraggableWithinBounds
					position={d.diagramPosition}
					selected={d.id === selectedDesk?.id}
					onMoved={onSelectedDeskMoved}
					onSelected={() => onDeskSelected && onDeskSelected(d.id)}
				/>
			))}
		/>
	);
}
