using LicenseManagement.API.Data;
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

    [Fact]
    public async Task RevokeLicenseAsync_ShouldReturnFalse_WhenLicenseDoesNotExist()
    {
        await using var dbContext = CreateDbContext();

        var encryptionService = new Mock<IEncryptionService>();
        var service = new LicenseService(
            encryptionService.Object,
            dbContext);

        var result = await service.RevokeLicenseAsync("INVALID-KEY");

        Assert.False(result);
    }

    [Fact]
    public async Task RevokeLicenseAsync_ShouldRevokeExistingLicense()
    {
        await using var dbContext = CreateDbContext();

        var license = new License
        {
            CompanyName = "Test Company",
            LicenseKey = "TEST-KEY",
            NumberOfUsers = 10,
            CreatedAt = DateTime.UtcNow,
            Status = LicenseStatus.Active
        };

        dbContext.Licenses.Add(license);
        await dbContext.SaveChangesAsync();

        var encryptionService = new Mock<IEncryptionService>();
        var service = new LicenseService(
            encryptionService.Object,
            dbContext);

        var result = await service.RevokeLicenseAsync("TEST-KEY");

        var updatedLicense = await dbContext.Licenses
            .FirstAsync(x => x.LicenseKey == "TEST-KEY");

        Assert.True(result);
        Assert.Equal(LicenseStatus.Revoked, updatedLicense.Status);
    }
}