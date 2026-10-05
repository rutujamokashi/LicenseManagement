using Microsoft.AspNetCore.Mvc.Testing;

namespace LicenseManagement.API.Tests;

public class HealthCheckTests
{
    [Fact]
    public async Task HealthEndpoint_ShouldReturnHealthy()
    {
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal("Healthy", content);
    }
}