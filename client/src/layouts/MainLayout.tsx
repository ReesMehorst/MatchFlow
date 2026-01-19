import { Outlet } from "react-router-dom";
import Header from "../components/Header";
import Footer from "../components/Footer";
import "../index.css"; // or import a dedicated global css file here (see section 2)

export default function MainLayout() {
    return (
        <div className="appShell">
            <Header />
            <main className="appMain" role="main">
                <Outlet />
            </main>
            <Footer />
        </div>
    );
}
