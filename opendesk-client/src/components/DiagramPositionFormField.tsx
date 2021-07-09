import {
	createStyles,
	InputAdornment,
	makeStyles,
	TextField,
	Theme,
} from "@material-ui/core";
import React from "react";
import { DiagramPosition } from "types";

export type DiagramPositionLimit = {
	min: [number, number];
	max: [number, number];
};

type DiagramPositionFormFieldProps = {
	dp: DiagramPosition;
	limit: DiagramPositionLimit;
	onChange: (dp: DiagramPosition) => void;
};

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		clampedWidth: {
			width: "50%",
		},
		noLeftBorderRadius: {
			borderTopLeftRadius: 0,
			borderBottomLeftRadius: 0,
		},
		noRightBorderRadius: {
			borderTopRightRadius: 0,
			borderBottomRightRadius: 0,
		},
	})
);

export default function DiagramPositionFormField({
	dp,
	limit,
	onChange,
}: DiagramPositionFormFieldProps) {
	const classes = useStyles();

	return (
		<>
			<TextField
				className={classes.clampedWidth}
				type="number"
				label="Diagram Position"
				variant="outlined"
				value={dp.x}
				InputProps={{
					classes: { root: classes.noRightBorderRadius },
					startAdornment: (
						<InputAdornment position="start">Horizontal</InputAdornment>
					),
				}}
				inputProps={{
					min: limit.min[1],
					max: limit.max[1],
				}}
				onChange={(e) => {
					const int = parseInt(e.currentTarget.value);
					if (!isNaN(int)) {
						onChange({ x: int, y: dp.y });
					}
				}}
			/>
			<TextField
				className={classes.clampedWidth}
				type="number"
				variant="outlined"
				value={dp.y}
				InputProps={{
					classes: { root: classes.noLeftBorderRadius },
					startAdornment: (
						<InputAdornment position="start">Veritcal</InputAdornment>
					),
				}}
				inputProps={{
					min: limit.min[0],
					max: limit.max[0],
				}}
				onChange={(e) => {
					const int = parseInt(e.currentTarget.value);
					if (!isNaN(int)) {
						onChange({ x: dp.x, y: int });
					}
				}}
			/>
		</>
	);
}
