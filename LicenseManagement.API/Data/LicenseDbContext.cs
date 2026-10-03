using LicenseManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LicenseManagement.API.Data;

public class LicenseDbContext : DbContext
{
    public LicenseDbContext(
        DbContextOptions<LicenseDbContext> options)
        : base(options)
    {
    }

    public DbSet<License> Licenses => Set<License>();
}