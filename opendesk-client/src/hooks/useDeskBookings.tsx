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

export const SelectedDeskBookingsContext = createContext<BookingsContextProps>({
	refreshBookings: () => {},
	bookingsState: [[], () => {}],
	newBookingStartState: [setHours(defaultDate, 6), () => {}],
	newBookingEndState: [setHours(defaultDate, 20), () => {}],
	isNewBookingValid: false,
});

export function DeskBookingsProvider({ children }: { children: ReactNode }) {
	const booking = useDeskBookingsProvider();

	return (
		<SelectedDeskBookingsContext.Provider value={booking}>
			{children}
		</SelectedDeskBookingsContext.Provider>
	);
}

function useDeskBookingsProvider(): BookingsContextProps {
	const {
		selectedDeskState: [desk],
	} = useOfficeDesks();

	const [bookings, setBookings] = useState<Booking[]>([]);
	const [start, setStart] = useState(setHours(defaultDate, 6));
	const [end, setEnd] = useState(setHours(defaultDate, 20));
	const [isNewValid, setIsNewValid] = useState(false);
	const [clashedBooking, setClashedBooking] = useState<Booking>();

	useEffect(() => {
		// Update bookings when desk is changed.
		refreshBookings();

		// If a desk is not selected, reset validity and clashed booking
		if (!desk) {
			setIsNewValid(false);
			setClashedBooking(undefined);
		}
	}, [desk]);

	useEffect(() => {
		let isInvalid = false;

		if (start && end) {
			for (let i = 0; i < bookings.length; i++) {
				const booking = bookings[i];

				isInvalid =
					// Start is between existing start and end
					(start >= booking.startDateTime && start < booking.endDateTime) ||
					// End is between existing start and end
					(end > booking.startDateTime && end <= booking.endDateTime) ||
					// Start is before existing start and end is after existing end (i.e. fully surrounds existing)
					(start <= booking.startDateTime && end >= booking.endDateTime);

				if (isInvalid) {
					setClashedBooking(booking);
					break;
				}
			}
		}

		if (!isInvalid) {
			setClashedBooking(undefined);
		}

		setIsNewValid(!isInvalid);
	}, [start, end, bookings]);

	function refreshBookings() {
		if (desk) {
			apiRequest<Booking[]>(`desks/${desk.id}/bookings`).then((res) => {
				if (res.outcome.isSuccess && res.data) {
					// Parse the bookings from the data, converting ISO date to JS Date Object when the JSON contains a date.
					let bookings = JSON.parse(JSON.stringify(res.data), (k, value) => {
						const isDate = k === "startDateTime" || k === "endDateTime";
						return isDate ? new Date(value) : value;
					});
					setBookings(bookings);
				}
			});
		}
	}

	return {
		refreshBookings,
		bookingsState: [bookings, setBookings],
		newBookingStartState: [start, setStart],
		newBookingEndState: [end, setEnd],
		isNewBookingValid: isNewValid,
		clashedBooking: clashedBooking,
	};
}

export function useDeskBookings() {
	return useContext(SelectedDeskBookingsContext);
}
