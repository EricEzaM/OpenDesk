import {
	createStyles,
	makeStyles,
	Paper,
	Table,
	TableBody,
	TableCell,
	TableContainer,
	TableHead,
	TableRow,
	Theme,
	Typography,
} from "@material-ui/core";
import clsx from "clsx";
import { format, isSameDay } from "date-fns";
import { useAuth } from "hooks/useAuth";
import { useBookings } from "hooks/useBookings";
import { useOfficeDesks } from "hooks/useOfficeDesks";
import { useMemo } from "react";

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		centeredCell: {
			textAlign: "center",
		},
		clashingBookingRow: {
			backgroundColor: theme.palette.error.light,
		},
		selfClashingBookingRow: {
			backgroundColor: theme.palette.info.light,
		},
	})
);

export default function BookingsTable() {
	// =============================================================
	// Hooks and Variables
	// =============================================================

	const classes = useStyles();

	const {
		desksState: [desks],
	} = useOfficeDesks();

	const {
		bookingsState: [bookings],
		newBookingStartState: [newStart],
		clashedBooking,
	} = useBookings();

	const { user } = useAuth();

	const sameDateBookings = useMemo(
		() => bookings.filter((b) => isSameDay(b.startDateTime, newStart)),
		[bookings, newStart]
	);

	// =============================================================
	// Render
	// =============================================================

	return (
		<div>
			<Typography variant="h6" component="h2">
				Bookings for {format(newStart, "dd/MM/yyyy")}
			</Typography>
			<TableContainer component={Paper}>
				<Table>
					<TableHead>
						<TableRow>
							<TableCell>Desk</TableCell>
							<TableCell>Name</TableCell>
							<TableCell>Booking</TableCell>
						</TableRow>
					</TableHead>
					<TableBody>
						{sameDateBookings.map((b) => (
							<TableRow
								key={b.id}
								className={clsx({
									[classes.selfClashingBookingRow]:
										clashedBooking?.id === b.id &&
										clashedBooking.user.id === user?.id,
									[classes.clashingBookingRow]:
										clashedBooking?.id === b.id &&
										clashedBooking.user.id !== user?.id,
								})}
							>
								<TableCell>
									{desks.find((d) => d.id === b.deskId)?.name}
								</TableCell>
								<TableCell>{b.user.displayName}</TableCell>
								<TableCell>
									{format(b.startDateTime, "haaa")} -{" "}
									{format(b.endDateTime, "haaa")}
								</TableCell>
							</TableRow>
						))}
						{sameDateBookings.length === 0 && (
							<TableRow>
								<TableCell colSpan={3} className={classes.centeredCell}>
									There are no bookings in the office on this day.
								</TableCell>
							</TableRow>
						)}
					</TableBody>
				</Table>
			</TableContainer>
		</div>
	);
}
