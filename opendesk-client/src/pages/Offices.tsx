import { Desk } from "types";

import OfficeSelector from "components/OfficeSelector";
import OfficeMap from "components/map/OfficeMap";
import BookingSubmissionForm from "components/BookingSubmissionForm";
import { useOfficeDesks } from "hooks/useOfficeDesks";
import { Grid, Typography } from "@material-ui/core";
import { useOffices } from "hooks/useOffices";
import BookingsOverview from "components/BookingsOverview";

function Offices() {
	// =============================================================
	// Hooks and Variables
	// =============================================================

	const {
		desksState: [desks, setDesks],
		selectedDeskState: [selectedDesk, setSelectedDesk],
	} = useOfficeDesks();

	const {
		selectedOfficeState: [selectedOffice],
	} = useOffices();

	// =============================================================
	// Effects
	// =============================================================

	// =============================================================
	// Functions
	// =============================================================

	function onDeskSelected(desk?: Desk) {
		setSelectedDesk(desk);
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
					<OfficeSelector />
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
				<BookingsOverview />
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
