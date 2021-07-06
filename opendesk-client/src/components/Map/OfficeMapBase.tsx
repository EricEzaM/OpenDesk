import { CRS, ImageOverlay as LImageOverlay, LatLngBounds, Map } from "leaflet";
import "leaflet/dist/leaflet.css";
import { useEffect, useRef } from "react";
import { ImageOverlay, MapContainer } from "react-leaflet";
import MapClickedAtLocationPopup from "./MapClickedAtLocationPopup";

const MAP_IMAGE_BORDER = 25;

interface OfficeMapBaseProps {
	image: string;
	markers?: JSX.Element[];
}

export default function OfficeMapBase({ image, markers }: OfficeMapBaseProps) {
	// =============================================================
	// Hooks & Variables
	// =============================================================

	const imageRef = useRef<LImageOverlay | null>(null);
	const mapRef = useRef<Map>();

	// =============================================================
	// Effects
	// =============================================================

	useEffect(() => {
		let img = new Image();
		img.src = image;

		img.onload = () => {
			// Update image bounds so image scales properly
			updateImageBounds(img);
			// Update maps bounds so that panning works properly for new image
			updateMapMaxBounds(img);
		};
	}, [image]);

	// =============================================================
	// Functions
	// =============================================================

	function updateMapMaxBounds(image: HTMLImageElement) {
		let min: [number, number] = [-MAP_IMAGE_BORDER, -MAP_IMAGE_BORDER];
		let max: [number, number] = [
			image.height + MAP_IMAGE_BORDER,
			image.width + MAP_IMAGE_BORDER,
		];

		// Update map bounds & center
		mapRef?.current?.setMaxBounds([min, max]);
		mapRef?.current?.setView([image.height / 2, image.width / 2]);
	}

	function updateImageBounds(image: HTMLImageElement) {
		imageRef?.current?.setBounds(
			new LatLngBounds([
				[0, 0],
				[image.height, image.width],
			])
		);
	}

	function onMapCreated(map: Map) {
		mapRef.current = map;
		let img = new Image();
		img.src = image;
		updateMapMaxBounds(img);
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
					// Disable zooming
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
						url={image}
						bounds={[
							[0, 0],
							[0, 0],
						]}
					/>

					{markers && markers.map((m) => m)}

					<MapClickedAtLocationPopup />
				</MapContainer>
			</div>
		</div>
	);
}
