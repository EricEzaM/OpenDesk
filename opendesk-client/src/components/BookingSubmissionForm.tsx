import { Reducer, useEffect, useReducer, useState } from "react";

import DatePicker from "components/DatePicker";
import { format, set } from "date-fns";

import { FORMAT_ISO_WITH_TZ_STRING } from "utils/dateUtils";
import apiRequest from "utils/requestUtils";
import { Booking } from "types";
import { useDeskBookings } from "hooks/useDeskBookings";
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
	const {
		selectedDeskState: [desk],
	} = useOfficeDesks();
	const {
		refreshBookings,
		newBookingStartState: [bookingStart, setBookingStart],
		newBookingEndState: [bookingEnd, setBookingEnd],
		isNewBookingValid,
	} = useDeskBookings();

	const [allowSubmit, setAllowSubmit] = useState(desk !== undefined);

	const [statusState, statusDispatch] = useReducer<Reducer<State, Action>>(
		bookingSubmissionStatusReducer,
		{
			success: undefined,
			message: "",
			errors: [],
		}
	);

	const classes = useStyles();

	useEffect(() => {
		console.log("valid", isNewBookingValid, "desk", desk);
		statusDispatch({
			type: ActionType.CLEAR,
		});
		setAllowSubmit(desk !== undefined && isNewBookingValid);
	}, [desk, isNewBookingValid]);

	function handleStartChange(date: Date) {
		if (date.getHours() > 20) {
			return;
		}
		// If start was moved to after end, adjust end to still be after start
		if (date > bookingEnd) {
			setBookingEnd(set(date, { hours: bookingEnd.getHours(), minutes: 0 }));
		}

		setBookingStart(date);
	}

	function handleEndChange(date: Date) {
		if (date.getHours() < 6) {
			return;
		}
		// If end date was moved to before start, adjust start.
		if (date < bookingStart) {
			setBookingStart(
				set(date, { hours: bookingStart.getHours(), minutes: 0 })
			);
		}

		setBookingEnd(date);
	}

	function SubmitBooking() {
		if (desk) {
			statusDispatch({
				type: ActionType.CLEAR,
			});
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
					statusDispatch({
						type: ActionType.SUCCESS,
					});
					refreshBookings();
				} else {
					statusDispatch({
						type: ActionType.FAILURE,
						message: res.outcome.message,
						errors: res.outcome.errors,
					});
				}
			});
		}
	}

	function renderTimePicker(label: string, onChange: () => void) {
		const labelId = `datepicker-${label.replace(" ", "-")}-label`;

		return (
			<FormControl variant="outlined" className={classes.formControl}>
				<InputLabel id={labelId}>{label}</InputLabel>
				<Select labelId={labelId} label={label}>
					<MenuItem value={8}>8am</MenuItem>
					<MenuItem value={10}>10am</MenuItem>
					<MenuItem value={12}>12pm</MenuItem>
					<MenuItem value={14}>2pm</MenuItem>
					<MenuItem value={16}>4pm</MenuItem>
					<MenuItem value={18}>6pm</MenuItem>
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
						className={classes.formControl}
					/>
				</Grid>
				<Grid item xs={6}>
					{renderTimePicker("Start Time", () => {})}
				</Grid>
				<Grid item xs={6}>
					{renderTimePicker("End Time", () => {})}
				</Grid>
				<Grid item xs={12} className={classes.flexEndItem}>
					<Button disabled variant="contained" color="primary">
						Book
					</Button>
				</Grid>
			</Grid>
		</div>
	);
}
