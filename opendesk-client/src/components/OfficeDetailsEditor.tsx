import {
	Button,
	ButtonGroup,
	Card,
	CardContent,
	createStyles,
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
import { useEffect, useRef, useState } from "react";
import { Office, OfficeImage } from "types";
import OfficeMap from "./map/OfficeMap";

interface OfficeDetailsPanelProps {
	office: Office | null;
	onSave: (
		name: string,
		location: string,
		sublocation: string,
		image?: File
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

export default function OfficeDetailsEditor({
	office,
	onSave,
	onDelete,
}: OfficeDetailsPanelProps) {
	const classes = useStyles();

	const {
		selectedLocationState: [selectedLocation, setSelectedLocation],
		selectedSublocationState: [selectedSublocation, setSelectedSublocation],
		locations,
		subLocations,
	} = useOfficeLocationFilter();

	const [name, setName] = useState("");
	const fileUploadInputElement = useRef<HTMLInputElement | null>(null);
	const [previewOfficeImage, setPreviewOfficeImage] = useState<OfficeImage>();

	useEffect(() => {
		setSelectedLocation(office?.location ?? "");
		setName(office?.name ?? "");
		setPreviewOfficeImage(office?.image);
	}, [office]);

	function onImageSelected(file?: File | null) {
		if (file) {
			const reader = new FileReader();
			reader.onload = (e) => {
				let image = new Image();
				const imageString = e.target?.result?.toString();
				if (!imageString) {
					return;
				}
				image.src = imageString;
				image.onload = (e) => {
					setPreviewOfficeImage({
						url: image.src,
						width: image.width,
						height: image.height,
					});
				};
			};

			reader.readAsDataURL(file);
		}
	}

	function onDeleteClicked() {
		onDelete();
	}

	function onSaveClicked() {
		onSave(
			name,
			selectedLocation,
			selectedSublocation,
			fileUploadInputElement?.current?.files?.[0]
		);
	}

	function onImageCleared() {
		setPreviewOfficeImage(office?.image);

		// Clear the file select input so that it can be cleared and the same image selected again.
		if (fileUploadInputElement?.current) {
			fileUploadInputElement.current.value = "";
		}
	}

	function isSaveEnabled() {
		if (!office) {
			return (
				name &&
				name.trim() &&
				selectedLocation &&
				selectedLocation.trim() &&
				selectedSublocation &&
				selectedSublocation.trim() &&
				previewOfficeImage != null
			);
		} else {
			return (
				office.name !== name ||
				office.location !== selectedLocation ||
				office.subLocation !== selectedSublocation ||
				previewOfficeImage?.url !== office.image.url
			);
		}
	}

	return (
		<Grid item md={9} xs={12}>
			<Card variant="outlined">
				<CardContent>
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
							<TextField
								variant="outlined"
								label="Name"
								type="text"
								value={name}
								onChange={(e) => setName(e.target.value)}
								className={classes.formControl}
							/>
						</Grid>
						<Grid item xs={6}>
							<Autocomplete
								freeSolo
								options={locations}
								value={office?.location ?? ""}
								onInputChange={(evt, val) => {
									setSelectedLocation(val ?? "");
								}}
								renderInput={(params) => (
									<TextField {...params} label="Location" variant="outlined" />
								)}
							/>
						</Grid>
						<Grid item xs={6}>
							<Autocomplete
								freeSolo
								options={subLocations}
								value={office?.subLocation ?? ""}
								onInputChange={(evt, val) => {
									setSelectedSublocation(val ?? "");
								}}
								renderInput={(params) => (
									<TextField
										{...params}
										label="Sub Location"
										variant="outlined"
									/>
								)}
							/>
						</Grid>
						<Grid item xs={12}>
							<ButtonGroup>
								<Button variant="contained" color="primary" component="label">
									Upload File
									<input
										type="file"
										accept="image/png"
										hidden
										ref={fileUploadInputElement}
										onChange={(e) => onImageSelected(e.target.files?.item(0))}
									/>
								</Button>
								{previewOfficeImage?.url !== office?.image.url && (
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
							<Button
								variant="contained"
								color="secondary"
								className={classes.saveButton}
								onClick={() => onSaveClicked()}
								disabled={!isSaveEnabled()}
							>
								Save
							</Button>
						</Grid>
						{previewOfficeImage && (
							<Grid item xs={12}>
								<OfficeMap image={previewOfficeImage} />
							</Grid>
						)}
					</Grid>
				</CardContent>
			</Card>
		</Grid>
	);
}
