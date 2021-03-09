import React from "react"

import { MapContainer, Marker, ImageOverlay } from "react-leaflet";
import { CRS, icon, point } from "leaflet";
import "leaflet/dist/leaflet.css"

import DeskLocationIcon from "./icons/desk-location.svg";
import DeskHighlightIcon from "./icons/desk-highlight.svg";

import MapClickedAtLocationPopup from "../map/helpers/MapClickedAtLocationPopup";
import OfficeImage from "../models/OfficeImage"
import Desk from "../models/Desk";

interface Props
{
  image: OfficeImage
  desks: Desk[]
  selectedDesk: Desk | null,
  onDeskSelected: (desk: Desk) => void
}

function OfficeMap({ image, desks, selectedDesk, onDeskSelected }: Props)
{
  let imageCoords = [image.size[1], image.size[0]]

  return (
    <MapContainer
        style={{ height: imageCoords[0] + "px", width: imageCoords[1] + "px" }}
        center={[imageCoords[0] / 2, imageCoords[1] / 2]}
        crs={CRS.Simple}
        attributionControl={false}
        // Disable dragging and zooming 
        zoom={0}
        dragging={false}
        zoomControl={false}
        scrollWheelZoom={false}
        touchZoom={false}
        doubleClickZoom={false}
        keyboard={false}
        boxZoom={false}
      >
        <ImageOverlay
          url={image.url}
          bounds={[[0, 0], [imageCoords[0], imageCoords[1]]]}
        />

        {desks.map(desk =>
        (
          <Marker position={desk.location}
            icon={icon({
              iconUrl: desk.id === selectedDesk?.id ? DeskHighlightIcon : DeskLocationIcon,
              iconSize: point(30, 30)
            })}
            eventHandlers={{
              click: () => {
                onDeskSelected(desk)
              }
            }}
          />
        ))}

        <MapClickedAtLocationPopup/>
			</MapContainer>
  )
}

export default OfficeMap