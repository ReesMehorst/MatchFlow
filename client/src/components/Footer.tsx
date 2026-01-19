import { Link } from "react-router-dom";
import "./Footer.css";
export default function Footer() {
    return (
        <footer className="siteFooter">
            <div className="container footerInner">
                <span>&#169; {new Date().getFullYear()} MatchFlow</span>

                <div className="footerLinks">
                    <Link className="footerLink" to="/privacy">Privacy</Link>
                    <span className="footerDot" aria-hidden="true">&#8226;</span>
                    <Link className="footerLink" to="/about">About</Link>
                    <span className="footerDot" aria-hidden="true">&#8226;</span>
                    <Link className="footerLink" to="/contact">Contact</Link>
                </div>
            </div>
        </footer>
    );
}