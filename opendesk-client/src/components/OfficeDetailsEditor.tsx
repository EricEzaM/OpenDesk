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
	imageUrl: string | null;
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

	const { control, handleSubmit, setValue, watch, trigger, reset, formState } =
		useForm<FormFields>({
			mode: "onBlur",
			defaultValues: {
				name: null,
				location: null,
				sublocation: null,
				imageUrl: null,
			},
		});
	const errors = formState.errors;

	const [selectedImageFile, setSelectedImageFile] = useState<File | null>(null);

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
	const imageUrl = watch("imageUrl");

	useEffect(() => {
		reset();
		setValue("name", office?.name ?? null);
		setValue("location", office?.location ?? null);
		setValue("sublocation", office?.subLocation ?? null);
		setValue("imageUrl", office?.imageUrl ?? null);
	}, [office]);

	useEffect(() => {
		setSelectedLocation(location ?? "");
	}, [location]);

	function processFileIntoImageUrl(file: File | null) {
		debugger;
		setSelectedImageFile(file);
		if (file) {
			const reader = new FileReader();
			reader.onload = (e) => {
				const dataUrl = e.target?.result?.toString();
				setValue("imageUrl", dataUrl ?? null);
				trigger("imageUrl");
			};

			reader.readAsDataURL(file);
		}
	}

	function onDeleteClicked() {
		onDelete();
	}

	function onImageCleared() {
		setValue("imageUrl", null);
	}

	function onSubmit(fields: FormFields) {
		// alert(JSON.stringify(fields, null, 2));

		const { name, location, sublocation } = fields;
		if (name && location && sublocation) {
			onSave(name, location, sublocation, selectedImageFile);
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
								<ButtonGroup>
									<Button variant="contained" color="primary" component="label">
										Upload Map Image
										<Controller
											name="imageUrl"
											rules={{ required: "An image is required." }}
											control={control}
											render={({ field }) => (
												<input
													type="file"
													accept="image/png"
													hidden
													onClick={(e) => {
														e.currentTarget.value = ""; // Clear the value so the same file can be selected again, if desired
														field.onBlur();
													}}
													onChange={(e) =>
														processFileIntoImageUrl(
															e.target.files?.item(0) ?? null
														)
													}
												/>
											)}
										/>
									</Button>
									{imageUrl && imageUrl !== office?.imageUrl && (
										<IconButton
											className={classes.clearFileButton}
											size="small"
											onClick={() => onImageCleared()}
											title="Clear selected image"
										>
											<Clear />
										</IconButton>
									)}
								</ButtonGroup>
								<FormHelperText error={Boolean(errors.imageUrl)}>
									{/* @ts-ignore Have to use ignore here since it thinks message does not exist on DeepMap */}
									{errors.imageUrl?.message}
								</FormHelperText>
							</FormControl>

							<Button
								variant="contained"
								color="secondary"
								className={classes.saveButton}
								type="submit"
								disabled={!formState.isValid}
							>
								Save
							</Button>
						</Grid>
						{imageUrl && (
							<Grid item xs={12}>
								<OfficeMap image={imageUrl} />
							</Grid>
						)}
					</Grid>
				</form>
			</CardContent>
		</Card>
	);
}
