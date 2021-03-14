import { useEffect, useState } from "react";
import Office from "./models/Office";
import apiRequest from "./utils/requestUtils";

interface Props {
	onChange?: (officeId: string) => void;
}

function OfficeSelector({ onChange }: Props) {
	const [offices, setOffices] = useState<Office[]>([]);
	const [error, setError] = useState<string | undefined>();

	useEffect(() => {
		apiRequest("/offices").then(
			(json) => {
				setOffices(json);
			},
			(error) => {
				setError(error);
			}
		);
	}, []);

	return (
		<div className="office-selector">
			<select
				className="office-selector__dropdown"
				onChange={(e) => onChange && onChange(e.target.value)}
			>
				<option value="" disabled selected>
					-- Select an Office --
				</option>
				{offices && offices.map((o) => <option value={o.id}>{o.name}</option>)}
			</select>
			{error && <div style={{ fontSize: "small", color: "red" }}>{error}</div>}
		</div>
	);
}

export default OfficeSelector;
