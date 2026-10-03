using LicenseManagement.API.DTOs;

namespace LicenseManagement.API.Services;

public interface ILicenseService
{
    Task<GenerateLicenseResponse> GenerateLicenseAsync(
    GenerateLicenseRequest request);

    Task<ValidateLicenseResponse> ValidateLicenseAsync(
        ValidateLicenseRequest request);

    Task<bool> RevokeLicenseAsync(string licenseKey);
}