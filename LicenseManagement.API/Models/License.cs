namespace LicenseManagement.API.Models;

public class License
{
    public int Id { get; set; }

    public string CompanyName { get; set; } = string.Empty;

    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public int NumberOfUsers { get; set; }

    public string LicenseKey { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public LicenseStatus Status { get; set; } = LicenseStatus.Active;
}