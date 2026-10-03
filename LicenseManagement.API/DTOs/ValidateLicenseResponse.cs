namespace LicenseManagement.API.DTOs;

public class ValidateLicenseResponse
{
    public bool IsValid { get; set; }

    public string Message { get; set; } = string.Empty;

    public string CompanyName { get; set; } = string.Empty;

    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public int NumberOfUsers { get; set; }

    public bool HasValidityPeriod { get; set; }
    public string Status { get; set; } = string.Empty;
}