import {
	ButtonBase,
	createStyles,
	Grid,
	makeStyles,
	Theme,
	Typography,
} from "@material-ui/core";
import { Business, Group, Room } from "@material-ui/icons";

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		button: {
			height: "clamp(100px, 40vh , 300px)",
			borderStyle: "solid",
			borderWidth: "1px",
			borderRadius: theme.shape.borderRadius,
			flexDirection: "column",
			width: "100%",
		},
		buttonTitle: {
			fontSize: "1.5rem",
		},
	})
);

export default function Manage() {
	const classes = useStyles();

	const iconStyleDefaults = {
		fontSize: 60,
		marginBottom: 10,
	};

	const thingos = [
		{
			title: "Offices",
			desc: "Create and edit offices.",
			icon: <Business color="primary" style={iconStyleDefaults} />,
		},
		{
			title: "Desks",
			desc: "Create and edit desks - including desk location, names and other information.",
			icon: <Room color="secondary" style={iconStyleDefaults} />,
		},
		{
			title: "Users",
			desc: "Manage users and their permissions.",
			icon: <Group style={iconStyleDefaults} />,
		},
	];

	return (
		<Grid container spacing={2} justify="center">
			{thingos.map((bd) => (
				<Grid key={bd.title} item sm={4} xs={12}>
					<ButtonBase className={classes.button} focusRipple>
						{bd.icon}
						<Typography className={classes.buttonTitle}>{bd.title}</Typography>
						<Typography variant="body1">{bd.desc}</Typography>
					</ButtonBase>
				</Grid>
			))}
		</Grid>
	);
}
