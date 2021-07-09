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
import DeskDetailsEditor from "components/DeskDetailsEditor";
import OfficeSelector from "components/OfficeSelector";
import { StatusData } from "components/StatusAlert";
import { useOfficeDesks } from "hooks/useOfficeDesks";
import { useOffices } from "hooks/useOffices";
import { usePageTitle } from "hooks/usePageTitle";
import { useEffect, useState } from "react";
import { Desk, DiagramPosition, ValidationError } from "types";
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

export default function ManageDesks() {
	const classes = useStyles();
	usePageTitle("Manage Desks");

	const {
		selectedOfficeState: [selectedOffice],
	} = useOffices();

	const {
		desksState: [desks],
		refreshDesks,
	} = useOfficeDesks();

	const [newDesk, setNewDesk] = useState<Desk | null>(null);
	const [selectedDesk, setSelectedDesk] = useState<Desk | null>(null);
	const [statusData, setStatusData] = useState<StatusData>({});

	const [filterValue, setFilterValue] = useState<string>();
	const [displayedDesks, setDisplayedDesks] = useState(desks);

	useEffect(() => {
		if (!filterValue) {
			setDisplayedDesks(desks);
			return;
		}

		const lowerFilter = filterValue.toLocaleLowerCase();

		setDisplayedDesks(
			desks?.filter((d) => d.name.toLocaleLowerCase().indexOf(lowerFilter) > -1)
		);
	}, [filterValue, desks]);

	function onDeskSelected(desk: Desk) {
		setNewDesk(null);
		setSelectedDesk(desk);
		setStatusData({});
	}

	function onNewDeskClicked() {
		const newDeskObj: Desk = {
			id: "new-desk",
			name: "",
			diagramPosition: { x: 0, y: 0 },
		};
		setNewDesk(newDeskObj);
		setSelectedDesk(newDeskObj);
		setStatusData({});
	}

	async function onSave(
		name: string,
		position: DiagramPosition
	): Promise<StatusData> {
		let method = newDesk ? "POST" : "PUT";
		let endpoint = newDesk
			? `offices/${selectedOffice?.id}/desks` // TODO: Fix undefined possiblity
			: `desks/${selectedDesk?.id}`;

		let body = {
			id: selectedDesk?.id,
			name: name,
			diagramPosition: position,
		};

		let res = await apiRequest<Desk>(endpoint, {
			method: method,
			headers: {
				"content-type": "application/json",
			},
			body: JSON.stringify(body),
		});

		if (res.data) {
			refreshDesks();
			onDeskSelected(res.data);
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

	let errorContent: React.ReactNode = null;
	if (!selectedOffice) {
		errorContent = <Typography>Select an office.</Typography>;
	} else if (desks && !desks.length) {
		errorContent = <Typography>There are no desks in this office.</Typography>;
	} else if (desks && desks.length && !displayedDesks.length) {
		errorContent = <Typography>No desks match the filter.</Typography>;
	}

	const desksIncludingNew = [...(newDesk ? [newDesk] : []), ...desks];

	return (
		<>
			<Grid container spacing={2}>
				<Grid item md={3} xs={12}>
					<Card variant="outlined" className={classes.officeListContainer}>
						<List>
							<Box marginBottom={1}>
								<OfficeSelector compact />
							</Box>
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
									onClick={() => onNewDeskClicked()}
								>
									New Desk
								</Button>
							</Box>
							{displayedDesks?.map((d) => (
								<ListItem
									key={d.id}
									button
									selected={selectedDesk?.id === d.id}
									onClick={(e) => onDeskSelected(d)}
								>
									<ListItemText primary={d.name} />
								</ListItem>
							))}
							{errorContent}
						</List>
					</Card>
				</Grid>
				{selectedDesk !== null && selectedOffice && (
					<Grid item md={9} xs={12}>
						<DeskDetailsEditor
							office={selectedOffice}
							desk={selectedDesk}
							desks={desksIncludingNew}
							onSave={(name: string, position: DiagramPosition) =>
								onSave(name, position)
							}
							onDelete={() => {}}
							statusData={{}}
							onNewDeskSelected={(deskId) => {
								const desk = desks.find((d) => d.id === deskId);
								if (desk) {
									onDeskSelected(desk);
								}
							}}
						/>
					</Grid>
				)}
			</Grid>
		</>
	);
}
