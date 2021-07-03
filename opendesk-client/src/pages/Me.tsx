import {
	createStyles,
	IconButton,
	makeStyles,
	Paper,
	Table,
	TableBody,
	TableCell,
	TableHead,
	TableRow,
	Theme,
	Tooltip,
} from "@material-ui/core";
import { Delete, Room } from "@material-ui/icons";
import { format } from "date-fns";
import { useAuth } from "hooks/useAuth";
import { usePageTitle } from "hooks/usePageTitle";
import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { FullBooking } from "types";
import apiRequest from "utils/requestUtils";

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		centeredCell: {
			textAlign: "center",
		},
	})
);

function Me() {
	usePageTitle("My Bookings");
	const classes = useStyles();

	const { user } = useAuth();
	const [userBookings, setUserBookings] = useState<FullBooking[]>([]);

	useEffect(() => {
		// API Call to get bookings and stuff
		apiRequest<FullBooking[]>(`users/${user?.id}/bookings`).then((res) => {
			if (res.data) {
				var bookings = JSON.parse(JSON.stringify(res.data), (k, value) => {
					const isDate = k === "startDateTime" || k === "endDateTime";
					return isDate ? new Date(value) : value;
				});

				setUserBookings(bookings);
			}
		});
	}, []);

	return (
		<div>
			<Paper>
				<Table size="small">
					<TableHead>
						<TableRow>
							<TableCell>Office</TableCell>
							<TableCell>Desk</TableCell>
							<TableCell>Date</TableCell>
							<TableCell>Time</TableCell>
							<TableCell>Action</TableCell>
						</TableRow>
					</TableHead>
					<TableBody>
						{userBookings
							.sort(
								(a, b) => a.startDateTime.getDate() - b.startDateTime.getDate()
							)
							.map((b) => (
								<TableRow key={b.id}>
									<TableCell>{b.office.name}</TableCell>
									<TableCell>{b.desk.name}</TableCell>
									<TableCell>{format(b.startDateTime, "dd/MM/yyyy")}</TableCell>
									<TableCell>
										{format(b.startDateTime, "haaa")} to{" "}
										{format(b.endDateTime, "haaa")}
									</TableCell>
									<TableCell>
										<Link to={`/offices/${b.office.id}/${b.desk.id}`}>
											<Tooltip title="Go to map">
												<IconButton>
													<Room />
												</IconButton>
											</Tooltip>
										</Link>
										<Tooltip title="Delete">
											<IconButton>
												<Delete />
											</IconButton>
										</Tooltip>
									</TableCell>
								</TableRow>
							))}
						{userBookings.length === 0 && (
							<TableRow>
								<TableCell colSpan={5} className={classes.centeredCell}>
									You have no bookings. &nbsp;
									<Link to="/offices">Make a booking</Link>
								</TableCell>
							</TableRow>
						)}
					</TableBody>
				</Table>
			</Paper>
		</div>
	);
}

export default Me;
