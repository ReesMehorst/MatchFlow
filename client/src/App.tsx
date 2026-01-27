import { BrowserRouter, Routes, Route } from "react-router-dom";
import MainLayout from "./layouts/MainLayout";
import HomePage from "./pages/HomePage/HomePage";

import LoginPage from "./pages/LoginPage/LoginPage";
import RegisterPage from "./pages/RegisterPage/RegisterPage";

import RequireAuth from "./components/RequireAuth";
import RequireUnauth from "./components/RequireUnauth";

import TeamsPage from "./pages/Teams/pages/TeamsPage";
import CreateTeamPage from "./pages/Teams/pages/CreateTeamPage";
import TeamPage from "./pages/Teams/pages/TeamPage";

import ProfilePage from "./pages/Profile/pages/ProfilePage";


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

                    <Route
                        path="/profile"
                        element={
                            <RequireAuth>
                                <ProfilePage />
                            </RequireAuth>
                        }
                    />

                    <Route path="/teams" element={<TeamsPage />} />
                    <Route path="/teams/create" element={<CreateTeamPage />} />
                    <Route path="/teams/:id" element={<TeamPage />} />


                </Route>
            </Routes>
        </BrowserRouter>
    );
}