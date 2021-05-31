import { useEffect, useState, useMemo } from "react";
import { Office } from "types";
import apiRequest from "utils/requestUtils";
import useOfficeDeskRouteParams from "hooks/useOfficeDeskRouteParams";
import { Breadcrumbs, Select, MenuItem } from "@material-ui/core";
import { NavigateNext } from "@material-ui/icons";

interface OfficeSelectorProps {
	onOfficeSelected?: (office?: Office) => void;
}

function OfficeSelector({ onOfficeSelected }: OfficeSelectorProps) {
	// =============================================================
	// Hooks and Variables
	// =============================================================

	const [selectedOfficeId, setSelectedOfficeId] = useState<string>("");
	const [offices, setOffices] = useState<Office[]>();
	const [selectedLocation, setSelectedLocation] = useState("");
	const [selectedSubLocation, setSelectedSublocation] = useState("");
	const { officeIdParam, setOfficeParam } = useOfficeDeskRouteParams();

	// Get unique locations.
	const locations = useMemo(
		() =>
			offices
				?.map((o) => o.location)
				.filter((ol, i, arr) => arr.indexOf(ol) === i),
		[offices]
	);

	// Get unique sublocations for selected location.
	const subLocations = useMemo(
		() =>
			offices
				?.filter((o) => o.location === selectedLocation)
				.map((o) => o.subLocation)
				.filter((osl, i, arr) => arr.indexOf(osl) === i),
		[offices, selectedLocation]
	);

	// Get offices, filtered by the selected location and sublocation.
	const availableOffices = useMemo(
		() =>
			offices?.filter(
				(o) =>
					o.location === selectedLocation &&
					o.subLocation === selectedSubLocation
			),
		[offices, selectedLocation, selectedSubLocation]
	);

	// =============================================================
	// Effects
	// =============================================================

	useEffect(() => {
		apiRequest<Office[]>("offices").then((res) => {
			if (res.outcome.isSuccess) {
				setOffices(res.data);
			}
		});
	}, []);

	useEffect(() => {
		// When offices are loaded, check if the route parameter can select an office.
		handleChange(officeIdParam);
	}, [offices]);

	useEffect(() => {
		// When the parameter is changed, update the selection.
		setSelectedOfficeId(officeIdParam ?? "");
		if (!officeIdParam) {
			handleChange(undefined);
		}
	}, [officeIdParam]);

	// =============================================================
	// Functions
	// =============================================================

	function handleChange(officeId?: string) {
		setSelectedOfficeId(officeId ?? "");

		let newOffice = offices?.find((o) => o.id === officeId);
		if (newOffice) {
			// Found a matching office.
			onOfficeSelected && onOfficeSelected(newOffice);
			setOfficeParam(officeId);
			setSelectedLocation(newOffice.location);
			setSelectedSublocation(newOffice.subLocation);
		} else if (offices) {
			// Offices loaded, but there was no match.
			onOfficeSelected && onOfficeSelected(undefined);
			setOfficeParam(undefined);
		}
	}

	// =============================================================
	// Render
	// =============================================================

	return (
		<>
			<Breadcrumbs separator={<NavigateNext />}>
				{/* Location */}
				<Select
					value={selectedLocation}
					onChange={(e) => {
						setSelectedLocation(e.target.value as string);
						// Reset downstream options.
						setSelectedSublocation("");
						handleChange("");
					}}
					displayEmpty
				>
					<MenuItem value="">
						<em>No Selection</em>
					</MenuItem>
					{locations?.map((ol) => (
						<MenuItem value={ol}>{ol}</MenuItem>
					))}
				</Select>

				{/* Sub Location */}

				<Select
					value={selectedSubLocation}
					onChange={(e) => {
						setSelectedSublocation(e.target.value as string);
						// Reset downstream options.
						handleChange("");
					}}
					disabled={selectedLocation === ""}
					displayEmpty
				>
					<MenuItem value="">
						<em>No Selection</em>
					</MenuItem>
					{subLocations?.map((osl) => (
						<MenuItem value={osl}>{osl}</MenuItem>
					))}
				</Select>

				{/* Office */}

				<Select
					value={selectedOfficeId}
					onChange={(e) => handleChange(e.target.value as string)}
					disabled={selectedSubLocation === ""}
					displayEmpty
				>
					<MenuItem value="">
						<em>No Selection</em>
					</MenuItem>
					{availableOffices?.map((o) => (
						<MenuItem value={o.id}>{o.name}</MenuItem>
					))}
				</Select>
			</Breadcrumbs>
		</>
	);
}

export default OfficeSelector;
