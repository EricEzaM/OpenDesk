import {
	Button,
	ButtonGroup,
	Card,
	CardContent,
	CircularProgress,
	createStyles,
	FormControl,
	FormHelperText,
	Grid,
	IconButton,
	makeStyles,
	TextField,
	Theme,
	Typography,
} from "@material-ui/core";
import { Clear } from "@material-ui/icons";
import { Autocomplete } from "@material-ui/lab";
import useOfficeLocationFilter from "hooks/useOfficeLocationFilter";
import { useEffect, useState } from "react";
import { Controller, RegisterOptions, useForm } from "react-hook-form";
import { Office } from "types";
import OfficeMapBase from "./map/OfficeMapBase";
import StatusAlert, { StatusData } from "./StatusAlert";

interface OfficeDetailsPanelProps {
	office: Office | null;
	onSave: (
		name: string,
		location: string,
		sublocation: string,
		image: File | null
	) => Promise<StatusData>;
	onDelete: () => void;
	statusData: StatusData;
}

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		clearFileButton: {
			backgroundColor: "#00000012",
		},
		saveButton: {
			marginLeft: theme.spacing(2),
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
	location: string | null;
	sublocation: string | null;
	file: File | null;
}

export default function OfficeDetailsEditor({
	office,
	onSave,
	onDelete,
}: OfficeDetailsPanelProps) {
	const classes = useStyles();

	const [statusData, setStatusData] = useState<StatusData>({});
	const [inProgress, setInProgress] = useState(false);

	const {
		selectedLocationState: [, setSelectedLocation],
		locations,
		subLocations,
	} = useOfficeLocationFilter();

	const [selectedImageDataUrl, setSelectedImageDataUrl] = useState<
		string | null
	>(null);

	const { control, formState, handleSubmit, watch, reset } =
		useForm<FormFields>({
			mode: "onBlur",
			defaultValues: {
				name: office?.name ?? null,
				location: office?.location ?? null,
				sublocation: office?.subLocation ?? null,
				file: null,
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

	const location = watch("location");
	const file = watch("file");

	useEffect(() => {
		setInProgress(false);
		setStatusData({});

		reset({
			name: office?.name ?? null,
			location: office?.location ?? null,
			sublocation: office?.subLocation ?? null,
			file: null,
		});

		if (office) {
			setSelectedImageDataUrl(office.image.uri);
		} else {
			setSelectedImageDataUrl(null);
		}
	}, [office]);

	useEffect(() => {
		setSelectedLocation(location ?? "");
	}, [location]);

	useEffect(() => {
		if (file) {
			const reader = new FileReader();
			reader.onload = (e) => {
				const dataUrl = e.target?.result?.toString();
				setSelectedImageDataUrl(dataUrl ?? null);
			};

			reader.readAsDataURL(file);
		} else {
			setSelectedImageDataUrl(office?.image.uri ?? null);
		}
	}, [file]);

	function onDeleteClicked() {
		onDelete();
	}

	function onSubmit(fields: FormFields) {
		const { name, location, sublocation, file } = fields;
		if (name && location && sublocation && ((file && !office) || office)) {
			setInProgress(true);
			setStatusData({});
			onSave(name, location, sublocation, file).then((res) => {
				setStatusData(res);
				setInProgress(false);
			});
		}
	}

	return (
		<Card variant="outlined">
			<CardContent>
				<form onSubmit={handleSubmit((data) => onSubmit(data))}>
					<Grid container spacing={2}>
						<Grid item xs={office ? 6 : 12}>
							<Typography variant="h4">Edit Office Details</Typography>
						</Grid>
						{/* Only enable delete button if office is an existing office */}
						{office && (
							<Grid item xs={6} className={classes.gridItemContentRight}>
								<Button
									variant="contained"
									disabled={!office}
									onClick={() => onDeleteClicked()}
								>
									Delete Office
								</Button>
							</Grid>
						)}
						<Grid item xs={12} md={4}>
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
						<Grid item xs={12} md={4}>
							<Controller
								control={control}
								name="location"
								rules={createRegisterOptions("Location")}
								render={({ field: { onChange, ...renderProps } }) => (
									<Autocomplete
										{...renderProps}
										freeSolo
										options={locations}
										onInputChange={(e, v) => onChange(v)}
										renderInput={(params) => (
											<TextField
												{...params}
												label="Location"
												variant="outlined"
												error={Boolean(errors.location)}
												helperText={errors.location?.message}
											/>
										)}
									/>
								)}
							/>
						</Grid>
						<Grid item xs={12} md={4}>
							<Controller
								control={control}
								name="sublocation"
								rules={createRegisterOptions("Sublocation")}
								render={({ field: { onChange, ...renderProps } }) => (
									<Autocomplete
										{...renderProps}
										freeSolo
										options={subLocations}
										onInputChange={(e, v) => onChange(v)}
										renderInput={(params) => (
											<TextField
												{...params}
												label="Sublocation"
												variant="outlined"
												error={Boolean(errors.sublocation)}
												helperText={errors.sublocation?.message}
											/>
										)}
									/>
								)}
							/>
						</Grid>
					</Grid>
					<Grid container spacing={2}>
						<Grid item>
							<FormControl>
								<Controller
									name="file"
									control={control}
									rules={{
										validate: (v) => {
											if (v === null && !Boolean(office)) {
												return "An image is required for the map.";
											}
										},
									}}
									render={({
										field: { value, onChange, onBlur, ...restField },
									}) => (
										<ButtonGroup>
											<Button
												variant="contained"
												color="secondary"
												component="label"
												onBlur={onBlur}
											>
												Upload Map Image
												<input
													{...restField}
													type="file"
													accept="image/png"
													hidden
													onClick={(e) => {
														e.currentTarget.value = ""; // Clear the value so the same file can be selected again, if desired
													}}
													onChange={(e) => {
														e.target.files?.item(0) &&
															onChange(e.target.files?.item(0));
														onBlur();
													}}
												/>
											</Button>
											{file && (
												<IconButton
													className={classes.clearFileButton}
													size="small"
													onClick={() => {
														onChange(null);
														onBlur();
													}}
													title="Clear selected image"
												>
													<Clear />
												</IconButton>
											)}
										</ButtonGroup>
									)}
								/>
								{Boolean(errors.file) && (
									<FormHelperText error>
										{/* @ts-ignore Have to use ignore here since it thinks message does not exist on DeepMap */}
										{errors.file?.message}
									</FormHelperText>
								)}
							</FormControl>

							<Button
								variant="contained"
								color="primary"
								className={classes.saveButton}
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
						<Grid item>
							{statusData.severity && <StatusAlert {...statusData} />}
						</Grid>
						{selectedImageDataUrl && (
							<Grid item xs={12}>
								<OfficeMapBase image={selectedImageDataUrl} />
							</Grid>
						)}
					</Grid>
				</form>
			</CardContent>
		</Card>
	);
}
