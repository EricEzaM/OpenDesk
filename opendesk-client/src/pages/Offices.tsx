import OfficeSelector from "components/OfficeSelector";
import OfficeMap from "components/map/OfficeMap";
import BookingSubmissionForm from "components/BookingSubmissionForm";
import { Box, Grid, Typography } from "@material-ui/core";
import { useOffices } from "hooks/useOffices";
import BookingsOverview from "components/BookingsTable";
import { useEffect } from "react";
import { useBookings } from "hooks/useBookings";
import { useOfficeDesks } from "hooks/useOfficeDesks";
import useOfficeDeskRouteParams from "hooks/useOfficeDeskRouteParams";

function Offices() {
	// =============================================================
	// Hooks and Variables
	// =============================================================
	const {
		selectedOfficeState: [selectedOffice],
	} = useOffices();

	const { deskIdParam, setDeskParam } = useOfficeDeskRouteParams();
	const {
		desksState: [desks],
		selectedDeskState: [selectedDesk, setSelectedDesk],
	} = useOfficeDesks();

	const {
		bookingsState: [bookings],
		newBookingStartState: [newBookingStart],
	} = useBookings();

	// =============================================================
	// Effects
	// =============================================================

	useEffect(() => {
		onDeskSelected(deskIdParam);
	}, [deskIdParam]);

	useEffect(() => {
		onDeskSelected(deskIdParam);
	}, [desks]);

	// =============================================================
	// Functions
	// =============================================================

	function onDeskSelected(deskId?: string) {
		let changedDesk = deskId && desks?.find((d) => d.id === deskId);
		if (changedDesk) {
			setSelectedDesk(changedDesk);
			setDeskParam(deskId);
		} else if (desks.length > 0) {
			// Only clear out the selected desk if desks have been loaded
			setSelectedDesk(undefined);
			setDeskParam(undefined);
		}
	}

	// =============================================================
	// Render
	// =============================================================

	return (
		<Grid container spacing={2}>
			<Grid item lg={3} xs={12}>
				<BookingSubmissionForm />
			</Grid>
			<Grid item lg={5} xs={12}>
				<div>
					<Typography variant="h6" component="h2">
						Location &amp; Desk Selection
					</Typography>
					<OfficeSelector />
					{selectedOffice && (
						<Box marginTop={1}>
							<OfficeMap
								image={selectedOffice.image}
								desks={desks}
								selectedDesk={selectedDesk}
								bookings={bookings}
								newBookingStart={newBookingStart}
								onDeskSelected={onDeskSelected}
							/>
						</Box>
					)}
				</div>
			</Grid>
			<Grid item lg={4} xs={12}>
				<BookingsOverview />
			</Grid>
		</Grid>
	);
}

export default Offices;
