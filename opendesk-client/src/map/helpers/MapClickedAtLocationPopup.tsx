import { LatLngExpression } from "leaflet"
import React, { useState } from "react"
import { Popup, useMapEvents } from "react-leaflet"

/**
 * Creates a popup at location clicked indicating the coordinates clicked.
 */
function MapClickedAtLocationPopup()
{
  const [position, setPosition] = useState<LatLngExpression>([0,0])

  useMapEvents({
    click(e)
    {
      setPosition(e.latlng)
    }
  })

  return position === null ? null : (
    <Popup position={position} autoPan={false}>
        Clicked {position.toString()}
    </Popup>
  )
}

export default MapClickedAtLocationPopup;