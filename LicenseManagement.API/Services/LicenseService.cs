using LicenseManagement.API.Data;
using LicenseManagement.API.DTOs;
using LicenseManagement.API.Models;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace LicenseManagement.API.Services;

public class LicenseService : ILicenseService
{
    private readonly IEncryptionService _encryptionService;
    private readonly LicenseDbContext _dbContext;

    public LicenseService(
        IEncryptionService encryptionService,
        LicenseDbContext dbContext)
    {
        _encryptionService = encryptionService;
        _dbContext = dbContext;
    }

    public async Task<GenerateLicenseResponse> GenerateLicenseAsync(GenerateLicenseRequest request)
    {
        ValidateRequest(request);

        var payload = new LicensePayload
        {
            CompanyName = request.CompanyName,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo,
            NumberOfUsers = request.NumberOfUsers
        };

        var json = JsonSerializer.Serialize(payload);

        var licenseKey = _encryptionService.Encrypt(json);
        var license = new License
        {
            CompanyName = payload.CompanyName,
            ValidFrom = payload.ValidFrom,
            ValidTo = payload.ValidTo,
            NumberOfUsers = payload.NumberOfUsers,
            LicenseKey = licenseKey,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Licenses.Add(license);
        await _dbContext.SaveChangesAsync();

        return new GenerateLicenseResponse
        {
            LicenseKey = licenseKey,
            CompanyName = payload.CompanyName,
            ValidFrom = payload.ValidFrom,
            ValidTo = payload.ValidTo,
            NumberOfUsers = payload.NumberOfUsers
        };
    }

    public async Task<ValidateLicenseResponse> ValidateLicenseAsync(ValidateLicenseRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.LicenseKey))
        {
            return InvalidLicense("License key is required.");
        }

        try
        {
            var license = await _dbContext.Licenses.FirstOrDefaultAsync(x => x.LicenseKey == request.LicenseKey);

            if (license == null)
            {
                return InvalidLicense(
                    "License key was not found.");
            }

            if (license.Status == LicenseStatus.Revoked)
            {
                return new ValidateLicenseResponse
                {
                    IsValid = false,
                    Message = "License has been revoked.",
                    CompanyName = license.CompanyName,
                    ValidFrom = license.ValidFrom,
                    ValidTo = license.ValidTo,
                    NumberOfUsers = license.NumberOfUsers,
                    HasValidityPeriod =
                        license.ValidFrom.HasValue ||
                        license.ValidTo.HasValue,
                    Status = "Revoked"
                };
            }

            var json = _encryptionService.Decrypt(
                license.LicenseKey);

            var payload = JsonSerializer.Deserialize<LicensePayload>(
                json);

            if (payload == null)
            {
                return InvalidLicense(
                    "Unable to read license information.");
            }

            if (!string.Equals(
                    payload.CompanyName.Trim(),
                    request.CompanyName.Trim(),
                    StringComparison.OrdinalIgnoreCase))
            {
                return InvalidLicense(
                    "Company name does not match the license.",
                    payload);
            }

            if (payload.NumberOfUsers != request.NumberOfUsers)
            {
                return InvalidLicense(
                    "Number of users do not match the license.",
                    payload);
            }

            // No validity configured
            if (!payload.ValidFrom.HasValue &&
                !payload.ValidTo.HasValue)
            {
                return new ValidateLicenseResponse
                {
                    IsValid = true,
                    Message =
                        "License is valid. No validity period is configured.",
                    CompanyName = payload.CompanyName,
                    NumberOfUsers = payload.NumberOfUsers,
                    HasValidityPeriod = false
                };
            }

            var today = DateTime.UtcNow.Date;

            if (payload.ValidFrom.HasValue && today < payload.ValidFrom.Value.Date)
            {
                return InvalidLicense(
                    "License is not active yet.",
                    payload);
            }

            if (payload.ValidTo.HasValue && today > payload.ValidTo.Value.Date)
            {
                return InvalidLicense(
                    "License has expired.",
                    payload);
            }

            if (request.NumberOfUsers <= 0)
            {
                throw new ArgumentException(
                    "Number of users must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(request.CompanyName))
            {
                throw new ArgumentException(
                    "Company name is required.");
            }
            var now = DateTime.UtcNow;


            if (license.ValidFrom.HasValue &&  now < license.ValidFrom.Value)
            {
                return new ValidateLicenseResponse
                {
                    IsValid = false,
                    Message = "License is not active yet.",
                    CompanyName = license.CompanyName,
                    ValidFrom = license.ValidFrom,
                    ValidTo = license.ValidTo,
                    NumberOfUsers = license.NumberOfUsers,
                    HasValidityPeriod = true,
                    Status = "Not Yet Active"
                };
            }

            if (license.ValidTo.HasValue && now > license.ValidTo.Value)
            {
                return new ValidateLicenseResponse
                {
                    IsValid = false,
                    Message = "License has expired.",
                    CompanyName = license.CompanyName,
                    ValidFrom = license.ValidFrom,
                    ValidTo = license.ValidTo,
                    NumberOfUsers = license.NumberOfUsers,
                    HasValidityPeriod = true,
                    Status = "Expired"
                };
            }

            return new ValidateLicenseResponse
            {
                IsValid = true,
                Message = "License is valid.",
                CompanyName = payload.CompanyName,
                ValidFrom = payload.ValidFrom,
                ValidTo = payload.ValidTo,
                NumberOfUsers = payload.NumberOfUsers,
                HasValidityPeriod = true
            };
        }
        catch (FormatException)
        {
            return InvalidLicense(
                "Invalid license key format.");
        }
        catch (CryptographicException)
        {
            return InvalidLicense(
                "License key could not be decrypted.");
        }
        catch (JsonException)
        {
            return InvalidLicense(
                "License data is invalid.");
        }
    }

    private static void ValidateRequest(
        GenerateLicenseRequest request)
    {
        if (request.ValidFrom.HasValue &&
            request.ValidTo.HasValue &&
            request.ValidFrom > request.ValidTo)
        {
            throw new ArgumentException(
                "Valid From date cannot be greater than Valid To date.");
        }

        if (request.ValidFrom.HasValue !=
            request.ValidTo.HasValue)
        {
            throw new ArgumentException(
                "Both Valid From and Valid To dates must be provided together.");
        }
    }

    private static ValidateLicenseResponse InvalidLicense(
        string message,
        LicensePayload? payload = null)
    {
        return new ValidateLicenseResponse
        {
            IsValid = false,
            Message = message,
            CompanyName = payload?.CompanyName ?? string.Empty,
            ValidFrom = payload?.ValidFrom,
            ValidTo = payload?.ValidTo,
            NumberOfUsers = payload?.NumberOfUsers ?? 0,
            HasValidityPeriod =
                payload?.ValidFrom.HasValue == true ||
                payload?.ValidTo.HasValue == true
        };
    }

    public async Task<bool> RevokeLicenseAsync(
    string licenseKey)
    {
        var license = await _dbContext.Licenses
            .FirstOrDefaultAsync(x => x.LicenseKey == licenseKey);

        if (license == null)
        {
            return false;
        }

        license.Status = LicenseStatus.Revoked;

        await _dbContext.SaveChangesAsync();

        return true;
    }


}