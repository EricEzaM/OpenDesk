import { set } from "date-fns";
import { setHours } from "date-fns/esm";
import {
	useContext,
	createContext,
	ReactNode,
	useState,
	useEffect,
} from "react";
import { Booking } from "types";
import apiRequest from "utils/requestUtils";
import { useOfficeDesks } from "./useOfficeDesks";
import { useOffices } from "./useOffices";

const defaultDate = set(new Date(), {
	hours: 0,
	minutes: 0,
	seconds: 0,
	milliseconds: 0,
});

interface BookingsContextProps {
	refreshBookings: () => void;
	bookingsState: [Booking[], (b: Booking[]) => void];
	newBookingStartState: [Date, (d: Date) => void];
	newBookingEndState: [Date, (d: Date) => void];
	isNewBookingValid: boolean;
	clashedBooking?: Booking;
}

export const BookingsContext = createContext<BookingsContextProps>({
	refreshBookings: () => {},
	bookingsState: [[], () => {}],
	newBookingStartState: [setHours(defaultDate, 6), () => {}],
	newBookingEndState: [setHours(defaultDate, 20), () => {}],
	isNewBookingValid: false,
});

export function BookingsProvider({ children }: { children: ReactNode }) {
	const booking = useBookingsProvider();

	return (
		<BookingsContext.Provider value={booking}>
			{children}
		</BookingsContext.Provider>
	);
}

function useBookingsProvider(): BookingsContextProps {
	const {
		selectedOfficeState: [selectedOffice],
	} = useOffices();

	const {
		selectedDeskState: [selectedDesk],
	} = useOfficeDesks();

	const [bookings, setBookings] = useState<Booking[]>([]);
	const [newStart, setNewStart] = useState(setHours(defaultDate, 6));
	const [newEnd, setNewEnd] = useState(setHours(defaultDate, 20));
	const [isNewValid, setIsNewValid] = useState(false);
	const [clashedBooking, setClashedBooking] = useState<Booking>();

	useEffect(() => {
		// Update bookings when office is changed.
		refreshBookings();
	}, [selectedOffice]);

	useEffect(() => {
		// If no selected desk, clear the state.
		if (!selectedDesk) {
			setIsNewValid(false);
			setClashedBooking(undefined);
			return;
		}

		let isInvalid = false;

		const sameDeskBookings = bookings.filter(
			(b) => b.deskId === selectedDesk?.id
		);

		if (newStart && newEnd) {
			for (let i = 0; i < sameDeskBookings.length; i++) {
				const booking = sameDeskBookings[i];

				isInvalid =
					// Start is between existing start and end
					(newStart >= booking.startDateTime &&
						newStart < booking.endDateTime) ||
					// End is between existing start and end
					(newEnd > booking.startDateTime && newEnd <= booking.endDateTime) ||
					// Start is before existing start and end is after existing end (i.e. fully surrounds existing)
					(newStart <= booking.startDateTime && newEnd >= booking.endDateTime);

				if (isInvalid) {
					setClashedBooking(booking);
					break;
				}
			}
		}

		if (!isInvalid) {
			// i.e. isValid
			setClashedBooking(undefined);
		}

		setIsNewValid(!isInvalid);
	}, [newStart, newEnd, bookings, selectedDesk]);

	function refreshBookings() {
		if (selectedOffice) {
			apiRequest<Booking[]>(`offices/${selectedOffice.id}/bookings`).then(
				(res) => {
					if (res.outcome.isSuccess && res.data) {
						// Parse the bookings from the data, converting ISO date to JS Date Object when the JSON contains a date.
						let bookings = JSON.parse(JSON.stringify(res.data), (k, value) => {
							const isDate = k === "startDateTime" || k === "endDateTime";
							return isDate ? new Date(value) : value;
						});
						setBookings(bookings);
					}
				}
			);
		}
	}

	return {
		refreshBookings,
		bookingsState: [bookings, setBookings],
		newBookingStartState: [newStart, setNewStart],
		newBookingEndState: [newEnd, setNewEnd],
		isNewBookingValid: isNewValid,
		clashedBooking: clashedBooking,
	};
}

export function useBookings() {
	return useContext(BookingsContext);
}
