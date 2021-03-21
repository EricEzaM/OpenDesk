import { Link } from "react-router-dom";
import SignInOut from "components/auth/SignInOut";

function Nav() {
	return (
		<nav className="nav">
			<Link to="/">
				<h1 className="nav__title">OpenDesk</h1>
			</Link>
			{/* <h1 className="main-title">OpenDesk</h1> */}
			<ul className="nav__links-list">
				<li>
					<Link to="/">Home</Link>
				</li>
				<li>
					<Link to="/me">My Bookings</Link>
				</li>
				<li>
					<Link to="/about">About</Link>
				</li>
			</ul>
			<SignInOut />
		</nav>
	);
}

export default Nav;
