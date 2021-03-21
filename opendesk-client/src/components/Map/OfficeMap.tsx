import { useEffect, useRef } from "react";

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

const MAP_IMAGE_BORDER = 25;

interface Props {
	image: OfficeImage;
	desks: Desk[];
	selectedDesk?: Desk;
	onDeskSelected: (desk: Desk) => void;
}

function OfficeMap({ image, desks, selectedDesk, onDeskSelected }: Props) {
	// =============================================================
	// Hooks & Variables
	// =============================================================

	const { officeId: pOfficeId, deskId: pDeskId } = useParams<any>();

	const imageRef = useRef<LImageOverlay>();
	const mapRef = useRef<Map>();

	const history = useHistory();
	const match = useRouteMatch();

	let imageBounds: [number, number] = [image.height, image.width];
	let imageCenter: [number, number] = [image.height / 2, image.width / 2];

	// =============================================================
	// Effects
	// =============================================================

	useEffect(() => {
		// Update image bounds so image scales properly
		updateImageBounds();
		// Update maps bounds so that panning works properly for new image
		updateMapMaxBounds();
	}, [image]);

	useEffect(() => {
		pDeskId && handleSelection(pDeskId);
	}, [desks]);

	// =============================================================
	// Functions
	// =============================================================

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

	function updateMapMaxBounds() {
		let min: [number, number] = [-MAP_IMAGE_BORDER, -MAP_IMAGE_BORDER];
		let max: [number, number] = [
			imageBounds[0] + MAP_IMAGE_BORDER,
			imageBounds[1] + MAP_IMAGE_BORDER,
		];

		mapRef?.current?.setMaxBounds([min, max]);
		// Update map center
		mapRef?.current?.setView(imageCenter);
	}

	function updateImageBounds() {
		imageRef?.current?.setBounds(new LatLngBounds([[0, 0], imageBounds]));
	}

	function onMapCreated(map: Map) {
		mapRef.current = map;
		updateMapMaxBounds();
	}

	// =============================================================
	// Render
	// =============================================================

	return (
		<div className="map-container">
			<div className="map-container__inner">
				<MapContainer
					whenCreated={onMapCreated}
					center={imageCenter}
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
					maxBoundsViscosity={1}
				>
					<ImageOverlay
						//@ts-ignore
						ref={imageRef}
						url={image.url}
						bounds={[[0, 0], imageBounds]}
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
