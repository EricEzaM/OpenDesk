import { compile } from "path-to-regexp";
import { useRouteMatch, useHistory, useParams } from "react-router-dom";

export interface MainRouteParams {
	officeId?: string;
	deskId?: string;
}

function useOfficeDeskRouteParams() {
	const {
		officeId: officeIdParam,
		deskId: deskIdParam,
	} = useParams<MainRouteParams>();
	const history = useHistory();
	const match = useRouteMatch();

	function setOfficeParam(value?: string) {
		history.push({
			pathname: compile(match.path)({
				deskId: deskIdParam,
				officeId: value,
			}),
		});
	}

	function setDeskParam(value?: string) {
		history.push({
			pathname: compile(match.path)({
				deskId: value,
				officeId: officeIdParam,
			}),
		});
	}

	return {
		officeIdParam,
		deskIdParam,
		setOfficeParam,
		setDeskParam,
	};
}

export default useOfficeDeskRouteParams;
