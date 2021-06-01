import {
	Paper,
	Table,
	TableBody,
	TableCell,
	TableContainer,
	TableHead,
	TableRow,
	Typography,
} from "@material-ui/core";

export default function BookingsOverview() {
	return (
		<div>
			<Typography variant="h6" component="h2">
				Bookings Overview
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
						{[
							{
								desk: "31.30",
								name: "John Smith",
								booking: "8am - 6pm",
							},
							{
								desk: "31.33",
								name: "Sue Storm",
								booking: "8am - 12pm",
							},
						].map((x) => (
							<TableRow key={x.booking}>
								<TableCell>{x.desk}</TableCell>
								<TableCell>{x.name}</TableCell>
								<TableCell>{x.booking}</TableCell>
							</TableRow>
						))}
					</TableBody>
				</Table>
			</TableContainer>
		</div>
	);
}
