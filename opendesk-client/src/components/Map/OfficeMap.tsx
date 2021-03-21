import React, { useEffect, useRef } from "react";

import { MapContainer, Marker, ImageOverlay } from "react-leaflet";
import {
	CRS,
	icon,
	point,
	ImageOverlay as LImageOverlay,
	LatLngBounds,
	Map,
} from "leaflet";
import "leaflet/dist/leaflet.css";

import DeskLocationIcon from "resources/desk-location.svg";
import DeskHighlightIcon from "resources/desk-highlight.svg";

import MapClickedAtLocationPopup from "./MapClickedAtLocationPopup";
import { Desk, OfficeImage } from "types";
import { useHistory, useParams } from "react-router";
import { compile } from "path-to-regexp";
import { useRouteMatch } from "react-router-dom";

interface Props {
	image: OfficeImage;
	desks: Desk[];
	selectedDesk?: Desk;
	onDeskSelected: (desk: Desk) => void;
}

function OfficeMap({ image, desks, selectedDesk, onDeskSelected }: Props) {
	const { officeId: pOfficeId, deskId: pDeskId } = useParams<any>();
	let imageBoundsMax = [image.height, image.width];

	let displayHeight = Math.min(image.width, 450); // maximum map display = 600 px

	const ref = useRef<LImageOverlay>();
	const mapRef = useRef<Map>();

	// Image Change
	useEffect(() => {
		// Update image bounds so image scales properly
		ref &&
			ref.current &&
			ref.current.setBounds(
				new LatLngBounds([
					[0, 0],
					[imageBoundsMax[0], imageBoundsMax[1]],
				])
			);

		// Update maps bounds so that panning works properly for new image
		mapRef &&
			mapRef.current &&
			mapRef.current.setMaxBounds([
				[-25, -25],
				[imageBoundsMax[0] + 25, imageBoundsMax[1] + 25],
			]);
	}, [image]);

	const history = useHistory();
	const match = useRouteMatch();

	useEffect(() => {
		if (pDeskId) {
			handleSelection(pDeskId);
		}
	}, [desks]);

	function handleSelection(deskId: string) {
		let changedDesk = desks.find((d) => d.id === deskId);
		changedDesk && onDeskSelected && onDeskSelected(changedDesk);
		history.replace({
			pathname: compile(match.path)({
				officeId: pOfficeId,
				deskId: deskId,
			}),
		});
	}

	return (
		<div className="map-container">
			<div
				className="map-container__inner"
				style={{
					height: displayHeight + "px",
				}}
			>
				<MapContainer
					whenCreated={(map) => (mapRef.current = map)}
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
					maxBounds={[
						[-25, -25],
						[imageBoundsMax[0] + 25, imageBoundsMax[1] + 25],
					]}
					maxBoundsViscosity={1}
				>
					<ImageOverlay
						//@ts-ignore
						ref={ref}
						url={image.url}
						bounds={[
							[0, 0],
							[imageBoundsMax[0], imageBoundsMax[1]],
						]}
					/>

					{desks.length > 0 &&
						desks.map((desk) => (
							<Marker
								position={[desk.diagramPosition.x, desk.diagramPosition.y]}
								icon={icon({
									iconUrl:
										desk.id === selectedDesk?.id
											? DeskHighlightIcon
											: DeskLocationIcon,
									iconSize: point(30, 30),
								})}
								eventHandlers={{
									click: () => {
										handleSelection(desk.id);
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
