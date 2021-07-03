import { usePageTitle } from "hooks/usePageTitle";

function About() {
	usePageTitle("About");

	return (
		<div>
			<p>Welcome to OpenDesk! (About Page)</p>
		</div>
	);
}

export default About;
