namespace LicenseManagement.API.Models;

public class LicensePayload
{
    public string CompanyName { get; set; } = string.Empty;

    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public int NumberOfUsers { get; set; }
}