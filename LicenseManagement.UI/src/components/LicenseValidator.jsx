import { useState } from "react";
import { validateLicense,revokeLicense } from "../services/licenseService";


function LicenseValidator() {

    const [formData, setFormData] = useState({
        licenseKey: "",
        companyName: "",
        numberOfUsers: 1
    });

    const [revokeMessage, setRevokeMessage] = useState("");

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

    const handleSubmit = async (event) => {
        event.preventDefault();

        setError("");
        setResult(null);
        setLoading(true);

        try {

            const request = {
                licenseKey: formData.licenseKey.trim(),
                companyName: formData.companyName.trim(),
                numberOfUsers: Number(formData.numberOfUsers)
            };

            const response = await validateLicense(request);

            setResult(response);

        } catch (error) {

            setError(error.message);

        } finally {

            setLoading(false);
        }
    };

    const handleReset = () => {
    setFormData({
        licenseKey: "",
        companyName: "",
        numberOfUsers: 1
    });

    setResult(null);
    setError("");
    setRevokeMessage("");
};

    const handleRevoke = async () => {
    if (!formData.licenseKey.trim()) {
        setError("Please enter a license key.");
        return;
    }

    setError("");
    setRevokeMessage("");

    try {
        const response = await revokeLicense(
            formData.licenseKey.trim()
        );

        setRevokeMessage(
            response.message || "License revoked successfully."
        );

        setResult(null);
    }
    catch (error) {
        setError(error.message);
    }
};

    return (
        <div className="license-validator">

            <h2>Validate License</h2>

            <form onSubmit={handleSubmit}>

                <div className="form-group">
                    <label>License Key</label>

                    <textarea
                        name="licenseKey"
                        value={formData.licenseKey}
                        onChange={handleChange}
                        placeholder="Paste license key"
                        rows="5"
                        required
                    />
                </div>

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
        {loading ? "Validating..." : "Validate License"}
    </button>

    <button
        type="button"
        className="revoke-button"
        onClick={handleRevoke}
    >
        Revoke License
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
            {revokeMessage && (
               <div className="success-message">
                     {revokeMessage}
                </div>
            )}

            {result && (
                <div
                    className={
                        result.isValid
                            ? "validation-result valid"
                            : "validation-result invalid"
                    }
                >

                    <h3>
                        {result.isValid
                            ? "License Valid"
                            : "License Invalid"}
                    </h3>

                    <p>
                        <strong>Status:</strong>{" "}
                        {result.status}
                    </p>

                    <p>
                        <strong>Message:</strong>{" "}
                        {result.message}
                    </p>

                    <p>
                        <strong>Company:</strong>{" "}
                        {result.companyName}
                    </p>

                    <p>
                        <strong>Users:</strong>{" "}
                        {result.numberOfUsers}
                    </p>

                    <p>
                        <strong>Validity Period:</strong>{" "}
                        {result.hasValidityPeriod
                            ? "Yes"
                            : "No"}
                    </p>

                    {result.hasValidityPeriod && (
                        <>
                            <p>
                                <strong>Valid From:</strong>{" "}
                                {result.validFrom
                                    ? new Date(
                                        result.validFrom
                                    ).toLocaleDateString()
                                    : "-"}
                            </p>

                            <p>
                                <strong>Valid To:</strong>{" "}
                                {result.validTo
                                    ? new Date(
                                        result.validTo
                                    ).toLocaleDateString()
                                    : "-"}
                            </p>
                        </>
                    )}

                </div>
            )}

        </div>
    );
}

export default LicenseValidator;