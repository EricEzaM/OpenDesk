import { CRS, ImageOverlay as LImageOverlay, LatLngBounds, Map } from "leaflet";
import "leaflet/dist/leaflet.css";
import { useEffect, useRef } from "react";
import { ImageOverlay, MapContainer } from "react-leaflet";
import MapClickedAtLocationPopup from "./MapClickedAtLocationPopup";

type MaxBounds = {
	min: [number, number];
	max: [number, number];
};

type OfficeMapBaseProps = {
	image: string;
	markers?: JSX.Element[];
};

export type SharedOfficeMapProps = {
	onMaxBoundsChanged?: (bounds: MaxBounds) => void;
};

export default function OfficeMapBase({
	image,
	markers,
	onMaxBoundsChanged,
}: OfficeMapBaseProps & SharedOfficeMapProps) {
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
		let min: [number, number] = [0, 0];
		let max: [number, number] = [image.height, image.width];

		// Update map bounds & center
		mapRef?.current?.setMaxBounds([min, max]);
		mapRef?.current?.setView([image.height / 2, image.width / 2]);

		onMaxBoundsChanged && onMaxBoundsChanged({ min, max });
	}

	function updateImageBounds(image: HTMLImageElement) {
		imageRef?.current?.setBounds(
			new LatLngBounds([
				[0, 0],
				[image.height, image.width],
			])
		);
	}

	function onMapCreatedLocal(map: Map) {
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
					whenCreated={onMapCreatedLocal}
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
					maxBoundsViscosity={0.8}
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
				</MapContainer>
			</div>
		</div>
	);
}
