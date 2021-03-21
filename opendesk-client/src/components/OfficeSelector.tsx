import { useEffect, useState } from "react";
import { useParams } from "react-router";
import { Office } from "types";
import apiRequest from "utils/requestUtils";

interface Props {
	onChange?: (office: Office) => void;
}

function OfficeSelector({ onChange }: Props) {
	const { officeId } = useParams<any>();

	const [offices, setOffices] = useState<Office[]>([]);
	const [error, setError] = useState<string | undefined>();

	function handleChange(officeId: string) {
		let changedOffice = offices.find((o) => o.id === officeId);
		changedOffice && onChange && onChange(changedOffice);
	}

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

	useEffect(() => {
		if (officeId) {
			handleChange(officeId);
		}
	}, [offices]);

	return (
		<div className="office-selector">
			<select
				className="office-selector__dropdown"
				onChange={(e) => handleChange(e.target.value)}
				defaultValue={officeId ?? ""}
			>
				<option value="" disabled>
					-- Select an Office --
				</option>
				{offices && offices.map((o) => <option value={o.id}>{o.name}</option>)}
			</select>
			{error && <div style={{ fontSize: "small", color: "red" }}>{error}</div>}
		</div>
	);
}

export default OfficeSelector;
