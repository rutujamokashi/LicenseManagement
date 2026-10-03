//const API_BASE_URL = "https://localhost:7263/api/Licenses";
const API_BASE_URL = "http://localhost:8080/api/Licenses";

export async function generateLicense(licenseData) {
    const response = await fetch(`${API_BASE_URL}/generate`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(licenseData)
    });

    if (!response.ok) {
        const errorText = await response.text();
        throw new Error(errorText || "Failed to generate license.");
    }

    return await response.json();
}
export async function validateLicense(licenseData) {
    const response = await fetch(`${API_BASE_URL}/validate`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(licenseData)
    });

    if (!response.ok) {
        const errorText = await response.text();
        throw new Error(errorText || "Failed to validate license.");
    }

    return await response.json();
}
export async function revokeLicense(licenseKey) {
    const response = await fetch(`${API_BASE_URL}/revoke`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            licenseKey
        })
    });

    if (!response.ok) {
        const errorText = await response.text();
        throw new Error(errorText || "Failed to revoke license.");
    }

    return await response.json();
}