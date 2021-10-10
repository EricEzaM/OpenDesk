import { divIcon, point } from "leaflet";
import ReactDOMServer from "react-dom/server";
import { Marker, MarkerProps } from "react-leaflet";
import { ReactComponent as DeskLocationIcon } from "resources/desk-location.svg";

export type DeskMarkerColouredProps = {
	fill: string;
	stroke: string;
};

export default function DeskMarkerColoured({
	fill,
	stroke,
	...props
}: DeskMarkerColouredProps & MarkerProps) {
	return (
		<Marker
			{...props}
			icon={divIcon({
				html: ReactDOMServer.renderToString(
					<DeskLocationIcon
						style={{
							width: "100%",
							height: "100%",
							fill: fill,
							stroke: stroke,
							strokeWidth: "2px",
						}}
					/>
				),
				iconAnchor: point(0, 30), // Anchor the icon so that the pointy end of the icon is at the location.
				iconSize: point(30, 30),
				className: "", // Do not delete: this stops white box from appearing behind marker icons.
			})}
		/>
	);
}
