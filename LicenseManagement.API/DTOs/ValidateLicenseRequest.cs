namespace LicenseManagement.API.DTOs;

public class ValidateLicenseRequest
{
    public string LicenseKey { get; set; } = string.Empty;

    public string CompanyName { get; set; } = string.Empty;

    public int NumberOfUsers { get; set; }
}