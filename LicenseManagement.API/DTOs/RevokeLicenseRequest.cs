namespace LicenseManagement.API.DTOs;

public class RevokeLicenseRequest
{
    public string LicenseKey { get; set; } = string.Empty;
}