import { Box, createStyles, makeStyles, Theme } from "@material-ui/core";
import { Alert, AlertTitle } from "@material-ui/lab";

export type StatusData = {
	severity?: "info" | "error" | "success";
	message?: string;
	errors?: string[];
};

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		alertRoot: {
			paddingBottom: 0,
			paddingTop: 0,
			lineHeight: "inherit",
		},
	})
);

export default function StatusAlert({ severity, message, errors }: StatusData) {
	const classes = useStyles();

	return (
		<>
			{severity && message && (
				<Alert severity={severity} className={classes.alertRoot}>
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
