import { Reducer, useEffect, useReducer, useState } from "react";

import { format, set, setHours } from "date-fns";

import { FORMAT_ISO_WITH_TZ_STRING } from "utils/dateUtils";
import apiRequest from "utils/requestUtils";
import { Booking } from "types";
import { useBookings } from "hooks/useBookings";
import { useOfficeDesks } from "hooks/useOfficeDesks";
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

interface State {
	success?: boolean;
	message: string;
	errors: string[];
}

enum ActionType {
	FAILURE,
	SUCCESS,
	CLEAR,
}

type Success = {
	readonly type: ActionType.SUCCESS;
};

type Failure = {
	readonly type: ActionType.FAILURE;
	readonly message: string;
	readonly errors: string[];
};

type Clear = {
	readonly type: ActionType.CLEAR;
};

type Action = Success | Failure | Clear;

function bookingSubmissionStatusReducer(state: State, action: Action): State {
	switch (action.type) {
		case ActionType.FAILURE:
			return { success: false, message: action.message, errors: action.errors };
		case ActionType.SUCCESS:
			return { success: true, message: "Success", errors: [] };
		case ActionType.CLEAR:
			return { success: undefined, message: "", errors: [] };
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
		selectedDeskState: [desk],
	} = useOfficeDesks();

	const {
		refreshBookings,
		bookingsState: [bookings],
		newBookingStartState: [bookingStart, setBookingStart],
		newBookingEndState: [bookingEnd, setBookingEnd],
		isNewBookingValid,
		clashedBooking,
	} = useBookings();

	const [allowSubmit, setAllowSubmit] = useState(false);

	const [statusState, statusDispatch] = useReducer<Reducer<State, Action>>(
		bookingSubmissionStatusReducer,
		{
			success: undefined,
			message: "",
			errors: [],
		}
	);

	// =============================================================
	// Effects
	// =============================================================

	useEffect(() => {
		statusDispatch({
			type: ActionType.CLEAR,
		});
		setAllowSubmit(desk !== undefined && isNewBookingValid);

		clashedBooking &&
			statusDispatch({
				type: ActionType.FAILURE,
				message: "Clash",
				errors: [
					`The booking clashes with another booking on this desk by ${
						clashedBooking.user.name
					} from ${format(clashedBooking.startDateTime, "haaa")} to ${format(
						clashedBooking.endDateTime,
						"haaa"
					)}`,
				],
			});
	}, [desk, isNewBookingValid, clashedBooking]);

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
		if (!desk) {
			return;
		}

		// Clear status when submitting a booking.
		statusDispatch({
			type: ActionType.CLEAR,
		});

		// Post the booking
		apiRequest<Booking>(`desks/${desk.id}/bookings`, {
			method: "POST",
			headers: {
				"content-type": "application/json",
			},
			body: JSON.stringify({
				startDateTime: format(bookingStart, FORMAT_ISO_WITH_TZ_STRING),
				endDateTime: format(bookingEnd, FORMAT_ISO_WITH_TZ_STRING),
			}),
		}).then((res) => {
			if (res.outcome.isSuccess) {
				refreshBookings();
				statusDispatch({
					type: ActionType.SUCCESS,
				});
			} else {
				statusDispatch({
					type: ActionType.FAILURE,
					message: res.outcome.message,
					errors: res.outcome.errors,
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
					{statusState.success !== undefined && (
						<div
							className={`booking-submission-result booking-submission-result--${
								statusState.success ? "success" : "failure"
							}`}
						>
							<p className="booking-submission-result__title">
								{statusState.message}
							</p>
							{statusState.errors.map((e) => (
								<div>{e}</div>
							))}
						</div>
					)}
				</Grid>
			</Grid>
		</div>
	);
}
