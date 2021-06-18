import { Reducer, useEffect, useMemo, useReducer, useState } from "react";
import { differenceInHours, format, isSameDay, set, setHours } from "date-fns";
import {
	createStyles,
	FormControl,
	Grid,
	InputLabel,
	makeStyles,
	MenuItem,
	Select,
	TextField,
	Theme,
	Button,
	Typography,
} from "@material-ui/core";
import { Alert, AlertTitle, Autocomplete } from "@material-ui/lab";

import apiRequest from "utils/requestUtils";
import { Booking, ValidationError } from "types";
import { useBookings } from "hooks/useBookings";
import { useOfficeDesks } from "hooks/useOfficeDesks";
import { useAuth } from "hooks/useAuth";
import { FORMAT_ISO_WITH_TZ_STRING } from "utils/dateUtils";
import { useOffices } from "hooks/useOffices";

interface StatusState {
	severity?: "info" | "error";
	message: string;
	errors: string[];
}

enum ActionType {
	FAILURE,
	SUCCESS,
	CLEAR,
	INFO,
}

type Failure = {
	readonly type: ActionType.FAILURE;
	readonly message: string;
	readonly errors: string[];
};

type Clear = {
	readonly type: ActionType.CLEAR;
};

type Info = {
	readonly type: ActionType.INFO;
	readonly message: string;
};

type Action = Failure | Clear | Info;

function bookingSubmissionStatusReducer(
	state: StatusState,
	action: Action
): StatusState {
	switch (action.type) {
		case ActionType.FAILURE:
			return {
				severity: "error",
				message: action.message,
				errors: action.errors,
			};
		case ActionType.INFO:
			return {
				severity: "info",
				message: action.message,
				errors: [],
			};
		case ActionType.CLEAR:
			return { severity: undefined, message: "", errors: [] };
		default:
			return state;
	}
}

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		formControl: {
			minWidth: 120,
			width: "100%",
		},
		flexEndItem: {
			display: "flex",
			justifyContent: "flex-end",
		},
	})
);

export default function BookingSubmissionForm() {
	// =============================================================
	// Hooks and Variables
	// =============================================================

	const classes = useStyles();

	const {
		desksState: [desks],
		selectedDeskState: [selectedDesk, setSelectedDesk],
	} = useOfficeDesks();

	const {
		selectedOfficeState: [selectedOffice],
	} = useOffices();

	const {
		bookingsState: [bookings],
		refreshBookings,
		newBookingStartState: [bookingStart, setBookingStart],
		newBookingEndState: [bookingEnd, setBookingEnd],
		isNewBookingValid,
		clashedBooking,
	} = useBookings();

	const { user } = useAuth();

	const [allowSubmit, setAllowSubmit] = useState(false);

	const [statusState, statusDispatch] = useReducer<
		Reducer<StatusState, Action>
	>(bookingSubmissionStatusReducer, {
		severity: undefined,
		message: "",
		errors: [],
	});

	const deskOptions = useMemo(
		() =>
			desks.map((d) => {
				const hoursBooked = bookings
					.filter((b) => b.deskId === d.id)
					.filter((b) => isSameDay(b.startDateTime, bookingStart))
					.map((b) => differenceInHours(b.endDateTime, b.startDateTime))
					.reduce((acc, cv) => (acc += cv), 0);

				const color =
					hoursBooked <= 0
						? "green"
						: hoursBooked <= 3
						? "yellow"
						: hoursBooked <= 6
						? "orange"
						: "red";

				return {
					desk: d,
					color: color,
					hoursBooked: hoursBooked,
				};
			}),
		[desks, bookings, bookingStart]
	);

	// =============================================================
	// Effects
	// =============================================================

	useEffect(() => {
		statusDispatch({
			type: ActionType.CLEAR,
		});
		setAllowSubmit(selectedDesk !== undefined && isNewBookingValid);

		if (clashedBooking) {
			const startTime = format(clashedBooking.startDateTime, "haaa");
			const endTime = format(clashedBooking.endDateTime, "haaa");

			if (clashedBooking.user.id === user?.id) {
				statusDispatch({
					type: ActionType.INFO,
					message: `You already have a booking on this desk from ${startTime} to ${endTime}`,
				});
			} else {
				statusDispatch({
					type: ActionType.FAILURE,
					message: "Clash",
					errors: [
						`The booking clashes with another booking on this desk by ${clashedBooking.user.displayName} from ${startTime} to ${endTime}`,
					],
				});
			}
		}
	}, [selectedDesk, isNewBookingValid, clashedBooking, user]);

	// =============================================================
	// Functions
	// =============================================================

	/**
	 * @param dateString In format "YYYY-MM-DD"
	 */
	function handleDateChange(dateString: string) {
		const date = new Date(dateString);
		const newDateOnly = new Date(
			date.getFullYear(),
			date.getMonth(),
			date.getDate()
		);

		const startTime = bookingStart && {
			hours: bookingStart.getHours(),
			minutes: bookingStart.getMinutes(),
		};

		const endTime = bookingEnd && {
			hours: bookingEnd.getHours(),
			minutes: bookingEnd.getMinutes(),
		};

		setBookingStart(set(newDateOnly, startTime));
		setBookingEnd(set(newDateOnly, endTime));
	}

	function handleStartTimeChange(hoursValue: number) {
		setBookingStart(setHours(bookingStart, hoursValue));
	}

	function handleEndTimeChange(hoursValue: number) {
		setBookingEnd(setHours(bookingEnd, hoursValue));
	}

	function submitBooking() {
		if (!selectedDesk) {
			return;
		}

		// Clear status when submitting a booking.
		statusDispatch({
			type: ActionType.CLEAR,
		});

		// Post the booking
		apiRequest<Booking>(`desks/${selectedDesk.id}/bookings`, {
			method: "POST",
			headers: {
				"content-type": "application/json",
			},
			body: JSON.stringify({
				startDateTime: format(bookingStart, FORMAT_ISO_WITH_TZ_STRING),
				endDateTime: format(bookingEnd, FORMAT_ISO_WITH_TZ_STRING),
			}),
		}).then((res) => {
			if (res.data) {
				refreshBookings();
			} else if (res.problem) {
				statusDispatch({
					type: ActionType.FAILURE,
					message: res.problem.title,
					errors: (res.problem?.errors as ValidationError[])?.map(
						(ve: ValidationError) => ve.message
					),
				});
			}
		});
	}

	// =============================================================
	// Render
	// =============================================================

	function renderTimePicker(
		label: string,
		selectedValue: number,
		onChange: (hours: number) => void
	) {
		const labelId = `datepicker-${label.replace(" ", "-")}-label`;

		return (
			<FormControl variant="outlined" className={classes.formControl}>
				<InputLabel id={labelId}>{label}</InputLabel>
				<Select
					labelId={labelId}
					label={label}
					onChange={(e) => onChange(e.target.value as number)}
					value={selectedValue}
				>
					<MenuItem value={6}>6am</MenuItem>
					<MenuItem value={8}>8am</MenuItem>
					<MenuItem value={10}>10am</MenuItem>
					<MenuItem value={12}>12pm</MenuItem>
					<MenuItem value={14}>2pm</MenuItem>
					<MenuItem value={16}>4pm</MenuItem>
					<MenuItem value={18}>6pm</MenuItem>
					<MenuItem value={20}>8pm</MenuItem>
				</Select>
			</FormControl>
		);
	}

	return (
		<div>
			<Grid container spacing={2}>
				<Grid item xs={12}>
					<Typography variant="h6" component="h2">
						Booking Details
					</Typography>
				</Grid>
				<Grid item xs={12}>
					<TextField
						variant="outlined"
						label="Date"
						type="date"
						defaultValue={format(new Date(), "yyyy-MM-dd")}
						onChange={(e) => handleDateChange(e.target.value)}
						className={classes.formControl}
					/>
				</Grid>
				<Grid item xs={6}>
					{renderTimePicker(
						"Start Time",
						bookingStart.getHours(),
						handleStartTimeChange
					)}
				</Grid>
				<Grid item xs={6}>
					{renderTimePicker(
						"End Time",
						bookingEnd.getHours(),
						handleEndTimeChange
					)}
				</Grid>
				<Grid item xs={12}>
					<TextField
						variant="outlined"
						label="Office"
						type="text"
						defaultValue="No Selection"
						disabled={true}
						value={selectedOffice?.name ?? "No Selection"}
						className={classes.formControl}
					></TextField>
				</Grid>
				<Grid item xs={12}>
					<Autocomplete
						options={deskOptions}
						getOptionLabel={(opt) => opt.desk.name}
						getOptionSelected={(opt, val) => opt.desk.id === val.desk.id}
						disabled={!Boolean(selectedOffice)}
						onChange={(evt, val) => setSelectedDesk(val?.desk)}
						renderInput={(params) => (
							<TextField
								{...params}
								disabled={!Boolean(selectedOffice)}
								label="Desk"
								variant="outlined"
								placeholder="Type to select desk..."
								className={classes.formControl}
								InputLabelProps={{
									shrink: true,
								}}
							/>
						)}
						renderOption={(option, state) => {
							return (
								<div
									style={{
										width: "100%",
										display: "flex",
										justifyContent: "space-between",
									}}
								>
									<span style={{ color: option.color }}>
										{option.desk.name}
									</span>
									<span>{option.hoursBooked} hours booked.</span>
								</div>
							);
						}}
					/>
				</Grid>
				<Grid item xs={12} className={classes.flexEndItem}>
					<Button
						variant="contained"
						color="primary"
						disabled={!allowSubmit}
						onClick={submitBooking}
					>
						Book
					</Button>
				</Grid>
				<Grid item xs={12}>
					{statusState.severity !== undefined && (
						<Alert severity={statusState.severity}>
							<AlertTitle>{statusState.message}</AlertTitle>
							<ul>
								{statusState.errors.map((e) => (
									<li>{e}</li>
								))}
							</ul>
						</Alert>
					)}
				</Grid>
			</Grid>
		</div>
	);
}
