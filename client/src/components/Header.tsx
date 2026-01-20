import { NavLink, Link } from "react-router-dom";
import logo from "../assets/MatchFlowLogo.svg";
import "./Header.css";
import { useAuth } from "../context/useAuth";
import { useEffect, useRef, useState } from "react";

export default function Header() {
    const { user, isAuthenticated, logout } = useAuth();
    const [open, setOpen] = useState(false);
    const menuRef = useRef<HTMLDivElement | null>(null);

    useEffect(() => {
        function onDoc(e: MouseEvent) {
            if (!menuRef.current) return;
            if (!menuRef.current.contains(e.target as Node)) setOpen(false);
        }
        document.addEventListener("click", onDoc);
        return () => document.removeEventListener("click", onDoc);
    }, []);

    function initials(name?: string) {
        if (!name) return "";
        const parts = name.trim().split(/\s+/);
        return (parts[0]?.[0] ?? "") + (parts.length > 1 ? parts[1][0] : "");
    }

    const roleLinks: { label: string; to: string; roles: string[] }[] = [
        { label: "Admin dashboard", to: "/admin", roles: ["Admin"] },
        { label: "My teams", to: "/teams", roles: ["User"] },
        { label: "Matches", to: "/matches", roles: ["User"] },
    ];

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
                    {!isAuthenticated && (
                        <>
                            <Link className="btn btnGhost" to="/login">Log in</Link>
                            <Link className="btn btnPrimary" to="/register">Get started</Link>
                        </>
                    )}

                    {isAuthenticated && user && (
                        <div className="userMenu" ref={menuRef}>
                            <button
                                className="userButton"
                                aria-haspopup="true"
                                aria-expanded={open}
                                onClick={() => setOpen((s) => !s)}
                            >
                                {user.teamLogoUrl ? (
                                    <img src={user.teamLogoUrl} alt={`${user.teamTag ?? "team"} logo`} className="avatar" />
                                ) : (
                                    <div className="avatar avatarInitials">{initials(user.displayName)}</div>
                                )}
                                <div className="userInfo">
                                    <div className="displayName">{user.displayName}</div>
                                    {user.teamTag && <div className="teamPill">{user.teamTag}</div>}
                                </div>
                            </button>

                            {open && (
                                <div className="userDropdown" role="menu">
                                    <div className="dropdownSection">
                                        {roleLinks
                                            .filter((rl) => rl.roles.some((r) => user.roles.includes(r)))
                                            .map((rl) => (
                                                <Link key={rl.to} to={rl.to} className="dropdownItem" role="menuitem" onClick={() => setOpen(false)}>
                                                    {rl.label}
                                                </Link>
                                            ))}
                                    </div>

                                    <div className="dropdownSection">
                                        <Link to="/profile" className="dropdownItem" role="menuitem" onClick={() => setOpen(false)}>
                                            Profile
                                        </Link>
                                        <button className="dropdownItem btnPlain" onClick={() => { logout(); setOpen(false); }}>
                                            Log out
                                        </button>
                                    </div>
                                </div>
                            )}
                        </div>
                    )}
                </div>
            </div>
        </header>
    );
}