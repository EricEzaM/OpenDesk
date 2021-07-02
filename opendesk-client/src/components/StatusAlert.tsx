import { Box } from "@material-ui/core";
import { Alert, AlertTitle } from "@material-ui/lab";

export type StatusData = {
	severity?: "info" | "error" | "success";
	message?: string;
	errors?: string[];
};

export default function StatusAlert({ severity, message, errors }: StatusData) {
	return (
		<>
			{severity && message && (
				<Alert severity={severity}>
					{errors && <AlertTitle>{message}</AlertTitle>}
					{!errors && (
						<Box fontWeight={500} fontSize="1rem">
							{message}
						</Box>
					)}
					{errors && (
						<ul>
							{errors?.map((e) => (
								<li>{e}</li>
							))}
						</ul>
					)}
				</Alert>
			)}
		</>
	);
}
