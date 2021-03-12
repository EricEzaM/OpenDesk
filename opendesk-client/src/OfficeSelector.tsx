import { useEffect, useState } from "react";
import Office from "./models/Office";

interface Props {
	onChange?: (officeId: string) => void;
}

function OfficeSelector({ onChange }: Props) {
	const [offices, setOffices] = useState<Office[]>([]);

	useEffect(() => {
		fetch(process.env.REACT_APP_API_URL + "/api/offices")
			.then((res) => res.json())
			.then(
				(data) => {
					setOffices(data);
				},
				(error) => {
					console.log(error);
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
		</div>
	);
}

export default OfficeSelector;
