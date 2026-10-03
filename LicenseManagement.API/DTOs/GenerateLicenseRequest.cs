using System.ComponentModel.DataAnnotations;

namespace LicenseManagement.API.DTOs;

public class GenerateLicenseRequest
{
    [Required]
    [MaxLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    [Range(1, int.MaxValue)]
    public int NumberOfUsers { get; set; }
}
