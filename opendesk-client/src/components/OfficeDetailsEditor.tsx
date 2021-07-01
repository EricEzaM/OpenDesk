import {
	Button,
	ButtonGroup,
	Card,
	CardContent,
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
import OfficeMap from "./map/OfficeMap";

interface OfficeDetailsPanelProps {
	office: Office | null;
	onSave: (
		name: string,
		location: string,
		sublocation: string,
		image: File | null
	) => void;
	onDelete: () => void;
}

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		formControl: {
			minWidth: 120,
			width: "100%",
		},
		clearFileButton: {
			backgroundColor: "#00000012",
		},
		saveButton: {
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

	const {
		selectedLocationState: [, setSelectedLocation],
		locations,
		subLocations,
	} = useOfficeLocationFilter();

	const [selectedImageDataUrl, setSelectedImageDataUrl] = useState<
		string | null
	>(null);

	const { control, formState, handleSubmit, watch, reset, setValue } =
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
		reset({
			name: office?.name ?? null,
			location: office?.location ?? null,
			sublocation: office?.subLocation ?? null,
			file: null,
		});

		if (office) {
			setSelectedImageDataUrl(office.imageUrl);
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
			setSelectedImageDataUrl(office?.imageUrl ?? null);
		}
	}, [file]);

	function onDeleteClicked() {
		onDelete();
	}

	function onSubmit(fields: FormFields) {
		const { name, location, sublocation, file } = fields;
		if (name && location && sublocation && ((file && !office) || office)) {
			onSave(name, location, sublocation, file);
		}
	}

	return (
		<Card variant="outlined">
			<CardContent>
				<form onSubmit={handleSubmit((data) => onSubmit(data))}>
					<Grid container spacing={2}>
						<Grid item xs={6}>
							<Typography variant="h4">Edit Details</Typography>
						</Grid>
						{/* Only show delete button if office is an existing office */}
						{office && (
							<Grid item xs={6} className={classes.gridItemContentRight}>
								<Button variant="contained" onClick={() => onDeleteClicked()}>
									Delete Office
								</Button>
							</Grid>
						)}
						<Grid item xs={12}>
							<Controller
								control={control}
								name="name"
								rules={createRegisterOptions("Name")}
								render={({ field: { value, ...renderProps } }) => (
									<TextField
										{...renderProps}
										className={classes.formControl}
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
						<Grid item xs={6}>
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
						<Grid item xs={6}>
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
						<Grid item xs={12}>
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
								disabled={(isValid && !isDirty) || !isValid}
							>
								Save
							</Button>
						</Grid>
						{selectedImageDataUrl && (
							<Grid item xs={12}>
								<OfficeMap image={selectedImageDataUrl} />
							</Grid>
						)}
					</Grid>
				</form>
			</CardContent>
		</Card>
	);
}
