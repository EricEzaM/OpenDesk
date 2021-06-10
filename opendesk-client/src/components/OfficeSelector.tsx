import { useEffect, useState, useMemo } from "react";
import { Breadcrumbs, Select, MenuItem } from "@material-ui/core";
import { NavigateNext } from "@material-ui/icons";
import { useOffices } from "hooks/useOffices";
import useOfficeDeskRouteParams from "hooks/useOfficeDeskRouteParams";

function OfficeSelector() {
	// =============================================================
	// Hooks and Variables
	// =============================================================

	const [selectedLocation, setSelectedLocation] = useState("");
	const [selectedSubLocation, setSelectedSublocation] = useState("");

	const { officeIdParam, setOfficeParam } = useOfficeDeskRouteParams();
	const {
		officesState: [offices],
		selectedOfficeState: [selectedOffice, setSelectedOffice],
	} = useOffices();

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

	// Handle office changes when the param id or office list is changed
	useEffect(() => {
		handleOfficeChange(officeIdParam);
	}, [officeIdParam, offices]);

	// =============================================================
	// Functions
	// =============================================================

	function handleLocationChange(location: string) {
		setSelectedLocation(location);
		// Reset downstream options.
		setSelectedSublocation("");
		handleOfficeChange("");
	}

	function handleSubLocationChange(subLocation: string) {
		setSelectedSublocation(subLocation);
		// Reset downstream options.
		handleOfficeChange("");
	}

	function handleOfficeChange(officeId?: string) {
		let newOffice = offices?.find((o) => o.id === officeId);
		if (newOffice) {
			setSelectedLocation(newOffice.location);
			setSelectedSublocation(newOffice.subLocation);
			setSelectedOffice(newOffice);
			// Update route parameter
			setOfficeParam(newOffice.id);
		} else if (offices) {
			// Office are loaded, but there was no match.
			setSelectedOffice(undefined);
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
						handleLocationChange(e.target.value as string);
					}}
					displayEmpty
				>
					<MenuItem value="">
						<em>No Selection</em>
					</MenuItem>
					{locations?.map((ol) => (
						<MenuItem key={ol} value={ol}>
							{ol}
						</MenuItem>
					))}
				</Select>

				{/* Sub Location */}

				<Select
					value={selectedSubLocation}
					onChange={(e) => {
						handleSubLocationChange(e.target.value as string);
					}}
					disabled={selectedLocation === ""}
					displayEmpty
				>
					<MenuItem value="">
						<em>No Selection</em>
					</MenuItem>
					{subLocations?.map((osl) => (
						<MenuItem key={osl} value={osl}>
							{osl}
						</MenuItem>
					))}
				</Select>

				{/* Office */}

				<Select
					value={selectedOffice?.id ?? ""}
					onChange={(e) => handleOfficeChange(e.target.value as string)}
					disabled={selectedSubLocation === ""}
					displayEmpty
				>
					<MenuItem value="">
						<em>No Selection</em>
					</MenuItem>
					{availableOffices?.map((o) => (
						<MenuItem key={o.id} value={o.id}>
							{o.name}
						</MenuItem>
					))}
				</Select>
			</Breadcrumbs>
		</>
	);
}

export default OfficeSelector;
