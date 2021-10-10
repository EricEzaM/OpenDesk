import {
	Button,
	Card,
	CardContent,
	CircularProgress,
	createStyles,
	Grid,
	makeStyles,
	TextField,
	Theme,
	Typography,
} from "@material-ui/core";
import { useEffect, useState } from "react";
import { Controller, RegisterOptions, useForm } from "react-hook-form";
import { Desk, DiagramPosition, Office } from "types";
import DiagramPositionFormField, {
	DiagramPositionLimit,
} from "./DiagramPositionFormField";
import OfficeMapMovableDesks from "./map/OfficeMapMovableDesks";
import StatusAlert, { StatusData } from "./StatusAlert";

interface OfficeDetailsPanelProps {
	office: Office;
	desks: Desk[];
	desk: Desk | null;
	onSave: (name: string, position: DiagramPosition) => Promise<StatusData>;
	onDelete: () => void;
	statusData: StatusData;
	onNewDeskSelected: (deskId: string) => void;
}

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		saveGridItem: {
			alignSelf: "flex-start",
		},
		saveButtonIcon: {
			marginLeft: theme.spacing(2),
		},
		gridItemContentRight: {
			display: "flex",
			justifyContent: "flex-end",
		},
	})
);

interface FormFields {
	name: string | null;
	diagramPosition: DiagramPosition | null;
}

export default function OfficeDetailsEditor({
	office,
	desk,
	desks,
	onSave,
	onDelete,
	onNewDeskSelected,
}: OfficeDetailsPanelProps) {
	const classes = useStyles();

	const [mapLimit, setMapLimit] = useState<DiagramPositionLimit>({
		min: [0, 0],
		max: [0, 0],
	});
	const [statusData, setStatusData] = useState<StatusData>({});
	const [inProgress, setInProgress] = useState(false);

	const {
		control,
		formState,
		handleSubmit,
		watch,
		reset,
		setValue,
		getValues,
	} = useForm<FormFields>({
		mode: "onBlur",
		defaultValues: {
			name: desk?.name ?? null,
			diagramPosition: desk?.diagramPosition ?? null,
		},
	});
	const { errors, isDirty, isValid } = formState;

	function createRegisterOptions(displayName: string): RegisterOptions {
		return {
			required: `${displayName} is required.`,
			pattern: {
				value: /[^\s]/,
				message: `${displayName} cannot be blank.`,
			},
		};
	}

	const name = watch("name");
	const dp = watch("diagramPosition");

	useEffect(() => {
		setInProgress(false);
		setStatusData({});

		reset({
			name: desk?.name ?? null,
			diagramPosition: desk?.diagramPosition ?? null,
		});
	}, [desk]);

	function onDeleteClicked() {
		onDelete();
	}

	function onDeskMoved(x: number, y: number) {
		setValue(
			"diagramPosition",
			{ x, y },
			{
				shouldDirty: true,
				shouldTouch: true,
				shouldValidate: true,
			}
		);
	}

	function onSubmit(fields: FormFields) {
		const { name, diagramPosition } = fields;
		if (name && diagramPosition) {
			setInProgress(true);
			setStatusData({});
			onSave(name, diagramPosition).then((res) => {
				setStatusData(res);
				setInProgress(false);
			});
		}
	}

	const creatingDesk = {
		id: "new-desk",
		name: name ?? "",
		diagramPosition: dp ?? { x: 0, y: 0 },
	};
	const desksWithNewDesk: Desk[] = [...(desk ? [] : [creatingDesk]), ...desks];
	const desksWithNewPositions = desksWithNewDesk.map((d, i) =>
		d.id === desk?.id
			? {
					...desks[i],
					name: getValues("name") ?? "",
					diagramPosition: getValues("diagramPosition") ?? { x: 0, y: 0 },
			  }
			: d
	);
	return (
		<Card variant="outlined">
			<CardContent>
				<form onSubmit={handleSubmit((data) => onSubmit(data))}>
					<Grid container spacing={2}>
						<Grid item xs={desk ? 6 : 12}>
							<Typography variant="h4">Edit Desk Details</Typography>
						</Grid>
						{/* Only show delete button if desk is an existing desk */}
						{desk && (
							<Grid item xs={6} className={classes.gridItemContentRight}>
								<Button variant="contained" onClick={() => onDeleteClicked()}>
									Delete Desk
								</Button>
							</Grid>
						)}
						<Grid item xs={12} md={3}>
							<Controller
								control={control}
								name="name"
								rules={createRegisterOptions("Name")}
								render={({ field: { value, ...renderProps } }) => (
									<TextField
										{...renderProps}
										fullWidth={true}
										value={value ?? ""}
										variant="outlined"
										label="Name"
										type="text"
										error={Boolean(errors.name)}
										helperText={errors.name?.message}
									/>
								)}
							/>
						</Grid>
						<Grid item>
							<Controller
								control={control}
								name="diagramPosition"
								render={({ field }) => (
									<DiagramPositionFormField
										dp={field.value ?? { x: 0, y: 0 }}
										limit={mapLimit}
										onChange={(dp) => {
											setValue("diagramPosition", dp, {
												shouldDirty: true,
												shouldTouch: true,
												shouldValidate: true,
											});
										}}
									/>
								)}
							/>
						</Grid>
					</Grid>
					<Grid container spacing={2}>
						<Grid item className={classes.saveGridItem}>
							<Button
								variant="contained"
								color="primary"
								type="submit"
								disabled={(isValid && !isDirty) || !isValid || inProgress}
							>
								Save
								{inProgress && (
									<CircularProgress
										className={classes.saveButtonIcon}
										size={24}
									/>
								)}
							</Button>
						</Grid>
						{statusData.severity && (
							<Grid item>
								<StatusAlert {...statusData} />
							</Grid>
						)}
						<Grid item xs={12}>
							<Typography variant="caption">
								Drag selected desk to change its location. Click on markers to
								switch desk selection.
							</Typography>
							<OfficeMapMovableDesks
								image={office.image.uri}
								desks={desksWithNewPositions}
								selectedDesk={desk ?? creatingDesk}
								onDeskSelected={(deskId) => onNewDeskSelected(deskId)}
								onSelectedDeskMoved={(x, y) => onDeskMoved(x, y)}
								onMaxBoundsChanged={(bounds) => {
									setMapLimit({
										min: bounds.min,
										max: bounds.max,
									});
								}}
							/>
						</Grid>
					</Grid>
				</form>
			</CardContent>
		</Card>
	);
}
