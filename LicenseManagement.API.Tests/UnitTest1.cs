using System.Text.Json;
using LicenseManagement.API.Data;
using LicenseManagement.API.DTOs;
using LicenseManagement.API.Models;
using LicenseManagement.API.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LicenseManagement.API.Tests;

public class LicenseServiceTests
{
    private static LicenseDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<LicenseDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new LicenseDbContext(options);
    }

    private static LicenseService CreateService(
        LicenseDbContext dbContext,
        Mock<IEncryptionService> encryptionService)
    {
        return new LicenseService(
            encryptionService.Object,
            dbContext);
    }

    [Fact]
    public async Task RevokeLicenseAsync_ShouldReturnFalse_WhenLicenseDoesNotExist()
    {
        await using var dbContext = CreateDbContext();

        var encryptionService = new Mock<IEncryptionService>();
        var service = CreateService(dbContext, encryptionService);

        var result = await service.RevokeLicenseAsync("INVALID-KEY");

        Assert.False(result);
    }

    [Fact]
    public async Task RevokeLicenseAsync_ShouldRevokeExistingLicense()
    {
        await using var dbContext = CreateDbContext();

        dbContext.Licenses.Add(new License
        {
            CompanyName = "Test Company",
            LicenseKey = "TEST-KEY",
            NumberOfUsers = 10,
            CreatedAt = DateTime.UtcNow,
            Status = LicenseStatus.Active
        });

        await dbContext.SaveChangesAsync();

        var encryptionService = new Mock<IEncryptionService>();
        var service = CreateService(dbContext, encryptionService);

        var result = await service.RevokeLicenseAsync("TEST-KEY");

        var updatedLicense = await dbContext.Licenses
            .FirstAsync(x => x.LicenseKey == "TEST-KEY");

        Assert.True(result);
        Assert.Equal(LicenseStatus.Revoked, updatedLicense.Status);
    }

    [Fact]
    public async Task ValidateLicenseAsync_ShouldReturnValid_WhenLicenseDetailsMatch()
    {
        await using var dbContext = CreateDbContext();

        dbContext.Licenses.Add(new License
        {
            CompanyName = "Test Company",
            LicenseKey = "TEST-KEY",
            NumberOfUsers = 10,
            CreatedAt = DateTime.UtcNow,
            Status = LicenseStatus.Active
        });

        await dbContext.SaveChangesAsync();

        var payload = new LicensePayload
        {
            CompanyName = "Test Company",
            NumberOfUsers = 10
        };

        var encryptionService = new Mock<IEncryptionService>();

        encryptionService
            .Setup(x => x.Decrypt("TEST-KEY"))
            .Returns(JsonSerializer.Serialize(payload));

        var service = CreateService(dbContext, encryptionService);

        var request = new ValidateLicenseRequest
        {
            LicenseKey = "TEST-KEY",
            CompanyName = "Test Company",
            NumberOfUsers = 10
        };

        var result = await service.ValidateLicenseAsync(request);

        Assert.True(result.IsValid);
        Assert.Equal("Test Company", result.CompanyName);
        Assert.Equal(10, result.NumberOfUsers);
    }

    [Fact]
    public async Task ValidateLicenseAsync_ShouldReturnInvalid_WhenCompanyNameDoesNotMatch()
    {
        await using var dbContext = CreateDbContext();

        dbContext.Licenses.Add(new License
        {
            CompanyName = "ABC Company",
            LicenseKey = "TEST-KEY",
            NumberOfUsers = 10,
            CreatedAt = DateTime.UtcNow,
            Status = LicenseStatus.Active
        });

        await dbContext.SaveChangesAsync();

        var payload = new LicensePayload
        {
            CompanyName = "ABC Company",
            NumberOfUsers = 10
        };

        var encryptionService = new Mock<IEncryptionService>();

        encryptionService
            .Setup(x => x.Decrypt("TEST-KEY"))
            .Returns(JsonSerializer.Serialize(payload));

        var service = CreateService(dbContext, encryptionService);

        var request = new ValidateLicenseRequest
        {
            LicenseKey = "TEST-KEY",
            CompanyName = "XYZ Company",
            NumberOfUsers = 10
        };

        var result = await service.ValidateLicenseAsync(request);

        Assert.False(result.IsValid);
        Assert.Equal(
            "Company name does not match the license.",
            result.Message);
    }

    [Fact]
    public async Task ValidateLicenseAsync_ShouldReturnInvalid_WhenNumberOfUsersDoesNotMatch()
    {
        await using var dbContext = CreateDbContext();

        dbContext.Licenses.Add(new License
        {
            CompanyName = "Test Company",
            LicenseKey = "TEST-KEY",
            NumberOfUsers = 10,
            CreatedAt = DateTime.UtcNow,
            Status = LicenseStatus.Active
        });

        await dbContext.SaveChangesAsync();

        var payload = new LicensePayload
        {
            CompanyName = "Test Company",
            NumberOfUsers = 10
        };

        var encryptionService = new Mock<IEncryptionService>();

        encryptionService
            .Setup(x => x.Decrypt("TEST-KEY"))
            .Returns(JsonSerializer.Serialize(payload));

        var service = CreateService(dbContext, encryptionService);

        var request = new ValidateLicenseRequest
        {
            LicenseKey = "TEST-KEY",
            CompanyName = "Test Company",
            NumberOfUsers = 20
        };

        var result = await service.ValidateLicenseAsync(request);

        Assert.False(result.IsValid);
        Assert.Equal(
            "Number of users do not match the license.",
            result.Message);
    }

    [Fact]
    public async Task ValidateLicenseAsync_ShouldReturnInvalid_WhenLicenseIsRevoked()
    {
        await using var dbContext = CreateDbContext();

        dbContext.Licenses.Add(new License
        {
            CompanyName = "Test Company",
            LicenseKey = "TEST-KEY",
            NumberOfUsers = 10,
            CreatedAt = DateTime.UtcNow,
            Status = LicenseStatus.Revoked
        });

        await dbContext.SaveChangesAsync();

        var encryptionService = new Mock<IEncryptionService>();
        var service = CreateService(dbContext, encryptionService);

        var request = new ValidateLicenseRequest
        {
            LicenseKey = "TEST-KEY",
            CompanyName = "Test Company",
            NumberOfUsers = 10
        };

        var result = await service.ValidateLicenseAsync(request);

        Assert.False(result.IsValid);
        Assert.Equal(
            "License has been revoked.",
            result.Message);
        Assert.Equal("Revoked", result.Status);
    }
}