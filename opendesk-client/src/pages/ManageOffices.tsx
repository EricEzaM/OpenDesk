import {
	Box,
	Button,
	Card,
	createStyles,
	Grid,
	List,
	ListItem,
	ListItemText,
	makeStyles,
	TextField,
	Theme,
	Typography,
} from "@material-ui/core";
import { Add } from "@material-ui/icons";
import OfficeDetailsEditor from "components/OfficeDetailsEditor";
import { StatusData } from "components/StatusAlert";
import { useOffices } from "hooks/useOffices";
import { usePageTitle } from "hooks/usePageTitle";
import { useEffect, useState } from "react";
import { Office, ValidationError } from "types";
import apiRequest from "utils/requestUtils";

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		officeListContainer: {
			padding: "1rem",
		},
		officeListHeader: {
			display: "flex",
			marginBottom: theme.spacing(2),
		},
		officeListFilter: {
			flexGrow: 1,
			marginRight: "1rem",
		},
	})
);

export default function ManageOffices() {
	const classes = useStyles();
	usePageTitle("Manage offices");

	const {
		officesState: [offices],
		refreshOffices,
	} = useOffices();

	const [selectedOffice, setSelectedOffice] = useState<Office | null>();
	const [statusData, setStatusData] = useState<StatusData>({});

	const [filterValue, setFilterValue] = useState<string>();
	const [displayedOffices, setDisplayedOffices] = useState(offices);

	useEffect(() => {
		if (!filterValue) {
			setDisplayedOffices(offices);
			return;
		}

		const lowerFilter = filterValue.toLocaleLowerCase();

		setDisplayedOffices(
			offices?.filter(
				(o) =>
					o.name.toLocaleLowerCase().indexOf(lowerFilter) > -1 ||
					o.location.toLocaleLowerCase().indexOf(lowerFilter) > -1 ||
					o.subLocation.toLocaleLowerCase().indexOf(lowerFilter) > -1
			)
		);
	}, [filterValue, offices]);

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
			<Grid container spacing={2}>
				<Grid item md={3} xs={12}>
					<Card variant="outlined" className={classes.officeListContainer}>
						<List>
							<Box className={classes.officeListHeader}>
								<TextField
									className={classes.officeListFilter}
									variant="outlined"
									size="small"
									placeholder="Filter..."
									inputProps={{
										onInput: (e) => setFilterValue(e.currentTarget.value),
									}}
								/>
								<Button
									variant="contained"
									color="primary"
									startIcon={<Add />}
									onClick={() => onNewOfficeClicked()}
								>
									New Office
								</Button>
							</Box>
							{displayedOffices?.map((o) => (
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
							{offices && offices.length === 0 && (
								<Typography>There are no offices.</Typography>
							)}
							{offices &&
								offices.length !== 0 &&
								displayedOffices?.length === 0 && (
									<Typography>No offices match the filter.</Typography>
								)}
						</List>
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
