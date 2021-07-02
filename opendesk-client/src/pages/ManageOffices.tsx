import {
	Box,
	Button,
	Card,
	CardContent,
	Grid,
	List,
	ListItem,
	ListItemText,
	Typography,
} from "@material-ui/core";
import { Add } from "@material-ui/icons";
import OfficeDetailsEditor from "components/OfficeDetailsEditor";
import { StatusData } from "components/StatusAlert";
import { useOffices } from "hooks/useOffices";
import { useState } from "react";
import { Office, ValidationError } from "types";
import apiRequest from "utils/requestUtils";

export default function ManageOffices() {
	const {
		officesState: [offices],
		refreshOffices,
	} = useOffices();

	const [selectedOffice, setSelectedOffice] = useState<Office | null>();
	const [statusData, setStatusData] = useState<StatusData>({});

	function onOfficeSelected(office: Office) {
		setSelectedOffice(office);
		setStatusData({});
	}

	function onNewOfficeClicked() {
		setSelectedOffice(null);
		setStatusData({});
	}

	async function onSave(
		name: string,
		location: string,
		sublocation: string,
		image: File | null
	): Promise<StatusData> {
		var formData = new FormData();

		formData.append("Name", name);
		formData.append("Location", location);
		formData.append("SubLocation", sublocation);
		if (image) {
			formData.append("Image", image);
		}

		let method = "POST"; // Create new office

		if (selectedOffice) {
			// Update existing office
			method = "PATCH";
			formData.append("Id", selectedOffice.id);
		}

		let res = await apiRequest<Office>("offices", {
			method: method,
			body: formData,
		});

		if (res.data) {
			refreshOffices();
			return {
				severity: "success",
				message: "Saved Successfully.",
			};
		} else {
			return {
				severity: "error",
				message: "Unable to save.",
				errors: (res.problem?.errors as ValidationError[])?.map(
					(ve: ValidationError) => ve.message
				),
			};
		}
	}

	function onDelete() {
		if (window.confirm("Are you sure?")) {
			// TODO: Implement
			alert("Not implemented yet");
		}
	}

	return (
		<>
			<Typography variant="h2" component="h1">
				Manage Offices
			</Typography>
			<Grid container spacing={2}>
				<Grid item md={3} xs={12}>
					<Card variant="outlined">
						<CardContent>
							<List>
								<Box textAlign="right" marginBottom={2}>
									<Button
										variant="contained"
										color="primary"
										startIcon={<Add />}
										onClick={() => onNewOfficeClicked()}
									>
										New Office
									</Button>
								</Box>
								{offices?.map((o) => (
									<ListItem
										key={o.id}
										button
										selected={selectedOffice?.id === o.id}
										onClick={(e) => onOfficeSelected(o)}
									>
										<ListItemText
											primary={o.name}
											secondary={`${o.location} / ${o.subLocation}`}
										/>
									</ListItem>
								))}
							</List>
						</CardContent>
					</Card>
				</Grid>
				{selectedOffice !== undefined && (
					<Grid item md={9} xs={12}>
						<OfficeDetailsEditor
							office={selectedOffice}
							onSave={onSave}
							onDelete={onDelete}
							statusData={statusData}
						/>
					</Grid>
				)}
			</Grid>
		</>
	);
}
