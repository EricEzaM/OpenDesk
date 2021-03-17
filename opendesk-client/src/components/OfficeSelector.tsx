import { useEffect, useState } from "react";
import { Office } from "types";
import apiRequest from "utils/requestUtils";

interface Props {
	onChange?: (office: Office) => void;
}

function OfficeSelector({ onChange }: Props) {
	const [offices, setOffices] = useState<Office[]>([]);
	const [error, setError] = useState<string | undefined>();

	useEffect(() => {
		apiRequest("offices").then(
			(res) => {
				if (res.ok) {
					setOffices(res.data);
				}
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
				onChange={(e) => {
					let changedOffice = offices.find((o) => o.id === e.target.value);
					changedOffice && onChange && onChange(changedOffice);
				}}
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
