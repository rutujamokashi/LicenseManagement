import { useState } from "react";
import { generateLicense } from "../services/licenseService";

function LicenseGenerator() {
    const [formData, setFormData] = useState({
        companyName: "",
        validFrom: "",
        validTo: "",
        numberOfUsers: 1
    });

    const [result, setResult] = useState(null);
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);

    const handleChange = (event) => {
        const { name, value } = event.target;

        setFormData((previous) => ({
            ...previous,
            [name]: value
        }));
    };

    const handleReset = () => {
    setFormData({
        companyName: "",
        validFrom: "",
        validTo: "",
        numberOfUsers: 1
    });

    setResult(null);
    setError("");
};

    const handleSubmit = async (event) => {
        event.preventDefault();

        setError("");
        setResult(null);
        setLoading(true);

        try {
            const request = {
                companyName: formData.companyName.trim(),
                validFrom: formData.validFrom || null,
                validTo: formData.validTo || null,
                numberOfUsers: Number(formData.numberOfUsers)
            };

            const response = await generateLicense(request);

            setResult(response);
        }
        catch (error) {
            setError(error.message);
        }
        finally {
            setLoading(false);
        }
    };

    return (
        <div className="license-generator">

            <h2>Generate License</h2>

            <form onSubmit={handleSubmit}>

                <div className="form-group">
                    <label>Company Name</label>

                    <input
                        type="text"
                        name="companyName"
                        value={formData.companyName}
                        onChange={handleChange}
                        placeholder="Enter company name"
                        required
                    />
                </div>

                <div className="form-row">

                    <div className="form-group">
                        <label>Valid From</label>

                        <input
                            type="date"
                            name="validFrom"
                            value={formData.validFrom}
                            onChange={handleChange}
                        />
                    </div>

                    <div className="form-group">
                        <label>Valid To</label>

                        <input
                            type="date"
                            name="validTo"
                            value={formData.validTo}
                            onChange={handleChange}
                        />
                    </div>

                </div>

                <div className="form-group">
                    <label>Number of Users</label>

                    <input
                        type="number"
                        name="numberOfUsers"
                        min="1"
                        value={formData.numberOfUsers}
                        onChange={handleChange}
                        required
                    />
                </div>

                <div className="button-group">

    <button
        type="submit"
        disabled={loading}
    >
        {loading ? "Generating..." : "Generate License"}
    </button>

    <button
        type="button"
        className="reset-button"
        onClick={handleReset}
    >
        Clear
    </button>

</div>

            </form>

            {error && (
                <div className="error-message">
                    {error}
                </div>
            )}

            {result && (
                <div className="license-result">

                    <h3>License Generated Successfully</h3>

                    <div className="result-item">
                        <strong>Company:</strong>
                        <span>{result.companyName}</span>
                    </div>

                    <div className="result-item">
                        <strong>Valid From:</strong>
                        <span>
                            {result.validFrom
                                ? new Date(result.validFrom).toLocaleDateString()
                                : "No validity"}
                        </span>
                    </div>

                    <div className="result-item">
                        <strong>Valid To:</strong>
                        <span>
                            {result.validTo
                                ? new Date(result.validTo).toLocaleDateString()
                                : "No validity"}
                        </span>
                    </div>

                    <div className="result-item">
                        <strong>Number of Users:</strong>
                        <span>{result.numberOfUsers}</span>
                    </div>

                    <div className="license-key-section">
                        <strong>Encrypted License Key</strong>

                        <textarea
                            value={result.licenseKey}
                            readOnly
                            rows="5"
                        />
                    </div>

                </div>
            )}

        </div>
    );
}

export default LicenseGenerator;