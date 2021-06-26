import { useMemo, useState } from "react";
import { Office } from "types";
import { useOffices } from "./useOffices";

interface OfficeLocationFilterProps {
	selectedLocationState: [string, (location: string) => void];
	selectedSublocationState: [string, (sublocation: string) => void];
	locations: string[];
	subLocations: string[];
	availableOffices: Office[];
}

export default function useOfficeLocationFilter(): OfficeLocationFilterProps {
	const {
		officesState: [offices],
	} = useOffices();

	const [selectedLocation, setSelectedLocation] = useState<string>("");
	const [selectedSubLocation, setSelectedSublocation] = useState<string>("");

	// Get unique locations.
	const locations = useMemo(
		() =>
			offices
				?.map((o) => o.location)
				.filter((ol, i, arr) => arr.indexOf(ol) === i) ?? [],
		[offices]
	);

	// Get unique sublocations for selected location.
	const subLocations = useMemo(
		() =>
			offices
				?.filter((o) => o.location === selectedLocation)
				.map((o) => o.subLocation)
				.filter((osl, i, arr) => arr.indexOf(osl) === i) ?? [],
		[offices, selectedLocation]
	);

	// Get offices, filtered by the selected location and sublocation.
	const availableOffices = useMemo(
		() =>
			offices?.filter(
				(o) =>
					o.location === selectedLocation &&
					o.subLocation === selectedSubLocation
			) ?? [],
		[offices, selectedLocation, selectedSubLocation]
	);

	return {
		selectedLocationState: [selectedLocation, setSelectedLocation],
		selectedSublocationState: [selectedSubLocation, setSelectedSublocation],
		locations,
		subLocations,
		availableOffices,
	};
}
