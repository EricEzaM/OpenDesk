import { useEffect, useRef, useState } from "react";
import { Office } from "types";
import apiRequest from "utils/requestUtils";
import useOfficeDeskRouteParams from "hooks/useOfficeDeskRouteParams";
import { Breadcrumbs, Select, MenuItem } from "@material-ui/core";
import { NavigateNext, ExpandMore } from "@material-ui/icons";

interface OfficeSelectorProps {
	onOfficeSelected?: (office?: Office) => void;
}

function OfficeSelector({ onOfficeSelected }: OfficeSelectorProps) {
	// =============================================================
	// Hooks and Variables
	// =============================================================

	const [selectedOfficeId, setSelectedOfficeId] = useState<string>("");
	const [offices, setOffices] = useState<Office[]>();

	const { officeIdParam, setOfficeParam } = useOfficeDeskRouteParams();

	const [menuAnchorEl, setMenuAnchorEl] = useState<Element>();

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
				<Select value="Brisbane, Australia">
					<MenuItem value="Brisbane, Australia">Brisbane, Australia</MenuItem>
					<MenuItem value="Level 11">Perth, Australia</MenuItem>
				</Select>
				<Select value="Edward Street">
					<MenuItem value="Edward Street">Edward Street</MenuItem>
					<MenuItem value="Level 11">Milton</MenuItem>
				</Select>
				<Select value="Level 3">
					<MenuItem value="Level 3">Level 3</MenuItem>
					<MenuItem value="Level 11">Level 11</MenuItem>
				</Select>
			</Breadcrumbs>
		</>
	);
}

export default OfficeSelector;
