using LicenseManagement.API.Models;

namespace LicenseManagement.API.Tests;

public class LicenseTests
{
    [Fact]
    public void NewLicense_ShouldHaveActiveStatus()
    {
        // Arrange & Act
        var license = new License();

        // Assert
        Assert.Equal(LicenseStatus.Active, license.Status);
    }
}