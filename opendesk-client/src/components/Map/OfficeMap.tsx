import { darken, lighten } from "@material-ui/core";
import { differenceInHours } from "date-fns";
import isSameDay from "date-fns/esm/fp/isSameDay/index.js";
import {
	CRS,
	divIcon,
	ImageOverlay as LImageOverlay,
	LatLngBounds,
	Map,
	point,
} from "leaflet";
import "leaflet/dist/leaflet.css";
import { useEffect, useRef } from "react";
import ReactDOMServer from "react-dom/server";
import { ImageOverlay, MapContainer, Marker } from "react-leaflet";
import { ReactComponent as DeskLocationIcon } from "resources/desk-location.svg";
import { Booking, Desk, OfficeImage } from "types";
import MapClickedAtLocationPopup from "./MapClickedAtLocationPopup";

const MAP_IMAGE_BORDER = 25;

interface OfficeMapProps {
	image: OfficeImage;
	desks?: Desk[];
	selectedDesk?: Desk;
	bookings?: Booking[];
	newBookingStart?: Date;
	onDeskSelected?: (deskId: string) => void;
}

function OfficeMap({
	image,
	desks,
	selectedDesk,
	bookings,
	newBookingStart,
	onDeskSelected,
}: OfficeMapProps) {
	// =============================================================
	// Hooks & Variables
	// =============================================================

	const imageRef = useRef<LImageOverlay | null>(null);
	const mapRef = useRef<Map>();

	// =============================================================
	// Effects
	// =============================================================

	useEffect(() => {
		// Update image bounds so image scales properly
		updateImageBounds(image);
		// Update maps bounds so that panning works properly for new image
		updateMapMaxBounds(image);
	}, [image]);

	// =============================================================
	// Functions
	// =============================================================

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
						ref={imageRef}
						url={image.url}
						bounds={[
							[0, 0],
							[image.height, image.width],
						]}
					/>

					{desks &&
						newBookingStart &&
						bookings &&
						desks.map((desk) => {
							const fillFree = "#49A078";
							const fillBooked = "#c42525";
							const fillHalf = "#de9a26";

							const hours = bookings
								?.filter(
									(b) =>
										b.deskId === desk.id &&
										isSameDay(b.startDateTime, newBookingStart)
								)
								.reduce(
									(acc, curr) =>
										(acc = differenceInHours(
											curr.endDateTime,
											curr.startDateTime
										)),
									0
								);

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
														desk.id === selectedDesk?.id
															? fill
															: lighten(fill, 0.6),
													stroke:
														desk.id === selectedDesk?.id
															? darken(fill, 0.6)
															: fill,
													strokeWidth: "2px",
												}}
											/>
										),
										iconSize: point(30, 30),
										className: "", // Do not delete: this stops white box from appearing behind marker icons.
									})}
									eventHandlers={{
										click: () => {
											onDeskSelected && onDeskSelected(desk.id);
										},
									}}
								/>
							);
						})}
					<MapClickedAtLocationPopup />
				</MapContainer>
			</div>
		</div>
	);
}

export default OfficeMap;
