import { useState } from "react";

import { Office, Desk } from "types";

import OfficeSelector from "components/OfficeSelector";
import apiRequest from "utils/requestUtils";
import DeskDetails from "components/DeskDetails";
import OfficeMap from "components/map/OfficeMap";
import Authenticated from "components/auth/Authenticated";
import Unauthenticated from "components/auth/Unauthenticated";
import BookingSubmissionForm from "components/BookingSubmissionForm";
import { useOfficeDesks } from "hooks/useOfficeDesks";
import {
	Grid,
	Paper,
	Table,
	TableBody,
	TableCell,
	TableContainer,
	TableHead,
	TableRow,
	Typography,
} from "@material-ui/core";

function Offices() {
	// =============================================================
	// Hooks and Variables
	// =============================================================

	const {
		desksState: [desks, setDesks],
		selectedDeskState: [selectedDesk, setSelectedDesk],
	} = useOfficeDesks();
	const [selectedOffice, setSelectedOffice] = useState<Office | undefined>();

	// =============================================================
	// Effects
	// =============================================================

	// =============================================================
	// Functions
	// =============================================================

	function onDeskSelected(desk?: Desk) {
		setSelectedDesk(desk);
	}

	function onOfficeSelected(office?: Office) {
		setSelectedOffice(office);

		office &&
			apiRequest<Desk[]>(`offices/${office.id}/desks`).then((res) => {
				if (res.outcome.isSuccess) {
					setDesks(res.data ?? []);
				}
			});
	}

	// =============================================================
	// Render
	// =============================================================

	return (
		<Grid container spacing={2}>
			<Grid item lg={3}>
				<BookingSubmissionForm />
			</Grid>
			<Grid item lg={5}>
				<div>
					<Typography variant="h6" component="h2">
						Location &amp; Desk Selection
					</Typography>
					<OfficeSelector onOfficeSelected={onOfficeSelected} />
					{selectedOffice && (
						<>
							<OfficeMap
								image={selectedOffice.image}
								onDeskSelected={onDeskSelected}
							/>
						</>
					)}
				</div>
			</Grid>
			<Grid item lg={4}>
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
			</Grid>
		</Grid>
		// <>
		// 	<Unauthenticated>
		// 		<p>Sign in is required</p>
		// 	</Unauthenticated>
		// 	<Authenticated>
		// 		<div>
		// 			<OfficeSelector onOfficeSelected={onOfficeSelected} />
		// 			<BookingSubmissionForm />
		// 		</div>
		// 		{selectedOffice && (
		// 			<>
		// 				<OfficeMap
		// 					image={selectedOffice.image}
		// 					onDeskSelected={onDeskSelected}
		// 				/>
		// 			</>
		// 		)}
		// 		{selectedDesk && <DeskDetails desk={selectedDesk} />}
		// 	</Authenticated>
		// </>
	);
}

export default Offices;
