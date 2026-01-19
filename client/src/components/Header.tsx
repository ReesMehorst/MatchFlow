import { NavLink, Link } from "react-router-dom";
import logo from "../assets/MatchFlowLogo.svg";
import "./Header.css";
export default function Header() {
    return (
        <header className="siteHeader">
            <div className="container headerInner">
                <Link to="/" className="brand" aria-label="MatchFlow home">
                    <img className="brandLogo" src={logo} alt="MatchFlow logo" />
                </Link>

                <nav className="nav" aria-label="Primary">
                    <NavLink className="navLink" to="/feed">Feed</NavLink>
                    <NavLink className="navLink" to="/teams">Teams</NavLink>
                    <NavLink className="navLink" to="/matches">Matches</NavLink>
                    <NavLink className="navLink" to="/games">Games</NavLink>
                </nav>

                <div className="headerActions">
                    <Link className="btn btnGhost" to="/login">Log in</Link>
                    <Link className="btn btnPrimary" to="/register">Get started</Link>
                </div>
            </div>
        </header>
    );
}