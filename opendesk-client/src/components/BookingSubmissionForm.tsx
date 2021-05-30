import { Reducer, useEffect, useReducer, useState } from "react";

import DatePicker from "components/DatePicker";
import { format, set } from "date-fns";

import { FORMAT_ISO_WITH_TZ_STRING } from "utils/dateUtils";
import apiRequest from "utils/requestUtils";
import { Booking } from "types";
import { useDeskBookings } from "hooks/useDeskBookings";
import { useOfficeDesks } from "hooks/useOfficeDesks";

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

	return (
		<div>
			<div className="booking-submission">
				<p className="booking-submission__text">From</p>
				<DatePicker
					placeholderText="Start Date & Time"
					selected={bookingStart}
					onChange={(date) => handleStartChange(date)}
				/>
				<p className="booking-submission__text">to</p>
				<DatePicker
					placeholderText="End Date & Time"
					selected={bookingEnd}
					onChange={(date) => handleEndChange(date)}
				/>
				<button
					className="booking-submission__submit"
					disabled={!allowSubmit}
					onClick={() => SubmitBooking()}
				>
					Book Desk
				</button>
			</div>
			<div>
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
			</div>
		</div>
	);
}
