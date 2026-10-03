import { useState } from "react";
import LicenseGenerator from "./components/LicenseGenerator";
import LicenseValidator from "./components/LicenseValidator";
import "./App.css";

function App() {
    const [activeTab, setActiveTab] = useState("generate");

    return (
        <div className="app">

            <header className="app-header">
                <div>
                    <h1>License Management</h1>
                    <p>
                        Generate, validate and manage software licenses
                    </p>
                </div>

                <div className="header-badge">
                    .NET 10 + React
                </div>
            </header>

            <main className="dashboard">

                <div className="dashboard-title">
                    <div>
                        <h2>License Dashboard</h2>
                        <p>
                            Manage application licenses from one place.
                        </p>
                    </div>
                </div>

                <div className="tabs">

                    <button
                        className={
                            activeTab === "generate"
                                ? "tab active"
                                : "tab"
                        }
                        onClick={() => setActiveTab("generate")}
                    >
                        Generate License
                    </button>

                    <button
                        className={
                            activeTab === "validate"
                                ? "tab active"
                                : "tab"
                        }
                        onClick={() => setActiveTab("validate")}
                    >
                        Validate License
                    </button>

                </div>

                <div className="tab-content">

                    {activeTab === "generate" && (
                        <LicenseGenerator />
                    )}

                    {activeTab === "validate" && (
                        <LicenseValidator />
                    )}

                </div>

            </main>

        </div>
    );
}

export default App;