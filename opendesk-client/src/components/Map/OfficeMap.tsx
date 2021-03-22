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
import useOfficeDeskRouteParams from "hooks/useOfficeDeskRouteParams";

const MAP_IMAGE_BORDER = 25;

interface Props {
	image: OfficeImage;
	desks?: Desk[];
	selectedDesk?: Desk;
	onDeskSelected: (desk?: Desk) => void;
}

function OfficeMap({ image, desks, selectedDesk, onDeskSelected }: Props) {
	// =============================================================
	// Hooks & Variables
	// =============================================================

	const { deskIdParam, setDeskParam } = useOfficeDeskRouteParams();

	const imageRef = useRef<LImageOverlay>();
	const mapRef = useRef<Map>();

	// let imageBounds: [number, number] = [image.height, image.width];
	// let imageCenter: [number, number] = [image.height / 2, image.width / 2];

	// =============================================================
	// Effects
	// =============================================================

	useEffect(() => {
		// Update image bounds so image scales properly
		updateImageBounds(image);
		// Update maps bounds so that panning works properly for new image
		updateMapMaxBounds(image);
	}, [image]);

	useEffect(() => {
		handleSelection(deskIdParam);
	}, [deskIdParam]);

	useEffect(() => {
		handleSelection(deskIdParam);
	}, [desks]);

	// =============================================================
	// Functions
	// =============================================================

	function handleSelection(deskId?: string) {
		let changedDesk = deskId && desks?.find((d) => d.id === deskId);
		if (changedDesk) {
			onDeskSelected && onDeskSelected(changedDesk);
			setDeskParam(deskId);
		} else if (desks) {
			// Only clear out the selected desk if desks have been loaded
			onDeskSelected && onDeskSelected(undefined);
			setDeskParam(undefined);
		}
	}

	function updateMapMaxBounds(image: OfficeImage) {
		let min: [number, number] = [-MAP_IMAGE_BORDER, -MAP_IMAGE_BORDER];
		let max: [number, number] = [
			image.height + MAP_IMAGE_BORDER,
			image.width + MAP_IMAGE_BORDER,
		];

		// Update map bounds & center
		mapRef?.current?.setMaxBounds([min, max]);
		mapRef?.current?.setView([image.height / 2, image.width / 2]);
	}

	function updateImageBounds(image: OfficeImage) {
		imageRef?.current?.setBounds(
			new LatLngBounds([
				[0, 0],
				[image.height, image.width],
			])
		);
	}

	function onMapCreated(map: Map) {
		mapRef.current = map;
		updateMapMaxBounds(image);
	}

	// =============================================================
	// Render
	// =============================================================

	return (
		<div className="map-container">
			<div className="map-container__inner">
				<MapContainer
					whenCreated={onMapCreated}
					center={[0, 0]}
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
						bounds={[
							[0, 0],
							[image.height, image.width],
						]}
					/>

					{desks &&
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
