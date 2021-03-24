import { useEffect, useState } from "react";
import { Office } from "types";
import apiRequest from "utils/requestUtils";
import useOfficeDeskRouteParams from "hooks/useOfficeDeskRouteParams";

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
		<div className="office-selector">
			<select
				className="office-selector__dropdown"
				onChange={(e) => handleChange(e.target.value)}
				value={selectedOfficeId}
			>
				<option value="" disabled>
					-- Select an Office --
				</option>
				{offices && offices.map((o) => <option value={o.id}>{o.name}</option>)}
			</select>
		</div>
	);
}

export default OfficeSelector;
