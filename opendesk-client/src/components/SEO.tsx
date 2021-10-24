import { usePageTitle } from "hooks/usePageTitle";
import { Helmet } from "react-helmet";

export default function SEO() {
	const title = usePageTitle();

	return (
		<Helmet
			title={title.length > 0 ? title : "OpenDesk"}
			titleTemplate={`OpenDesk | %s `}
		/>
	);
}
