import { divIcon, point } from "leaflet";
import React from "react";
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
	...rest
}: DeskMarkerColouredProps & MarkerProps) {
	return (
		<Marker
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
				iconSize: point(30, 30),
				className: "", // Do not delete: this stops white box from appearing behind marker icons.
			})}
			{...rest}
		/>
	);
}
