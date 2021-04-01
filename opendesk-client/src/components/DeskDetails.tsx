import React, { useEffect, useReducer, useState, Reducer } from "react";
import { format, set, setHours } from "date-fns";

import BookingsTimeline from "components/BookingsTimeline";
import DatePicker from "components/DatePicker";
import { Desk, Booking } from "types";

import "react-datepicker/dist/react-datepicker.css";
import apiRequest from "utils/requestUtils";
import { FORMAT_ISO_WITH_TZ_STRING } from "utils/dateUtils";

interface DeskDetailsProps {
	desk: Desk;
}

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

function DeskDetails({ desk }: DeskDetailsProps) {
	const defaultDate = set(new Date(), {
		hours: 0,
		minutes: 0,
		seconds: 0,
		milliseconds: 0,
	});

	// =============================================================
	// Hooks and Variables
	// =============================================================

	const [bookings, setBookings] = useState<Booking[]>([]);

	const [bkStart, setBkStart] = useState<Date>(setHours(defaultDate, 6));
	const [bkEnd, setBkEnd] = useState<Date>(setHours(defaultDate, 20));

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

	// Update bookings when desk is changed.
	useEffect(() => {
		statusDispatch({
			type: ActionType.CLEAR,
		});
		apiRequest<Booking[]>(`desks/${desk.id}/bookings`).then((res) => {
			res.outcome.isSuccess && res.data && handleBookingsApiResponse(res.data);
		});
	}, [desk]);

	// =============================================================
	// Functions
	// =============================================================

	function handleBookingsApiResponse(apiResponseBookings: Booking[]) {
		// Parse the bookings from the data, converting ISO date to JS Date Object when the JSON contains a date.
		let bookings = JSON.parse(
			JSON.stringify(apiResponseBookings),
			(k, value) => {
				const isDate = k === "startDateTime" || k === "endDateTime";
				return isDate ? new Date(value) : value;
			}
		);
		setBookings(bookings);
	}

	function handleStartChange(date: Date) {
		if (date.getHours() >= 20) {
			return;
		}
		// If start was moved to after end, adjust end to still be after start
		if (date > bkEnd) {
			setBkEnd(set(date, { hours: bkEnd.getHours(), minutes: 0 }));
		}

		setBkStart(date);
	}

	function handleEndChange(date: Date) {
		if (date.getHours() <= 6) {
			return;
		}
		// If end date was moved to before start, adjust start.
		if (date < bkStart) {
			setBkStart(set(date, { hours: bkStart.getHours(), minutes: 0 }));
		}

		setBkEnd(date);
	}

	function SubmitBooking() {
		statusDispatch({
			type: ActionType.CLEAR,
		});
		apiRequest<Booking>(`desks/${desk.id}/bookings`, {
			method: "POST",
			headers: {
				"content-type": "application/json",
			},
			body: JSON.stringify({
				startDateTime: format(bkStart, FORMAT_ISO_WITH_TZ_STRING),
				endDateTime: format(bkEnd, FORMAT_ISO_WITH_TZ_STRING),
			}),
		}).then((res) => {
			if (res.outcome.isSuccess) {
				statusDispatch({
					type: ActionType.SUCCESS,
				});

				apiRequest<Booking[]>(`desks/${desk.id}/bookings`).then((res) => {
					res.outcome.isSuccess &&
						res.data &&
						handleBookingsApiResponse(res.data);
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

	return (
		<div className="desk-details">
			{desk && (
				<div className="desk-details__title">
					<h3>{desk.name}</h3>
				</div>
			)}

			<div>
				<div className="desk-details__datepickers">
					<p className="desk-details__datepickers-text">From</p>
					<DatePicker
						placeholderText="Start Date & Time"
						selected={bkStart}
						onChange={(date) => handleStartChange(date)}
					/>
					<p className="desk-details__datepickers-text">to</p>
					<DatePicker
						placeholderText="End Date & Time"
						selected={bkEnd}
						onChange={(date) => handleEndChange(date)}
					/>
					<button
						className="desk-details__submit"
						onClick={() => SubmitBooking()}
					>
						Book Desk
					</button>
				</div>
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

			<BookingsTimeline
				existingBookings={bookings}
				newBookingStart={bkStart}
				newBookingEnd={bkEnd}
			/>
		</div>
	);
}

export default DeskDetails;
