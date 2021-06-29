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
import { useOffices } from "hooks/useOffices";
import { useState } from "react";
import { Office } from "types";
import apiRequest from "utils/requestUtils";

export default function ManageOffices() {
	const {
		officesState: [offices],
		refreshOffices,
	} = useOffices();

	const [selectedOffice, setSelectedOffice] = useState<Office | null>();

	function onOfficeSelected(office: Office) {
		setSelectedOffice(office);
	}

	function onNewOfficeClicked() {
		setSelectedOffice(null);
	}

	function onSave(
		name: string,
		location: string,
		sublocation: string,
		image?: File
	) {
		var formData = new FormData();

		formData.append("Name", name);
		formData.append("Location", location);
		formData.append("SubLocation", sublocation);
		if (image) {
			formData.append("Image", image);
		}

		if (!selectedOffice) {
			// Create new Office
			if (!image) {
				return;
			}
			apiRequest("offices", {
				method: "POST",
				body: formData,
			}).then((res) => {
				refreshOffices();
				setSelectedOffice(undefined);
			});
		} else {
			formData.append("Id", selectedOffice.id);
			// Update existing office
			apiRequest("offices", {
				method: "PATCH",
				body: formData,
			}).then((res) => {
				refreshOffices();
			});
		}
	}

	function onDelete() {
		if (window.confirm("Are you sure?")) {
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
					<OfficeDetailsEditor
						office={selectedOffice}
						onSave={onSave}
						onDelete={onDelete}
					/>
				)}
			</Grid>
		</>
	);
}
