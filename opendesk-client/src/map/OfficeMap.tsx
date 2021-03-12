import React from "react";

import { MapContainer, Marker, ImageOverlay } from "react-leaflet";
import { CRS, icon, point } from "leaflet";
import "leaflet/dist/leaflet.css";

import DeskLocationIcon from "./icons/desk-location.svg";
import DeskHighlightIcon from "./icons/desk-highlight.svg";

import MapClickedAtLocationPopup from "../map/helpers/MapClickedAtLocationPopup";
import OfficeImage from "../models/OfficeImage";
import Desk from "../models/Desk";

interface Props {
	image: OfficeImage;
	desks: Desk[];
	selectedDesk: Desk | null;
	onDeskSelected: (desk: Desk) => void;
}

function OfficeMap({ image, desks, selectedDesk, onDeskSelected }: Props) {
	let imageBoundsMax = [image.size[1], image.size[0]];

	let height = Math.min(image.size[0], 450); // maximum map display = 600 px

	return (
    <div style={{
      margin: "0.5em",
    }}
    >
			<div
				style={{
					// height: imageCoords[0] + "px",
					// width: imageCoords[1] + "px",
					height: height + "px",
					width: "100%",
					margin: "0 auto",
				}}
			>
				<MapContainer
					style={{
						height: "100%",
						width: "100%",
						border: "solid 5px black",
					}}
					center={[imageBoundsMax[0] / 2, imageBoundsMax[1] / 2]}
					crs={CRS.Simple}
					attributionControl={false}
					// Disable dragging and zooming
					zoom={-5}
					dragging={true}
					zoomControl={false}
					scrollWheelZoom={false}
					touchZoom={false}
					doubleClickZoom={false}
					keyboard={false}
					boxZoom={false}
					maxBounds={[[-25, -25], [imageBoundsMax[0] + 25, imageBoundsMax[1] + 25]]}
					maxBoundsViscosity={1}
				>
					<ImageOverlay
						url={image.url}
						bounds={[
							[0, 0],
							[imageBoundsMax[0], imageBoundsMax[1]],
						]}
					/>

					{desks.map((desk) => (
						<Marker
							position={desk.location}
							icon={icon({
								iconUrl:
									desk.id === selectedDesk?.id
										? DeskHighlightIcon
										: DeskLocationIcon,
								iconSize: point(30, 30),
							})}
							eventHandlers={{
								click: () => {
									onDeskSelected(desk);
								},
							}}
						/>
					))}

					<MapClickedAtLocationPopup />
				</MapContainer>
			</div>
		</div>
	);
}

export default OfficeMap;
