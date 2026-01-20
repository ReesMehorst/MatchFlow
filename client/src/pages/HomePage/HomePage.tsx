import { Link } from "react-router-dom";
import "./HomePage.css";

export default function HomePage() {
    return (
        <div className="home">
            <main className="homeMain">
                <section className="hero" aria-labelledby="hero-title">
                    <div className="heroContent">
                        <p className="pill">Match tracking &#8226; Team profiles &#8226; Social posts</p>
                        <h1 id="hero-title" className="heroTitle">
                            MatchFlow helps teams organize matches and share progress.
                        </h1>
                        <p className="heroSubtitle">
                            Create teams, schedule matches, track players and results, and post highlights in one place.
                        </p>

                        <div className="heroCtas">
                            <Link className="btn btnPrimary btnLg" to="/register">Create your team</Link>
                            <Link className="btn btnSecondary btnLg" to="/matches">Browse matches</Link>
                        </div>

                        <div className="heroMeta">
                            <span className="metaItem">Role-based access</span>
                            <span className="metaDot" aria-hidden="true">&#8226;</span>
                            <span className="metaItem">Fast filtering</span>
                            <span className="metaDot" aria-hidden="true">&#8226;</span>
                            <span className="metaItem">Accessible UI</span>
                        </div>
                    </div>

                    <aside className="heroPanel" aria-label="Preview">
                        <div className="panelCard">
                            <div className="panelHeader">
                                <span className="panelBadge">Preview</span>
                                <span className="panelTitle">Today</span>
                            </div>

                            <div className="panelRow">
                                <div className="panelDot" aria-hidden="true" />
                                <div className="panelText">
                                    <strong>Match scheduled</strong>
                                    <div className="panelSub">Team A vs Team B &#8226; 19:30</div>
                                </div>
                            </div>

                            <div className="panelRow">
                                <div className="panelDot panelDotAlt" aria-hidden="true" />
                                <div className="panelText">
                                    <strong>New post</strong>
                                    <div className="panelSub">"Clip of the final round"</div>
                                </div>
                            </div>

                            <div className="panelRow">
                                <div className="panelDot panelDotAlt2" aria-hidden="true" />
                                <div className="panelText">
                                    <strong>Roster updated</strong>
                                    <div className="panelSub">2 participants added</div>
                                </div>
                            </div>
                        </div>
                    </aside>
                </section>

                <section className="section" aria-labelledby="features-title">
                    <div className="sectionHeader">
                        <h2 id="features-title" className="sectionTitle">Core features</h2>
                        <p className="sectionSubtitle">Everything you need for teams and matches.</p>
                    </div>

                    <div className="grid3">
                        <article className="featureCard">
                            <h3 className="featureTitle">Teams</h3>
                            <p className="featureText">Create a team profile, manage members, and connect your games.</p>
                        </article>

                        <article className="featureCard">
                            <h3 className="featureTitle">Matches</h3>
                            <p className="featureText">Schedule matches, track status, score, and participants per team.</p>
                        </article>

                        <article className="featureCard">
                            <h3 className="featureTitle">Feed</h3>
                            <p className="featureText">Share posts, comment, and like highlights. public or team-only.</p>
                        </article>
                    </div>
                </section>

                <section className="section" aria-labelledby="how-title">
                    <div className="sectionHeader">
                        <h2 id="how-title" className="sectionTitle">How it works</h2>
                        <p className="sectionSubtitle">A simple flow from setup to sharing.</p>
                    </div>

                    <ol className="steps">
                        <li className="step">
                            <span className="stepNr">1</span>
                            <div>
                                <strong>Create a team</strong>
                                <div className="stepText">Add members and select the game(s) you play.</div>
                            </div>
                        </li>
                        <li className="step">
                            <span className="stepNr">2</span>
                            <div>
                                <strong>Schedule matches</strong>
                                <div className="stepText">Set date/time, teams, and status-update score when finished.</div>
                            </div>
                        </li>
                        <li className="step">
                            <span className="stepNr">3</span>
                            <div>
                                <strong>Post highlights</strong>
                                <div className="stepText">Share achievements, clips, and discuss with comments.</div>
                            </div>
                        </li>
                    </ol>

                    <div className="ctaStrip">
                        <div>
                            <h3 className="ctaTitle">Ready to set up MatchFlow?</h3>
                            <p className="ctaText">Start with a team, then add your first match.</p>
                        </div>
                        <Link className="btn btnPrimary btnLg" to="/register">Get started</Link>
                    </div>
                </section>
            </main>
        </div>
    );
}