using LicenseManagement.API.DTOs;
using LicenseManagement.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace LicenseManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LicensesController : ControllerBase
{
    private readonly ILicenseService _licenseService;

    public LicensesController(
        ILicenseService licenseService)
    {
        _licenseService = licenseService;
    }

    [HttpPost("generate")]
    public async Task<ActionResult<GenerateLicenseResponse>> GenerateLicense(
    [FromBody] GenerateLicenseRequest request)
    {
        var response = await _licenseService.GenerateLicenseAsync(request);

        return Ok(response);
    }
    [HttpPost("validate")]
    public async Task<ActionResult<ValidateLicenseResponse>> ValidateLicense(
    [FromBody] ValidateLicenseRequest request)
    {
        var response = await _licenseService.ValidateLicenseAsync(request);

        return Ok(response);
    }
    [HttpPost("revoke")]
    public async Task<IActionResult> RevokeLicense(
    [FromBody] RevokeLicenseRequest request)
    {
        var result = await _licenseService
            .RevokeLicenseAsync(request.LicenseKey);

        if (!result)
        {
            return NotFound(new
            {
                message = "License key was not found."
            });
        }

        return Ok(new
        {
            message = "License revoked successfully."
        });
    }
}