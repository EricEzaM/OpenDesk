import React, { useEffect, useRef } from "react";
import ReactDOMServer from "react-dom/server";

import { MapContainer, Marker, ImageOverlay } from "react-leaflet";
import {
	CRS,
	point,
	ImageOverlay as LImageOverlay,
	LatLngBounds,
	Map,
	divIcon,
} from "leaflet";
import "leaflet/dist/leaflet.css";

import { ReactComponent as DeskLocationIcon } from "resources/desk-location.svg";

import MapClickedAtLocationPopup from "./MapClickedAtLocationPopup";
import { OfficeImage } from "types";
import useOfficeDeskRouteParams from "hooks/useOfficeDeskRouteParams";
import { useOfficeDesks } from "hooks/useOfficeDesks";
import { useBookings } from "hooks/useBookings";
import { differenceInHours } from "date-fns";
import { darken, lighten } from "@material-ui/core";
import isSameDay from "date-fns/esm/fp/isSameDay/index.js";

const MAP_IMAGE_BORDER = 25;

interface OfficeMapProps {
	image: OfficeImage;
}

function OfficeMap({ image }: OfficeMapProps) {
	// =============================================================
	// Hooks & Variables
	// =============================================================

	const imageRef = useRef<LImageOverlay>();
	const mapRef = useRef<Map>();

	const { deskIdParam, setDeskParam } = useOfficeDeskRouteParams();
	const {
		desksState: [desks],
		selectedDeskState: [selectedDesk, setSelectedDesk],
	} = useOfficeDesks();

	const {
		bookingsState: [bookings],
		newBookingStartState: [newBookingStart]
	} = useBookings();

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
			setSelectedDesk(changedDesk);
			setDeskParam(deskId);
		} else if (desks.length > 0) {
			// Only clear out the selected desk if desks have been loaded
			setSelectedDesk(undefined);
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
						desks.map((desk) => {

							const fillFree = "#49A078";
							const fillBooked = "#c42525"; 
							const fillHalf = "#de9a26";

							const hours = bookings
								.filter(b => b.deskId === desk.id && isSameDay(b.startDateTime, newBookingStart))
								.reduce((acc, curr) => acc = differenceInHours(curr.endDateTime, curr.startDateTime), 0)

							let fill = fillFree;
							if (hours > 7) {
								fill = fillBooked;
							} else if (hours > 0) {
								fill = fillHalf;
							}

							return (
								<Marker
									position={[desk.diagramPosition.x, desk.diagramPosition.y]}
									icon={divIcon({
										html: ReactDOMServer.renderToString(
											<DeskLocationIcon
												style={{
													width: "100%",
													height: "100%",
													fill:
														desk.id === selectedDesk?.id ? fill : lighten(fill, 0.6),
													stroke: desk.id === selectedDesk?.id ? darken(fill, 0.6) : fill,
													strokeWidth: "2px",
												}}
											/>
										),
										iconSize: point(30, 30),
										className: "", // Do not delete: this stops white box from appearing behind marker icons.
									})}
									eventHandlers={{
										click: () => {
											handleSelection(desk.id);
										},
									}}
								/>
								)
						})
					}
					<MapClickedAtLocationPopup />
				</MapContainer>
			</div>
		</div>
	);
}

export default OfficeMap;
