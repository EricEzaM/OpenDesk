import { useEffect, useMemo } from "react";
import {
	Breadcrumbs,
	Select,
	MenuItem,
	ListSubheader,
	FormControl,
	InputLabel,
	makeStyles,
	Theme,
	createStyles,
} from "@material-ui/core";
import { NavigateNext } from "@material-ui/icons";
import { useOffices } from "hooks/useOffices";
import useOfficeDeskRouteParams from "hooks/useOfficeDeskRouteParams";
import useOfficeLocationFilter from "hooks/useOfficeLocationFilter";
import { Office } from "types";

type OfficeMapping = {
	[key: string]: Office[];
};

type OfficeSelectorProps = {
	compact?: boolean;
};

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		fullWidth: {
			width: "100%",
			minWidth: 120,
		},
	})
);

function OfficeSelector({ compact = false }: OfficeSelectorProps) {
	// =============================================================
	// Hooks and Variables
	// =============================================================

	const classes = useStyles();

	const { officeIdParam, setOfficeParam } = useOfficeDeskRouteParams();
	const {
		officesState: [offices],
		selectedOfficeState: [selectedOffice, setSelectedOffice],
	} = useOffices();

	const {
		availableOffices,
		locations,
		subLocations,
		selectedLocationState: [selectedLocation, setSelectedLocation],
		selectedSublocationState: [selectedSubLocation, setSelectedSublocation],
	} = useOfficeLocationFilter();

	const officeMapping = useMemo<OfficeMapping>(
		() =>
			offices
				// Sort by location and then sublocation
				?.sort(
					(o1, o2) =>
						o1.location.localeCompare(o2.location) ||
						o1.subLocation.localeCompare(o2.subLocation)
				)
				// Create a dictionary where the keys are "Location / Sublocation" and the values are the offices within those combinations
				.reduce((map, office) => {
					const key = `${office.location} / ${office.subLocation}`;
					const currValue = [...(map[key] || [])];

					return {
						...map,
						[key]: [...currValue, office],
					};
				}, {} as OfficeMapping) ?? ({} as OfficeMapping),
		[offices]
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
			{!compact && (
				<Breadcrumbs separator={<NavigateNext />}>
					{/* Location */}
					<FormControl>
						<InputLabel shrink>Location</InputLabel>
						<Select
							value={selectedLocation ?? ""}
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
					</FormControl>

					{/* Sub Location */}
					<FormControl>
						<InputLabel shrink>Sub Location</InputLabel>
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
					</FormControl>

					{/* Office */}
					<FormControl>
						<InputLabel shrink>Office</InputLabel>
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
					</FormControl>
				</Breadcrumbs>
			)}
			{compact && (
				<FormControl className={classes.fullWidth}>
					<InputLabel shrink>Select an Office...</InputLabel>
					<Select
						classes={{
							root: classes.fullWidth,
						}}
						value={selectedOffice?.id ?? ""}
						onChange={(e) => handleOfficeChange(e.target.value as string)}
						displayEmpty
					>
						<MenuItem value="">
							<em>No Selection</em>
						</MenuItem>
						{Object.keys(officeMapping).map((k) => [
							<ListSubheader>{k}</ListSubheader>,
							officeMapping[k]?.map((o) => (
								<MenuItem value={o.id}>{o.name}</MenuItem>
							)),
						])}
					</Select>
				</FormControl>
			)}
		</>
	);
}

export default OfficeSelector;
