import React, { useState } from "react";

import OfficePlanImage from "./OfficePlanImage2.png";
import Desk from "./models/Desk";
import OfficeImage from "./models/OfficeImage";
import OfficeMap from "./map/OfficeMap";
import DeskDetails from "./desks/DeskDetails";

function App()
{
  const [selectedDesk, setSelectedDesk] = useState<Desk | null>(null)

  let officeImage : OfficeImage = {
    url: OfficePlanImage,
    // size: [659, 503]
    size: [864 * 1.3, 435 * 1.3]
  }

  let desks: Desk[] = [
    {
      id: "1",
      location: [366, 215],
      name: "Desk 1"
    },
    {
      id: "2",
      location: [366, 365],
      name: "Desk 2"
    },
    {
      id: "3",
      location: [363, 497],
      name: "Desk 3"
    },
    {
      id: "4",
      location: [422, 220],
      name: "Desk 4"
    },
  ]

  function onDeskSelected(desk: Desk)
  {
    setSelectedDesk(desk)
    console.log("Selected desk " + desk.name)
  }

  return (
    <div style={{ maxWidth: "1024px", margin: "0 auto" }}>
      <h1 style={{ textAlign: "center" }}>OpenDesk</h1>
      <div style={{textAlign:"center", marginTop: "0.5em"}}>
        <select style={{ margin: "0 auto" }}>
          <option>Office 1</option>
          <option>Office 2</option>
          <option>Office 3 With a Really Long name</option>
        </select>
      </div>
      <OfficeMap image={officeImage} desks={desks} selectedDesk={selectedDesk} onDeskSelected={onDeskSelected}/>
      <DeskDetails desk={selectedDesk} />
    </div>
	);
}

export default App;
