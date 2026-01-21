import { BrowserRouter, Routes, Route } from "react-router-dom";
import MainLayout from "./layouts/MainLayout";
import HomePage from "./pages/HomePage/HomePage";
import LoginPage from "./pages/LoginPage/LoginPage";
import RegisterPage from "./pages/RegisterPage/RegisterPage";
import RequireUnauth from "./components/RequireUnauth";
import TeamsPage from "./pages/Teams/pages/TeamsPage";

export default function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route element={<MainLayout />}>
                    <Route path="/" element={<HomePage />} />
                    <Route
                        path="/login"
                        element={
                            <RequireUnauth>
                                <LoginPage />
                            </RequireUnauth>
                        }
                    />
                    <Route
                        path="/register"
                        element={
                            <RequireUnauth>
                                <RegisterPage />
                            </RequireUnauth>
                        }
                    />
                    <Route path="/teams" element={<TeamsPage />} />
                </Route>
            </Routes>
        </BrowserRouter>
    );
}