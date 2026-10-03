namespace LicenseManagement.API.DTOs;

public class GenerateLicenseResponse
{
    public string LicenseKey { get; set; } = string.Empty;

    public string CompanyName { get; set; } = string.Empty;

    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public int NumberOfUsers { get; set; }
}