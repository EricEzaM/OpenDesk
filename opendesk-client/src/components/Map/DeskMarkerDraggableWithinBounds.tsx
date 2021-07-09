import { darken } from "@material-ui/core";
import { LatLngBounds, LatLngBoundsExpression, Marker } from "leaflet";
import { useMap } from "react-leaflet";
import { DiagramPosition } from "types";
import DeskMarkerColoured from "./DeskMarkerColoured";

export type DeskMarkerDraggableWithinBoundsProps = {
	position: DiagramPosition;
	selected: boolean;
	onSelected: () => void;
	onMoved: (x: number, y: number) => void;
};

export default function DeskMarkerDraggableWithinBounds({
	position,
	selected,
	onSelected,
	onMoved,
}: DeskMarkerDraggableWithinBoundsProps) {
	const selectedColour = "#49A078";
	const otherColor = "#aaaaaa";

	const map = useMap();

	function boundsFromExpression(exp: LatLngBoundsExpression) {
		return new LatLngBounds([0, 0], [0, 0]).extend(exp);
	}

	function updatePositionAfterDrag(marker: Marker) {
		const newPos = marker.getLatLng();
		if (map.options.maxBounds) {
			const mapBounds = boundsFromExpression(map.options.maxBounds);
			if (mapBounds?.contains(newPos)) {
				// Move allowed, move to the new position
				onMoved(newPos.lng, newPos.lat);
			} else {
				// Move not allowed, move to the pre-drag position
				marker.setLatLng([position.y, position.x]);
				onMoved(position.x, position.y);
			}
		} else {
			console.error(
				"Map does not have maxbounds set. Map maxbounds is required."
			);
		}
	}

	return (
		<DeskMarkerColoured
			position={[position.y, position.x]}
			fill={selected ? selectedColour : otherColor}
			stroke={selected ? darken(selectedColour, 0.6) : darken(otherColor, 0.6)}
			draggable={selected}
			eventHandlers={{
				click: () => onSelected && onSelected(),
				dragend: (de) => updatePositionAfterDrag(de.target as Marker),
			}}
		/>
	);
}
